// <copyright file="BarCodes.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace MNet.SLOTaxService.Services
{
  internal class BarCodes
  {
    private BarCodes(XmlDocument invoice)
    {
      this.invoice = invoice;
    }

    private XmlDocument invoice;
  }
}
