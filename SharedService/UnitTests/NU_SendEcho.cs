// <copyright file="NU_SendEcho.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using MNet.SLOTaxService.Messages;
using MNet.SLOTaxService.Services;
using NUnit.Framework;

namespace MNet.SLOTaxService.UnitTests
{
  [TestFixture]
  internal class NU_SendEcho : NU_Base
  {
    [Test]
    public void SendEchoOK1()
    {
      ReturnValue rv = this.taxService.SendEcho("123 echo 123");
      Assert.IsNullOrEmpty(rv.ErrorMessage);
      Assert.AreEqual(rv.Step, SendingStep.MessageSend);
      Assert.True(rv.Success);
      Assert.IsNotNull(rv.MessageSendToFurs);
      Assert.IsNotNull(rv.MessageReceivedFromFurs);
      Assert.IsNullOrEmpty(rv.ProtectedID);
      Assert.IsNullOrEmpty(rv.UniqueInvoiceID);
    }

    [Test]
    public void SendEchoERR()
    {
      // FURS can't handle special characters
      ReturnValue rv = this.taxService.SendEcho("<<>>");
      Assert.IsNotNullOrEmpty(rv.ErrorMessage);
      Assert.AreEqual(rv.Step, SendingStep.MessageValidated);
      Assert.False(rv.Success);
      Assert.IsNotNull(rv.MessageSendToFurs);
      Assert.IsNull(rv.MessageReceivedFromFurs);
      Assert.IsNullOrEmpty(rv.ProtectedID);
      Assert.IsNullOrEmpty(rv.UniqueInvoiceID);
    }

    [Test]
    public void SendWholeTest()
    {
      var certificateHelper = new Certificates();
      var certificate = certificateHelper.GetByTaxNumber(MyTaxNumber);
      var settings = Settings.CreateTestSettings(certificate);
      var ts = TaxService.Create(settings);

      ReturnValue rv = ts.SendEcho("This is echo message");
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