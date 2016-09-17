using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Ouay_HackZurich.Sensors
{
    class PIR
    {
        static GpioPin pinPIR;
        GpioPinValue pinPIRValue;
        public event EventHandler MotionDetected;

        public PIR()
        {
            pinPIR.DebounceTimeout = TimeSpan.FromMinutes(1);
            pinPIR.ValueChanged += PinPIR_ValueChanged;
            MotionDetected += PIR_MotionDetected;
        }
        private void PinPIR_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            pinPIRValue = pinPIR.Read();
            if (pinPIRValue == GpioPinValue.High)
            {
                Debug.WriteLine("Motion detected! grand'pa is alive!" + pinPIRValue);
                OnMotionDetected(EventArgs.Empty);
            }

        }

        protected void OnMotionDetected(EventArgs e)
        {
            if (MotionDetected != null)
            {
                MotionDetected(this, e);
            }
        }

        private void PIR_MotionDetected(object sender, EventArgs e)
        {
            // do some stuff with the pir sensor
            Debug.WriteLine("event called and handeled");
        }

        /*initialise GPIO pins*/
        public static void InitGPIO_PIR()
        {
            var gpio = GpioController.GetDefault();
            if (gpio == null)
            {
                pinPIR = null;
            }

            /* open the pins using the gpio controller */
            pinPIR = gpio.OpenPin(20);

            /* pins as input */
            pinPIR.SetDriveMode(GpioPinDriveMode.Input);

        }

    }
}
