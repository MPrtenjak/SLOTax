﻿// <copyright file="ReturnValue.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System.Xml;
using MNet.SLOTaxService.Services;
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

  public class ReturnValue : IReturnValue
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

    public MessageType MessageType { get; private set; }
    public SendingStep Step { get; private set; }
    public XmlDocument MessageSendToFurs { get; private set; }
    public XmlDocument MessageReceivedFromFurs { get; private set; }
    public bool Success { get; private set; }
    public string ErrorMessage { get; private set; }
    public ErrorMessageSource ErrorMessageSource { get; private set; }
    public string ProtectedID { get; private set; }
    public string UniqueInvoiceID { get; private set; }
    public BarCodes BarCodes { get; private set; }

    private ReturnValue(SendingStep step, IMessage message, string errorMessage)
    {
      this.Step = step;

      if (message == null)
      {
        this.Success = false;
        this.ErrorMessage = errorMessage;
        this.MessageType = MessageType.Unknown;
        this.ErrorMessageSource = ErrorMessageSource.System;
        return;
      }

      this.MessageType = message.MessageType;
      this.originalMessage = message.Message;
      this.MessageSendToFurs = (step >= SendingStep.SoapEnvelopeGenerated) ? message.MessageSendToFurs : null;
      this.MessageReceivedFromFurs = (step >= SendingStep.MessageSend) ? message.MessageReceivedFromFurs : null;

      if (!string.IsNullOrEmpty(errorMessage))
      {
        // Even in case of an error, we can still calculate ZOI and Barcode
        this.CrocessCalculate(message);

        this.Success = false;
        this.ErrorMessage = errorMessage;
        this.ErrorMessageSource = ErrorMessageSource.System;
        return;
      }

      this.Success = true;
      this.ErrorMessage = this.ProtectedID = this.UniqueInvoiceID = string.Empty;

      // only calculation is special case
      if (message.MessageAction == MessageAction.Calculate)
        this.CrocessCalculate(message);
      else
        this.ProcessSend();
    }

    private void CrocessCalculate(IMessage message)
    {
      this.GetProtectedID(message.Message);
    }

    private void ProcessSend()
    {
      this.GetProtectedID(this.originalMessage);
      if (this.MessageReceivedFromFurs == null)
      {
        this.Success = false;
        this.ErrorMessage = "Unknown error";
        this.ErrorMessageSource = ErrorMessageSource.System;
      }

      XmlNode errMsgNode = XmlHelperFunctions.GetSubNode(this.MessageReceivedFromFurs.DocumentElement, "fu:ErrorMessage");
      XmlNode errMsgCodeNode = XmlHelperFunctions.GetSubNode(this.MessageReceivedFromFurs.DocumentElement, "fu:ErrorCode");
      if (errMsgNode != null)
      {
        this.Success = false;
        string id = (errMsgCodeNode == null) ? string.Empty : string.Format("[{0}]: ", errMsgCodeNode.InnerText);
        this.ErrorMessage = id + errMsgNode.InnerText;
        this.ErrorMessageSource = ErrorMessageSource.Furs;
      }

      XmlNode uniqueInvoiceIDNode = XmlHelperFunctions.GetSubNode(this.MessageReceivedFromFurs.DocumentElement, "fu:UniqueInvoiceID");
      if (uniqueInvoiceIDNode != null)
        this.UniqueInvoiceID = uniqueInvoiceIDNode.InnerText;
    }

    private void GetProtectedID(XmlDocument checkedDocument)
    {
      XmlNode id = XmlHelperFunctions.GetSubNode(checkedDocument.DocumentElement, "fu:ProtectedID");
      this.ProtectedID = (id == null) ? string.Empty : id.InnerText;
      this.BarCodes = (id == null) ? null : BarCodes.Create(checkedDocument);
    }

    private XmlDocument originalMessage;
  }
}