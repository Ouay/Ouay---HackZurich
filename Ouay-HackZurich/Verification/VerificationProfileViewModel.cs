using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;

namespace Ouay_HackZurich.Verification
{
	class VerificationProfileViewModel : ViewModelBase
	{
		public VerificationProfileViewModel(OxfordSpeakerIdRestClient oxfordRestClient)
		{
			this.oxfordRestClient = oxfordRestClient;

			this.enrolCommand = new SimpleCommand(this.OnEnrolCommand, false);

			this.verifyCommand = new SimpleCommand(this.OnVerifyCommand, false);
		}

		public VerificationProfile Profile
		{
			get { return (this.profile); }
			set { base.SetProperty(ref this.profile, value); this.EnableEnrolCommand(); this.EnableVerifyCommand(); }
		}

		void EnableEnrolCommand()
		{
			this.enrolCommand.Enable(this.profile.EnrollmentsCount < 3);
		}

		void EnableVerifyCommand()
		{
			this.verifyCommand.Enable(this.profile.EnrollmentsCount >= 3);
		}

		async Task OnVerifyOrEnrolCommandAsync(Func<IInputStream, MessageDialog, Task> innerAction)
		{
			var phrase = await VerificationPhraseList.GetVerificationPhraseForProfileAsync(
			  this.profile);

			// lazy, throwing up a dialog from here. Really!?!?!
			var dialog = new MessageDialog(
			  $"click the OK button to start the recorder",
			  "recording");

			UICommand command = new UICommand("OK");

			dialog.Commands.Add(command);

			if ((await dialog.ShowAsync()) == command)
			{
				var audioRecorder = new AudioRecorder();

				var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(
				  Guid.NewGuid().ToString());

				await audioRecorder.StartRecordToFileAsync(file);

				dialog.Content = $"Say the phrase \"{phrase.Phrase}\" then click OK when you've finished";

				await dialog.ShowAsync();

				await audioRecorder.StopRecordAsync();

				// Ok, we now have a file full of audio to send to the service.
				using (var stream = await file.OpenReadAsync())
				{
					try
					{
						await innerAction(stream, dialog);
					}
					catch (Exception ex)
					{
						await this.ShowErrorAsync(ex.Message);
					}
				}
			}
		}

		async void OnEnrolCommand()
		{
			await this.OnVerifyOrEnrolCommandAsync(
			  async (stream, dialog) =>
			  {
				  try
				  {
					  var result = await this.oxfordRestClient.EnrollAsync(this.profile, stream);

					  this.profile.EnrollmentStatus = result.EnrollmentStatus;
					  this.profile.EnrollmentsCount = result.EnrollmentsCount;
					  this.profile.RemainingEnrollmentsCount = result.RemainingEnrollments;

					  this.EnableEnrolCommand();
					  this.EnableVerifyCommand();

					  dialog.Title = "Recognised";
					  dialog.Content = $"The service heard you say [{result.Phrase}]";
					  await dialog.ShowAsync();
				  }
				  catch (Exception ex)
				  {
					  await this.ShowErrorAsync(ex.Message);
				  }
			  }
			);
		}

		async void OnVerifyCommand()
		{
			await this.OnVerifyOrEnrolCommandAsync(
			  async (stream, dialog) =>
			  {
				  try
				  {
					  var result = await this.oxfordRestClient.VerifyAsync(this.profile, stream);

					  dialog.Title = $"Speech {result.Result}";
					  dialog.Content = $"The service heard you say [{result.Phrase}] with {result.Confidence} confidence";
					  await dialog.ShowAsync();
				  }
				  catch (Exception ex)
				  {
					  await this.ShowErrorAsync(ex.Message);
				  }
			  }
			);
		}

		async Task ShowErrorAsync(string error)
		{
			var dialog = new MessageDialog(error, "failed");
			await dialog.ShowAsync();
		}

		public ICommand EnrolCommand { get { return (this.enrolCommand); } }

		public ICommand VerifyCommand { get { return (this.verifyCommand); } }

		OxfordSpeakerIdRestClient oxfordRestClient;
		VerificationProfile profile;
		SimpleCommand enrolCommand;
		SimpleCommand verifyCommand;
	}
}