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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Ouay_HackZurich
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

		/* speech recognizer specific to Ouay */
		Ouay_SpeechRecognition SR;
		Ouay_SpeechSynthesis SS;

		/* GPIO hardware */
		OuayGPIO gpio;

        public MainPage()
        {
            this.InitializeComponent();
			SS = new Ouay_SpeechSynthesis(this.media);
			SS.Talk("The speech Synthesiser has finished setup.");
			SR = new Ouay_SpeechRecognition();
			SR.OnEnterResult += new SpeechRecognitionEventHandler(enterEvent);
			SR.OnExitResult += new SpeechRecognitionEventHandler(exitEvent);

			gpio = new OuayGPIO(); // Beware of null exceptions
			//gpio.MotionDetected += motionDetected();
        }

		private EventHandler motionDetected()
		{
			throw new NotImplementedException();
		}

		private void exitEvent(object source, SREventArgs e)
		{
			// TODO: set timer 
			TimerOutFor.setupTimer(e.GetInfo(), HandleDelayAlert );

			// TODO: make answer

		}

		private void HandleDelayAlert(object sender, object e)
		{
			BlueMixCom.Alert("Late Alert");
		}

		private async void enterEvent(object source, SREventArgs e)
		{

			// TODO: notify database about the arrival.
			await BlueMixCom.SendEntrance(DateTime.Now);

			// TODO: make answer
		}
	}
}
