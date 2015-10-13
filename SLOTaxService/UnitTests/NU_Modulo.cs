// <copyright file="NU_Modulo.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Text;
using MNet.SLOTaxService.Modulo;
using MNet.SLOTaxService.Utils;
using NUnit.Framework;

namespace MNet.SLOTaxService.UnitTests
{
  internal class NU_Modulo
  {
    [Test]
    public void Modulo10LuhnTest1()
    {
      string value = "543778220634452";
      int modulo10 = this.luhn.CalculateModulo10(value);
      Assert.AreEqual(modulo10, 0);
    }

    [Test]
    public void Modulo10LuhnTest2()
    {
      string value = "22317508792368707511223440252897316675512345678150815101332";
      int modulo10 = this.luhn.CalculateModulo10(value);
      Assert.AreEqual(modulo10, 7);
    }

    [Test]
    public void Modulo10EasyTest1()
    {
      string value = "543778220634452";
      int modulo10 = this.easy.CalculateModulo10(value);
      Assert.AreEqual(modulo10, 2);
    }

    [Test]
    public void Modulo10EasyTest2()
    {
      string value = "22317508792368707511223440252897316675512345678150815101332";
      int modulo10 = this.easy.CalculateModulo10(value);
      Assert.AreEqual(modulo10, 1);
    }

    [Test]
    public void Modulo10RandomTest()
    {
      Random rnd = new Random();

      for (int i = 0; i < 10; i++)
      {
        int len = rnd.Next(10, 23);

        StringBuilder sb = new StringBuilder(len + 2);
        for (int j = 0; j < len; j++)
          sb.Append((char)('0' + rnd.Next(0, 9)));

        this.doCheck(sb.ToString(), this.luhn);
        this.doCheck(sb.ToString(), this.easy);
      }
    }

    private void doCheck(string randomValue, IModulo modulo)
    {
      string value = modulo.AppendModulo10(randomValue);
      bool check = modulo.CheckModulo10(value);
      Assert.True(check);
    }

    private Modulo10_Luhn luhn = new Modulo10_Luhn();
    private Modulo10_Easy easy = new Modulo10_Easy();
  }
}