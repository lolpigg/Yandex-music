using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Yandex.Music
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> filenames = new List<string>();
        List<string> filepaths = new List<string>();
        bool Playing = false;
        bool again = false;
        bool RandomOn = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void audioSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Media.Position = new TimeSpan(Convert.ToInt64(audioSlider.Value));
        }

        private void Media_MediaOpened(object sender, RoutedEventArgs e)
        {
            audioSlider.Maximum = Media.NaturalDuration.TimeSpan.Ticks;
            long a = (Media.NaturalDuration.TimeSpan.Ticks);
            double b = (new TimeSpan(a)).TotalSeconds;
            int minutes = (int)(b / 60);
            int seconds = (int)(b % 60);
            int hours = (int)(minutes / 60);
            minutes = (int)(minutes % 60);
            AllTime.Text = (hours + ":" + minutes + ":" + seconds);
        }

        private void MusicPath_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                filenames.Clear();
                filepaths.Clear();
                Clearing();
                string name = "";
                string[] filepaths1 = Directory.GetFiles(dialog.FileName, @"*.mp3");
                filepaths1 = filepaths1.Concat(Directory.GetFiles(dialog.FileName, @"*.m4a")).ToArray();
                filepaths1 = filepaths1.Concat(Directory.GetFiles(dialog.FileName, @"*.wav")).ToArray();
                foreach (var item in filepaths1)
                {
                    filepaths.Add(item);
                }
                foreach (string file in filepaths)
                {
                    for (int i = 0; i < file.Length; i++)
                    {
                        if (file[i] == '\\')
                        {
                            name = "";
                        }
                        else
                        {
                            name += file[i];
                        }
                    }
                    filenames.Add(name);
                }
            }
            filepaths.Sort();
            filenames.Sort();
            foreach (var name in filenames)
            {
                ListBox1.Items.Add(name);
            }
            Media.Source = new Uri(filepaths[0]);
            Media.Play();
            Playing = true;
            soundSlider.Value = 10;
            ListBox1.SelectedIndex = 0;
        }

        private void soundSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Media.Volume = soundSlider.Value / 10;
        }

        private void PlayBut_Click(object sender, RoutedEventArgs e)
        {
            if (Media.CanPause && Playing)
            {
                Media.Pause();
                Playing = false;
            }
            else if (!Playing && filepaths.Count() != 0)
            {
                Media.Play();
                Playing = true;
            }
        }

        private void Back1_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox1.SelectedIndex > 0)
            {
                ListBox1.SelectedIndex--;
                ChangeMusic();
            }
        }
        private void ChangeMusic()
        {
            if (ListBox1.SelectedIndex != -1)
            {
                Media.Source = new Uri(filepaths[ListBox1.SelectedIndex]);
                audioSlider.Value = 0;
                Media.Play();
                Potochek();
            }
        }

        private void ListBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeMusic();
        }

        private void Skip1_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox1.SelectedIndex < filepaths.Count)
            {
                ListBox1.SelectedIndex++;
                ChangeMusic();
            }
        }

        private void Again1_Click(object sender, RoutedEventArgs e)
        {
            if (!again)
            {
                Again1.BorderBrush = Brushes.DarkBlue;
                again = true;
            }
            else
            {
                Again1.BorderBrush = Brushes.White;
                again = false;
            }
        }

        private async void Potochek()
        {
            while (true)
            {
                await Task.Delay(1000);
                Now.Text = Math.Floor(Media.Position.TotalHours).ToString() + ":" + (Math.Floor(Media.Position.TotalMinutes)%60).ToString() + ":" + (Math.Floor(Media.Position.TotalSeconds)%60).ToString();
                audioSlider.Value = Media.Position.Ticks;
            }
        }
        private void Media_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (again) ChangeMusic();
            else if (ListBox1.SelectedIndex < filepaths.Count)
            {
                ListBox1.SelectedIndex++;
                ChangeMusic();
            }
        }

        private void Randm_Click(object sender, RoutedEventArgs e)
        {
            if (!RandomOn)
            {
                Random random = new Random();
                int n = filepaths.Count;
                while (n > 1)
                {
                    n--;
                    int i = random.Next(n + 1);
                    string temp = filepaths[i];
                    filepaths[i] = filepaths[n];
                    filepaths[n] = temp;
                    temp = filenames[i];
                    filenames[i] = filenames[n];
                    filenames[n] = temp;
                }
                Clearing();
                RandomOn = true;
                Randm.BorderBrush = Brushes.DarkBlue;
            }
            else
            {
                filepaths.Sort();
                filenames.Sort();
                Clearing();
                RandomOn = false;
                Randm.BorderBrush = Brushes.White;
            }
        }
        private void Clearing()
        {
            ListBox1.Items.Clear();
            foreach (var item in filenames)
            {
                ListBox1.Items.Add(item);
            }
            Media.Stop();
        }
    }
}
