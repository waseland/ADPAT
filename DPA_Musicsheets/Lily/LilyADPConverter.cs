using DPA_Musicsheets.MusicComponentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Lily
{
    public class LilyADPConverter
    {
        private LilypondMusicalSymbolBuilder lilypondMusicalSymbolBuilder;

        private enum contentType 
        {
            none,
            alternativeBlok,
            alternative
        }

        public LilyADPConverter()
        {
            lilypondMusicalSymbolBuilder = new LilypondMusicalSymbolBuilder();
        }

        public ADPSheet ReadFile(string _path)
        {
            ADPNote latestNote = new ADPNote();
            latestNote.Octave = 4;
            latestNote.Key = "C";
            int[] timeSignature = new int[2];

            string[] lilypondFileContents = System.IO.File.ReadAllText(_path).Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray();
            for (int i = 0; i < lilypondFileContents.Length; i++)
            {
                lilypondFileContents[i] = lilypondFileContents[i].Replace("\r\n", string.Empty);
                lilypondFileContents[i] = lilypondFileContents[i].Replace("\n", string.Empty);
            }

            return ConvertContent(lilypondFileContents);
        }

        public ADPSheet ConvertContent(string[] _content)
        {
            int[] timeSignature = new int[2];
            ADPNote latestNote = new ADPNote();
            latestNote.Key = "C";
            latestNote.Octave = 4;

            ADPMusicalSymbol tempMusicalSymbol;

            ADPBar tempBar;
            int alternativeNr = 0;

            string tempo = "error";
            string key = "error";

            contentType type = contentType.none;

            ADPSheet adps = new ADPSheet();
            ADPTrack adpt = new ADPTrack();
            adpt.Name = "LilypondTrack";

            List<ADPMusicalSymbol> notes = new List<ADPMusicalSymbol>();
            List<List<ADPMusicalSymbol>> alternatives = new List<List<ADPMusicalSymbol>>();

            for (int i = 2; i < _content.Length; i++)
            {
                string temp = _content[i];
                switch (_content[i])
                {
                    case "":
                        break;
                    case "\\tempo":
                        tempo = _content[i + 1];
                        i++;
                        break;
                    case "\\time":
                        string str = _content[i + 1];
                        timeSignature[0] = (int)Char.GetNumericValue(str[0]);
                        timeSignature[1] = (int)Char.GetNumericValue(str[2]);
                        i++;
                        break;
                    case "\\repeat":
                        tempBar = new ADPBar();
                        tempBar.MusicalSymbols = notes;
                        int[] ts = new int[2];
                        ts[0] = timeSignature[0];
                        ts[1] = timeSignature[1];
                        tempBar.TimeSignature = ts;
                        adpt.Bars.Add(tempBar);
                        notes = new List<ADPMusicalSymbol>();
                        i++;
                        i++;
                        break;
                    case "\\alternative":
                        //type = contentType.alternativeBlok;
                        break;
                    case "\\clef":
                        key = _content[i + 1];
                        i++;
                        break;
                    case "|": //add maatstreep / new bar?
                        if (type == contentType.alternative)
                        {
                            //Had een Alternative maatstreep gemaakt
                        }
                        else
                        {
                            tempBar = new ADPBar();
                            tempBar.MusicalSymbols = notes;
                            ts = new int[2];
                            ts[0] = timeSignature[0];
                            ts[1] = timeSignature[1];
                            tempBar.TimeSignature = ts;
                            adpt.Bars.Add(tempBar);
                            notes = new List<ADPMusicalSymbol>();
                        }
                        break;
                    case "{": //add alternative if alternativeblock
                        if (type == contentType.alternativeBlok)
                        {
                            type = contentType.alternative;
                            alternatives.Add(new List<ADPMusicalSymbol>());
                        }
                        break;
                    case "}":
                        //close alternative if alternativeblock
                        //if (type == contentType.alternative)
                        //{
                        //    type = contentType.alternativeBlok;
                        //    alternativeNr++;
                        //}

                        //else
                        //{
                        //    type = contentType.none;
                        //}
                        if (notes.Count > 0)
                        {
                            ADPBar tempBar2 = new ADPBar();
                            tempBar2.MusicalSymbols = notes;
                            tempBar2.TimeSignature = timeSignature;
                            adpt.Bars.Add(tempBar2);
                            notes = new List<ADPMusicalSymbol>();
                        }
                        break;
                    case "}}":  //End of File
                        break;
                    case "~":
                        break;
                    default:

                        if (type == contentType.alternative)
                        {
                            //add alternative note
                            tempMusicalSymbol = lilypondMusicalSymbolBuilder.BuildMusicalSymbol(_content[i], latestNote);
                            if(tempMusicalSymbol is ADPNote)
                            {
                                latestNote = (ADPNote)tempMusicalSymbol;
                            }
                            alternatives[alternativeNr].Add(lilypondMusicalSymbolBuilder.BuildMusicalSymbol(_content[i], latestNote));
                        }
                        else
                        {
                            //add normal note
                            tempMusicalSymbol = lilypondMusicalSymbolBuilder.BuildMusicalSymbol(_content[i], latestNote);
                            if (tempMusicalSymbol is ADPNote)
                            {
                                latestNote = (ADPNote)tempMusicalSymbol;
                            }
                            notes.Add(tempMusicalSymbol);
                        }
                        break;
                }
            }
            adps.Tracks.Add(adpt); //Only need to add one track
            return adps;
        }
    }
}
