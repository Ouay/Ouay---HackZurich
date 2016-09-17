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

		public async Task<bool> Talk(string message)
		{
			try
			{
				var stream = await speechSynthesizer.SynthesizeTextToStreamAsync(message);
				ME.SetSource(stream, stream.ContentType);
				ME.Play();
				return true;
			}
			catch(Exception ex)
			{
				ex.ToString();
				return false;
			}
		}

        string[] answers = { "Welcome back", "Hey, you", "good evening", "hello" };
        string[] byebye = { "Have a nice day", "See you later", "I will miss you", "bye bye" };
        int variable = 0, variable2 = 0;
        public async Task<bool?> WelcomeMessage()
        {
            await Talk(answers[variable]);
            if (variable < 3) { variable++; }
            else { variable = 0; }
			return null;
        }

        public async Task<bool?> byeMessage()
        {
            await Talk(byebye[variable2]);
            if (variable2 < 3) { variable2++; }
            else { variable2 = 0; }
			return null;
        }
	}
}
