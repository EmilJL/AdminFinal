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
using InfoScreenAdminBusiness;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InfoScreenAdminGUI
{
    public enum ContentDialogResult
    {
        Ok,
        Fail,
        Cancel,
        Nothing
    }
    public sealed partial class NewUser : ContentDialog
    {
        private string desiredName;
        private string desiredPassword;

        public string DesiredName { get { return desiredName; } private set { desiredName = value; } }
        public string DesiredPassword { get { return desiredPassword; } private set { desiredPassword = value; } }
        public ContentDialogResult Result { get; private set; }
        public ContentDialog cd { get; private set; }
        public NewUser()
        {
            this.InitializeComponent();
            Result = ContentDialogResult.Nothing;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if(TBoxPassword.Password == TBoxConfirmPassword.Password)
            {
                desiredName = TBoxUsername.Text;
                desiredPassword = TBoxPassword.Password;
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
