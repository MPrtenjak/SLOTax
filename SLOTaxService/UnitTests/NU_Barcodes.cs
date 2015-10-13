// <copyright file="NU_Barcodes.cs" company="MNet">
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
  internal class NU_Barcodes
  {
    [Test]
    public void ConvertHex2DecTest()
    {
      string hexNumber = "a7e5f55e1dbb48b799268e1a6d8618a3";
      string dec = BarCodesHelpers.HexToDecimal(hexNumber);

      StringAssert.AreEqualIgnoringCase(dec, "223175087923687075112234402528973166755");
    }

    [Test]
    public void generateBarCodeTest1()
    {
      IModulo modulo = new Modulo10_Luhn();
      string barCode = BarCodesHelpers.GenerateCode("a7e5f55e1dbb48b799268e1a6d8618a3", "12345678", Convert.ToDateTime("2015-08-15 10:13:32"), modulo);
      StringAssert.AreEqualIgnoringCase(barCode, "223175087923687075112234402528973166755123456781508151013327");
    }

    [Test]
    public void generateBarCodeTest2()
    {
      IModulo modulo = new Modulo10_Easy();
      string barCode = BarCodesHelpers.GenerateCode("1234567890abcdef", "24578436", Convert.ToDateTime("2017-01-11 18:00:32"), modulo);
      StringAssert.AreEqualIgnoringCase(barCode, "000000000000000000001311768467294899695245784361701111800329");
    }

    [Test]
    public void splitTestErr()
    {
      string code = "87648732165498743659874326"; // not corect length

      for (int i = 1; i < 10; i++)
      {
        string[] lines = BarCodesHelpers.SplitCode(code, i);

        if (i == 1)
          Assert.AreEqual(lines[0], code);
        else
        {
          for (int t = 0; t < Math.Min(i, 6); t++)
            Assert.IsNullOrEmpty(lines[t]);
        }
      }
    }

    [Test]
    public void splitTestOK1()
    {
      string code = "223175087923687075112234402528973166755123456781508151013321";
      string[] lines = BarCodesHelpers.SplitCode(code, 1);

      Assert.AreEqual(lines.Length, 1);
      StringAssert.AreEqualIgnoringCase(lines[0], code);
    }

    [Test]
    public void splitTestOK2()
    {
      string code = "223175087923687075112234402528973166755123456781508151013321";
      string[] lines = BarCodesHelpers.SplitCode(code, 2);

      Assert.AreEqual(lines.Length, 2);
      StringAssert.AreEqualIgnoringCase(lines[0], "41223175087923687075112234402528");
      StringAssert.AreEqualIgnoringCase(lines[1], "42973166755123456781508151013321");
    }

    [Test]
    public void splitTestOK3()
    {
      string code = "223175087923687075112234402528973166755123456781508151013321";
      string[] lines = BarCodesHelpers.SplitCode(code, 3);

      Assert.AreEqual(lines.Length, 3);
      StringAssert.AreEqualIgnoringCase(lines[0], "4122317508792368707511");
      StringAssert.AreEqualIgnoringCase(lines[1], "4222344025289731667551");
      StringAssert.AreEqualIgnoringCase(lines[2], "4323456781508151013321");
    }

    [Test]
    public void splitTestOK4()
    {
      string code = "223175087923687075112234402528973166755123456781508151013321";
      string[] lines = BarCodesHelpers.SplitCode(code, 4);

      Assert.AreEqual(lines.Length, 4);
      StringAssert.AreEqualIgnoringCase(lines[0], "441223175087923687");
      StringAssert.AreEqualIgnoringCase(lines[1], "442075112234402528");
      StringAssert.AreEqualIgnoringCase(lines[2], "443973166755123456");
      StringAssert.AreEqualIgnoringCase(lines[3], "444781508151013321");
    }

    [Test]
    public void splitTestOK5()
    {
      string code = "223175087923687075112234402528973166755123456781508151013321";
      string[] lines = BarCodesHelpers.SplitCode(code, 5);

      Assert.AreEqual(lines.Length, 5);
      StringAssert.AreEqualIgnoringCase(lines[0], "41223175087923");
      StringAssert.AreEqualIgnoringCase(lines[1], "42687075112234");
      StringAssert.AreEqualIgnoringCase(lines[2], "43402528973166");
      StringAssert.AreEqualIgnoringCase(lines[3], "44755123456781");
      StringAssert.AreEqualIgnoringCase(lines[4], "45508151013321");
    }

    [Test]
    public void splitTestOK6()
    {
      string code = "223175087923687075112234402528973166755123456781508151013321";
      string[] lines = BarCodesHelpers.SplitCode(code, 6);

      Assert.AreEqual(lines.Length, 6);
      StringAssert.AreEqualIgnoringCase(lines[0], "412231750879");
      StringAssert.AreEqualIgnoringCase(lines[1], "422368707511");
      StringAssert.AreEqualIgnoringCase(lines[2], "432234402528");
      StringAssert.AreEqualIgnoringCase(lines[3], "449731667551");
      StringAssert.AreEqualIgnoringCase(lines[4], "452345678150");
      StringAssert.AreEqualIgnoringCase(lines[5], "468151013321");
    }
  }
}
