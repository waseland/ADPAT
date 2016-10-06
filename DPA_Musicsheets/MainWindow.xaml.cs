﻿using DPA_Musicsheets.Models;
using Microsoft.Win32;
using PSAMControlLibrary;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DPA_Musicsheets
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MidiPlayer _player;
        public ObservableCollection<MidiTrack> MidiTracks { get; private set; }

        // De OutputDevice is een midi device of het midikanaal van je PC.
        // Hierop gaan we audio streamen.
        // DeviceID 0 is je audio van je PC zelf.
        private OutputDevice _outputDevice = new OutputDevice(0);

        public MainWindow()
        {
            this.MidiTracks = new ObservableCollection<MidiTrack>();
            InitializeComponent();
            DataContext = MidiTracks;
            FillPSAMViewer();
            //FillPSAMViewer();
            //notenbalk.LoadFromXmlFile("Resources/example.xml");
        }

        private void ShowTrack(MyTrack _myTrack, int _timeSignatureUp, int _timeSignatureDown)
        {
            Console.WriteLine("New Version!!!!!");
            staff.ClearMusicalIncipit();

            staff.AddMusicalSymbol(new Clef(ClefType.GClef, 2));
            staff.AddMusicalSymbol(new TimeSignature(TimeSignatureType.Numbers, (uint)_timeSignatureUp, (uint)_timeSignatureDown));
            staff.AddMusicalSymbol(new Barline());

            foreach (MyMusicalSymbol tempNote in _myTrack.Notes)
            {
                if (tempNote.IsPause)
                {
                    //rest
                    staff.AddMusicalSymbol(new Rest(tempNote.Duration));
                } else
                {
                    //note
                    if (tempNote.HasDot)
                    {
                        staff.AddMusicalSymbol(new Note(tempNote.Key, tempNote.Alter, tempNote.Octave, tempNote.Duration, NoteStemDirection.Up, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single }) { NumberOfDots = 1 });
                    } else
                    {
                        staff.AddMusicalSymbol(new Note(tempNote.Key, tempNote.Alter, tempNote.Octave, tempNote.Duration, NoteStemDirection.Up, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single }));
                    }
                }

                if (tempNote.IsEndOfBar)
                {
                    staff.AddMusicalSymbol(new Barline());
                }
            }
        }

        private void FillTestPSAMViewer()
        {
            IEnumerable<MidiTrack> testList = MidiReader.ReadMidi(txt_MidiFilePath.Text);
            KeyCodeConvertor kc = new KeyCodeConvertor();

            staff.ClearMusicalIncipit();

            staff.AddMusicalSymbol(new Clef(ClefType.GClef, 2));
            staff.AddMusicalSymbol(new TimeSignature(TimeSignatureType.Numbers, 4, 4));

            Note tempNote = null;

            //Console.WriteLine("#######################################################################################################");
            //Console.WriteLine("#######################################################################################################");
            //Console.WriteLine("####################################### --- New Track --- #############################################");
            //Console.WriteLine("#######################################################################################################");
            //Console.WriteLine("#######################################################################################################");

            for (int noteCount = 0; noteCount < testList.ElementAt(1).Messages.Count; noteCount++)
            {
                tempNote = kc.getNote(testList.ElementAt(1).Messages.ElementAt(noteCount));
                if(tempNote != null)
                {
                    staff.AddMusicalSymbol(tempNote);
                }
            }
        }

        private void FillPSAMViewer()
        {
            //String s = txt_MidiFilePath.Text;
            IEnumerable<MidiTrack> testList = MidiReader.ReadMidi(txt_MidiFilePath.Text);
            staff.ClearMusicalIncipit();
            // Clef = sleutel
            staff.AddMusicalSymbol(new Clef(ClefType.GClef, 2));
            staff.AddMusicalSymbol(new TimeSignature(TimeSignatureType.Numbers, 4, 4));
            /* 
                The first argument of Note constructor is a string representing one of the following names of steps: A, B, C, D, E, F, G. 
                The second argument is number of sharps (positive number) or flats (negative number) where 0 means no alteration. 
                The third argument is the number of an octave. 
                The next arguments are: duration of the note, stem direction and type of tie (NoteTieType.None if the note is not tied). 
                The last argument is a list of beams. If the note doesn't have any beams, it must still have that list with just one 
                    element NoteBeamType.Single (even if duration of the note is greater than eighth). 
                    To make it clear how beamlists work, let's try to add a group of two beamed sixteenths and eighth:
                        Note s1 = new Note("A", 0, 4, MusicalSymbolDuration.Sixteenth, NoteStemDirection.Down, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Start, NoteBeamType.Start});
                        Note s2 = new Note("C", 1, 5, MusicalSymbolDuration.Sixteenth, NoteStemDirection.Down, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Continue, NoteBeamType.End });
                        Note e = new Note("D", 0, 5, MusicalSymbolDuration.Eighth, NoteStemDirection.Down, NoteTieType.None,new List<NoteBeamType>() { NoteBeamType.End });
                        viewer.AddMusicalSymbol(s1);
                        viewer.AddMusicalSymbol(s2);
                        viewer.AddMusicalSymbol(e); 
            */

            staff.AddMusicalSymbol(new Note("A", 1, 4, MusicalSymbolDuration.Sixteenth, NoteStemDirection.Down, NoteTieType.Start, new List<NoteBeamType>() { NoteBeamType.Start, NoteBeamType.Start }));
            staff.AddMusicalSymbol(new Note("A", 0, 4, MusicalSymbolDuration.Sixteenth, NoteStemDirection.Down, NoteTieType.Stop, new List<NoteBeamType>() { NoteBeamType.Start, NoteBeamType.Start }));
            staff.AddMusicalSymbol(new Note("C", 1, 5, MusicalSymbolDuration.Sixteenth, NoteStemDirection.Down, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Continue, NoteBeamType.End }));
            staff.AddMusicalSymbol(new Note("D", 0, 5, MusicalSymbolDuration.Eighth, NoteStemDirection.Down, NoteTieType.Start, new List<NoteBeamType>() { NoteBeamType.End }));
            staff.AddMusicalSymbol(new Barline());

            staff.AddMusicalSymbol(new Note("D", 0, 5, MusicalSymbolDuration.Whole, NoteStemDirection.Down, NoteTieType.Stop, new List<NoteBeamType>() { NoteBeamType.Single }));
            staff.AddMusicalSymbol(new Note("E", 0, 4, MusicalSymbolDuration.Quarter, NoteStemDirection.Up, NoteTieType.Start, new List<NoteBeamType>() { NoteBeamType.Single }) { NumberOfDots = 1 });
            staff.AddMusicalSymbol(new Barline());

            //staff.AddMusicalSymbol(new Rest
            staff.AddMusicalSymbol(new Note("C", 0, 4, MusicalSymbolDuration.Half, NoteStemDirection.Up, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single }));
            staff.AddMusicalSymbol(
                new Note("E", 0, 4, MusicalSymbolDuration.Half, NoteStemDirection.Up, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single })
                { IsChordElement = true });
            staff.AddMusicalSymbol(
                new Note("G", 0, 4, MusicalSymbolDuration.Half, NoteStemDirection.Up, NoteTieType.None, new List<NoteBeamType>() { NoteBeamType.Single })
                { IsChordElement = true });
            staff.AddMusicalSymbol(new Barline());
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if(_player != null)
            {
                _player.Dispose();
            }

            _player = new MidiPlayer(_outputDevice);
            _player.Play(txt_MidiFilePath.Text);
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Midi Files(.mid)|*.mid" };
            if (openFileDialog.ShowDialog() == true)
            {
                txt_MidiFilePath.Text = openFileDialog.FileName;
                //FillTestPSAMViewer();
                MidiConverter midiConverter = new MidiConverter();
                MyMusicSheet mss = midiConverter.convertMidi(txt_MidiFilePath.Text);
                ShowTrack(mss.Tracks[1], mss.TimeSignature[0], mss.TimeSignature[1]);
            }
        }
        
        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (_player != null)
                _player.Dispose();
        }

        private void btn_ShowContent_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("HIEEEEEEEEEEEEEEEEER!!!!!!!!!!!!");
            string extension = txt_MidiFilePath.Text.Split('.').Last();

            if (extension == "mid")
            {
                ShowMidiTracks(MidiReader.ReadMidi(txt_MidiFilePath.Text));
                MidiConverter midiConverter = new MidiConverter();
                MyMusicSheet mss = midiConverter.convertMidi(txt_MidiFilePath.Text);
                ShowTrack(mss.Tracks[1], mss.TimeSignature[0], mss.TimeSignature[1]);
            }
            // TODO: add lilypond file extension
        }

        private void ShowMidiTracks(IEnumerable<MidiTrack> midiTracks)
        {
            MidiTracks.Clear();
            foreach (var midiTrack in midiTracks)
            {
                MidiTracks.Add(midiTrack);
            }

            tabCtrl_MidiContent.SelectedIndex = 0;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _outputDevice.Close();
            if (_player != null)
            {
                _player.Dispose();
            }
        }
    }
}
