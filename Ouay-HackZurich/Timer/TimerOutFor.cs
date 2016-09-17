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
		public int Time { get; }


		public TimerOutFor(int _time)
		{
			Time = _time;
			DispatcherTimer myTimer = new DispatcherTimer();
			myTimer.Interval = new TimeSpan(Time, 0, 0);
			myTimer.Tick += MyTimer_Tick;
			myTimer.Start();
		}

		private void MyTimer_Tick(object sender, object e)
		{
			//Do stuff there
		}
	}
}
