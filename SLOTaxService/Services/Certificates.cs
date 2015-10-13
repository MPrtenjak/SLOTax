// <copyright file="Certificates.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace MNet.SLOTaxService.Services
{
  public class Certificates
  {
    public X509Certificate2 GetBySerialNumber(string serialNumber)
    {
      return this.GetBySerialNumber(serialNumber, StoreLocation.CurrentUser, StoreName.My);
    }

    public X509Certificate2 GetBySerialNumber(string serialNumber, StoreLocation storeLocation, StoreName storeName)
    {
      X509Store store = new X509Store(storeName, storeLocation);
      store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
      var matchingCertificates = store.Certificates.Find(X509FindType.FindBySerialNumber, serialNumber, true);
      store.Close();

      if ((matchingCertificates == null) || (matchingCertificates.Count < 1))
        throw new Exception("Ne najdem digitalnega potrdila / Can't find certificate");

      if (matchingCertificates.Count > 1)
        throw new Exception("Digitalno potrdilo ni edinstveno / Certificate not unique");

      return matchingCertificates[0];
    }

    public X509Certificate2 GetByTaxNumber(string taxNumber)
    {
      return this.GetByTaxNumber(taxNumber, StoreLocation.CurrentUser, StoreName.My);
    }

    public X509Certificate2 GetByTaxNumber(string taxNumber, StoreLocation storeLocation, StoreName storeName)
    {
      X509Store store = new X509Store(storeName, storeLocation);
      store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
      var matchingCertificates = store.Certificates.Find(X509FindType.FindByIssuerDistinguishedName, "CN=Tax CA Test, O=state-institutions, C=SI", true);
      store.Close();

      X509Certificate2 certificate = null;
      foreach (X509Certificate2 cert in matchingCertificates)
      {
        if (cert.SubjectName.Name.IndexOf(taxNumber) > 0)
        {
          if (certificate == null)
            certificate = cert;
          else
            throw new Exception("Digitalno potrdilo ni edinstveno / Certificate not unique");
        }
      }

      if (certificate == null)
        throw new Exception("Ne najdem digitalnega potrdila / Can't find certificate");

      return certificate;
    }

    public X509Certificate2 GetFromFile(string certificateFile, string password)
    {
      if (!File.Exists(certificateFile))
        throw new Exception("Ne najdem digitalnega potrdila / Can't find certificate");

      return new X509Certificate2(certificateFile, password);
    }

    public List<X509Certificate2> GetAllFursCertificates()
    {
      List<X509Certificate2> result = new List<X509Certificate2>();

      List<StoreLocation> certificateStores = new List<StoreLocation>() { StoreLocation.CurrentUser, StoreLocation.LocalMachine };
      foreach (var cerStore in certificateStores)
      {
        X509Store store = new X509Store(cerStore);
        store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
        foreach (var cert in store.Certificates.Find(X509FindType.FindByIssuerDistinguishedName, "CN=Tax CA Test, O=state-institutions, C=SI", true))
          result.Add(cert);
        store.Close();
      }

      return result;
    }

    internal static RSACryptoServiceProvider getCryptoProvider(X509Certificate2 certificate)
    {
      RSACryptoServiceProvider privateKey = (RSACryptoServiceProvider)certificate.PrivateKey;

      CspParameters cspParameters = new CspParameters();
      cspParameters.KeyContainerName = privateKey.CspKeyContainerInfo.KeyContainerName;
      cspParameters.KeyNumber = (privateKey.CspKeyContainerInfo.KeyNumber == KeyNumber.Exchange) ? 1 : 2;

      return new RSACryptoServiceProvider(cspParameters);
    }
  }
}