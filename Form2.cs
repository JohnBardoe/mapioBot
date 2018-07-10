using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapioBot
{
    public partial class Form2 : Form
    {
        public Point mouseLoc;
        public Form2()
        {
            InitializeComponent();
            this.Update();
            textBox1.Text = SessionSettings.Default.ip;
            textBox2.Text = SessionSettings.Default.rps.ToString();
        }
        private void exitForm()
        {
            this.Owner.Show();
            this.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            exitForm();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseLoc = new Point(-e.X, -e.Y);
        }
        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (!(c > '0' || c < '9'|| c == '.' || c == ','))
                    return false;
            }
            return true;
        }
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePose = Control.MousePosition;
                mousePose.Offset(mouseLoc.X, mouseLoc.Y);
                Location = mousePose;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string ipRaw = textBox1.Text;
            ipRaw.Replace("https://", "");
            ipRaw.Replace("http://", "");
            if (ipRaw[ipRaw.Length - 1] == '/')
                ipRaw = ipRaw.Remove(ipRaw.Length - 1);
            if (ipRaw.Length > 8)
                SessionSettings.Default.ip = ipRaw;
            else
            {
                MessageBox.Show("Ip can't be null");
                return;
            }
            if (!IsDigitsOnly(textBox2.Text))
                MessageBox.Show("Delay must consist of digits only");
            else if(textBox2.Text.Length == 0)
            {
                MessageBox.Show("Delay can't be null");
                return;
            }
            else
            {
                double dst;
                if (Double.TryParse(textBox2.Text, out dst))
                    SessionSettings.Default.rps = dst;
                else
                    MessageBox.Show("Failed parsing double (check rps textbox)");         
                SessionSettings.Default.Save();
                exitForm();
            }
        }
    }
}
