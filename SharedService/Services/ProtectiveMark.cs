// <copyright file="ProtectiveMark.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;
using System.Xml;
using MNet.SLOTaxService.Utils;

namespace MNet.SLOTaxService.Services
{
  internal class ProtectiveMark
  {
    public string CalculateMD5Hash(byte[] input)
    {
      byte[] data = this.md5Hash.ComputeHash(input);

      StringBuilder sBuilder = new StringBuilder();
      for (int i = 0; i < data.Length; i++)
        sBuilder.Append(data[i].ToString("x2"));

      return sBuilder.ToString();
    }

    public string Calculate(string input, RSACryptoServiceProvider provider)
    {
      byte[] podatki = Encoding.ASCII.GetBytes(input);
      byte[] signature = provider.SignData(podatki, CryptoConfig.MapNameToOID("SHA256"));

      return this.CalculateMD5Hash(signature);
    }

    public string Calculate(XmlElement invoice, RSACryptoServiceProvider provider)
    {
      StringBuilder fullText = new StringBuilder(200);

      fullText.Append(this.getNodeValue(invoice, "fu:TaxNumber"));
      fullText.Append(this.getNodeValue(invoice, "fu:IssueDateTime"));
      fullText.Append(this.getNodeValue(invoice, "fu:InvoiceNumber"));
      fullText.Append(this.getNodeValue(invoice, "fu:BusinessPremiseID"));
      fullText.Append(this.getNodeValue(invoice, "fu:ElectronicDeviceID"));
      fullText.Append(this.getNodeValue(invoice, "fu:InvoiceAmount"));

      return this.Calculate(fullText.ToString(), provider);
    }

    private string getNodeValue(XmlElement parentElement, string nodeName)
    {
      XmlNode node = XmlHelperFunctions.GetFirstSubNode(parentElement, nodeName);
      if (node == null) return string.Empty;

      return node.InnerText;
    }

    private MD5 md5Hash = MD5.Create();
  }
}