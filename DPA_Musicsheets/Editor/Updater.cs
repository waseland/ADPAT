using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using DPA_Musicsheets.Commands;
using System.Timers;

namespace DPA_Musicsheets.Editor
{
    class Updater
    {

        private CommandTarget target;
        private System.Timers.Timer t;

        public Updater(CommandTarget _target)
        {
            target = _target;
            initTimer();
        }

        private void initTimer()
        {
            t = new System.Timers.Timer();
            t.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            t.Interval = 1500;
            t.Enabled = true;
            t.Stop();
        }

        public void StopThread()
        {
            t.Stop();
        }

        public void StartThread()
        {
            t.Start();
        }

        void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            target.UpdateBarlinesFromLilypond();
            t.Stop();
        }
    }
}
