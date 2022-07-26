using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace TestChangePbPageCode
{
    public partial class Form1 : Form
    {
        string dir;
        bool IsEdit;
        bool Start;
        public Form1()
        {
            InitializeComponent();
            dir = System.IO.Directory.GetCurrentDirectory();
        }
        public static int i3FrameworkDxDll;
        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            
        }
        int SHOW;
        private void Initialize()
        {
            Thread Initialize = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if(!IsEdit)
                    {
                        i3FrameworkDxDll = (int)Memory.Read<uint>(Memory.i3FrameworkDx + 0x0298740); 
                        int O1 = (int)Memory.Read<int>(i3FrameworkDxDll + 0x4);
                        int O2 = (int)Memory.Read<int>(O1 + 0x458);
                        int O3 = (int)Memory.Read<int>(O2 + 0x0);
                        int O4 = (int)Memory.Read<int>(O3 + 0x10);
                        int O5 = (int)Memory.Read<int>(O4 + 0x0);
                        int O6 = (int)Memory.Read<int>(O5 + 0x3E0);
                        SHOW = (int)Memory.Read<int>(O6 + 0x14C);
                        if(SHOW == 1254)
                        {
                            Memory.Write<int>(O6 + 0x14C, (int)numericUpDown1.Value);
                        }
                        if(SHOW == numericUpDown1.Value)
                        {
                            Text = "PageCode -> " + SHOW.ToString();
                            label2.Text = "Changed PageCode to " + SHOW + " !";
                            IsEdit = true;
                        }
                    }
                    Thread.Sleep(10);
                }
            }));
            Initialize.SetApartmentState(ApartmentState.STA);
            Initialize.IsBackground = true;
            Initialize.Start();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(Path.Combine(dir, "PointBlank.exe")))
                {
                    if (Memory.Init())
                    {
                        label2.Text = "PointBlank is running! Please close PointBlank and try again";
                        return;
                    }
                    Process.Start("PointBlank.exe", dir);
                    Thread.Sleep(1000);
                    Start = true;
                    TryInit();
                }
                else if (!File.Exists(Path.Combine(dir, "PointBlank.exe")))
                {
                    label2.Text = "Warning!! -- PointBlank not found!";
                    return;
                }
            }
            catch(Exception ex)
            {

            }
        }
        private void TryInit()
        {
            Thread TryGetInit = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if(Start)
                    {
                        if (Memory.WindowsName == "LEUCO SHELL")
                        {
                            //MessageBox.Show("PointBlank is not running!", "Warning", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information);
                            label2.Text = "Warning!! -- PointBlank failed to start!";
                            break;
                        }
                        if (Memory.Init() && Memory.WindowsName != "LEUCO SHELL" && Memory.TryGetModule)
                        {
                            label2.Text = "Trying to change!";
                            Initialize();
                            Start = false;

                        }
                        Thread.Sleep(10);
                    }
                    
                }
            }));
            TryGetInit.SetApartmentState(ApartmentState.STA);
            TryGetInit.IsBackground = true;
            TryGetInit.Start();
        }
    }
}
