// <copyright file="MainWindow.xaml.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using MNet.SLOTaxService;
using MNet.SLOTaxService.Messages;
using MNet.SLOTaxService.Services;

namespace MNet.SLOTaxGuiTest
{
  public partial class MainWindow : Window, INotifyPropertyChanged
  {
    public ObservableCollection<Tuple<string, string>> FursEndPoints { get; set; }
    public ObservableCollection<X509Certificate2> Certificates { get; set; }

    public MainWindow()
    {
      this.InitializeComponent();

      this.init();

      this.DataContext = this;
    }

    private void init()
    {
      this.FursEndPoints = new ObservableCollection<Tuple<string, string>>();
      this.FursEndPoints.Add(new Tuple<string, string>("TEST / TEST", @"https://blagajne-test.fu.gov.si:9002/v1/cash_registers"));
      this.FursEndPoints.Add(new Tuple<string, string>("Produkcija / Production", @"https://blagajne.fu.gov.si:9003/v1/cash_registers"));

      Certificate cert = new Certificate();
      this.Certificates = new ObservableCollection<X509Certificate2>();
      foreach (var certificate in cert.GetAllFursCertificates())
        this.Certificates.Add(certificate);

      this.cbType.SelectedIndex = 0;
      this.cbCertificates.SelectedIndex = 0;
    }

    private Stream getResource(string name)
    {
      return typeof(MainWindow).Assembly.GetManifestResourceStream(@"MNet.SLOTaxGuiTest.Resources." + name);
    }

    private string prettyXml(Stream stream)
    {
      using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
      {
        string wholeXML = reader.ReadToEnd();
        return this.prettyXml(wholeXML);
      }
    }

    private string prettyXml(string xml)
    {
      var stringBuilder = new StringBuilder();
      var element = XElement.Parse(xml);

      var settings = new XmlWriterSettings();
      settings.OmitXmlDeclaration = true;
      settings.Indent = true;
      settings.NewLineOnAttributes = true;

      using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
      {
        element.Save(xmlWriter);
      }

      return stringBuilder.ToString();
    }

    #region INotifyPropertyChanged Members
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler handler = this.PropertyChanged;
      if (handler != null)
        handler(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    #region EventHandlers
    private void btnExampleClick(object sender, RoutedEventArgs e)
    {
      string resourceName = null;
      if (sender == this.btnEcho) resourceName = "Echo.xml";
      else if (sender == this.btnInvoice) resourceName = "Invoice.xml";
      else if (sender == this.btnBusinessPremises) resourceName = "BusinessPremises.xml";

      if (!string.IsNullOrEmpty(resourceName))
        this.tbInput.Text = this.prettyXml(this.getResource(resourceName));
    }

    private void btnSend(object sender, RoutedEventArgs e)
    {
      if (this.Certificates.Count < 1)
      {
        MessageBox.Show("Ne najdem digitalnih potrdil! / Can't find digital certificates!");
        return;
      }

      var certificate = this.Certificates[this.cbCertificates.SelectedIndex];
      var endPoint = this.FursEndPoints[this.cbType.SelectedIndex].Item2;
      Settings settings = Settings.Create(certificate, endPoint, "http://www.fu.gov.si/");
      TaxService ts = TaxService.Create(settings);

      ReturnValue rv = ts.Send(this.tbInput.Text);
    }
    #endregion
  }
}
