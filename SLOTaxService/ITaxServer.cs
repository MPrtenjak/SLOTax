// <copyright file="ITaxServer.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System.Xml;
using MNet.SLOTaxService.Messages;
using MNet.SLOTaxService.Services;

namespace MNet.SLOTaxService
{
  public interface ITaxServer
  {
    Settings Settings { get; }

    string CalculateProtectiveMark(XmlDocument message);

    ReturnValue SendEcho(string message);

    ReturnValue SendBusinessPremise(XmlDocument message);

    ReturnValue SendInvoice(XmlDocument message);
  }
}