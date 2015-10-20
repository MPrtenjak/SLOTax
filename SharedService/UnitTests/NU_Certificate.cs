// <copyright file="NU_Certificate.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using MNet.SLOTaxService.Services;
using NUnit.Framework;

namespace MNet.SLOTaxService.UnitTests
{
  /*
    This TestFixture works with TEST sertificates created with makecert.exe.
    file.cer and inStore.cer are in Resources directory
    Import "inStore" certificate to computer certificate store

    Utils:
      makecert -# 12345 inStore.cer

      makecert -# 123456 -r -pe -n "CN=MNet" -b 01/01/2015 -e 01/01/2040 -sky exchange file.cer -sv file.pvk
      pvk2pfx.exe -pvk file.pvk -spc file.cer -pfx file.pfx
  */

  [TestFixture]
  internal class NU_Certificate
  {
    [Test]
    public void FileCertificateOKTest1()
    {
      string appPath = new Uri(Path.GetDirectoryName(typeof(NU_Certificate).Assembly.EscapedCodeBase)).LocalPath;
      string fileCertPath = Path.Combine(appPath, @"UnitTests\resources\file.cer");

      X509Certificate2 cert = this.certificate.GetFromFile(fileCertPath, null);
      StringAssert.AreEqualIgnoringCase(cert.SerialNumber, "01E240");

      Assert.Throws<System.Exception>(() => this.certificate.GetFromFile(@"unknown certificate file", null));
    }

    [Test]
    public void FileCertificateOKTest2()
    {
      string appPath = new Uri(Path.GetDirectoryName(typeof(NU_Certificate).Assembly.EscapedCodeBase)).LocalPath;
      string fileCertPath = Path.Combine(appPath, @"UnitTests\resources\file.pfx");

      X509Certificate2 cert = this.certificate.GetFromFile(fileCertPath, null);
      StringAssert.AreEqualIgnoringCase(cert.SerialNumber, "01E240");

      Assert.Throws<System.Exception>(() => this.certificate.GetFromFile(@"unknown certificate file", null));
    }

    [Test]
    public void FileCertificateErrorTest()
    {
      Assert.Throws<System.Exception>(() => this.certificate.GetFromFile(@"unknown certificate file", null));
    }

    [Test]
    public void StoreCertificateErrorTest()
    {
      string serialNumber = "xyz";
      Assert.Throws<System.Exception>(() => this.certificate.GetBySerialNumber(serialNumber));
    }

    private Certificates certificate = new Certificates();
  }
}