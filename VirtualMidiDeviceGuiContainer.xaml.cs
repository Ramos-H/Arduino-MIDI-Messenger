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
using System.Collections.Generic;
using System.Threading;

namespace ArduinoMIDIMessenger
{
	/// <summary>
	/// Interaction logic for Virtual_Midi_Device_Gui_Container.xaml
	/// </summary>
	public partial class VirtualMidiDeviceGuiContainer : UserControl
	{
		public VirtualMidiDevice virtualDevice;
		public MainWindow deviceListHost;

		public VirtualMidiDeviceGuiContainer(int index, MainWindow newDeviceListHost, VirtualMidiDevice newVirtualMidiDevice)
		{
			InitializeComponent();
			VirtualDeviceHeader.Header += index.ToString();
			virtualDevice = newVirtualMidiDevice;
			deviceListHost = newDeviceListHost;
			SerialPortText.Text += virtualDevice.sPort.PortName;
			//Setup the sensitivity values
			for (int sensitivityIndex = 0; sensitivityIndex < 256; sensitivityIndex++)
			{
				SensitivityValueBox.Items.Add(sensitivityIndex.ToString());
			}
			SensitivityValueBox.SelectedIndex = 0;

			//Setup the notes
			for (int noteIndex = 0; noteIndex < 2; noteIndex++)
			{
				AddNoteInfo();
			}

			virtualDevice.isInitialized = true;
		}

		~VirtualMidiDeviceGuiContainer()
		{
			virtualDevice = null;
			deviceListHost = null;
		}

		public void AddNoteInfo()
		{
			if(virtualDevice.noteInfos.Count >= 6)
			{
				MessageBox.Show("Maximum note info count exceeded. Cancelling add note info operation.");
				return;
			}

			int newChannelNumber = 0;
			int newSemitone = 0;
			int newOctave = 5;
			if(virtualDevice.noteInfos.Count > 0)
			{
				newChannelNumber = virtualDevice.noteInfos[virtualDevice.noteInfos.Count - 1].channelNumber;
				newSemitone = virtualDevice.noteInfos[virtualDevice.noteInfos.Count - 1].semitone + 1;
				newOctave = virtualDevice.noteInfos[virtualDevice.noteInfos.Count - 1].octave;
				if(newSemitone > 7)
				{
					newSemitone = 0;
					newOctave++;
				}
			} 
			NoteInfo newNoteInfo = new NoteInfo(newChannelNumber, newOctave, newSemitone);
			virtualDevice.noteInfos.Add(newNoteInfo);
			NoteInfoGui currentNoteInfoGui = new NoteInfoGui(this, newNoteInfo);
			NoteInfoGuiList.Items.Insert(virtualDevice.noteInfos.Count - 1, currentNoteInfoGui);
		}

		public void RemoveNoteInfo(NoteInfoGui noteInfoGuiToRemove)
		{
			virtualDevice.noteInfos.Remove(noteInfoGuiToRemove.currentNoteInfo);
			NoteInfoGuiList.Items.Remove(noteInfoGuiToRemove);
		}

		private void SensitivityIncrementButton_Click(object sender, RoutedEventArgs e)
		{
			SensitivitySlider.Value += 1;
		}

		private void SensitvityDecrementButton_Click(object sender, RoutedEventArgs e)
		{
			SensitivitySlider.Value -= 1;
		}

		private void SensitivitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			SensitivityValueBox.SelectedIndex = Convert.ToInt32(SensitivitySlider.Value);
		}

		private void AddNoteInfoButton_Click(object sender, RoutedEventArgs e)
		{
			AddNoteInfo();
		}

		private void RemoveDeviceButton_Click(object sender, RoutedEventArgs e)
		{
			deviceListHost.RemoveVirtualDevice(this);
		}

		private void AddInputRuleButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
