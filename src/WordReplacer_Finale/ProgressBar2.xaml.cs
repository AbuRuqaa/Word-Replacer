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
using System.Windows.Shapes;
using System.IO;



namespace WordReplacer_Finale
    
{
   
    public partial class ProgressBar2 : Window
    {
         //this.Closing += onClosing();

            
            
        public bool test = false;  
        MainWindow MainClass = new MainWindow();
         public ProgressBar2()
        {
            InitializeComponent();
        }
        
        private  void onClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            pbStatus.Value = 0;
            this.Hide();
        }

    }
 
}
