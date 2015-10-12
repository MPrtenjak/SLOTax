// <copyright file="Modulo10_Easy.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNet.SLOTaxService.Utils
{
  internal class Modulo10_Easy : IModulo 
  {
    public int CalculateModulo10(string value)
    {
      int len = value.Length;
      int sum = 0;
      for (int i = len - 1; i >= 0; i--)
      {
        int tmp = (value[i] - '0');
        sum += tmp;
      }

      int modulo10 = sum % 10;
      return modulo10;
    }

    public string AppendModulo10(string value)
    {
      int modulo10 = this.CalculateModulo10(value);
      return value + (char)('0' + modulo10);
    }

    public bool CheckModulo10(string value)
    {
      if (string.IsNullOrEmpty(value))
        return false;

      int test = (int)(value.Last() - '0');
      value = value.Remove(value.Length - 1);
      int sumOfDigits = value.Where((e) => e >= '0' && e <= '9')
                             .Select(e => (int)(e - '0'))
                             .Sum(e => e);

      return sumOfDigits % 10 == test;
    }
  }
}
