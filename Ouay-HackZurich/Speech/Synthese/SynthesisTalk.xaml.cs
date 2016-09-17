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
using System;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;	

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Ouay_HackZurich.Speech.Synthesis
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SynthesisTalk : Page
    {
        SpeechSynthesizer speechSynthesizer;

        public SynthesisTalk()
        {
            this.InitializeComponent();
            speechSynthesizer = new SpeechSynthesizer();

        }

        private void Speak_Click(object sender, RoutedEventArgs e)
        {
            Talk(textToSpeak.Text);
        }

        private async void Talk(string message)
        {
            var stream = await speechSynthesizer.SynthesizeTextToStreamAsync(message);
            media.SetSource(stream, stream.ContentType);
            media.Play();
        }
    }
}
