// <copyright file="BarCodesHelpers.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNet.SLOTaxService.Modulo;

namespace MNet.SLOTaxService.Utils
{
  internal class BarCodesHelpers
  {
    // function from https://stackoverflow.com/questions/16965915
    public static string HexToDecimal(string hex)
    {
      List<int> dec = new List<int> { 0 };   // decimal result

      foreach (char c in hex)
      {
        int carry = Convert.ToInt32(c.ToString(), 16);

        // initially holds decimal value of current hex digit;
        // subsequently holds carry-over for multiplication
        for (int i = 0; i < dec.Count; ++i)
        {
          int val = (dec[i] * 16) + carry;
          dec[i] = val % 10;
          carry = val / 10;
        }

        while (carry > 0)
        {
          dec.Add(carry % 10);
          carry /= 10;
        }
      }

      var chars = dec.Select(d => (char)('0' + d));
      var cArr = chars.Reverse().ToArray();
      return new string(cArr);
    }

    public static string GenerateCode(string protectedIDHex, string taxNumber, DateTime timeStamp, IModulo modulo)
    {
      string decNumber = HexToDecimal(protectedIDHex).PadLeft(39, '0');

      StringBuilder sb = new StringBuilder(70);
      sb.Append(decNumber);
      sb.Append(taxNumber);
      sb.Append(timeStamp.ToString("yyMMddHHmmss"));

      return modulo.AppendModulo10(sb.ToString());
    }

    public static string[] SplitCode(string code, int lines)
    {
      if (lines < 1) lines = 1;
      if (lines > 6) lines = 6;

      string[] result = new string[lines];

      if (lines == 1)
      {
        result[0] = code;
        return result;
      }

      if (code.Length == 60)
      {
        int size = code.Length / lines;
        string prefix = (lines == 4) ? "44" : "4";
        for (int line = 0; line < lines; line++)
          result[line] = string.Format("{0}{1}{2}", prefix, line + 1, code.Substring(line * size, size));
      }
      else
      {
        for (int line = 0; line < lines; line++)
          result[line] = string.Empty;
      }

      return result;
    }
  }
}
