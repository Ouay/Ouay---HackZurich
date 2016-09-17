using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using CognitiveAPIWrapper.Audio;
using CognitiveAPIWrapper.SpeakerVerification;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.Storage;
using Ouay_HackZurich.BlueMix;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Ouay_HackZurich
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		static readonly string cognitiveApiKey = "9fad28eeac5f4a19a39d39824158d801";
		
		public MainPage()
		{
			this.InitializeComponent();
			Setup();
		}

		private async void Setup()
		{
			List<StdTime> list = await BlueMixCom.GetNormalRange();
			bool a = await BlueMixCom.SendEntrance(DateTime.Now);
		}



//		#region SpeakerVerificationBrocken
//		async void OnClearAllAsync(object sender, RoutedEventArgs e)
//		{
//			VerificationClient client = new VerificationClient(cognitiveApiKey);

//			var profiles = await client.GetVerificationProfilesAsync();

//			foreach (var profile in profiles)
//			{
//				await client.RemoveVerificationProfileAsync(profile.VerificationProfileId);
//			}
//		}
//		async void OnGetRandomPhraseAsync(object sender, RoutedEventArgs e)
//		{
//			// VerificationClient is my wrapper for the verification REST API.
//			// It needs my Cognitive speaker recognition API key in order to work.
//			VerificationClient verificationClient = new VerificationClient(cognitiveApiKey);

//			// This calls the 'list all supported verification phrases' REST API
//			// and then simply chooses one of the return phrases at random
//			string randomlySelectedVerificationPhrase =
//			  await verificationClient.GetRandomVerificationPhraseAsync();

//			// Display that phrase back in the UI.
//			this.txtPhrase.Text = randomlySelectedVerificationPhrase;
//		}
//		async void OnEnrollAsync(object sender, RoutedEventArgs e)
//		{
//			// VerificationClient is my wrapper for the verification REST API.
//			// It needs my Cognitive speaker recognition API key in order to work.
//			VerificationClient verificationClient = new VerificationClient(cognitiveApiKey);

//			// This calls the 'create profile' REST API and returns the GUID of the
//			// new profile.
//			Guid profileId = await verificationClient.AddVerificationProfileAsync();

//			// Display the profile ID in the UI.
//			this.txtProfileId.Text = profileId.ToString();

//			bool enrolled = false;
//			int i = 3;
//			do
//			{
//				textToSay.Text = "Still " + i + " Enrollement";

//				// Wrapper class which uses AudioGraph to record audio to a file over a specified
//				// period of time.
//				StorageFile recordedAudioFile =
//				  await CognitiveAudioGraphRecorder.RecordToTemporaryFileAsync(TimeSpan.FromSeconds(10));

//				// This calls the 'create enrollment' API with the speech stream and 
//				// decodes the returned JSON.
//				VerificationEnrollmentResult result =
//				  await verificationClient.EnrollRecordedSpeechForProfileIdAsync(
//					profileId, recordedAudioFile);

//				// Get rid of the recorded speech.
//				await recordedAudioFile.DeleteAsync();

//				// Do we need to do more enrollments? Note - this check is probably
//				// over-simplistic.
//				enrolled = (result.RemainingEnrollments == 0);
//				if (i > result.RemainingEnrollments)
//					textToSay.Text += " Encore";
//				i = result.RemainingEnrollments;
//			} while (!enrolled);
//			textToSay.Text = "End of Enrollement";
//		}
//		async void OnVerifyAsync(object sender, RoutedEventArgs e)
//		{
//			// Take the user's profile ID back from the UI as we haven't stored
//			// it anywhere.
//			Guid profileId = Guid.Parse(this.txtProfileId.Text);

//			// Prompt the user to speak.
//			ConfirmMessageAsync("Speak");

//			// Wrapper class which uses AudioGraph to record audio to a file over a specified
//			// period of time.
//			StorageFile recordedFile =
//			  await CognitiveAudioGraphRecorder.RecordToTemporaryFileAsync(
//				TimeSpan.FromSeconds(10));

//			// VerificationClient is my wrapper for the verification REST API.
//			// It needs my Cognitive speaker recognition API key in order to work.
//			VerificationClient verificationClient = new VerificationClient(
//			  cognitiveApiKey);

//			VerificationResult result =
//			  await verificationClient.VerifyRecordedSpeechForProfileIdAsync(
//				profileId, recordedFile);

//			// Get rid of the recorded audio file.
//			await recordedFile.DeleteAsync();

//			ConfirmMessageAsync(
//			  $"Your speech was {result.Result}ed with {result.Confidence} confidence");
//		}
//		void ConfirmMessageAsync(string text)
//		{
//			textToSay.Text = text;
//		}
//#endregion
	}
}
