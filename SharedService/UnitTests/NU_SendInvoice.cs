// <copyright file="NU_SendInvoice.cs" company="MNet">
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
  internal class NU_SendInvoice : NU_Base
  {
    [Test]
    public void SendWrongXmlDocument1()
    {
      XmlDocument xmlDoc = this.getXml("ErrBusinessPremise.xml");
      ReturnValue rv = this.taxService.SendInvoice(xmlDoc);
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
      XmlDocument xmlDoc = this.getXml("ErrInvoice1.xml");
      ReturnValue rv = this.taxService.SendInvoice(xmlDoc);
      Assert.IsNotNull(rv.ErrorMessage);
      Assert.AreEqual(rv.Step, SendingStep.MessageReceived);
      Assert.False(rv.Success);
      Assert.IsNull(rv.MessageSendToFurs);
      Assert.IsNull(rv.MessageReceivedFromFurs);
      Assert.IsNullOrEmpty(rv.ProtectedID);
      Assert.IsNullOrEmpty(rv.UniqueInvoiceID);
    }

    [Test]
    public void SendWrongXmlDocument3()
    {
      XmlDocument xmlDoc = this.getXml("ErrInvoice2.xml");
      ReturnValue rv = this.taxService.SendInvoice(xmlDoc);
      Assert.IsNotNull(rv.ErrorMessage);
      Assert.AreEqual(rv.Step, SendingStep.MessageSigned);
      Assert.False(rv.Success);
      Assert.IsNotNull(rv.MessageSendToFurs);
      Assert.IsNull(rv.MessageReceivedFromFurs);
      Assert.IsNullOrEmpty(rv.ProtectedID);
      Assert.IsNullOrEmpty(rv.UniqueInvoiceID);
    }

    [Test]
    public void SendWrongXmlDocument4()
    {
      XmlDocument xmlDoc = this.getXml("ErrInvoice3.xml");
      ReturnValue rv = this.taxService.SendInvoice(xmlDoc);
      Assert.IsNotNull(rv.ErrorMessage);
      Assert.AreEqual(rv.Step, SendingStep.MessageSigned);
      Assert.False(rv.Success);
      Assert.IsNotNull(rv.MessageSendToFurs);
      Assert.IsNull(rv.MessageReceivedFromFurs);
      Assert.IsNullOrEmpty(rv.ProtectedID);
      Assert.IsNullOrEmpty(rv.UniqueInvoiceID);
    }

    [Test]
    public void SendOKXmlDocument1()
    {
      XmlDocument xmlDoc = this.getXml("OKInvoice1.xml");
      ReturnValue rv = this.taxService.SendInvoice(xmlDoc);
      Assert.IsNullOrEmpty(rv.ErrorMessage);
      Assert.AreEqual(rv.Step, SendingStep.MessageSend);
      Assert.True(rv.Success);
      Assert.IsNotNull(rv.MessageSendToFurs);
      Assert.IsNotNull(rv.MessageReceivedFromFurs);
      Assert.IsNotNullOrEmpty(rv.ProtectedID);
      Assert.IsNotNullOrEmpty(rv.UniqueInvoiceID);
    }

    [Test]
    public void SendOKXmlDocument2()
    {
      XmlDocument xmlDoc = this.getXml("OKInvoice2.xml");
      ReturnValue rv = this.taxService.SendInvoice(xmlDoc);
      Assert.IsNullOrEmpty(rv.ErrorMessage);
      Assert.AreEqual(rv.Step, SendingStep.MessageSend);
      Assert.True(rv.Success);
      Assert.IsNotNull(rv.MessageSendToFurs);
      Assert.IsNotNull(rv.MessageReceivedFromFurs);
      Assert.IsNotNullOrEmpty(rv.ProtectedID);
      Assert.IsNotNullOrEmpty(rv.UniqueInvoiceID);
    }

    [Test]
    public void SendOKXmlDocument3()
    {
      XmlDocument xmlDoc = this.getXml("OKInvoice3.xml");
      ReturnValue rv = this.taxService.SendInvoice(xmlDoc);
      Assert.IsNullOrEmpty(rv.ErrorMessage);
      Assert.AreEqual(rv.Step, SendingStep.MessageSend);
      Assert.True(rv.Success);
      Assert.IsNotNull(rv.MessageSendToFurs);
      Assert.IsNotNull(rv.MessageReceivedFromFurs);
      Assert.IsNotNullOrEmpty(rv.ProtectedID);
      Assert.IsNotNullOrEmpty(rv.UniqueInvoiceID);
    }

    [Test]
    public void CalculateProtectedMarkERR()
    {
      XmlDocument xmlDoc = this.getXml("ErrInvoice1.xml");
      ReturnValue rv = this.taxService.CalculateProtectiveMark(xmlDoc);
      Assert.IsNotNullOrEmpty(rv.ErrorMessage);
      Assert.AreEqual(rv.Step, SendingStep.MessageReceived);
      Assert.False(rv.Success);
      Assert.IsNull(rv.MessageSendToFurs);
      Assert.IsNull(rv.MessageReceivedFromFurs);
      Assert.IsNullOrEmpty(rv.ProtectedID);
      Assert.IsNullOrEmpty(rv.UniqueInvoiceID);
    }

    [Test]
    public void CalculateProtectedMarkOK()
    {
      XmlDocument xmlDoc = this.getXml("OkInvoice1.xml");
      ReturnValue rv = this.taxService.CalculateProtectiveMark(xmlDoc);
      Assert.IsNullOrEmpty(rv.ErrorMessage);
      Assert.AreEqual(rv.Step, SendingStep.MessageChecked);
      Assert.True(rv.Success);
      Assert.IsNull(rv.MessageSendToFurs);
      Assert.IsNull(rv.MessageReceivedFromFurs);
      Assert.IsNotNullOrEmpty(rv.ProtectedID);
      Assert.IsNullOrEmpty(rv.UniqueInvoiceID);
    }
  }
}