using DPA_Musicsheets.MusicComponentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets
{
    public class NoteToLilypondConverter
    {
        public string getLilypond(ADPSheet musicSheet)
        {
            string result = "";
            result += "\\relative c' {\n";
            result += "\\clef treble\n";
            result += "\\time ";
            result += musicSheet.Tracks[1].Bars[1].TimeSignature[0];
            result += "/";
            result += musicSheet.Tracks[1].Bars[1].TimeSignature[1];
            result += "\n";
            result += "\\tempo 4=120\n";
            
            // add notes
            foreach(ADPBar bar in musicSheet.Tracks[1].Bars)
            {
                result += convertBar(bar);
            }

            result += "}";

            return result;
        }

        private string convertBar(ADPBar bar)
        {
            string result = "";
            foreach(ADPMusicalSymbol musicalSymbol in bar.MusicalSymbols)
            {
                if (musicalSymbol is ADPNote)
                {
                    result += ((ADPNote)musicalSymbol).Key.Substring(0,1).ToLower();
                    if(((ADPNote)musicalSymbol).Alter == 1)
                    {
                        // C#
                        result += "is";
                    } else if (((ADPNote)musicalSymbol).Alter == 2)
                    {
                        // Bb
                        result += "es";
                    }

                    result += calculateOctave();
                } else
                {
                    result += "r";
                }

                result += musicalSymbol.Duration;
                result += " ";
            }
            result += "|\n";

            return result;
        }

        private string calculateOctave()
        {
            //TODO kijken voor octaaf hoogte 
            return "";
            //throw new NotImplementedException();
        }
    }
}
