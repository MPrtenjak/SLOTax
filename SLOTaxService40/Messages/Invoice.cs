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
      this.checkRoot(MessageType.Invoice);

      if (this.MessageAction == MessageAction.Calculate)
      {
        this.checkData();
        return;
      }

      this.checkHeader();
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

        invoice.AppendChild(XmlHelperFunctions.CreateElement(message, this.Settings.FursXmlNamespace, "ProtectedID", protectedIDValue));
      }
    }
  }
}