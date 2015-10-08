// <copyright file="IMessage.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System.Xml;

namespace MNet.SLOTaxService.Messages
{
  internal enum MessageType
  {
    Invoice,
    BuisinessPremise,
    Echo,
    Unknown,
  }

  internal enum MessageAction
  {
    Calculate,
    Send,
  }

  internal interface IMessage
  {
    XmlDocument Message { get; }
    XmlDocument MessageSendToFurs { get; }
    XmlDocument MessageReceivedFromFurs { get; }

    MessageType MessageType { get; }
    MessageAction MessageAction { get; }

    void Check();

    void SurroundWithSoap();

    void Sign();

    void Validate();

    void SendToFURS();
  }
}