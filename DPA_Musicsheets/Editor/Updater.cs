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
            target = _target; //target is needed to call theUpdateBarlinesFromLilypond method after 1.5 seconds had passed in the editor without editing anything
            initTimer();
        }

        private void initTimer() //initializes the timer that will take care of the barline update
        {
            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(onTimedEvent);
            timer.Interval = 1500;
            timer.Enabled = true;
            timer.Stop();
        }

        public void StopThread() // stops/resets the timer
        {
            timer.Stop();
        }

        public void StartThread() // starts the timer
        {
            timer.Start();
        }

        private void onTimedEvent(object source, ElapsedEventArgs e) //This gets called everytime the interval of the timer has passed (1.5 seconds), it updates the barlines and stops the timer
        {
            target.UpdateBarlinesFromLilypond();
            timer.Stop();
        }
    }
}
