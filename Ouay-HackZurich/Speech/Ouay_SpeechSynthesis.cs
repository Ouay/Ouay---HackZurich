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
			switch (Weather.Weather.isNormal(await Weather.Weather.GetCurrent()))
			{
				case 1:
					Talk("It's very hot today, don't forget your water bottle.");
					break;

				case 2:
					Talk("It's very cold today, take it easy on the road.");
					break;

				case 3:
					Talk("It's very windy today, pay attention to flying cats.");
					break;

				default:
					Talk(byebye[Rand(answers.Length)]);
					break;
			}
			return null;
        }
	}
}
