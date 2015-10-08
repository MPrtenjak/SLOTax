// <copyright file="NU_SendBusinessPremise.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System.Xml;
using MNet.SLOTaxService.Messages;
using NUnit.Framework;

namespace MNet.SLOTaxService.UnitTests
{
  [TestFixture]
  internal class NU_SendBusinessPremise : NU_Base
  {
    [Test]
    public void SendWrongXmlDocument1()
    {
      XmlDocument xmlDoc = this.getXml("ErrInvoice1.xml");
      ReturnValue rv = this.taxService.SendBusinessPremise(xmlDoc);
      Assert.IsNotNull(rv.ErrorMessage);
      Assert.AreEqual(rv.Step, SendingStep.MessageReceived);
      Assert.False(rv.Success);
      Assert.IsNull(rv.MessageSendToFurs);
      Assert.IsNull(rv.MessageReceivedFromFurs);
      Assert.IsNullOrEmpty(rv.ProtectedID);
      Assert.IsNullOrEmpty(rv.UniqueInvoiceID);
    }

    [Test]
    public void SendWrongXmlDocument2()
    {
      XmlDocument xmlDoc = this.getXml("ErrBusinessPremise.xml");
      ReturnValue rv = this.taxService.SendBusinessPremise(xmlDoc);
      Assert.IsNotNull(rv.ErrorMessage);
      Assert.AreEqual(rv.Step, SendingStep.MessageSigned);
      Assert.False(rv.Success);
      Assert.IsNotNull(rv.MessageSendToFurs);
      Assert.IsNull(rv.MessageReceivedFromFurs);
      Assert.IsNullOrEmpty(rv.ProtectedID);
      Assert.IsNullOrEmpty(rv.UniqueInvoiceID);
    }

    [Test]
    public void SendOKXmlDocument()
    {
      XmlDocument xmlDoc = this.getXml("OKBusinessPremise.xml");
      ReturnValue rv = this.taxService.SendBusinessPremise(xmlDoc);
      Assert.IsNullOrEmpty(rv.ErrorMessage);
      Assert.AreEqual(rv.Step, SendingStep.MessageSend);
      Assert.True(rv.Success);
      Assert.IsNotNull(rv.MessageSendToFurs);
      Assert.IsNotNull(rv.MessageReceivedFromFurs);
      Assert.IsNullOrEmpty(rv.ProtectedID);
      Assert.IsNullOrEmpty(rv.UniqueInvoiceID);
    }
  }
}