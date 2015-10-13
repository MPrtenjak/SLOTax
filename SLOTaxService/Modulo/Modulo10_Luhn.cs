// <copyright file="Modulo10_Luhn.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System.Linq;
using MNet.SLOTaxService.Modulo;

namespace MNet.SLOTaxService.Utils
{
  internal class Modulo10_Luhn : IModulo
  {
    public int CalculateModulo10(string value)
    {
      int len = value.Length;
      int sum = 0;
      int multiplicator = 2;
      for (int i = len - 1; i >= 0; i--)
      {
        int tmp = (value[i] - '0') * multiplicator;
        if (tmp >= 10)
          sum += (tmp / 10) + (tmp % 10);
        else
          sum += tmp;

        multiplicator = (multiplicator == 2) ? 1 : 2;
      }

      int modulo10 = sum % 10;
      if (modulo10 > 0) modulo10 = 10 - modulo10;

      return modulo10;
    }

    public string AppendModulo10(string value)
    {
      int modulo10 = this.CalculateModulo10(value);
      return value + (char)('0' + modulo10);
    }

    // function from http://www.codeproject.com/Tips/515367/Validate-credit-card-number-with-Mod-algorithm
    public bool CheckModulo10(string value)
    {
      //// check whether input string is null or empty
      if (string.IsNullOrEmpty(value))
      {
        return false;
      }

      /*
        1. Starting with the check digit double the value of every other digit
        2. If doubling of a number results in a two digits number, add up
           the digits to get a single digit number. This will results in eight single digit numbers
        3. Get the sum of the digits
      */
      int sumOfDigits = value.Where((e) => e >= '0' && e <= '9')
                      .Reverse()
                      .Select((e, i) => ((int)e - 48) * (i % 2 == 0 ? 1 : 2))
                      .Sum((e) => (e / 10) + (e % 10));

      //// If the final sum is divisible by 10, then the credit card number
      //   is valid. If it is not divisible by 10, the number is invalid.
      return sumOfDigits % 10 == 0;
    }
  }
}