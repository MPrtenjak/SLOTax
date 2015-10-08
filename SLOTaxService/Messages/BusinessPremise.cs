// <copyright file="BusinessPremise.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Xml;
using MNet.SLOTaxService.Services;
using MNet.SLOTaxService.Utils;

namespace MNet.SLOTaxService.Messages
{
  internal class BusinessPremise : BaseMessage, IMessage
  {
    public static BusinessPremise Create(XmlDocument message, Settings settings)
    {
      return new BusinessPremise(message, settings);
    }

    public void Check()
    {
      this.checkRoot(MessageType.BusinessPremise);
      this.checkHeader();
      this.checkData();
    }

    public void Sign()
    {
      this.executeSign();
    }

    private BusinessPremise(XmlDocument message, Settings settings) :
      base(message, settings, MessageAction.Send)
    {
    }

    private void checkData()
    {
      XmlNode businessPremise = XmlHelperFunctions.GetSubNode(this.Message.DocumentElement, "fu:BusinessPremise");
      if (businessPremise == null)
        throw new ArgumentNullException("BusinessPremise");
    }
  }
}