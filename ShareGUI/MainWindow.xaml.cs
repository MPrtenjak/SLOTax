// <copyright file="MainWindow.xaml.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml;
using MNet.SLOTaxService;
using MNet.SLOTaxService.Messages;
using MNet.SLOTaxService.Services;

namespace MNet.SLOTaxGuiTest
{
  public partial class MainWindow : Window, INotifyPropertyChanged
  {
    public ObservableCollection<Tuple<string, string>> FursEndPoints { get; set; }
    public ObservableCollection<X509Certificate2> Certificates { get; set; }
    public ObservableCollection<string> Timeouts { get; set; }

    public MainWindow()
    {
      this.InitializeComponent();

      this.Init();

      this.DataContext = this;
    }

    private void Init()
    {
      this.FursEndPoints = new ObservableCollection<Tuple<string, string>>();
      this.FursEndPoints.Add(new Tuple<string, string>("TEST / TEST (https://blagajne-test.fu.gov.si:9002/v1/cash_registers)", @"https://blagajne-test.fu.gov.si:9002/v1/cash_registers"));
      this.FursEndPoints.Add(new Tuple<string, string>("Produkcija / Production (https://blagajne.fu.gov.si:9003/v1/cash_registers)", @"https://blagajne.fu.gov.si:9003/v1/cash_registers"));

      Certificates cert = new Certificates();
      this.Certificates = new ObservableCollection<X509Certificate2>();
      foreach (var certificate in cert.GetAllFursCertificates())
        this.Certificates.Add(certificate);

      this.Timeouts = new ObservableCollection<string>();
      for (int i = 1; i < 11; i++)
        this.Timeouts.Add(string.Format("{0} sec", i));

      this.cbType.SelectedIndex = 0;
      this.cbCertificates.SelectedIndex = 0;
      this.cbTimeout.SelectedIndex = 1;
      this.ShowExample(this.btnEcho);
      this.ShowResults(null);
    }

    private Stream GetResource(string name)
    {
      return typeof(MainWindow).Assembly.GetManifestResourceStream(@"MNet.SLOTaxGuiTest.Resources." + name);
    }

    private string PrettyXml(Stream stream)
    {
      using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
      {
        string wholeXML = reader.ReadToEnd();
        return this.PrettyXml(wholeXML);
      }
    }

    private string PrettyXml(string xmlString)
    {
      XmlDocument xmlDoc = new XmlDocument();
      xmlDoc.PreserveWhitespace = true;
      xmlDoc.LoadXml(xmlString);

      return this.PrettyXml(xmlDoc);
    }

    private string PrettyXml(XmlDocument xmlDoc)
    {
      var settings = new XmlWriterSettings();
      settings.OmitXmlDeclaration = false;
      settings.Indent = true;
      settings.NewLineOnAttributes = true;

      var stringBuilder = new StringBuilder();
      using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
      {
        xmlDoc.Save(xmlWriter);
      }

      return stringBuilder.ToString();
    }

    private void ShowExample(object sender)
    {
      string resourceName = null;
      if (sender == this.btnEcho) resourceName = "Echo.xml";
      else if (sender == this.btnInvoice) resourceName = "Invoice.xml";
      else if (sender == this.btnBusinessPremises) resourceName = "BusinessPremises.xml";

      if (!string.IsNullOrEmpty(resourceName))
        this.tbInput.Text = this.PrettyXml(this.GetResource(resourceName));
    }

    private void ExecuteSend()
    {
      if (this.Certificates.Count < 1)
      {
        MessageBox.Show("Ne najdem digitalnih potrdil! / Can't find digital certificates!");
        return;
      }

      var certificate = this.Certificates[this.cbCertificates.SelectedIndex];
      var endPoint = this.FursEndPoints[this.cbType.SelectedIndex].Item2;
      Settings settings = Settings.Create(certificate, endPoint, "http://www.fu.gov.si/", this.cbTimeout.SelectedIndex + 1);
      TaxService ts = TaxService.Create(settings);

      ReturnValue rv = ts.Send(this.tbInput.Text);
      this.ProcesReturnValue(rv);
    }

