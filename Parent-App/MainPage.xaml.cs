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
using Windows.ApplicationModel.Background;
using BackgroundNotification;
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

		public MainPage()
		{
			this.InitializeComponent();
			SetupBackground();
		}

		private async void SetupBackground()
		{
			var exampleTaskName = "OuayNotifications";

			foreach (var cur in BackgroundTaskRegistration.AllTasks)
				if (cur.Value.Name == exampleTaskName)
					return;

			// Windows Phone app must call this to use trigger types (see MSDN)
			await BackgroundExecutionManager.RequestAccessAsync();

			// register a new task
			BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder { Name = exampleTaskName, TaskEntryPoint = "BackgroundNotification.OuayNotification" };
			taskBuilder.SetTrigger(new SystemTrigger(SystemTriggerType.UserPresent, false));
			try
			{
				BackgroundTaskRegistration myTask = taskBuilder.Register();
			}
			catch (Exception e)
			{
				e.ToString();
			}
		}
	}
}
