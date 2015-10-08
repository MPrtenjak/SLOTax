// <copyright file="MainWindow.xaml.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using MNet.SLOTaxService.Services;

namespace SLOTaxGuiTest
{
  public partial class MainWindow : Window, INotifyPropertyChanged
  {
    public ObservableCollection<X509Certificate2> certificates { get; set; }

    public MainWindow()
    {
      this.InitializeComponent();

      this.init();
    }

    private void init()
    {
      Certificate cert = new Certificate();

      this.certificates = new ObservableCollection<X509Certificate2>();
      foreach (var certificate in cert.GetAllFursCertificates())
      {
        this.certificates.Add(certificate);
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
    #endregion
  }
}
