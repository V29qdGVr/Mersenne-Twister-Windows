using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MersenneTwister
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void GenerateAndSaveToFileAsync(uint seed)
        {
            MersenneTwister mersenneTwister = new MersenneTwister(seed);
            byte[] tenMillionBytes = new byte[10000000];

            for (uint i = 0; i < 2500000; i++)
            {
                uint generatedInteger = mersenneTwister.Random();
                uint arrayIndex = i * 4;
                this.InsertIntegerIntoByteArray(tenMillionBytes, generatedInteger, arrayIndex);

                uint bytesGenerated = (i + 1) * 4;
                if (bytesGenerated % 1000000 == 0)
                {
                    Debug.WriteLine(bytesGenerated + " bytes generated");
                }
            }

            FileSavePicker picker = new FileSavePicker();
            picker.FileTypeChoices.Add("Binary file", new List<string> {".bin"});
            picker.SuggestedFileName = "output";
            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                await FileIO.WriteBytesAsync(file, tenMillionBytes);
                this.ShowToastNotification("MersenneTwister", "The file is ready!");
            }
        }

        private void InsertIntegerIntoByteArray(byte[] byteArray, uint integer, uint index)
        {
            byte[] integerBytes = BitConverter.GetBytes(integer);
            byteArray[index + 0] = integerBytes[3];
            byteArray[index + 1] = integerBytes[2];
            byteArray[index + 2] = integerBytes[1];
            byteArray[index + 3] = integerBytes[0];
        }

        private void ShowToastNotification(string title, string stringContent)
        {
            ToastNotifier ToastNotifier = ToastNotificationManager.CreateToastNotifier();
            Windows.Data.Xml.Dom.XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            Windows.Data.Xml.Dom.XmlNodeList toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode(title));
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(stringContent));
            Windows.Data.Xml.Dom.IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            Windows.Data.Xml.Dom.XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.SMS");

            ToastNotification toast = new ToastNotification(toastXml);
            toast.ExpirationTime = DateTime.Now.AddSeconds(3);
            ToastNotifier.Show(toast);
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Button button = sender as Button;
            var textBox = (button.Parent as StackPanel).Children[2] as TextBox;
            uint seed = (uint)Int32.Parse(textBox.Text);
            this.GenerateAndSaveToFileAsync(seed);
        }
    }
}
