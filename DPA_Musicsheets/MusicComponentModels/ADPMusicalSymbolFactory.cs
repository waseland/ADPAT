
namespace DPA_Musicsheets.MusicComponentModels
{
    public class ADPMusicalSymbolFactory
    {
        public ADPMusicalSymbol getMusicalSymbol(string[] inputStrings)
        {
            if (inputStrings[0].Equals("rest"))
            {
                ADPRest resultMusicalSymbol = new ADPRest();
                resultMusicalSymbol.Duration = int.Parse(inputStrings[1]);
                return resultMusicalSymbol;
            } else
            {
                ADPNote resultMusicalSymbol = new ADPNote();
                resultMusicalSymbol.Duration = int.Parse(inputStrings[1]);
                resultMusicalSymbol.AmountOfDots = int.Parse(inputStrings[2]);
                resultMusicalSymbol.Key = inputStrings[3];
                resultMusicalSymbol.Alter = int.Parse(inputStrings[4]);
                resultMusicalSymbol.Octave = int.Parse(inputStrings[5]);
                

                return resultMusicalSymbol;
            }
        }
    }
}
