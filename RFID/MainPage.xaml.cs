using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.CommunityToolkit;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.CommunityToolkit.Extensions;
using Plugin.NFC;

namespace RFID
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            if(CrossNFC.Current.IsAvailable && CrossNFC.Current.IsEnabled)
            {
                CrossNFC.Current.OnMessageReceived += Current_OnMessageReceived;
                CrossNFC.Current.OnTagDiscovered += Current_OnTagDiscovered;
                CrossNFC.Current.OnNfcStatusChanged += Current_OnNfcStatusChanged;
            }
        }

        private async void Current_OnNfcStatusChanged(bool isEnabled)
        {
            await this.DisplayToastAsync($"NFC : {isEnabled}", 5000);
        }

        private async void Current_OnTagDiscovered(ITagInfo tagInfo, bool format)
        {
            await this.DisplayToastAsync("Data was read successfully", 5000);
        }

        private async void Current_OnMessageReceived(ITagInfo tagInfo)
        {
            await this.DisplayToastAsync(tagInfo.SerialNumber, 5000);
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            await this.DisplayToastAsync("Data persisted successfully", 5000);
        }

        async void Button_Clicked_1(System.Object sender, System.EventArgs e)
        {
            await this.DisplayToastAsync("Data was read successfully", 5000);
        }
    }
}
