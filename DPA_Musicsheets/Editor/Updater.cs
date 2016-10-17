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
        private System.Timers.Timer timer;

        public Updater(CommandTarget _target)
        {
            target = _target;
            initTimer();
        }

        private void initTimer()
        {
            timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Interval = 1500;
            timer.Enabled = true;
            timer.Stop();
        }

        public void StopThread()
        {
            timer.Stop();
        }

        public void StartThread()
        {
            timer.Start();
        }

        void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            target.UpdateBarlinesFromLilypond();
            timer.Stop();
        }
    }
}
