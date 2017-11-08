using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ControlServoMotor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static GpioPin pinL;
        public static GpioPinValue pinLValue;

        public static GpioPin pinR;
        public static GpioPinValue pinRValue;


        private const int Button_PIN = 21;
        private GpioPin pin1;
        
        private const int Button_PIN2 = 20;
        private GpioPin pin2;
        
        private static IAsyncAction workItemThread;
        public static GpioController gpio;

        public MainPage()
        {
            this.InitializeComponent();

            gpio = GpioController.GetDefault();

            InitGPIO();
        }

        public static void PWM_R()
        {   

            workItemThread = Windows.System.Threading.ThreadPool.RunAsync(
                 (source) =>
                 {
                     var stopwatch = Stopwatch.StartNew();

                     // setup, ensure pins initialized
                     ManualResetEvent mre = new ManualResetEvent(false);
                     mre.WaitOne(1500);
                     var startTime = stopwatch.ElapsedMilliseconds;

                     ulong pulseTicks = ((ulong)(Stopwatch.Frequency) / 1000) * 2;
                     ulong delta;
                     while (stopwatch.ElapsedMilliseconds - startTime <= 300)
                     {
                         pinR.Write(GpioPinValue.High);
                         ulong starttick = (ulong)(stopwatch.ElapsedTicks);
                         while (true)
                         {
                             delta = (ulong)(stopwatch.ElapsedTicks) - starttick;
                             if (delta > pulseTicks) break;
                         }
                         pinR.Write(GpioPinValue.Low);
                         starttick = (ulong)(stopwatch.ElapsedTicks);
                         while (true)
                         {
                             delta = (ulong)(stopwatch.ElapsedTicks) - starttick;
                             if (delta > pulseTicks ) break;
                         }
                     }
                     stopwatch.Stop();
                 }, WorkItemPriority.High);
            
        }

        public static void PWM_L()
        {
            
            workItemThread = Windows.System.Threading.ThreadPool.RunAsync(
                 (source) =>
                 {
                     var stopwatch = Stopwatch.StartNew();
                    // setup, ensure pins initialized
                     ManualResetEvent mre = new ManualResetEvent(false);
                     mre.WaitOne(1500);
                     var startTime = stopwatch.ElapsedMilliseconds;

                     ulong pulseTicks = ((ulong)(Stopwatch.Frequency) / 1000) * 2;
                     ulong delta;
                     while (stopwatch.ElapsedMilliseconds - startTime <= 300)
                     {
                         pinL.Write(GpioPinValue.High);
                         ulong starttick = (ulong)(stopwatch.ElapsedTicks);
                         while (true)
                         {
                             delta = starttick - (ulong)(stopwatch.ElapsedTicks);
                             if (delta > pulseTicks) break;
                         }
                         pinL.Write(GpioPinValue.Low);
                         starttick = (ulong)(stopwatch.ElapsedTicks);
                         while (true)
                         {
                             delta = (ulong)(stopwatch.ElapsedTicks) - starttick;
                             if (delta > pulseTicks * 10) break;
                         }
                     }
                     stopwatch.Stop();
                 }, WorkItemPriority.High);
            

        }

        private void InitGPIO()
        {
            if (gpio == null)
            {
                pinL = null;
                pinR = null;
                return;
            }

            pinL = gpio.OpenPin(17);
            pinLValue = GpioPinValue.High;
            pinL.SetDriveMode(GpioPinDriveMode.Output);


            pinR = gpio.OpenPin(18);
            pinRValue = GpioPinValue.High;
            pinR.SetDriveMode(GpioPinDriveMode.Output);


            pin1 = gpio.OpenPin(Button_PIN);
            pin1.SetDriveMode(GpioPinDriveMode.Input);

            pin2 = gpio.OpenPin(Button_PIN2);
            pin2.SetDriveMode(GpioPinDriveMode.Input);

            if (pin1.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                pin1.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                pin1.SetDriveMode(GpioPinDriveMode.Input);
            pin1.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            pin1.ValueChanged += buttonPin_ValueChanged_R;

            if (pin2.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                pin2.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                pin2.SetDriveMode(GpioPinDriveMode.Input);
            pin2.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            pin2.ValueChanged += buttonPin_ValueChanged_L;
        }

        private void buttonPin_ValueChanged_L(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.FallingEdge)
            {
                PWM_L();
            }
        }

        private void buttonPin_ValueChanged_R(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.FallingEdge)   
            {
                PWM_R();
            }
        }

        public static void PWM_L2(int pinNumber)
        {
            var stopwatch = Stopwatch.StartNew();





            workItemThread = Windows.System.Threading.ThreadPool.RunAsync(
                 (source) =>
                 {
                     // setup, ensure pins initialized
                     ManualResetEvent mre = new ManualResetEvent(false);
                     mre.WaitOne(1500);
                     //var startTime = stopwatch.ElapsedMilliseconds;
                     ulong pulseTicks = Convert.ToUInt32(((ulong)(Stopwatch.Frequency) / 1000) * 2.94) ;
                     ulong hightTicks = Convert.ToUInt32(pulseTicks * 0.3);
                     ulong lowTicks = Convert.ToUInt32(pulseTicks * 0.7);
                     while (true)//stopwatch.ElapsedMilliseconds  <= 3000)
                     {
                         pinL.Write(GpioPinValue.High);
                         ulong endtick = (ulong)(stopwatch.ElapsedTicks)+ hightTicks;
                         while (true)
                         {
                             if ((ulong)stopwatch.ElapsedTicks >=endtick) break;
                         }
                         pinL.Write(GpioPinValue.Low);
                         endtick = (ulong)(stopwatch.ElapsedTicks)+lowTicks;
                         while (true)
                         {
                             //delta = (ulong)(stopwatch.ElapsedTicks) - starttick;
                             if ((ulong)stopwatch.ElapsedTicks >= endtick) break;
                         }
                     }
                 }, WorkItemPriority.High);
        }

    }
}
