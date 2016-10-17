using System.Collections.ObjectModel;

namespace DPA_Musicsheets
{
    public class MidiTrack
    {
        public string TrackName { get; set; }
        public ObservableCollection<string> Messages { get; private set; }

        public MidiTrack()
        {
            this.Messages = new ObservableCollection<string>();
        }
    }
}