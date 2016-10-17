using DPA_Musicsheets.Commands;
using System.Timers;

namespace DPA_Musicsheets.Editor
{
    class Updater
    {

        private CommandTarget target;
        private Timer timer;

        public Updater(CommandTarget _target)
        {
            target = _target;
            initTimer();
        }

        private void initTimer()
        {
            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(onTimedEvent);
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

        private void onTimedEvent(object source, ElapsedEventArgs e)
        {
            target.UpdateBarlinesFromLilypond();
            timer.Stop();
        }
    }
}
