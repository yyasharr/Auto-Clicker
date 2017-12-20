using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UITesting;
using System.Runtime.InteropServices;

namespace Auto_Clicker
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        int seconds;
        public Form1()
        {
            InitializeComponent();
            ClickJobWorker.WorkerReportsProgress = true;
            ClickJobWorker.WorkerSupportsCancellation = true;
        }



        private void StartButton_Click(object sender, EventArgs e)
        {
            string input = textBox1.Text;

            int n = int.MaxValue;

            if (int.TryParse(input, out n) && n > 0)
            {
                seconds = n;
                label2.Text = "Running";
                label2.ForeColor = System.Drawing.Color.Green;
                StartButton.Enabled = false;
                ClickJobWorker.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Please enter a valid number bigger than 1.");
            }

        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            ClickJobWorker.CancelAsync();
        }

        private void ClickJobWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label2.Text = "Stopped";
            label2.ForeColor = System.Drawing.Color.Red;
            StartButton.Enabled = true;
        }

        public void ClickJobWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            

            BackgroundWorker worker = sender as BackgroundWorker;

            while (true)
            {
                for (int i = 0; i < 2*seconds; i++)
                {
                    System.Threading.Thread.Sleep(500); //wait half a second and each time, check if cancel has been clicked
                    if (ClickJobWorker.CancellationPending == true)
                    {
                        ClickJobWorker.CancelAsync();
                        e.Cancel = true;
                        return;
                    }
                }
                MouseClicker();
            }
            

        }
        void MouseClicker()
        {
            //Call the imported function with the cursor's current position
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }
    }
}
