// <copyright file="TaxService.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System.Xml;
using MNet.SLOTaxService.Messages;
using MNet.SLOTaxService.Services;
using MNet.SLOTaxService.Utils;

namespace MNet.SLOTaxService
{
  public class TaxService : ITaxServer
  {
    public static TaxService Create(Settings settings)
    {
      return new TaxService(settings);
    }

    public Settings Settings { get; private set; }

    public ReturnValue CalculateProtectiveMark(XmlDocument message)
    {
      return this.Execute(Invoice.Create(message, this.Settings, MessageAction.Calculate));
    }

    public ReturnValue Send(string message)
    {
      try
      {
        XmlDocument xmlDoc = XmlHelperFunctions.CreateNewXmlDocument();
        xmlDoc.LoadXml(message);
        return this.Send(xmlDoc);
      }
      catch (System.Exception ex)
      {
        return ReturnValue.Error(SendingStep.MessageReceived, null, ex.Message);
      }
    }

    public ReturnValue Send(XmlDocument message)
    {
      string root = message.DocumentElement.LocalName;

      XmlNode node = XmlHelperFunctions.GetSubNode(message.DocumentElement, "fu:InvoiceRequest");
      if (string.Compare(root, "InvoiceRequest", true) == 0) return this.SendInvoice(message);
      if (string.Compare(root, "BusinessPremiseRequest", true) == 0) return this.SendBusinessPremise(message);
      if (string.Compare(root, "EchoRequest", true) == 0) return this.SendEcho(message);

      return ReturnValue.Error(SendingStep.MessageReceived, null, "Neznani dokument / Unknown document");
    }

    public ReturnValue SendEcho(string message)
    {
      return this.Execute(Echo.Create(message, this.Settings));
    }

    public ReturnValue SendEcho(XmlDocument message)
    {
      return this.Execute(Echo.Create(message, this.Settings));
    }

    public ReturnValue SendBusinessPremise(XmlDocument message)
    {
      return this.Execute(BusinessPremise.Create(message, this.Settings));
    }

    public ReturnValue SendInvoice(XmlDocument message)
    {
      return this.Execute(Invoice.Create(message, this.Settings, MessageAction.Send));
    }

    private ReturnValue Execute(IMessage msg)
    {
      return this.Execute(msg, true);
    }

    private ReturnValue Calculate(IMessage msg)
    {
      return this.Execute(msg, false);
    }

    private ReturnValue Execute(IMessage msg, bool send)
    {
      SendingStep step = SendingStep.MessageReceived;
      try
      {
        msg.Check();
        step = SendingStep.MessageChecked;

        if (msg.MessageAction == MessageAction.Calculate)
          return ReturnValue.ProtectiveMarkCalculated(step, msg);

        msg.SurroundWithSoap();
        step = SendingStep.SoapEnvelopeGenerated;

        msg.Sign();
        step = SendingStep.MessageSigned;

        msg.Validate();
        step = SendingStep.MessageValidated;

        msg.SendToFURS();
        step = SendingStep.MessageSend;

        return ReturnValue.Sent(step, msg);
      }
      catch (System.Exception ex)
      {
        return ReturnValue.Error(step, msg, ex.Message);
      }
    }

    private TaxService(Settings settings)
    {
      this.Settings = settings;
    }
  }
}