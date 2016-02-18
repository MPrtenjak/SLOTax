// <copyright file="NU_ProtectiveMark.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using MNet.SLOTaxService.Services;
using NUnit.Framework;

namespace MNet.SLOTaxService.UnitTests
{
  [TestFixture]
  internal class NU_ProtectiveMark
  {
    [Test]
    public void CalculateMD5HashTest()
    {
      string input = null;
      StringAssert.AreEqualIgnoringCase(this.doConvert(input), "d41d8cd98f00b204e9800998ecf8427e");

      input = @"test";
      StringAssert.AreEqualIgnoringCase(this.doConvert(input), "098f6bcd4621d373cade4e832627b4f6");

      input = @"    test 123";
      StringAssert.AreEqualIgnoringCase(this.doConvert(input), "7c5fc5fe4db05ef5acffcde661afbf2c");

      input = @"test   123   ";
      StringAssert.AreEqualIgnoringCase(this.doConvert(input), "717A2A7C9D9B68700F6DCAB4E72D4243");

      input = @"Visual Studio Express editions provide free tools to develop applications for a specific platform";
      StringAssert.AreEqualIgnoringCase(this.doConvert(input), "A24E623498B26735E3B6DD8989C38178");
    }

    [Test]
    public void CalculateTest()
    {
      string appPath = new Uri(Path.GetDirectoryName(typeof(NU_Certificate).Assembly.EscapedCodeBase)).LocalPath;
      string fileCertPath = Path.Combine(appPath, @"UnitTests\resources\file.pfx");

      Certificates cert = new Certificates();
      X509Certificate2 fileCert = cert.GetFromFile(fileCertPath, null);

      RSACryptoServiceProvider provider = Certificates.GetCryptoProvider(fileCert);

      string input = @"test";
      string output = this.pm.Calculate(input, provider);
      Assert.AreEqual(output, "0b77e05986aea28cace92a85a4443838");
    }

    private string doConvert(string input)
    {
      byte[] value = this.convertToByte(input);
      return this.pm.CalculateMD5Hash(value);
    }

    private byte[] convertToByte(string input)
    {
      if (input == null) input = string.Empty;
      return Encoding.ASCII.GetBytes(input);
    }

    private ProtectiveMark pm = new ProtectiveMark();
  }
}