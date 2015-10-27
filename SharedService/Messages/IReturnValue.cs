// <copyright file="IReturnValue.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System.Xml;
using MNet.SLOTaxService.Services;

namespace MNet.SLOTaxService.Messages
{
  public interface IReturnValue
  {
    BarCodes BarCodes { get; }
    string ErrorMessage { get; }
    ErrorMessageSource ErrorMessageSource { get; }
    XmlDocument MessageReceivedFromFurs { get; }
    XmlDocument MessageSendToFurs { get; }
    MessageType MessageType { get; }
    string ProtectedID { get; }
    SendingStep Step { get; }
    bool Success { get; }
    string UniqueInvoiceID { get; }
  }
}