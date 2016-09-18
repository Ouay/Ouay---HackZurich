using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Ouay_HackZurich.Timer
{
	class TimerOutFor
	{
		public static int Time { get; set; }
		private static DispatcherTimer myTimer;

		public async static void setupTimer(int _time, EventHandler<object> mydelegate, CoreDispatcher mainDispatcher)
		{
			mainDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				Time = _time;
				myTimer = new DispatcherTimer();
				myTimer.Interval = new TimeSpan(Time, 0, 0);
				myTimer.Tick += mydelegate;
				myTimer.Start();
			});
		}

		public async static void stopTimer(CoreDispatcher mainDispatcher)
		{
			mainDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				if (myTimer != null)
					myTimer.Stop();
			});
		}
	}
}
