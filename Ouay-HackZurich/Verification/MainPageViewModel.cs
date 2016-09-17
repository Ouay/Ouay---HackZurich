using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;

namespace Ouay_HackZurich.Verification
{
	// lazy not injecting this from a container.
	class MainPageViewModel : ViewModelBase
	{
		public MainPageViewModel()
		{
			this.verificationProfiles = new ObservableCollection<VerificationProfileViewModel>();

			this.getVerificationProfilesCommand =
			  new SimpleCommand(this.OnGetVerificationProfilesCommand);

			this.addNewProfileCommand =
			  new SimpleCommand(this.OnAddNewProfileCommand);

			this.removeProfileCommand =
			  new SimpleCommand(this.OnRemoveProfileCommand, false);
		}
		public ICommand RemoveProfileCommand
		{
			get
			{
				return (this.removeProfileCommand);
			}
		}
		public VerificationProfileViewModel SelectedVerificationProfile
		{
			get
			{
				return (this.selectedVerificationProfile);
			}
			set
			{
				base.SetProperty(ref this.selectedVerificationProfile, value);

				this.EnableRemoveCommand();
			}
		}
		void EnableRemoveCommand()
		{
			this.removeProfileCommand.Enable(
			  this.SelectedVerificationProfile != null);
		}
		async Task SetBusyAndMakeOxfordRestCall(Func<Task> call)
		{
			this.IsBusy = true;

			try
			{
				await call();
			}
			catch (Exception ex)
			{
				await this.ShowErrorAsync(ex.Message);
			}
			finally
			{
				this.IsBusy = false;
			}
		}
		async void OnRemoveProfileCommand()
		{
			await this.SetBusyAndMakeOxfordRestCall(async () =>
			{
				await this.OxfordRestClient.RemoveVerificationProfileAsync(
			this.SelectedVerificationProfile.Profile);

				await this.OnGetVerificationProfilesAsync();
			});
		}
		public ICommand AddNewProfileCommand
		{
			get
			{
				return (this.addNewProfileCommand);
			}
		}
		public bool IsBusy
		{
			get
			{
				return (this.isBusy);
			}
			set
			{
				base.SetProperty(ref this.isBusy, value);
			}
		}
		async void OnAddNewProfileCommand()
		{
			await this.SetBusyAndMakeOxfordRestCall(
			  async () =>
			  {
				  await this.OxfordRestClient.AddVerificationProfileAsync();
				  await this.OnGetVerificationProfilesAsync();
			  }
			);
		}
		async void OnGetVerificationProfilesCommand()
		{
			await this.SetBusyAndMakeOxfordRestCall(
			  async () =>
			  {
				  await this.OnGetVerificationProfilesAsync();
			  }
			);
		}
		async Task OnGetVerificationProfilesAsync()
		{
			this.VerificationProfiles.Clear();

			var results = await this.OxfordRestClient.GetVerificationProfilesAsync();

			foreach (var result in results)
			{
				this.VerificationProfiles.Add(
				  new VerificationProfileViewModel(this.OxfordRestClient)
				  {
					  Profile = result
				  }
				);
			}
		}
		public ICommand GetVerificationProfilesCommand
		{
			get
			{
				return (this.getVerificationProfilesCommand);
			}
		}
		public ObservableCollection<VerificationProfileViewModel> VerificationProfiles
		{
			get
			{
				return (this.verificationProfiles);
			}
			private set
			{
				base.SetProperty(ref this.verificationProfiles, value);
			}
		}
		OxfordSpeakerIdRestClient OxfordRestClient
		{
			get
			{
				if (this.oxfordSpeakerIdRestClient == null)
				{
					this.oxfordSpeakerIdRestClient = new OxfordSpeakerIdRestClient();
				}
				return (this.oxfordSpeakerIdRestClient);
			}
		}
		// lazy putting this here in a view model.
		async Task ShowErrorAsync(string error)
		{
			var dialog = new MessageDialog(error, "Woops!");
			await dialog.ShowAsync();
		}
		bool isBusy;

		// lazy, should put these in a dictionary and bind them from there.
		SimpleCommand getVerificationProfilesCommand;
		SimpleCommand addNewProfileCommand;
		SimpleCommand removeProfileCommand;
		ObservableCollection<VerificationProfileViewModel> verificationProfiles;
		OxfordSpeakerIdRestClient oxfordSpeakerIdRestClient;
		VerificationProfileViewModel selectedVerificationProfile;
	}
}
