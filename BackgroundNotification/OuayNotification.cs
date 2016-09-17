using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml;

namespace BackgroundNotification
{
	public sealed class OuayNotification : IBackgroundTask
	{
		private string UrlGetNotif = @"https://ouay.mybluemix.net/poll_notifications";
		BackgroundTaskDeferral _deferral; //for asynchronous operations
		string lastKey = "-1";
		string firstInfo = "Something happens at home";
		string secondInfo = "Click here to get more information";

		public async void Run(IBackgroundTaskInstance taskInstance)
		{
			_deferral = taskInstance.GetDeferral();
			IPropertySet roamingProperties = ApplicationData.Current.LocalSettings.Values;
			if (roamingProperties.ContainsKey("LastKey"))
			{
				lastKey = roamingProperties["LastKey"].ToString();
			}
			else
			{
				roamingProperties["LastKey"] = lastKey;
			}
			string tmp = await GetNbOfServerItem();
			if (tmp != lastKey)
			{
				lastKey = tmp;
				roamingProperties["LastKey"] = lastKey;
				NotifyMe();
			}
			//_deferral.Complete();
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
								Text = ""
							},
							new AdaptiveText()
							{
								Text = ""
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
						SelectionBoxId = "Get info"
					},

					new ToastButtonDismiss()
				}
				}
			};
		}

		private async Task<string> GetNbOfServerItem()
		{
			try
			{
				var httpClient = new HttpClient();
				var content = await httpClient.GetStringAsync(UrlGetNotif);
				JArray array = JArray.Parse(content);
				foreach (JObject o in array.Children<JObject>())
				{
					foreach (JProperty p in o.Properties())
					{
						if (p.Name == "what")
							return (string)p.Value;
						if (p.Name == "info1")
						{
							if (p.Value.ToString() != "")
								firstInfo = (string)p.Value;
						}
						if (p.Name == "info2")
						{
							if (p.Value.ToString() != "")
								secondInfo = (string)p.Value;
						}
					}
				}
			}
			catch(Exception e)
			{
				e.ToString();
			}
			return "-1";
		}
	}
}
