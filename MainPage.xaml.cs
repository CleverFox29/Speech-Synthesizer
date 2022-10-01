using System;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using ProgressBar = Microsoft.UI.Xaml.Controls.ProgressBar;

namespace Speech_Synthesizer
{
    public sealed partial class MainPage : Page
    {
        private static readonly SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        ContentDialog dia = new ContentDialog();
        public MainPage()
        {
            this.InitializeComponent();
            EnableCheck();
            speak.Content = "▶️ Play";
            pitch.Value = 100;
            volume.Value = 100;
            speakingRate.Value = 100;
            EnableCheck();
            
            VoiceInitialiser();
        }
        private void Speak_Click(object sender, RoutedEventArgs e)
        {
            switch (speak.Content)
            {
                case "▶️ Play":
                    speak.Content = "⏹️ Stop";
                    DisableAll();
                    Message("Synthesizing",null);
                    Talk(Speech.Text);
                    break;
                default:
                    media.Stop();
                    EnableAll();
                    speak.Content = "▶️ Play";
                    break;
            }

        }
        private async void Talk(string message)
        {
            SpeechSynthesisStream stream = await synthesizer.SynthesizeTextToStreamAsync(message);
            media.SetSource(stream, stream.ContentType);
            media.Play();
            dia.Hide();
        }
        private void VoiceInitialiser()
        {
            Voice.Items.Clear();

            foreach (VoiceInformation voice in SpeechSynthesizer.AllVoices)
            {
                ComboBoxItem item = new ComboBoxItem()
                {
                    Name = voice.DisplayName,
                    Tag = voice,
                    Content = voice.DisplayName + " (Language: " + voice.Language + ", Gender: "+voice.Gender+")"
                };
                Voice.Items.Add(item);
            }
            Voice.SelectedIndex = 0;
            EnableCheck();
        }
        private void Speech_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableCheck();
        }
        private void EnableCheck()
        {
            if ((Speech.Text.Trim() != "")
                && (Voice.SelectedIndex != -1))
            {
                speak.IsEnabled = true;
                save.IsEnabled = true;
            }
            else
            {
                speak.IsEnabled = false;
                save.IsEnabled = false;
            }
        }
        private void Voice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Voice.SelectedIndex != -1)
                synthesizer.Voice = ((ComboBoxItem)Voice.SelectedItem).Tag as VoiceInformation;
            EnableCheck();
        }
        private void Media_MediaEnded(object sender, RoutedEventArgs e)
        {
            media.Stop();
            EnableAll();
            speak.Content = "▶️ Play";
        }
        private void Pitch_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            synthesizer.Options.AudioPitch = pitch.Value / 100.0;
        }
        private void Volume_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            synthesizer.Options.AudioVolume = volume.Value / 100.0;
        }
        private void SpeakingRate_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            synthesizer.Options.SpeakingRate = speakingRate.Value / 100.0;
        }
        private  void DisableAll()
        {
            Speech.IsEnabled = false;
            Voice.IsEnabled = false;
            pitch.IsEnabled = false;
            volume.IsEnabled = false;
            speakingRate.IsEnabled = false;
        }
        private  void EnableAll()
        {
            Speech.IsEnabled = true;
            Voice.IsEnabled = true;
            pitch.IsEnabled = true;
            volume.IsEnabled = true;
            speakingRate.IsEnabled = true;
        }
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.Desktop
            };
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                Message("Creating...", null);
                try
                {
                    StorageFile file = await folder.CreateFileAsync("Narration.mp3", CreationCollisionOption.GenerateUniqueName);
                    SpeechSynthesisStream stream = await synthesizer.SynthesizeTextToStreamAsync(Speech.Text);
                    using (var reader = new DataReader(stream))
                    {
                        await reader.LoadAsync((uint)stream.Size);
                        IBuffer buffer = reader.ReadBuffer((uint)stream.Size);
                        await FileIO.WriteBufferAsync(file, buffer);
                    }
                    Message("Done", "File created in " + folder.Path);
                }
                catch (Exception x)
                {
                    Message("Error occured", x.ToString());
                }
            }
        }
        private async void Message(string s, string x)
        {
            if (VisualTreeHelper.GetOpenPopups(Window.Current).Count > 0)
                dia.Hide();

            dia = new ContentDialog();
            if (x == null)
            {
                dia.Title = s; 
                ProgressBar progressBar = new ProgressBar();
                progressBar.IsIndeterminate = true;
                dia.Content = progressBar;
            }
            else
            {
                dia.Title = s;
                dia.Content = x;
                dia.PrimaryButtonText = "OK";
            }

            
            await dia.ShowAsync();

        }
    }
}

