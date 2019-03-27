using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InfoScreenAdminGUI
{
    public sealed partial class UserLogIn : ContentDialog
    {
        public string InputUsername { get; private set; }
        public string InputPassword { get; private set; }
        public string ConsoleOutput { get; private set; }
        public ContentDialogResult Result { get; private set; }
        public UserLogIn()
        {
            this.InitializeComponent();
            Result = ContentDialogResult.Nothing;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if(string.IsNullOrWhiteSpace(InputUsername) && string.IsNullOrWhiteSpace(InputPassword))
            {
                InputUsername = TBoxUsername.Text;
                InputPassword = TBoxPassword.Password;
                Result = ContentDialogResult.Ok;
            } else
            {
                Result = ContentDialogResult.Fail;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Result = ContentDialogResult.Cancel;
        }
    }
}
