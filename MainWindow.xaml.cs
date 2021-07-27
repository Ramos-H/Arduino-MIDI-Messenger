using System;
using System.Collections.Generic;
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
using Sanford.Multimedia.Midi;
using System.IO.Ports;

namespace ArduinoMIDIMessenger
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>

    public partial class MainWindow : Window
    {
		public List<VirtualMidiDevice> virtualMidiDevices = new List<VirtualMidiDevice>();
		public Thread updateThread;
		string[] comPorts = new string[0];

		public MainWindow()
        {
            InitializeComponent();
			comPorts = SerialPort.GetPortNames();
			UpdateComPortDisplayList();
			updateThread = new Thread(Update);
			updateThread.Start();
		}

		public void Update()
		{
			while(true)
			{
				comPorts = SerialPort.GetPortNames();
			}
		}

		void UpdateComPortDisplayList()
		{
			if (COMPPortList.Items.Count > 0)
			{
				COMPPortList.Items.Clear();
			}

			if(comPorts.Length == 0)
			{
				COMPPortList.Items.Add("No devices detected.");
				COMPPortList.IsEnabled = false;
			}
			else
			{
				COMPPortList.Items.Add("No device selected.");
				for (int portIndex = 0; portIndex < comPorts.Length; portIndex++)
				{
					COMPPortList.Items.Add(comPorts[portIndex]);
				}
				COMPPortList.IsEnabled = true;
			}
			COMPPortList.SelectedIndex = 0;
		}

		private void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			AddVirtualDevice();
		}

		public void AddVirtualDevice()
		{
			if (COMPPortList.SelectedIndex > 0)
			{
				VirtualMidiDevice newVirtualMidiDevice = new VirtualMidiDevice(COMPPortList.SelectedValue.ToString());
				virtualMidiDevices.Add(newVirtualMidiDevice);
				VirtualMidiDeviceGuiContainer newVirtualMidiDeviceGui = new VirtualMidiDeviceGuiContainer(VirtualMidiDeviceGuiList.Items.Count, this, newVirtualMidiDevice);
				VirtualMidiDeviceGuiList.Items.Add(newVirtualMidiDeviceGui);
			}
		}

		public void RemoveVirtualDevice(VirtualMidiDeviceGuiContainer virtualDeviceToRemove)
		{
			virtualMidiDevices.Remove(virtualDeviceToRemove.virtualDevice);
			VirtualMidiDeviceGuiList.Items.Remove(virtualDeviceToRemove);
		}

		private void RefreshCOMPortListButton_Click(object sender, RoutedEventArgs e)
		{
			UpdateComPortDisplayList();
		}
	}
}
