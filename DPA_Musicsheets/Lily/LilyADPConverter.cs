using DPA_Musicsheets.MusicComponentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Lily
{
    public class LilyADPConverter : ADPFileConverter
    {
        private enum contentType 
        {
            none,
            alternativeBlok,
            alternative
        }

        public LilyADPConverter()
        {
            ext = ".ly";
        }

        public override ADPSheet ReadFile(string _path)
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

        public ADPSheet ConvertText(string text)
        {
            ADPNote latestNote = new ADPNote();
            latestNote.Octave = 4;
            latestNote.Key = "C";
            int[] timeSignature = new int[2];

            string[] lilypondFileContents = text.Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray();
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
                            string[] inputStrings = convertToInputStrings(_content[i], latestNote);
                            if (inputStrings != null)
                            {
                                tempMusicalSymbol = musicalSymbolFactory.getMusicalSymbol(inputStrings);
                                if (tempMusicalSymbol is ADPNote)
                                {
                                    latestNote = (ADPNote)tempMusicalSymbol;
                                }
                                alternatives[alternativeNr].Add(tempMusicalSymbol);
                            }
                        }
                        else
                        {
                            //add normal note
                            string[] inputStrings = convertToInputStrings(_content[i], latestNote);
                            if (inputStrings != null)
                            {
                                tempMusicalSymbol = musicalSymbolFactory.getMusicalSymbol(inputStrings);
                                if (tempMusicalSymbol is ADPNote)
                                {
                                    latestNote = (ADPNote)tempMusicalSymbol;
                                }
                                notes.Add(tempMusicalSymbol);
                            }
                        }
                        break;
                }
            }
            adps.Tracks.Add(adpt); //Only need to add one track
            return adps;
        }

        private string[] convertToInputStrings(string musicalSymbolInfo, ADPNote latestNote)
        {
            string[] resultInputStrings = new string[6];

            string key = musicalSymbolInfo.Substring(0, 1);
            
            string durationValue = Regex.Match(musicalSymbolInfo, @"\d+").Value;
            if(durationValue == "")
            {
                return null;
            }
            int duration = Int32.Parse(durationValue);

            if (key.Equals("r"))
            {
                resultInputStrings[0] = "rest";
                resultInputStrings[1] = "" + duration;
            } else
            {
                int amountOfDots = 0;

                while (musicalSymbolInfo.Contains("."))
                {
                    int x = musicalSymbolInfo.IndexOf(".");
                    musicalSymbolInfo = musicalSymbolInfo.Remove(x, 1);
                    amountOfDots++;
                }

                resultInputStrings[0] = "note";
                resultInputStrings[1] = "" + duration;
                resultInputStrings[2] = "" + amountOfDots;
                resultInputStrings[3] = key.ToUpper();

                if (musicalSymbolInfo.Contains("is"))
                {
                    resultInputStrings[4] = "" + 1;
                }
                else if (musicalSymbolInfo.Contains("es"))
                {
                    resultInputStrings[4] = "" + 2;
                }
                else
                {
                    resultInputStrings[4] = "" + 0;
                }

                resultInputStrings[5] = "" + calculateOctave(musicalSymbolInfo, latestNote);
            }

            return resultInputStrings;
        }

        private int calculateOctave(string noteInfo, ADPNote latestNote)
        {
            string newKey = noteInfo.Substring(0, 1).ToUpper();
            int resultOctave;

            string[] keys = { "C", "D", "E", "F", "G", "A", "B", "C", "D", "E", "F", "G", "A", "B", "C", "D", "E", "F", "G", "A", "B" };

            int[] differences = new int[3];

            int latestNoteIndex = Array.IndexOf(keys, latestNote.Key.Substring(0, 1), 7);

            int newKeyIndex1 = Array.IndexOf(keys, newKey, 0);
            int newKeyIndex2 = Array.IndexOf(keys, newKey, 7);
            int newKeyIndex3 = Array.IndexOf(keys, newKey, 14);

            differences[0] = Math.Abs(latestNoteIndex - newKeyIndex1);
            differences[1] = Math.Abs(latestNoteIndex - newKeyIndex2);
            differences[2] = Math.Abs(latestNoteIndex - newKeyIndex3);

            differences.Min();
            if (differences[0] == differences.Min())
            {
                // lower octave
                resultOctave = latestNote.Octave - 1;
            }
            else if (differences[1] == differences.Min())
            {
                //middle octave
                resultOctave = latestNote.Octave;
            }
            else
            {
                // higher octave
                resultOctave = latestNote.Octave + 1;
            }

            while (noteInfo.Contains("'"))
            {
                resultOctave++;
                noteInfo = noteInfo.Remove(noteInfo.IndexOf("'"), 1);
            }

            while (noteInfo.Contains(","))
            {
                resultOctave--;
                noteInfo = noteInfo.Remove(noteInfo.IndexOf(","), 1);
            }

            return resultOctave;
        }
    }
}
