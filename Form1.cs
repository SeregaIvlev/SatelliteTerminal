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
        SerialPort port = new SerialPort();

        public Form1()
        {
            InitializeComponent();

            if (port.IsOpen)
            {
                port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            }

            
        }

        [STAThread]

        private void Receive()
        {
            byte[] Buff;
            int k = 0;
            do
            {
                Thread.Sleep(50);
                Buff = new byte[port.BytesToRead];
                port.Read(Buff, 0, port.BytesToRead);
                k++;
            }
            while (Buff.Length == 0 && k < 40);
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

        private void button10_Click(object sender, EventArgs e)
        {
        
        
            
            string[] ports = SerialPort.GetPortNames();
            Console.WriteLine("Выберите порт:");
            listBox1.Items.AddRange(ports);

        }
        private void button11_Click_1(object sender, EventArgs e)
        {
            try
            {
                // настройки порта
                port.PortName = listBox1.SelectedItem.ToString();
                port.BaudRate = 115200;
                port.DataBits = 8;
                port.Parity = System.IO.Ports.Parity.Even;
                port.StopBits = System.IO.Ports.StopBits.One;
                port.ReadTimeout = 2000;
                port.WriteTimeout = 1000;
                port.Open();
                byte[] a = { 0x57 };
                port.Write(a, 0, a.Length);

                
                //port.Close();
            }
            catch (Exception a)
            {
                Console.WriteLine("ERROR: невозможно открыть порт:" + a.ToString());
                return;
            }

        }


        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            tbOutput.Text += port.ReadExisting();
        }
    }
}
