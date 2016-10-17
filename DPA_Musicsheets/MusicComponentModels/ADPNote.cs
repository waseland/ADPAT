
namespace DPA_Musicsheets.MusicComponentModels
{
    public class ADPNote : ADPMusicalSymbol
    {
        public string Key { get; set; }
        public int Alter { get; set; }
        public int Octave { get; set; }
        public int AmountOfDots { get; set; }

        public ADPNote()
        {
            AmountOfDots = 0;
        }
    }
}
