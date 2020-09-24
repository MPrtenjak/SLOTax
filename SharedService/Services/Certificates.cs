﻿// <copyright file="Certificates.cs" company="MNet">
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
    public X509Certificate2 GetByThumbprint(string thumbprint)
    {
      // not working properly
      // var matchingCertificates = this.findAllCertificatesInAllStores(this.allStores(), (store) => { return store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false); });

      // work around
      var matchingCertificates = this.findAllCertificatesInAllStores(this.allStores(),
        (store) => this.filterCertificates(store.Certificates,
          (cert) => string.Compare(cert.Thumbprint, thumbprint, true) == 0));

      return this.getSingleCertificate(matchingCertificates);
    }

    public X509Certificate2 GetBySerialNumber(string serialNumber)
    {
      // not working properly
      // var matchingCertificates = this.findAllCertificatesInAllStores(this.allStores(), (store) => { return store.Certificates.Find(X509FindType.FindBySerialNumber, serialNumber, true); });

      // work around
      var matchingCertificates = this.findAllCertificatesInAllStores(this.allStores(),
        (store) => this.filterCertificates(store.Certificates,
          (cert) => string.Compare(cert.SerialNumber, serialNumber, true) == 0));

      return this.getSingleCertificate(matchingCertificates);
    }

    public X509Certificate2 GetBySerialNumber(string serialNumber, StoreLocation storeLocation, StoreName storeName)
    {
      X509Store store = new X509Store(storeName, storeLocation);

      // not working properly
      // var matchingCertificates = this.findAllCertificatesInStore(store, (s) => { return s.Certificates.Find(X509FindType.FindBySerialNumber, serialNumber, true); });

      // work around
      var matchingCertificates = this.filterCertificates(store.Certificates,
          (cert) => string.Compare(cert.SerialNumber, serialNumber, true) == 0);

      return this.getSingleCertificate(matchingCertificates);
    }

    public X509Certificate2 GetByTaxNumber(string taxNumber)
    {
      var matchingCertificates = this.getAllFursCertificates();
      matchingCertificates = this.filterCertificates(matchingCertificates, (cert) => { return cert.SubjectName.Name.IndexOf(taxNumber) > 0; });

      return this.getSingleCertificate(matchingCertificates);
    }

    public X509Certificate2 GetByTaxNumber(string taxNumber, StoreLocation storeLocation, StoreName storeName)
    {
      X509Store store = new X509Store(storeName, storeLocation);
      var matchingCertificates = this.findAllCertificatesInStore(store, (s) => { return s.Certificates.Find(X509FindType.FindByIssuerDistinguishedName, "CN=Tax CA Test, O=state-institutions, C=SI", true); });
      matchingCertificates.AddRange(this.findAllCertificatesInStore(store, (s) => { return s.Certificates.Find(X509FindType.FindByIssuerDistinguishedName, "CN=TaxCA, O=state-institutions, C=SI", true); }));
      matchingCertificates = this.filterCertificates(matchingCertificates, (cert) => { return cert.SubjectName.Name.IndexOf(taxNumber) > 0; });

      return this.getSingleCertificate(matchingCertificates);
    }

    public X509Certificate2 GetFromFile(string certificateFile, string password)
    {
      if (!File.Exists(certificateFile))
        throw new Exception("Ne najdem digitalnega potrdila / Can't find certificate");

      return new X509Certificate2(certificateFile, password);
    }

    public List<X509Certificate2> GetAllFursCertificates()
    {
      var matchingCertificates = this.getAllFursCertificates();

      List<X509Certificate2> list = new List<X509Certificate2>();
      foreach (var cert in matchingCertificates)
        list.Add(cert);

      return list;
    }

    internal static RSACryptoServiceProvider getCryptoProvider(X509Certificate2 certificate)
    {
      RSACryptoServiceProvider privateKey = (RSACryptoServiceProvider)certificate.PrivateKey;

      CspParameters cspParameters = new CspParameters
      {
        KeyContainerName = privateKey.CspKeyContainerInfo.KeyContainerName,
        KeyNumber = (privateKey.CspKeyContainerInfo.KeyNumber == KeyNumber.Exchange) ? 1 : 2
      };

      if (privateKey.CspKeyContainerInfo.MachineKeyStore)
        cspParameters.Flags |= CspProviderFlags.UseMachineKeyStore;
      else
        cspParameters.Flags &= (~CspProviderFlags.UseMachineKeyStore);

      return new RSACryptoServiceProvider(cspParameters);
    }

    private X509Certificate2Collection findAllCertificatesInStore(X509Store store, Func<X509Store, X509Certificate2Collection> searchAction)
    {
      store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
      var matchingCertificates = searchAction(store);
      store.Close();

      return matchingCertificates;
    }

    private X509Certificate2Collection findAllCertificatesInAllStores(IList<X509Store> stores, Func<X509Store, X509Certificate2Collection> searchAction)
    {
      X509Certificate2Collection matchingCertificates = new X509Certificate2Collection();

      foreach (var store in stores)
        matchingCertificates.AddRange(this.findAllCertificatesInStore(store, searchAction));

      return matchingCertificates;
    }

    private X509Certificate2Collection getAllFursCertificates()
    {
      X509Certificate2Collection certificates = this.findAllCertificatesInAllStores(this.allStores(), (store) => { return store.Certificates.Find(X509FindType.FindByIssuerDistinguishedName, "CN=Tax CA Test, O=state-institutions, C=SI", true); });
      certificates.AddRange(this.findAllCertificatesInAllStores(this.allStores(), (store) => { return store.Certificates.Find(X509FindType.FindByIssuerDistinguishedName, "CN=TaxCA, O=state-institutions, C=SI", true); }));

      return certificates;
    }

    private X509Certificate2Collection filterCertificates(X509Certificate2Collection certificates, Func<X509Certificate2, bool> filter)
    {
      X509Certificate2Collection matchingCertificates = new X509Certificate2Collection();

      foreach (X509Certificate2 cert in certificates)
      {
        if (filter(cert))
          matchingCertificates.Add(cert);
      }

      return matchingCertificates;
    }

    private X509Certificate2 getSingleCertificate(X509Certificate2Collection certificates)
    {
      if ((certificates == null) || (certificates.Count < 1))
        throw new Exception("Ne najdem digitalnega potrdila / Can't find certificate");

      if (certificates.Count > 1)
        throw new Exception("Digitalno potrdilo ni edinstveno / Certificate not unique");

      return certificates[0];
    }

    private IList<X509Store> allStores()
    {
      return new List<X509Store>()
      {
        new X509Store(StoreLocation.CurrentUser),
        new X509Store(StoreLocation.LocalMachine)
      };
    }
  }
}