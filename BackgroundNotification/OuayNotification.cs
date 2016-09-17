using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using Windows.UI.Xaml;

namespace BackgroundNotification
{
	public sealed class OuayNotification : IBackgroundTask
	{
		int nbOfItems = 0;
		private string UrlGetNotif = @"https://ouay.mybluemix.net/poll_notifications";
		BackgroundTaskDeferral _deferral; //for asynchronous operations


		public async void Run(IBackgroundTaskInstance taskInstance)
		{
			_deferral = taskInstance.GetDeferral();
			Setup();
			int tmp = await GetNbOfServerItem(); if (tmp > nbOfItems) {if(nbOfItems != 0) NotifyMe(); nbOfItems = tmp; };
			_deferral.Complete();
		}

		private void NotifyMe()
		{
			ToastContent content = GenerateToastContent();
			ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(content.GetXml()));
		}

		private ToastContent GenerateToastContent()
		{
			return new ToastContent()
			{
				Launch = "action=viewEvent&eventId=1983",

				Scenario = ToastScenario.Default,
				Visual = new ToastVisual()
				{
					BindingGeneric = new ToastBindingGeneric()
					{
						Children =
						{
							new AdaptiveText()
							{
								Text = "Someone enter your house"
							},
							new AdaptiveText()
							{
								Text = "Blablaba"
							}
						}
					}
				},
				Actions = new ToastActionsCustom()
				{
					Buttons =
				{
					new ToastButtonSnooze()
					{
						SelectionBoxId = "snoozeTime"
					},

					new ToastButtonDismiss()
				}
				}
			};
		}

		private async void Setup()
		{
			nbOfItems = await GetNbOfServerItem();
			return;
		}

		private async Task<int> GetNbOfServerItem()
		{
			try
			{
				var httpClient = new HttpClient();
				var content = await httpClient.GetStringAsync(UrlGetNotif);
				JArray array = await Task.Run(() => JArray.Parse(content));
				Debug.WriteLine("Get new entrance to notification Server");
				return array.Count;
			}
			catch
			{
				Debug.WriteLine("Cannot Get entrance notification to server");
			}
			return 1;
		}
	}
}
