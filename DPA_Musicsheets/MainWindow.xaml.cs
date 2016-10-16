using DPA_Musicsheets.Adapter;
using DPA_Musicsheets.Commands;
using DPA_Musicsheets.Controller;
using DPA_Musicsheets.Controllers;
using DPA_Musicsheets.Editor;
using DPA_Musicsheets.Lily;
using DPA_Musicsheets.Midi;
using DPA_Musicsheets.MusicComponentModels;
using Microsoft.Win32;
using PSAMControlLibrary;
using PSAMWPFControlLibrary;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class MainWindow : Window, CommandTarget
    {
        private MidiPlayer _player;
        private int savedMementos;
        private int currentMemento;
        private Originator originator;
        private CareTaker careTaker;
        private ADPKeyHandler keyHandler;
        private PSAMAdapter psamAdapter;
        private ADPFileConverter firstFileConverter;
        private bool needsSaving;
        private LilyADPConverter lycon;
        private FileExporter fileExporter;


        public ObservableCollection<MidiTrack> MidiTracks { get; private set; }

        private OutputDevice _outputDevice = new OutputDevice(0);

        public MainWindow()
        {
            lycon = new LilyADPConverter();
            fileExporter = new FileExporter();
            this.MidiTracks = new ObservableCollection<MidiTrack>();
            keyHandler = new ADPKeyHandler(this);
            psamAdapter = new PSAMAdapter();
            InitializeComponent();
            DataContext = MidiTracks;
            ShowSampleVisualisation();
            initializeEditor();
            initializeFileConverters();
            needsSaving = false;
        }

        private void initializeFileConverters()
        {
            LilyADPConverter lyConverter = new LilyADPConverter();
            MidiADPConverter midConverter = new MidiADPConverter();

            lyConverter.SetNextADPFileConverter(midConverter);

            firstFileConverter = lyConverter;
        }

        private void initializeEditor()
        {
            savedMementos = 0;
            currentMemento = 0;
            originator = new Originator();
            careTaker = new CareTaker();

            SetNewState();

            ReEvaluateButtons();
        }

        private void ShowSheetVisualisation(ADPTrack _adpTrack)
        {
            barlinesScrollViewer.Content = psamAdapter.GetSheetVisualisation(_adpTrack);
        }

        private void ShowSampleVisualisation()
        {
            barlinesScrollViewer.Content = psamAdapter.GetSampleVisualisation();
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

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            CareTaker c = careTaker;
            if (currentMemento >= 1)
            {
                currentMemento--;
                lilypondText.Text = originator.restoreFromMemento(careTaker.get(currentMemento));
            }


            ReEvaluateButtons();
        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            if (currentMemento < savedMementos)
            {
                currentMemento++;
                lilypondText.Text = originator.restoreFromMemento(careTaker.get(currentMemento - 1));
            }

            ReEvaluateButtons();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            Console.WriteLine("----------------------------- Update --------------------------");
        }

        private void ReEvaluateButtons()
        {
            if (savedMementos > 1 && currentMemento > 0)
            {
                btnUndo.IsEnabled = true;
            }
            else
            {
                btnUndo.IsEnabled = false;
            }

            if (currentMemento < savedMementos)
            {
                btnRedo.IsEnabled = true;
            }
            else
            {
                btnRedo.IsEnabled = false;
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            // TODO remove
            OpenFile();
        }
        
        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (_player != null)
                _player.Dispose();
        }

        private void btn_ShowContent_Click(object sender, RoutedEventArgs e)
        {
            string extension = txt_MidiFilePath.Text.Split('.').Last();

            if (extension == "mid")
            {
                ShowMidiTracks(MidiReader.ReadMidi(txt_MidiFilePath.Text));
                //MidiConverter midiConverter = new MidiConverter();
                //MyMusicSheet mss = midiConverter.convertMidi(txt_MidiFilePath.Text);
                //ShowTrack(mss.Tracks[1], mss.TimeSignature[0], mss.TimeSignature[1]);
                MidiADPConverter midiConverter = new MidiADPConverter();
                ADPSheet sheet = midiConverter.ReadFile(txt_MidiFilePath.Text);
                ShowSheetVisualisation(sheet.Tracks[1]);
            }
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
            if(needsSaving)
            {
                string sMessageBoxText = "You haven't saved your progress. Do you want to save this before closing?";
                string sCaption = "Save";

                MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

                switch (rsltMessageBox)
                {
                    case MessageBoxResult.Yes:
                        SaveFileToLilypond();
                        _outputDevice.Close();
                        if (_player != null)
                        {
                            _player.Dispose();
                        }
                        break;

                    case MessageBoxResult.No:
                        Console.WriteLine("CLOSING");
                        _outputDevice.Close();
                        if (_player != null)
                        {
                            _player.Dispose();
                        }
                        break;
                }
            }
        }

        public void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Midi Files(.mid)|*.mid|Lily files (*.ly*)|*.ly*" };
            if (openFileDialog.ShowDialog() == true)
            {
                txt_MidiFilePath.Text = openFileDialog.FileName;
                ADPSheet sheet = firstFileConverter.Handle(openFileDialog.FileName);
                if(sheet != null)
                {
                    ShowSheetVisualisation(sheet.getTrack());
                    NoteToLilypondConverter ntlc = new NoteToLilypondConverter();
                    lilypondText.Text = ntlc.getLilypond(sheet);
                    SetNewState();
                }
            }
        }

        public void SaveFileToLilypond()
        {
            fileExporter.SaveAsLilypond(lilypondText.Text);
            SaveAsLilypond saveAsLilypond = new SaveAsLilypond(lilypondText.Text);
            needsSaving = false;
        }

        public void SaveFileToPdf()
        {
            fileExporter.LilypondToPDF(lilypondText.Text);
        }

        public string UpdateBarlinesFromLilypond()
        {
            return this.lilypondText.Dispatcher.Invoke(
                () =>
                {
                    ADPSheet sheet = lycon.ConvertText(lilypondText.Text);
                    if (sheet != null)
                    {
                        ShowSheetVisualisation(sheet.getTrack());
                    }
                    return this.lilypondText.Text;
                }
            );
        }

        public void AddTekstAtSelection(string _text)
        {
            int selectionStart = lilypondText.SelectionStart;
            lilypondText.SelectedText = _text;
            SetNewState();
        }

        private void OnKeyDown(object sender, RoutedEventArgs e)
        {
            keyHandler.OnKeyPressed();
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Back)
            {
                keyHandler.OnKeyPressed();
            }
        }

        public void SetNewState()
        {
            needsSaving = true;
            originator.setState(lilypondText.Text);
            careTaker.add(originator.storeInMemento());
            savedMementos++;
            currentMemento++;

            ReEvaluateButtons();
        }

        public void AddBarlinesToEditor()
        {
            int[] timeSignature = new int[2];

            string[] lilypondContents = lilypondText.Text.Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray(); // get time signature
            for (int i = 0; i < lilypondContents.Length; i++)
            {
                lilypondContents[i] = lilypondContents[i].Replace("\r\n", string.Empty);
                lilypondContents[i] = lilypondContents[i].Replace("\n", string.Empty);
                if (lilypondContents[i].Contains("time"))
                {
                    string str = lilypondContents[i + 1];
                    timeSignature[0] = (int)Char.GetNumericValue(str[0]);
                    timeSignature[1] = (int)Char.GetNumericValue(str[2]);
                }
            }

            if (timeSignature != null)
            {
                string selectedpart = lilypondText.SelectedText;
                double barLengthCounter = 0;
                string[] notes = selectedpart.Split(' ');
                string result = "";

                double singleBeat = (double)(1.0 / timeSignature[1]);
                double lengthPerBar = timeSignature[0] * singleBeat;

                if (selectedpart == "" || selectedpart.Contains("{") || selectedpart.Contains("}") || selectedpart.Contains("\\"))
                {
                    return;
                }

                for (int x = 0; x < notes.Length; x++)
                {
                    string durationValue = Regex.Match(notes[x], @"\d+").Value;
                    if (durationValue != "")
                    {
                        int duration = Int32.Parse(durationValue);
                        double tempNoteLength = 0;
                        result += notes[x] + " ";

                        switch (duration)
                        {
                            case 1:
                                tempNoteLength = 1.0;
                                break;
                            case 2:
                                tempNoteLength += 0.5;
                                break;
                            case 4:
                                tempNoteLength += 0.25;
                                break;
                            case 8:
                                tempNoteLength = 0.125;
                                break;
                            case 16:
                                tempNoteLength = 0.0625;
                                break;
                        }
                        if (notes[x].Contains("."))
                        {
                            tempNoteLength = (double)((double)tempNoteLength + (double)(tempNoteLength / 2));
                        };

                        barLengthCounter = (double)((double)barLengthCounter + (double)tempNoteLength);
                        if (barLengthCounter >= lengthPerBar)
                        {
                            if (notes[x + 1].Contains("|\n"))
                            {
                                barLengthCounter = 0;
                            }
                            else
                            {
                                result += " | ";
                                barLengthCounter = 0;
                            }
                        }
                    }
                }

                string s = lilypondText.Text.Replace(selectedpart, result);
                lilypondText.Text = s;
            }
        }
    }
}
