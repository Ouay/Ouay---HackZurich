using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Parent_App
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
		int nbOfItems = 0;
		private string UrlGetNotif = @"https://ouay.mybluemix.net/poll_notifications";

		public MainPage()
		{
			this.InitializeComponent();
			Setup();
			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = new TimeSpan(0, 0, 5);
			timer.Tick += async (sender, args) => 
			{
				int tmp = await GetNbOfServerItem(); if (tmp > nbOfItems) { nbOfItems = tmp; NotifyMe(); };
			};
			timer.Start();
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
					Inputs =
					{
					new ToastSelectionBox("snoozeTime")
					{
						DefaultSelectionBoxItemId = "15",
						Items =
						{
							new ToastSelectionBoxItem("1", "1 minute"),
							new ToastSelectionBoxItem("15", "15 minutes"),
							new ToastSelectionBoxItem("60", "1 hour"),
							new ToastSelectionBoxItem("240", "4 hours"),
							new ToastSelectionBoxItem("1440", "1 day")
						}
					}
				},
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
