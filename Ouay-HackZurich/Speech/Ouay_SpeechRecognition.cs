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

	// Make event handler for speech recognition events
	public delegate void SpeechRecognitionEventHandler(object source, SREventArgs e);

	// special event argument class
	public class SREventArgs : EventArgs
	{
		private int EventInfo;
		public SREventArgs(int value)
		{
			EventInfo = value;
		}
		public int GetInfo()
		{
			return EventInfo;
		}
	}

	class Ouay_SpeechRecognition
	{
		/// <summary>
		/// Speech Recogniser
		/// </summary>
		private SpeechRecognizer _speechRecognizer;

		/// <summary>
		/// Boolean that represent the state of Listening of the System
		/// </summary>
		private bool isListening = false;

		// One event for each kind of outcome
		public event SpeechRecognitionEventHandler OnEnterResult;
		public event SpeechRecognitionEventHandler OnExitResult;

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
			Debug.WriteLine("Speech setup completed");
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

			var storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Speech/SRGS/SRGS_Main.xml"));
			var fileConstraint = new SpeechRecognitionGrammarFileConstraint(storageFile, "ExitEnter");


			_speechRecognizer.Constraints.Add(fileConstraint);
			await _speechRecognizer.CompileConstraintsAsync();

			_speechRecognizer.ContinuousRecognitionSession.Completed += ContinuousRecognitionSession_Completed;
			_speechRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;

			isListening = true;
			Debug.WriteLine("Speech Recognizer intialization completed");

		}

		/// <summary>
		/// Restart continuous recognition session by itself when completed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private async void ContinuousRecognitionSession_Completed(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionCompletedEventArgs args)
		{
			Debug.WriteLine("Speech recognition completed");
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
			Debug.WriteLine("Speech recognition result generated.");
			if (args.Result.Confidence == SpeechRecognitionConfidence.Medium || args.Result.Confidence == SpeechRecognitionConfidence.High)
			{
				Debug.WriteLine("Heard: " + args.Result.Text);
			}
			else
			{
				Debug.WriteLine("I didn't get that, but I  did hear: "+ args.Result.Text);
			}
			HandleSpeech(args);
		}

		/// <summary>
		/// Handle any good Speech input
		/// </summary>
		/// <param name="args">data from the speech recognition</param>
		private void HandleSpeech(SpeechContinuousRecognitionResultGeneratedEventArgs args)
		{
			
			string actionCase = args.Result.SemanticInterpretation.Properties["case"][0];

			Debug.WriteLine("Detected case: " + actionCase);

			if (actionCase == "exit")
			{
				string hours = args.Result.SemanticInterpretation.Properties["hours"][0];
				Debug.WriteLine("Time out of home: " + hours + " hours.");

				OnExitResult?.Invoke(this, new SREventArgs(Int32.Parse(hours)));
			}
			else if (actionCase == "enter")
			{
				OnEnterResult?.Invoke(this, new SREventArgs(0));	
			}
		}
	}
}
