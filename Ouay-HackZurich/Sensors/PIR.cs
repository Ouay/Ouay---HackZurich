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
        GpioController gpio2;
        GpioPinValue pinPIRValue;
        public event EventHandler MotionDetected;

        public PIR()
        {
            pinPIR.DebounceTimeout = TimeSpan.FromMinutes(1);
            pinPIR.ValueChanged += PinPIR_ValueChanged;
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

        /*initialise GPIO pins*/
        public static void InitGPIO_PIR()
        {
            var gpio2 = GpioController.GetDefault();
            if (gpio2 == null)
            {
                pinPIR = null;
            }

            /* open the pins using the gpio controller */
            pinPIR = gpio2.OpenPin(20);

            /* pins as input */
            pinPIR.SetDriveMode(GpioPinDriveMode.Input);

        }

    }
}
