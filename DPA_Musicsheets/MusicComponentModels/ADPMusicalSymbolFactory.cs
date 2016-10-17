
namespace DPA_Musicsheets.MusicComponentModels
{
    public class ADPMusicalSymbolFactory
    {
        public ADPMusicalSymbol GetMusicalSymbol(string[] _inputStrings)
        {
            if (_inputStrings[0].Equals("rest"))
            {
                ADPRest resultMusicalSymbol = new ADPRest();
                resultMusicalSymbol.Duration = int.Parse(_inputStrings[1]);
                return resultMusicalSymbol;
            } else
            {
                ADPNote resultMusicalSymbol = new ADPNote();
                resultMusicalSymbol.Duration = int.Parse(_inputStrings[1]);
                resultMusicalSymbol.AmountOfDots = int.Parse(_inputStrings[2]);
                resultMusicalSymbol.Key = _inputStrings[3];
                resultMusicalSymbol.Alter = int.Parse(_inputStrings[4]);
                resultMusicalSymbol.Octave = int.Parse(_inputStrings[5]);
                
                return resultMusicalSymbol;
            }
        }
    }
}
