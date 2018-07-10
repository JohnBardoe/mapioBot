using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using jsonSender;
using System.IO;
using System.Xml;
using System.Threading;

namespace MapioBot
{
    public partial class Form1 : Form
    {
        public Point MouseLoc;
        private volatile bool _stillRunning = false;
        private volatile string base_url = "http://" + SessionSettings.Default.ip;
        public Form1()
        {
            InitializeComponent();
            this.Update();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            MouseLoc = new Point(-e.X, -e.Y);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePose = Control.MousePosition;
                mousePose.Offset(MouseLoc.X, MouseLoc.Y);
                Location = mousePose;
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            AppendTextBox("Wiping started..." + Environment.NewLine);
            await Sender.MakeAsyncRequest("", base_url + "/wipe");
            AppendTextBox("Wiping completed!" + Environment.NewLine);
        }

        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }
            textBox1.AppendText(value);
        }

        private async Task startBotNet()
        {
            string filepath = Environment.CurrentDirectory;
            DirectoryInfo d = new DirectoryInfo(filepath);
            List<Task> t = new List<Task>();
            FileInfo[] dr = d.GetFiles("*.gpx");
            int delay = (int)Math.Truncate(dr.Length/SessionSettings.Default.rps * 1000);
            if (dr.Length == 0)
            {
                MessageBox.Show("No .gpx files in current directory");
                return;
            }
            AppendTextBox("Connecting to server..." + Environment.NewLine);
            foreach (var file in dr)
            {
                t.Add(Task.Run(() =>
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(file.Name);
                    XmlNodeList nList = doc.GetElementsByTagName("trkpt");
                    foreach (XmlNode n in nList)
                    {
                        double lat = XmlConvert.ToDouble(n.Attributes[0].InnerText),
                               lon = XmlConvert.ToDouble(n.Attributes[1].InnerText);
#pragma warning disable CS4014 // meh nevermind i'm just too bored to make it look more elegant
                        Sender.MakeAsyncRequest(new UserCoords //it's not 'async' but rather 'sync' here :/
                        {
                            userID = "228",
                            latitude = lat,
                            longitude = lon
                        }, base_url + "/send_user_coordinates/");

#pragma warning restore CS4014 // ===============================================================
                        AppendTextBox("Movement:" + lat + ':' + lon + ": 228 " + Environment.NewLine);
                        if (!_stillRunning)
                            return;
                        Thread.Sleep(delay);
                    }
                }));
                Thread.Sleep(1000);
            }
            while (t.Count > 0)
            {
                Task firstFinishedTask = await Task.WhenAny(t);
                t.Remove(firstFinishedTask);
                AppendTextBox("Thread finished! " + t.Count + " left!" + Environment.NewLine);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == "Stop")
            {
                _stillRunning = false;
                button3.Text = "Start";
                label1.Text = "Status: Idle";
                textBox1.AppendText("Stopped." + Environment.NewLine);
            }
            else
            {
                _stillRunning = true;
                button3.Text = "Stop";
                label1.Text = "Status: Running...";
                textBox1.AppendText("Starting..." + Environment.NewLine);
                var task = Task.Run(async () => { await startBotNet(); });
            }

        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            MouseLoc = new Point(-e.X, -e.Y);
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePose = Control.MousePosition;
                mousePose.Offset(MouseLoc.X, MouseLoc.Y);
                Location = mousePose;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (_stillRunning)
            {
                MessageBox.Show("Stop the bot to access settings");
                return;
            }
            Form2 f2 = new Form2();
            f2.Owner = this;
            f2.Show();
            this.Hide();
        }
    }
}
