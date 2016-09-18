using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Core;
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
			await ME.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
			 {
				 try
				 {
					 var stream = await speechSynthesizer.SynthesizeTextToStreamAsync(message);

					 ME.SetSource(stream, stream.ContentType);
					 ME.Play();
				 }
				 catch (Exception ex)
				 {
					 ex.ToString();
				 }
			 });
		}

        string[] answers = { "Welcome back", "Hey, you", "good evening", "hello" };
        string[] byebye = { "Have a nice day", "See you later", "I will miss you", "bye bye" };
		string[] dangers = { "It's rainy today so don't forget your umbrella, byebye", "It's very hot today, don't foret to take a water bottle, have a nice day.", "It's freezing outside so watch out for black ice, see you later."};

		public async Task<bool?> WelcomeMessage()
		{
			Talk(answers[Rand(answers.Length)]);
			return null;
        }

		private int Rand(int max)
		{
			Random r = new Random();
			return r.Next(max);
		}

		public async Task<bool?> byeMessage()
        {
			Talk(dangers[Rand(dangers.Length)]);
			//Talk(byebye[Rand(byebye.Length)]);
					
			return null;
        }
	}
}
