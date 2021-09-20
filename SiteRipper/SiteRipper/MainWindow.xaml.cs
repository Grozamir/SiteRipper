using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Windows.Threading;
using System.Security.Permissions;

namespace SiteRipper
{
    public partial class MainWindow : Window
    {
        Stopwatch stopwatch = new Stopwatch();
        string path=(@"c:\temp\SiteRipper.txt");
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextSite.Text=null;

            string inputSite = InputedSite.Text;

            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(DateTime.Now.ToString("HH:mm:ss tt") + inputSite);
                }
            }
            else
            {
                Log(inputSite);
            }
            

            WebClient client = new WebClient();
            try
            {
                byte[] sourceSite = client.DownloadData(inputSite);
                string textSite;
                inputSite = System.Text.Encoding.UTF8.GetString(sourceSite);

                //Get only text
                textSite = Regex.Replace(inputSite, "<.*?>", String.Empty);
                textSite = Regex.Replace(textSite, "&\\S+;", " ");

                if (CheckBoxСonditionsNum.IsChecked == false)
                {
                    textSite = Regex.Replace(textSite, "\\d", " ");
                }
                if (CheckBoxСonditionsOneWork.IsChecked == false)
                {
                    textSite = Regex.Replace(textSite, "\\s\\D\\s", " ");
                }

                textSite = Regex.Replace(textSite, "\\W", " ");
                textSite = Regex.Replace(textSite, "\\s+", " ");
                textSite = textSite.Trim(' ');
                textSite = textSite.ToLower();
                //----------------------------------------------------------------

                Log("Link valid!");

                stopwatch.Start();

                string[] words = textSite.Split(' ');
                FindDuplicates(words);
            }
            catch
            {
                Log("Link not valid!");
                TextSite.Text = "Неправельная ссылка!Попробуйте ввести ещё раз!";
            }
        }
        private void FindDuplicates(string[] works)
        {
            HashSet<string> foundStrings = new HashSet<string>();
            HashSet<string> duplicates = new HashSet<string>();

            foreach (var item in works)
            {
                if (foundStrings.Contains(item))
                {
                    duplicates.Add(item);
                }
                else
                {
                    foundStrings.Add(item);
                }

                DoEvents();
            }

            string tempWork;
            string[] tempArray = duplicates.ToArray();

            for (int i = 0; i < tempArray.Length-1; i++)
            {
                for (int j = i+1; j < tempArray.Length; j++)
                {
                    if (works.Count(p => tempArray[i] == p)< works.Count(p => tempArray[j] == p))
                    {
                        tempWork = tempArray[i];
                        tempArray[i] = tempArray[j];
                        tempArray[j] = tempWork;
                    }
                    DoEvents();
                }
                DoEvents();
            }

            duplicates.Clear();
            for (int i = 0; i < tempArray.Length; i++)
            {
                duplicates.Add(tempArray[i]);

                DoEvents();
            }

            foreach (var item in duplicates)
            {
                TextSite.Text += "\n" + item + " - " + works.Count(i => item == i);

                DoEvents();

            }
            foreach (var item in foundStrings)
            {
                TextSite.Text += "\n" + item + " - 1";

                DoEvents();
            }


            stopwatch.Stop();
            Log(stopwatch.ElapsedMilliseconds + "ms");

        }
        //This method is needed so that the cycle does not crash !!!
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }
        public object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;

            return null;
        }
        //----------------------------------------------------------------
        private void Log(string s)
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(DateTime.Now.ToString("[HH:mm:sstt] ") + s);
            }
        }

        private void InputedSite_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
