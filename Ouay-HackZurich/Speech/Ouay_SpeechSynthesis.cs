using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;

namespace Ouay_HackZurich.Speech
{
	class Ouay_SpeechSynthesis
	{
		private SpeechSynthesizer speechSynthesizer;
		private MediaElement ME;

		public Ouay_SpeechSynthesis(MediaElement ME)
			{
			speechSynthesizer = new SpeechSynthesizer();
			this.ME = ME;
		}

		public async void Talk(string message)
			{
				var stream = await speechSynthesizer.SynthesizeTextToStreamAsync(message);
				ME.SetSource(stream, stream.ContentType);
				ME.Play();
			}
	}
}
