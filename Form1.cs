using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;
namespace SatelliteTerminal
{
    public partial class Form1 : Form
    {
        SerialPort port = new SerialPort("COM1", 115200, Parity.None);

        public Form1()
        {
            InitializeComponent();            
        }
        
        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void read_gpgga(SerialPort p, string s)
        {
            int len = 0, i = 0;
            while (s[i] != '\n' && s[i] != '\r')
            {
                if (s.Length - 1 == i && s[i] != '\n' && s[i] != '\r')
                    s += p.ReadChar();
                i++;
            }

            while (s[i - len] != '$' && len <= i)
                len++;
            if (len > i)
                return;
            string gpgga = s.Substring(i - len, len);
            Console.Write(gpgga);
            string[] tokens = gpgga.Split(',', '*');
            string
                header = tokens[0],
                utc_str = tokens[1],
                Latitude = tokens[2],
                latDir = tokens[3],
                Longitude = tokens[4],
                longdir = tokens[5],
                quality = tokens[6],
                satcnt = tokens[7],
                hdop = tokens[8],
                alt = tokens[9],
                alt_units = tokens[10],
                undulation = tokens[11],
                undulation_units = tokens[12],
                age = tokens[13],
                st_id = tokens[14],
                chksum = tokens[15];
            tabPage5.Invoke((MethodInvoker)delegate
            {
                label4.Text = st_id;
                label17.Text = string.Format("{0} {1}", Latitude, latDir);
                label16.Text = string.Format("{0} {1}", Longitude, longdir);
                label10.Text = string.Format("{0} {1}", alt, alt_units);
                label29.Text = string.Format("UTC {0}", utc_str);
            });
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            listBox1.Items.AddRange(ports);
        }
        private void button11_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (port.IsOpen)
                    port.Close();
                // настройки порта
                port.PortName = listBox1.SelectedItem.ToString();
                port.ReadTimeout = 2000;
                port.WriteTimeout = 1000;
                if (port.IsOpen)
                    port.Close();
                port.Open();
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    if (i != tabControl1.SelectedIndex)
                        ((Control)tabControl1.TabPages[i]).Enabled = true;
                }
            }
            catch (Exception a)
            {
                Console.WriteLine("ERROR: невозможно открыть порт:" + a.ToString());

                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    if (i != tabControl1.SelectedIndex)
                        ((Control)tabControl1.TabPages[i]).Enabled = false;
                }
                return;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.Items.AddRange(SerialPort.GetPortNames());
            if (port == null)
            {
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    if (i != tabControl1.SelectedIndex)
                        ((Control)tabControl1.TabPages[i]).Enabled = false;
                }
            }
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("Не найдено ни одного порта, проверте подключение");
                return;
            }
            listBox1.SelectedIndex = 0;
            port.DataReceived += Port_DataReceived;
        }
        
        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            richTextBox1.Invoke((MethodInvoker)delegate
            {
                var s = port.ReadExisting();
                richTextBox1.Text += s;
                if (s.ToLower().Contains("$gpgga"))
                    read_gpgga((SerialPort)sender, s);
            });
        }

        byte[] parsestr(string s)
        {
            int i = 0;
            LinkedList<Byte> ret = new LinkedList<byte>();
            while (i < s.Length)
            {
                if (s[i] == '\\' && i + 1 < s.Length)
                {
                    switch (s[i + 1])
                    {
                        case 'n':
                            ret.AddLast((byte)'\n');
                            break;
                        case 't':
                            ret.AddLast((byte)'\t');
                            break;
                        case 'r':
                            ret.AddLast((byte)'\r');
                            break;
                        case 'a':
                            ret.AddLast((byte)'\a');
                            break;
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case 'A':
                        case 'B':
                        case 'C':
                        case 'D':
                        case 'E':
                        case 'F':
                            string e = "";
                            e += s[i + 1];
                            ret.AddLast(Convert.ToByte(e, 16));
                            break;

                        case '\\':
                            ret.AddLast((byte)'\\');
                            break;
                    }
                    i++;
                }
                else
                    ret.AddLast((byte)s[i]);
                i++;

            }
            return ret.ToArray();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (port == null || !SerialPort.GetPortNames().Contains(port.PortName))
                return;
            if (!port.IsOpen)
                port.Open();
            if (e.KeyCode == Keys.Enter && !e.Control)
            {
                byte[] s = parsestr(textBox1.Text);
                richTextBox1.Text += Encoding.Default.GetString(s);
                port.Write(s, 0, s.Length);
                return;
            }
            if (e.KeyCode == Keys.Enter && e.Control)
            {
                byte[] data = null;
                if (openFileDialog1.ShowDialog() != DialogResult.OK)
                    return;
                data = System.IO.File.ReadAllBytes(openFileDialog1.FileName);
                port.Write(data, 0, data.Length);
                return;
            }
            if (e.KeyCode == Keys.Escape)
            {
                richTextBox1.Text = "";
                if (port.IsOpen)
                {
                    port.Close();
                    port.Open();
                }
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void radioButton11_CheckedChanged(object sender, EventArgs e)
        {
            //e
            port.Parity = Parity.Even;

            if (port.IsOpen)
                port.Close();
            if (SerialPort.GetPortNames().Contains(port.PortName))
                port.Open();
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            // 9600
            port.BaudRate = 9600;

            if (port.IsOpen)
                port.Close();
            if (SerialPort.GetPortNames().Contains(port.PortName))
                port.Open();
        }

        private void radioButton13_CheckedChanged(object sender, EventArgs e)
        {
            // 57600
            port.BaudRate = 57600;

            if (port.IsOpen)
                port.Close();
            if (SerialPort.GetPortNames().Contains(port.PortName))
                port.Open();
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            // 56000
            port.BaudRate = 56000;

            if (port.IsOpen)
                port.Close();
            if (SerialPort.GetPortNames().Contains(port.PortName))
                port.Open();
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            //115200
            port.BaudRate = 115200;

            if (port.IsOpen)
                port.Close();
            if (SerialPort.GetPortNames().Contains(port.PortName))
                port.Open();
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            //256000
            port.BaudRate = 256000;

            if (port.IsOpen)
                port.Close();
            if (SerialPort.GetPortNames().Contains(port.PortName))
                port.Open();
        }

        private void radioButton12_CheckedChanged(object sender, EventArgs e)
        {
            //n
            port.Parity = Parity.None;

            if (port.IsOpen)
                port.Close();
            if (SerialPort.GetPortNames().Contains(port.PortName))
                port.Open();
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            // o
            port.Parity = Parity.Odd;

            if (port.IsOpen)
                port.Close();
            if (SerialPort.GetPortNames().Contains(port.PortName))
                port.Open();
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            //s
            port.Parity = Parity.Space;

            if (port.IsOpen)
                port.Close();
            if (SerialPort.GetPortNames().Contains(port.PortName))
                port.Open();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (port.IsOpen)
            {
                port.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            port.DataReceived += gpgga_parser;
        }

        private void gpgga_parser(object sender, SerialDataReceivedEventArgs e)
        {
            
        }
    }
}
