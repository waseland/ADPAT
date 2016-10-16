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
            _outputDevice.Close();
            if (_player != null)
            {
                _player.Dispose();
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
            originator.setState(lilypondText.Text);
            careTaker.add(originator.storeInMemento());
            savedMementos++;
            currentMemento++;

            ReEvaluateButtons();
        }

        public void AddBarlinesToEditor() //Werkt op het moment alleen met 4/4 maatsoort
        {
            //int[] timeSignature = new int[2];

            //string[] lilyPondContents = lilypondText.Text.Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray(); // get time signature (skip this, it only works for 4/4)
            //for (int i = 0; i < lilyPondContents.Length; i++)
            //{
            //    lilyPondContents[i] = lilyPondContents[i].Replace("\r\n", string.Empty);
            //    lilyPondContents[i] = lilyPondContents[i].Replace("\n", string.Empty);
            //    if (lilyPondContents[i].Contains("time"))
            //    {
            //        string str = lilyPondContents[i + 1];
            //        timeSignature[0] = (int)Char.GetNumericValue(str[0]);
            //        timeSignature[1] = (int)Char.GetNumericValue(str[2]);


            //    }
            //}

            int e = lilypondText.Text.Length;
            string selectedpart = lilypondText.SelectedText;
            double counter = 0;
            string[] notes = selectedpart.Split(' ');
            string result = "";
            if (selectedpart == "" || selectedpart.Contains("{") || selectedpart.Contains("}") || selectedpart.Contains("\\"))
            {
                return;
            }

            for (int x = 0; x < notes.Length; x++)
            {
                string temp = Regex.Match(notes[x], @"\d+").Value;
                if (temp == "")
                {
                    //stop
                }
                else
                {
                    int d = Int32.Parse(temp);
                    double y = 0;
                    result += notes[x] + " ";

                    switch (d) //works for 4/4
                    {
                        case 1:
                            y += 4;
                            break;
                        case 2:
                            y += 2;
                            break;
                        case 4:
                            y += 1;
                            break;
                        case 8:
                            y += 0.5;
                            break;
                        case 16:
                            y += 0.25;
                            break;
                    }
                    if (notes[x].Contains("."))
                    {
                        y = y + (y / 2);
                    };

                    counter += y;
                    if (counter >= 4) //only works for 4/4
                    {
                        if (notes[x + 1].Contains("|\n"))
                        {
                            counter = 0;
                        }
                        else
                        {
                            result += "| ";
                            counter = 0;
                        }


                    }
                }

            }

            string s = lilypondText.Text.Replace(selectedpart, result);
            lilypondText.Text = s;
        }
    }
}
