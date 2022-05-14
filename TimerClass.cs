using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatelliteTerminal
{
    class TimerClass
    {
        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();

        public void StartTimer()
        {
            myTimer.Tick += new EventHandler(TimerEventProcessor);

            myTimer.Interval = 5000;
            myTimer.Start();
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            myTimer.Stop();

            Console.WriteLine("STOPPED");

            //myTimer.Enabled = true;

        }
    }
}