    private void ProcesReturnValue(ReturnValue rv)
    {
      if (rv.MessageSendToFurs != null) this.tbToFurs.Text = this.PrettyXml(rv.MessageSendToFurs);
      if (rv.MessageReceivedFromFurs != null) this.tbFromFurs.Text = this.PrettyXml(rv.MessageReceivedFromFurs);

      this.tbError.Text = rv.ErrorMessage;
      this.tbEOR.Text = rv.UniqueInvoiceID;
      this.tbZOI.Text = rv.ProtectedID;
      this.tbBarcode.Text = string.Empty;

      if (rv.BarCodes != null)
      {
        this.tbBarcode.Text = rv.BarCodes.BarCodeValue;

        //// in xaml this is better option, but with rv.BarCodes.DrawQRCode, the usage case of the library is clearer
        //// this.imgBarcode.Text = this.tbBarcode.Text;

        Image img = rv.BarCodes.DrawQRCode(180, ImageFormat.Png);
        this.imgBarcode.Source = this.ConvertDrawingImageToWPFImage(img);
      }

      this.ShowResults(rv);
    }

    private System.Windows.Media.ImageSource ConvertDrawingImageToWPFImage(System.Drawing.Image gdiImg)
    {
      System.Windows.Controls.Image img = new System.Windows.Controls.Image();

      System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(gdiImg);
      IntPtr hBitmap = bmp.GetHbitmap();
      return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
    }

    private void ShowResults(ReturnValue rv)
    {
      this.pnlSuccess.Visibility = ((rv == null) || (!rv.Success)) ? Visibility.Collapsed : Visibility.Visible;
      this.pnlError.Visibility = ((rv == null) || (string.IsNullOrEmpty(rv.ErrorMessage))) ? Visibility.Collapsed : Visibility.Visible;
      this.pnlEOR.Visibility = ((rv == null) || (string.IsNullOrEmpty(rv.UniqueInvoiceID))) ? Visibility.Collapsed : Visibility.Visible;
      this.pnlZOI.Visibility = ((rv == null) || (string.IsNullOrEmpty(rv.ProtectedID))) ? Visibility.Collapsed : Visibility.Visible;
      this.pnlBarcode.Visibility = ((rv == null) || (rv.BarCodes == null)) ? Visibility.Collapsed : Visibility.Visible;
      this.pnlBarcode1.Visibility = this.pnlBarcode.Visibility;

      this.pnlResult.Visibility = ((this.pnlSuccess.Visibility == Visibility.Visible) ||
                                   (this.pnlError.Visibility == Visibility.Visible) ||
                                   (this.pnlEOR.Visibility == Visibility.Visible) ||
                                   (this.pnlZOI.Visibility == Visibility.Visible) ||
                                   (this.pnlBarcode.Visibility == Visibility.Visible)) ? Visibility.Visible : Visibility.Collapsed;
    }

    private void OpenFile()
    {
      Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

      dlg.DefaultExt = ".XML";
      dlg.Filter = "XML Files (*.xml)|*.xml";

      bool? result = dlg.ShowDialog();
      if ((result.HasValue) && (result.Value))
      {
        string[] lines = File.ReadAllLines(dlg.FileName);
        this.tbInput.Text = string.Join("\n", lines);
      }
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler handler = this.PropertyChanged;
      if (handler != null)
        handler(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion INotifyPropertyChanged Members

    #region EventHandlers

    private void btnExampleClick(object sender, RoutedEventArgs e)
    {
      this.ShowExample(sender);
    }

    private void btnSend(object sender, RoutedEventArgs e)
    {
      this.ExecuteSend();
    }

    private void btnOpen_Click(object sender, RoutedEventArgs e)
    {
      this.OpenFile();
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
      e.Handled = true;
    }

    #endregion EventHandlers
  }
}