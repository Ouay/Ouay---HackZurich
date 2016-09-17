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
		OuayGPIO gpio;

        public MainPage()
        {
            this.InitializeComponent();
			SS = new Ouay_SpeechSynthesis(this.media);
			SS.Talk("The speech Synthesiser has finished setup.");
            SR = new Ouay_SpeechRecognition();
            SR.setupSpeechRecognition();
            SR.onenterresult += new SpeechRecognitionEventHandler(enterEvent);
            SR.OnExitResult += new SpeechRecognitionEventHandler(exitEvent);
            SS.Talk("Speech recognition has finished setting up.");
            SR.StartSpeechRecognition();

            gpio = new OuayGPIO(); // Beware of null exceptions
            //Setup(); 
        }

		private void exitEvent(object source, SREventArgs e)
		{
            // TODO: set timer 

            // make answer
            SS.byeMessage();

			throw new NotImplementedException();
		}

		private void enterEvent(object source, SREventArgs e)
		{

            // TODO: notify database about the arrival.
            // TODO: Check if arrival time is normal.

            // make answer
            SS.WelcomeMessage();


            throw new NotImplementedException();
		}

		//private async void Setup()
		//{
		//	// dummy value that's out of range in order to fire an alert
		//	//bool a = await BlueMix.BlueMixCom.SendEntrance(DateTime.Now);
		//}
	}
}
