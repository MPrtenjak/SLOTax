// <copyright file="PasswordBox.xaml.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System.Windows;

namespace MNet.SLOTaxGuiTest
{
  public partial class PasswordBox : Window
  {
    public string Password { get; set; }
    public bool Commit { get; set; }

    public PasswordBox()
    {
      this.InitializeComponent();
      this.Commit = false;

      this.passwordBox.Focus();
    }

    private void btnCancel(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void btnOK(object sender, RoutedEventArgs e)
    {
      this.Commit = true;
      this.Password = this.passwordBox.Password;
      this.Close();
    }
  }
}
