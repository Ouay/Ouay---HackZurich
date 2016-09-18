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
using Ouay_HackZurich.Speech;
using Ouay_HackZurich.GPIO;
using System.Threading.Tasks;
using Ouay_HackZurich.Timer;
using Ouay_HackZurich.BlueMix;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Ouay_HackZurich
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

		/* speech recognizer specific to Ouay */
		private Ouay_SpeechRecognition SR;
		private Ouay_SpeechSynthesis SS;

		/* GPIO hardware */
		private OuayGPIO gpio;

		/* dispatcher */
		 private CoreDispatcher dispatcher;

		public MainPage()
        {
            this.InitializeComponent();

			dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

			SS = new Ouay_SpeechSynthesis(this.media);
			SS.Talk("The speech Synthesiser has finished setup.");
			SR = new Ouay_SpeechRecognition();
			SR.OnEnterResult += new SpeechRecognitionEventHandler(enterEvent);
			SR.OnExitResult += new SpeechRecognitionEventHandler(exitEvent);

			//gpio = new OuayGPIO(); // Beware of null exceptions
			//gpio.MotionDetected += motionDetected;
        }

		private void motionDetected(object sender, EventArgs e)
		{
			//TODO: do something here 
		}

		private async void exitEvent(object source, SREventArgs e)
		{
			// Set timer once the person goes out.
			TimerOutFor.setupTimer(e.GetInfo(), HandleDelayAlert, dispatcher);

			// Say goodbye to person
			SR.pauseSpeechRecognition();
			await SS.byeMessage();
			await Task.Delay(1000);
			SR.resumeSpeechRecognition();
		}

		private void HandleDelayAlert(object sender, object e)
		{
			// Alert about a late arrival at home
			BlueMixCom.Alert("Late Alert");
		}

		private async void enterEvent(object source, SREventArgs e)
		{
			// Send entrance time to database to know if everything is ok
			await BlueMixCom.SendEntrance(DateTime.Now);

			// Stop timer tracking time out of house
			TimerOutFor.stopTimer(dispatcher);

			// Welcome the person home
			SR.pauseSpeechRecognition();
			await SS.WelcomeMessage();
			await Task.Delay(1000);
			SR.resumeSpeechRecognition();
		}
	}
}
