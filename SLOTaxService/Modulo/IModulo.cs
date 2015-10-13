// <copyright file="IModulo.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

namespace MNet.SLOTaxService.Modulo
{
  internal interface IModulo
  {
    int CalculateModulo10(string value);

    string AppendModulo10(string value);

    bool CheckModulo10(string value);
  }
}