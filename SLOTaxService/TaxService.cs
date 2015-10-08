// <copyright file="TaxService.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System.Xml;
using MNet.SLOTaxService.Messages;
using MNet.SLOTaxService.Services;

namespace MNet.SLOTaxService
{
  public class TaxService : ITaxServer
  {
    public static TaxService Create(Settings settings)
    {
      return new TaxService(settings);
    }

    public Settings Settings { get; private set; }

    public string CalculateProtectiveMark(XmlDocument message)
    {
      ReturnValue rv = this.execute(Invoice.Create(message, this.Settings, MessageAction.Calculate));
      return (rv.Success) ? rv.ProtectedID : null;
    }

    public ReturnValue SendEcho(string message)
    {
      return this.execute(Echo.Create(message, this.Settings));
    }

    public ReturnValue SendBusinessPremise(XmlDocument message)
    {
      return this.execute(BuisinessPremise.Create(message, this.Settings));
    }

    public ReturnValue SendInvoice(XmlDocument message)
    {
      return this.execute(Invoice.Create(message, this.Settings, MessageAction.Send));
    }

    private ReturnValue execute(IMessage msg)
    {
      return this.execute(msg, true);
    }

    private ReturnValue calculate(IMessage msg)
    {
      return this.execute(msg, false);
    }

    private ReturnValue execute(IMessage msg, bool send)
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