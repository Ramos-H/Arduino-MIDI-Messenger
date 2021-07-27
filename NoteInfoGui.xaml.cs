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

namespace ArduinoMIDIMessenger
{
	/// <summary>
	/// Interaction logic for NoteInfoGui.xaml
	/// </summary>
	public partial class NoteInfoGui : UserControl
	{
		public VirtualMidiDeviceGuiContainer virtualDeviceHost;
		public NoteInfo currentNoteInfo = new NoteInfo();

		public NoteInfoGui(VirtualMidiDeviceGuiContainer newVirtualDeviceHost, NoteInfo newNoteInfo)
		{
			InitializeComponent();
			virtualDeviceHost = newVirtualDeviceHost;
			currentNoteInfo = newNoteInfo;
			ChannelNumberList.SelectedIndex = currentNoteInfo.channelNumber;
			OctaveNumberList.SelectedIndex = currentNoteInfo.octave;
			NoteList.SelectedIndex = currentNoteInfo.semitone;
		}

		~NoteInfoGui()
		{
			virtualDeviceHost = null;
			currentNoteInfo = null;
		}

		private void ChannelIncrementButton_Click(object sender, RoutedEventArgs e)
		{
			ChannelNumberList.SelectedIndex += 1;
		}

		private void ChannelDecrementButton_Click(object sender, RoutedEventArgs e)
		{
			ChannelNumberList.SelectedIndex -= 1;
		}

		private void ChannelNumberList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(ChannelNumberList.SelectedIndex < 0)
			{
				ChannelNumberList.SelectedIndex = 0;
			} else if (ChannelNumberList.SelectedIndex > ChannelNumberList.Items.Count - 1)
			{
				ChannelNumberList.SelectedIndex = ChannelNumberList.Items.Count - 1;
			}

			currentNoteInfo.channelNumber = ChannelNumberList.SelectedIndex;
		}

		private void OctaveIncrementButton_Click(object sender, RoutedEventArgs e)
		{
			OctaveNumberList.SelectedIndex += 1;
		}

		private void OctaveDecrementButton_Click(object sender, RoutedEventArgs e)
		{
			OctaveNumberList.SelectedIndex -= 1;
		}

		private void OctaveNumberList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (OctaveNumberList.SelectedIndex < 0)
			{
				OctaveNumberList.SelectedIndex = 0;
			}
			else if (OctaveNumberList.SelectedIndex > OctaveNumberList.Items.Count - 1)
			{
				OctaveNumberList.SelectedIndex = OctaveNumberList.Items.Count - 1;
			}

			currentNoteInfo.octave = OctaveNumberList.SelectedIndex;
		}

		private void NoteIncrementButton_Click(object sender, RoutedEventArgs e)
		{
			NoteList.SelectedIndex += 1;
		}

		private void NoteDecrementButton_Click(object sender, RoutedEventArgs e)
		{
			NoteList.SelectedIndex -= 1;
		}

		private void NoteList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (NoteList.SelectedIndex < 0)
			{
				NoteList.SelectedIndex = 0;
			}
			else if (NoteList.SelectedIndex > NoteList.Items.Count - 1)
			{
				NoteList.SelectedIndex = NoteList.Items.Count - 1;
			}

			currentNoteInfo.semitone = NoteList.SelectedIndex;
		}

		private void RemoveNoteButton_Click(object sender, RoutedEventArgs e)
		{
			virtualDeviceHost.RemoveNoteInfo(this);
		}
	}
}
