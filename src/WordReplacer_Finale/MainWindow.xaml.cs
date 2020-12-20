using System;
using System.Collections.Generic;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Text.RegularExpressions;
using Xceed.Document;
using Xceed.Words;
using System.Threading;
using System.ComponentModel;




namespace WordReplacer_Finale
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private CommonOpenFileDialog dialog = new CommonOpenFileDialog();//this is the folder browse dialog
        private string folder_path;
        private string replaced_word;
        private bool closePbWindow;
        string[] files;
        private int filePercantage = 0;

        private string new_word;
        private bool isCaseSenstive;
        public static ProgressBar2 pb = new ProgressBar2();
        public BackgroundWorker backgroundWorker1 = new BackgroundWorker();



        public MainWindow()
        {

            InitializeComponent();

        }


        //open the folder dialog and save the folder path
        private void Browse_Click(object sender, EventArgs e)
        {
            dialog.InitialDirectory = @"C:\";//This is were the browse dialog will start
            dialog.EnsureReadOnly = true;
            dialog.IsFolderPicker = true;//The user will be able to choose folders
            dialog.ShowDialog();
            try
            {
                folder_path = dialog.FileName;//get the choosen folder
                ChooseFolderBox.Text = folder_path; //Change the dialog text box for the choosen folder
            }
            catch { }

        }
        private void onClosing(object sender, System.ComponentModel.CancelEventArgs q)
        {
            pb.Close();
            this.backgroundWorker1.CancelAsync();
            

        }
        private void required_field()
        {
            MessageBox.Show("You must fill all the field before replacing", "Required field", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        //Excute this code when Submit button is clicked 
        public void Submit_Click(object sender, EventArgs l)
        {
            string _space = " ";
            //Get values from text boxes
            replaced_word = _space + ReplacedWord.Text + _space;

            new_word = _space + NewWord.Text + _space;

            isCaseSenstive = case_sesntive.IsChecked ?? false;
            //making an instence of the backgorundworker methods
            try
            {

                this.backgroundWorker1.WorkerReportsProgress = true;//Enable the program to report the progress to the progressbar
                this.backgroundWorker1.WorkerSupportsCancellation = true;//Enable Cancellation
                this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
                this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);


                if (String.IsNullOrEmpty(replaced_word) || String.IsNullOrEmpty(new_word) || String.IsNullOrEmpty(this.folder_path))//This will execute if there's an empty field
                {
                    required_field();
                }
                else
                {
                    files = Directory.GetFiles(folder_path, "*.docx", SearchOption.AllDirectories);//Get all of the files from the choosen directory
                    if (check_files())//check if there's any docx files
                    {
                        pb.Show();//show the progress bar
                        pb.Visibility = Visibility.Visible;
                        submitButton.IsEnabled = false;
                        backgroundWorker1.RunWorkerAsync();//call DoWord method 
                        cancelButton.IsEnabled = true;

                    }
                }

            }
            catch
            {
            }
        }




        public void cancel_process()
        {
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.CancelAsync();

            closePbWindow = true;


        }

        public bool popUpControl(string file)
        {
            //if a file didn't open this will show up as a popup message
            MessageBoxResult result = MessageBox.Show($"The following file is broken: {file}", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            //if user clicked at "ok" button the method will return true, if he closed the messagebox it will return false and reset the program
            Console.WriteLine(result);
            if (result == MessageBoxResult.OK)
                return true;
            else
            {
                cancel_process();
                return false;
            }

        }

        public void reset_buttons()//this method is called when the replace process is finished or cancel button has been pressed
        {
            //Enable the submit button and disable the cancel button
            submitButton.IsEnabled = true;
            cancelButton.IsEnabled = false;
        }
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs p)
        {
            if (closePbWindow)
            {

                closePbWindow = false;
                pb.Visibility = Visibility.Hidden;
                pb.pbStatus.Value = 0;
            }
            else
                //Update progressbar  value
                pb.pbStatus.Value = Convert.ToDouble(p.ProgressPercentage);

        }
        public void Cancel_OnClick(object sender, RoutedEventArgs e)//Cancel the replacing process
        {
            reset_buttons();
            this.backgroundWorker1.CancelAsync();
        }
        public bool check_files()
        {

            if (files.Length == 0)
            {
                MessageBoxResult empty = MessageBox.Show($"There is no word docs file found", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            bool checkEValue = true;
            BackgroundWorker worker = sender as BackgroundWorker;
            double filesCount = files.Length;
            double counter = 0;
            //if there were no files in that directory the below messagebox will show, else it will continue replacing files

            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                foreach (string file in files)
                {
                    if (!this.backgroundWorker1.CancellationPending)
                    {
                        try
                        {
                            if (backgroundWorker1.CancellationPending)
                            {
                                closePbWindow = true;
                                worker.ReportProgress(0);
                                e.Cancel = true;

                            }
                            //Replace process  

                            var doc = Xceed.Words.NET.DocX.Load(file);//load the file
                            if (!isCaseSenstive)
                            {
                                doc.ReplaceText(searchValue: replaced_word, newValue: new_word, false, RegexOptions.IgnoreCase);//this method will replace the words in files
                            }
                            else
                            {
                                doc.ReplaceText(searchValue: replaced_word, newValue: new_word, false);
                            }

                            doc.Save();
                            counter++;//number of files that have been replaced

                            filePercantage = Convert.ToInt32((counter / filesCount) * 100);

                            worker.ReportProgress(filePercantage);//Update the number of files that have been replaced.

                        }
                        //this error shows when a file is broken
                        catch (System.IO.FileFormatException c)
                        {
                            checkEValue = popUpControl(file);

                            //if the user clicked "ok" then the below if condition will execute other wise the loop will break
                            if (checkEValue == true)
                            {
                                //if the  broken file was the last then it will update the progressbar and break the loop else it will continue
                                if (counter + 1 == filesCount)
                                {

                                    worker.ReportProgress(100);
                                    this.backgroundWorker1.CancelAsync();
                                    e.Cancel = true;
                                    break;



                                }
                                else
                                {
                                    counter++;
                                    continue;
                                }

                            }


                            else
                            {
                                worker.ReportProgress(0);

                                break;
                            }
                        }
                        catch (System.IO.IOException s)
                        {
                            Console.WriteLine(counter);
                            Console.WriteLine(filesCount);
                            if (counter + 1 == filesCount)
                            {
                                Console.WriteLine(1);
                                worker.ReportProgress(100);
                                this.backgroundWorker1.CancelAsync();
                                e.Cancel = true;
                                break;
                            }
                            else
                            {
                                MessageBox.Show($"Can't open  {file} because its been used by another process", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                counter++;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        closePbWindow = true;
                        e.Cancel = true;
                        worker.ReportProgress(0);
                    }
                }

            }




            this.Dispatcher.Invoke(() =>
            {

                this.backgroundWorker1.CancelAsync();
                e.Cancel = true;
                reset_buttons();
            });

        }

    }
}
