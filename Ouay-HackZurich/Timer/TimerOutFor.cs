using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Ouay_HackZurich.Timer
{
	class TimerOutFor
	{
		public static int Time { get; set; }
		private static DispatcherTimer myTimer;

		public static void setupTimer(int _time, EventHandler<object> mydelegate)
		{
			Time = _time;
			myTimer = new DispatcherTimer();
			myTimer.Interval = new TimeSpan(Time, 0, 0);
			myTimer.Tick += mydelegate;
			myTimer.Start();
		}

		public static void stopTimer()
		{
			myTimer.Stop();
		}
	}
}
