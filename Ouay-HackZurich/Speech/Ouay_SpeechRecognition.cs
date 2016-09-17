using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;
using Windows.UI.Core;

namespace Ouay_HackZurich.Speech
{
	class Ouay_SpeechRecognition
	{
		private SpeechRecognizer _speechRecognizer;
		private SpeechRecognitionResult _speechResult;
		/// <summary>
		/// Boolean that represent the state of Listening of the System
		/// </summary>
		public bool isListening = false;


		/// <summary>
		/// Constructor
		/// </summary>
		public Ouay_SpeechRecognition()
		{
			setupSpeechRecognition();
		}


		private async void setupSpeechRecognition()
		{

			var permissionGained = await Permissions.MicrophonePermissions.RequestMicrophonePermission();

			if (!permissionGained)
				return;

			await InitializeRecognizer(SpeechRecognizer.SystemSpeechLanguage);
			await _speechRecognizer.ContinuousRecognitionSession.StartAsync();
		}


		/// <summary>
		/// Initialise a recognition system with a specified language
		/// </summary>
		/// <param name="systemSpeechLanguage"></param>
		/// <returns></returns>

		private async Task InitializeRecognizer(Language systemSpeechLanguage)

		{

			if (_speechRecognizer != null)

			{

				_speechRecognizer.ContinuousRecognitionSession.Completed -= ContinuousRecognitionSession_Completed;

				_speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;

				_speechRecognizer.Dispose();

				_speechRecognizer = null;

			}

			_speechRecognizer = new SpeechRecognizer(systemSpeechLanguage);

			var storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Speech/SRGS/SRGS_Test.xml"));
			var listConstraint = new SpeechRecognitionGrammarFileConstraint(storageFile, "ExitEnter");


			_speechRecognizer.Constraints.Add(listConstraint);
			await _speechRecognizer.CompileConstraintsAsync();
			_speechRecognizer.ContinuousRecognitionSession.Completed += ContinuousRecognitionSession_Completed;
			_speechRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;

			isListening = true;

		}

		/// <summary>
		/// Restart continuous recognition session by itself when completed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private async void ContinuousRecognitionSession_Completed(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionCompletedEventArgs args)
		{
			if (_speechRecognizer.State == SpeechRecognizerState.Idle)
			{
				await _speechRecognizer.ContinuousRecognitionSession.StartAsync();
				Debug.WriteLine("Starting speech recognition.");
			}
		}

		/// <summary>
		/// Result generated when recognize a constraint
		/// </summary>
		/// <param name="sender">Session</param>
		/// <param name="args">Result</param>

		private void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
		{
			if (args.Result.Confidence == SpeechRecognitionConfidence.Medium || args.Result.Confidence == SpeechRecognitionConfidence.High)
			{
				Debug.WriteLine("Heard: " + args.Result.Text);
			}
			else
			{
				Debug.WriteLine("I didn't get that. I heard: "+ args.Result.Text);
			}
		}

	}
}
