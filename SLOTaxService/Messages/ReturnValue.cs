// <copyright file="ReturnValue.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System.Xml;
using MNet.SLOTaxService.Utils;

namespace MNet.SLOTaxService.Messages
{
  public enum SendingStep
  {
    MessageReceived,
    MessageChecked,
    SoapEnvelopeGenerated,
    MessageSigned,
    MessageValidated,
    MessageSend,
  }

  public class ReturnValue
  {
    internal static ReturnValue ProtectiveMarkCalculated(SendingStep step, IMessage message)
    {
      return new ReturnValue(step, message, null);
    }

    internal static ReturnValue Sent(SendingStep step, IMessage message)
    {
      return new ReturnValue(step, message, null);
    }

    internal static ReturnValue Error(SendingStep step, IMessage msg, string errorMsg)
    {
      return new ReturnValue(step, msg, errorMsg);
    }

    public SendingStep Step { get; private set; }
    public XmlDocument MessageSendToFurs { get; private set; }
    public XmlDocument MessageReceivedFromFurs { get; private set; }
    public bool Success { get; private set; }
    public string ErrorMessage { get; private set; }
    public string ProtectedID { get; private set; }
    public string UniqueInvoiceID { get; private set; }

    private ReturnValue(SendingStep step, IMessage message, string errorMessage)
    {
      this.Step = step;

      if (message == null)
      {
        this.Success = false;
        this.ErrorMessage = errorMessage;
        this.messageType = MessageType.Unknown;
        return;
      }

      this.messageType = message.MessageType;
      this.originalMessage = message.Message;
      this.MessageSendToFurs = (step >= SendingStep.SoapEnvelopeGenerated) ? message.MessageSendToFurs : null;
      this.MessageReceivedFromFurs = (step >= SendingStep.MessageSend) ? message.MessageReceivedFromFurs : null;

      if (!string.IsNullOrEmpty(errorMessage))
      {
        this.Success = false;
        this.ErrorMessage = errorMessage;
        return;
      }

      this.Success = true;
      this.ErrorMessage = this.ProtectedID = this.UniqueInvoiceID = string.Empty;

      // only calculation is special case
      if (message.MessageAction == MessageAction.Calculate)
        this.processCalculate(message);
      else
        this.processSend();
    }

    private void processCalculate(IMessage message)
    {
      this.ProtectedID = this.getProtectedID(message.Message);
    }

    private void processSend()
    {
      this.ProtectedID = this.getProtectedID(this.originalMessage);
      if (this.MessageReceivedFromFurs == null)
      {
        this.Success = false;
        this.ErrorMessage = "Unknown error";
      }

      XmlNode errMsgNode = XmlHelperFunctions.GetSubNode(this.MessageReceivedFromFurs.DocumentElement, "fu:ErrorMessage");
      XmlNode errMsgCodeNode = XmlHelperFunctions.GetSubNode(this.MessageReceivedFromFurs.DocumentElement, "fu:ErrorCode");
      if (errMsgNode != null)
      {
        this.Success = false;
        string id = (errMsgCodeNode == null) ? string.Empty : string.Format("[{0}]: ", errMsgCodeNode.InnerText);
        this.ErrorMessage = id + errMsgNode.InnerText;
      }

      XmlNode uniqueInvoiceIDNode = XmlHelperFunctions.GetSubNode(this.MessageReceivedFromFurs.DocumentElement, "fu:UniqueInvoiceID");
      if (uniqueInvoiceIDNode != null)
        this.UniqueInvoiceID = uniqueInvoiceIDNode.InnerText;
    }

    private string getProtectedID(XmlDocument checkedDocument)
    {
      XmlNode id = XmlHelperFunctions.GetSubNode(checkedDocument.DocumentElement, "fu:ProtectedID");
      return (id == null) ? string.Empty : id.InnerText;
    }

    private MessageType messageType;
    private XmlDocument originalMessage;
  }
}