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
		public const string ALERT_TITLE = "NFC";
		public const string MIME_TYPE = "application/com.companyname.nfcsample";

		NFCNdefTypeFormat _type;
		bool _makeReadOnly = false;
		bool _eventsAlreadySubscribed = false;
		bool _isDeviceiOS = false;

		public MainPage()
        {
            InitializeComponent();

            if(CrossNFC.Current.IsAvailable && CrossNFC.Current.IsEnabled)
            {
                CrossNFC.Current.StartListening();

                CrossNFC.Current.OnMessageReceived += Current_OnMessageReceived;
                CrossNFC.Current.OnTagDiscovered += Current_OnTagDiscovered;
                CrossNFC.Current.OnNfcStatusChanged += Current_OnNfcStatusChanged;
            }
        }

		/// <summary>
		/// Event raised when data has been published on the tag
		/// </summary>
		/// <param name="tagInfo">Published <see cref="ITagInfo"/></param>
		async void Current_OnMessagePublished(ITagInfo tagInfo)
		{
			try
			{
				//ChkReadOnly.IsChecked = false;
				CrossNFC.Current.StopPublishing();
				if (tagInfo.IsEmpty)
					await this.DisplayToastAsync("Formatting tag operation successful");
				else
					await this.DisplayToastAsync("Writing tag operation successful");
			}
			catch (Exception ex)
			{
				await this.DisplayToastAsync(ex.Message);
			}
		}

		private async void Current_OnNfcStatusChanged(bool isEnabled)
        {
            await this.DisplayToastAsync($"NFC : {isEnabled}", 5000);
        }

        private async void Current_OnMessageReceived(ITagInfo tagInfo)
        {
            await this.DisplayToastAsync(tagInfo.SerialNumber, 5000);
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            CrossNFC.Current.StartPublishing();
            CrossNFC.Current.PublishMessage(new TagInfo()
            {
                
            });
            await this.DisplayToastAsync("Data persisted successfully", 5000);
        }

        async void Button_Clicked_1(System.Object sender, System.EventArgs e)
        {
            await this.DisplayToastAsync("Data was read successfully", 5000);
        }


		/// <summary>
		/// Event raised when a NFC Tag is discovered
		/// </summary>
		/// <param name="tagInfo"><see cref="ITagInfo"/> to be published</param>
		/// <param name="format">Format the tag</param>
		async void Current_OnTagDiscovered(ITagInfo tagInfo, bool format)
		{
			if (!CrossNFC.Current.IsWritingTagSupported)
			{
				await this.DisplayToastAsync("Writing tag is not supported on this device");
				return;
			}

			try
			{
				NFCNdefRecord record = null;
				switch (_type)
				{
					case NFCNdefTypeFormat.WellKnown:
						record = new NFCNdefRecord
						{
							TypeFormat = NFCNdefTypeFormat.WellKnown,
							MimeType = MIME_TYPE,
							Payload = NFCUtils.EncodeToByteArray("Plugin.NFC is awesome!"),
							LanguageCode = "en"
						};
						break;
					case NFCNdefTypeFormat.Uri:
						record = new NFCNdefRecord
						{
							TypeFormat = NFCNdefTypeFormat.Uri,
							Payload = NFCUtils.EncodeToByteArray("https://github.com/franckbour/Plugin.NFC")
						};
						break;
					case NFCNdefTypeFormat.Mime:
						record = new NFCNdefRecord
						{
							TypeFormat = NFCNdefTypeFormat.Mime,
							MimeType = MIME_TYPE,
							Payload = NFCUtils.EncodeToByteArray("Plugin.NFC is awesome!")
						};
						break;
					default:
						break;
				}

				if (!format && record == null)
					throw new Exception("Record can't be null.");

				tagInfo.Records = new[] { record };

				if (format)
					CrossNFC.Current.ClearMessage(tagInfo);
				else
				{
					CrossNFC.Current.PublishMessage(tagInfo, _makeReadOnly);
				}
			}
			catch (Exception ex)
			{
				await this.DisplayToastAsync(ex.Message);
			}
		}
	}
}
