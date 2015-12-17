// <copyright file="Invoice.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Xml;
using MNet.SLOTaxService.Services;
using MNet.SLOTaxService.Utils;

namespace MNet.SLOTaxService.Messages
{
  internal class Invoice : BaseMessage, IMessage
  {
    public static Invoice Create(XmlDocument message, Settings settings, MessageAction messageAction)
    {
      return new Invoice(message, settings, messageAction);
    }

    public void Check()
    {
      this.CheckRoot(MessageType.Invoice);

      if (this.MessageAction == MessageAction.Calculate)
      {
        this.checkData();
        return;
      }

      this.CheckHeader();
      this.checkData();
    }

    public void Sign()
    {
      this.executeSign();
    }

    private Invoice(XmlDocument message, Settings settings, MessageAction messageAction) :
      base(message, settings, messageAction)
    {
    }

    private void checkData()
    {
      XmlNode invoice = XmlHelperFunctions.GetSubNode(this.Message.DocumentElement, "fu:Invoice");
      if (invoice != null)
      {
        this.checkAndCalculateProtectedID(this.Message, invoice);
        return;
      }

      XmlNode salesBookInvoice = XmlHelperFunctions.GetSubNode(this.Message.DocumentElement, "fu:SalesBookInvoice");
      if (salesBookInvoice == null)
        throw new ArgumentNullException("Invoice");
    }

    private void checkAndCalculateProtectedID(XmlDocument message, XmlNode invoice)
    {
      // field ProtectedID is mandatory, but if it is not suplied it is going to be calculated!
      XmlNodeList protectedIDs = (invoice as XmlElement).GetElementsByTagName("fu:ProtectedID");
      if (protectedIDs.Count == 0)
      {
        ProtectiveMark pm = new ProtectiveMark();
        string protectedIDValue = pm.Calculate(invoice as XmlElement, Settings.CryptoProvider);

        // ProtectedID is not the last element in XML Schema, so we must put it in the right spot
        //    we are going from bottom up, searching for the right spot
        XmlNode protectedIDNode = XmlHelperFunctions.CreateElement(message, this.Settings.FursXmlNamespace, "ProtectedID", protectedIDValue);

        XmlNode currentNode = invoice.LastChild;
        while ((currentNode != null) && (this.isNodeAfterProtectedID(currentNode)))
          currentNode = currentNode.PreviousSibling;

        if (currentNode != null)
          invoice.InsertAfter(protectedIDNode, currentNode);
      }
    }

    private bool isNodeAfterProtectedID(XmlNode node)
    {
      // nodes which are not elements (aka. spaces...)
      if (node.NodeType != XmlNodeType.Element) return true;

      // some nodes in Invoice element are after ProtectedID and it this is one of those nodes, function returns true
      if (string.Compare(node.LocalName, "SubsequentSubmit", true) == 0) return true;
      if (string.Compare(node.LocalName, "ReferenceInvoice", true) == 0) return true;
      if (string.Compare(node.LocalName, "ReferenceSalesBook", true) == 0) return true;
      if (string.Compare(node.LocalName, "SpecialNotes", true) == 0) return true;

      return false;
    }
  }
}