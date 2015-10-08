// <copyright file="BuisinessPremise.cs" company="MNet">
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
  internal class BuisinessPremise : BaseMessage, IMessage
  {
    public static BuisinessPremise Create(XmlDocument message, Settings settings)
    {
      return new BuisinessPremise(message, settings);
    }

    public void Check()
    {
      this.checkRoot(MessageType.BuisinessPremise);
      this.checkHeader();
      this.checkData();
    }

    public void Sign()
    {
      this.executeSign();
    }

    private BuisinessPremise(XmlDocument message, Settings settings) :
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