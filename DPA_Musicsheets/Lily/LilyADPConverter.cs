using DPA_Musicsheets.Models;
using DPA_Musicsheets.MusicComponentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Lily
{
    class LilyADPConverter
    {

        private string[] lilyPondContents;
        private ADPNote latestNote;
        private int[] timeSignature;


        private enum contentType
        {
            none,
            alternativeBlok,
            alternative
        }

        public LilyADPConverter(string path)
        {

            latestNote = new ADPNote();
            latestNote.Octave = 4;
            latestNote.Key = "C";
            timeSignature = new int[2];

            lilyPondContents = System.IO.File.ReadAllText(path).Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray();
            for (int i = 0; i < lilyPondContents.Length; i++)
            {
                lilyPondContents[i] = lilyPondContents[i].Replace("\r\n", string.Empty);
                lilyPondContents[i] = lilyPondContents[i].Replace("\n", string.Empty);
                //if (lilyPondContents[i].Contains("time"))
                //{
                //    string str = lilyPondContents[i + 1];
                //    timeSignature[0] = (int)Char.GetNumericValue(str[0]);
                //    timeSignature[1] = (int)Char.GetNumericValue(str[2]);


                //}
            }
        }


        public ADPSheet readContent()
        {
            int alternativeNr = 0;

            string tempo = "error";
            string key = "error";

            contentType type = contentType.none;

            ADPSheet adps = new ADPSheet();
            ADPTrack adpt = new ADPTrack();
            adpt.Name = "LilypondTrack";

            List<ADPMusicalSymbol> notes = new List<ADPMusicalSymbol>();                        
            List<List<ADPMusicalSymbol>> alternatives = new List<List<ADPMusicalSymbol>>();     

            for (int i = 2; i < lilyPondContents.Length; i++)
            {
                string temp = lilyPondContents[i];
                switch (lilyPondContents[i])
                {
                    case "":
                        break;
                    case "\\tempo":
                        tempo = lilyPondContents[i + 1];
                        i++;
                        break;
                    case "\\time":
                        string str = lilyPondContents[i + 1];
                        timeSignature[0] = (int)Char.GetNumericValue(str[0]);
                        timeSignature[1] = (int)Char.GetNumericValue(str[2]);
                        i++;
                        break;
                    case "\\repeat":
                        ADPBar tempBar = new ADPBar();
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
                        key = lilyPondContents[i + 1];
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
                        if(notes.Count > 0)
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
                            alternatives[alternativeNr].Add(createNote(lilyPondContents[i]));
                        }
                        else
                        {
                            //add normal note
                            notes.Add(createNote(lilyPondContents[i]));
                        }
                        break;
                }
            }
            adps.Tracks.Add(adpt); //Only need to add one track
            return adps;
        }

        public ADPMusicalSymbol createNote(string note)
        {
            ADPMusicalSymbol mms;

            string toonhoogte = note.Substring(0, 1);
            string temp = Regex.Match(note, @"\d+").Value;
            int d = Int32.Parse(temp);

            if (toonhoogte == "r")
            {
                mms = new ADPRest();
                mms.Duration = d;
            }
            else
            {
                mms = new ADPNote();
                int dots = 0;
                ((ADPNote)mms).Key = toonhoogte.ToUpper();
                ((ADPNote)mms).Octave = setCurrentOctaaf(note);
                ((ADPNote)mms).Duration = d;
                while (note.Contains("."))
                {
                    int x = note.IndexOf(".");
                    note = note.Remove(x, 1);
                    dots++;
                }
                ((ADPNote)mms).AmountOfDots = dots;

                if (note.Contains("is"))
                {
                    ((ADPNote)mms).Alter = 1;
                }
                else if (note.Contains("es"))
                {
                    ((ADPNote)mms).Alter = 2;
                }
                else
                {
                    ((ADPNote)mms).Alter = 0;
                }
                latestNote = ((ADPNote)mms);
            }
            return mms;
        }


        private int setCurrentOctaaf(string note)
        {
            string newKey = note.Substring(0, 1).ToUpper();
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

            while (note.Contains("'"))
            {
                resultOctave++;
                note = note.Remove(note.IndexOf("'"), 1);
            }

            while (note.Contains(","))
            {
                resultOctave--;
                note = note.Remove(note.IndexOf(","), 1);
            }

            return resultOctave;
        }

    }
}
