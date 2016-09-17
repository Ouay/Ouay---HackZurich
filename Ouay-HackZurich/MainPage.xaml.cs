using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Ouay_HackZurich.Speech

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Ouay_HackZurich
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        int stateButton = 0;

        /* GPIO pins */
        GpioPin pinPIR;
        GpioPin pinButton;
        GpioPin pinLED;
        GpioPin pinRelay;

        GpioPinValue pinPIRValue;
        GpioPinValue pinButtonValue;

        /*GPIO Controller */
        GpioController gpio;

		/*Speech recogniser */
		Ouay_SpeechRecognition SR;

        public event EventHandler MotionDetected;

        public MainPage()
        {
            this.InitializeComponent();
			SR = new Ouay_SpeechRecognition();

            InitGPIO();

            pinButton.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            pinPIR.DebounceTimeout = TimeSpan.FromMinutes(1);
            pinPIR.ValueChanged += PinPIR_ValueChanged;
            pinButton.ValueChanged += PinButton_ValueChanged;
            MotionDetected += MainPage_MotionDetected;
        }

        private void MainPage_MotionDetected(object sender, EventArgs e)
        {
            // do some stuff with the pir sensor
            Debug.WriteLine("event called and handeled");
        }

        protected void OnMotionDetected(EventArgs e)
        {
            if (MotionDetected != null)
            {
                MotionDetected(this, e);
            }
        }

        private void PinButton_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            pinButtonValue = pinButton.Read();
            if (pinButtonValue == GpioPinValue.High)
            {
                Debug.WriteLine("Button pressed!");
                if (stateButton == 0)
                {
                    /* power the plug (relay) on */
                    pinRelay.Write(GpioPinValue.High); // power on the relay that powers the 220V plug.
                    stateButton = 1;
                }
                else
                {
                    /* switches off the plug (relay)*/
                    pinRelay.Write(GpioPinValue.Low);
                    stateButton = 0;
                }
               
            }
        }

        private void PinPIR_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            pinPIRValue = pinPIR.Read();
            if(pinPIRValue == GpioPinValue.High)
            {
                Debug.WriteLine("Motion detected! grand'pa is alive!" + pinPIRValue);
                OnMotionDetected(EventArgs.Empty);
            }
            
        }

        /*initialise GPIO pins*/
        public void InitGPIO()
        {
            var gpio = GpioController.GetDefault();
            if (gpio == null)
            {
                pinPIR = null;
                pinButton = null;
                pinLED = null;
                pinRelay = null;
            }

            /* open the pins using the gpio controller */
            pinPIR = gpio.OpenPin(20);
            pinButton = gpio.OpenPin(16);
            pinLED = gpio.OpenPin(5);
            pinRelay = gpio.OpenPin(21);

            pinLED.Write(GpioPinValue.High); // when HIGH, led is off. make sure that when we start using the pin, it's off.
            pinRelay.Write(GpioPinValue.Low); // same idea but reversed

            /* pins as output */
            pinLED.SetDriveMode(GpioPinDriveMode.Output); //when low, the current flow from 3.3V to 0V (pinLED)
            pinRelay.SetDriveMode(GpioPinDriveMode.Output);

            /* pins as input */
            pinPIR.SetDriveMode(GpioPinDriveMode.Input);
            pinButton.SetDriveMode(GpioPinDriveMode.Input);

        }


    }
}
