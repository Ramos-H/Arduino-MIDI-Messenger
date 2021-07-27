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
using System.IO;
using System.IO.Ports;

namespace ArduinoMIDIMessenger
{
	public class VirtualMidiDevice
	{
		public List<NoteInfo> noteInfos = new List<NoteInfo>();
		public int sensitivityOffset;

		public OutputDevice outputDev;
		public SerialPort sPort;

		public bool isInitialized = false;
		public bool isProcessingNotes = false;
		public bool isIgnoring = false;

		public VirtualMidiDevice(string inputComPort)
		{
			sPort = new SerialPort();
			try
			{
				sPort.PortName = inputComPort;
				sPort.BaudRate = 115200;
				sPort.Parity = Parity.None;
				sPort.StopBits = StopBits.One;
				sPort.DataBits = 8;
				sPort.Handshake = Handshake.None;
				sPort.DtrEnable = true;
				sPort.RtsEnable = true;
				sPort.DataReceived += SPort_DataReceived;
				sPort.Open();
			}
			catch (IOException)
			{
				MessageBox.Show("The serial port could not be accessed! Make sure no other applications are connected to it.", "Error: Serial Port could not be accessed");
			}
			outputDev = new OutputDevice(OutputDevice.DeviceCount - 1);
		}

		private void SPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			if (isInitialized)
			{
				isProcessingNotes = true;
				ReadAndParseAnalogValues();
				SendAnalogNoteToggleEvent();
				isProcessingNotes = false;
			}
		}

		~VirtualMidiDevice()
		{
			outputDev.Dispose();
			noteInfos = null;
			sPort.Close();
			sPort.Dispose();
		}

		public void ReadAndParseAnalogValues()
		{
			string analogInput = String.Empty;
			if(sPort.BytesToRead > 23)
			{
				analogInput = sPort.ReadExisting();
				sPort.DiscardInBuffer();

				if(analogInput.Contains('|'))
				{
					List<string> values = new List<string>(analogInput.Split('|'));
					analogInput = values[values.Count - 2];
				 }
			}
			int[] newAnalogValues = new int[6];//Container for the new values.
			List<string> splittedValues = new List<string>(analogInput.Split(','));//Split the received string based on "|"

			if(splittedValues.Count > 6)
			{
				MessageBox.Show("Something's wrong here.");
				MessageBox.Show(analogInput);
				return;
			}

			//Parse the values from string to int.
			for (int valueIndex = 0; valueIndex < splittedValues.Count; valueIndex++)
			{
				Int32.TryParse(splittedValues[valueIndex], out newAnalogValues[valueIndex]);
			}
			//Update the velocities of each note info.
			for (int newValueIndex = 0; newValueIndex < noteInfos.Count; newValueIndex++)
			{
				int newVelocity = newAnalogValues[newValueIndex];
				noteInfos[newValueIndex].Velocity = newVelocity;
			}
		}

		public void SendAnalogNoteToggleEvent()
		{
			//For every note that's configured
			for (int noteIndex = 0; noteIndex < noteInfos.Count; noteIndex++)
			{
				//Calculate the MIDI note index of the current note/semitone based on the user's selected settings (Octave, and Note/Pitch)
				int currentNoteValue = noteInfos[noteIndex].semitone + 12 * (noteInfos[noteIndex].octave + 1);
				if (currentNoteValue > 127) { currentNoteValue = 127; } //Clamp the value in case it exceeds the number that is allowed.

				int currentvelocity = noteInfos[noteIndex].Velocity;

				//If the user "pressed" a note
				if (noteInfos[noteIndex].Velocity != 0)
				{
					//Check if the note is not playing
					if (!noteInfos[noteIndex].isPlaying)
					{
						//Send the message to the output device to play the note.
						outputDev.Send(new ChannelMessage(ChannelCommand.NoteOn, noteInfos[noteIndex].channelNumber, currentNoteValue, noteInfos[noteIndex].Velocity));
						noteInfos[noteIndex].isPlaying = true; //Set the flag to signal the app that the note is really playing.
					}
					else if(noteInfos[noteIndex].isPlaying)//Check if the note is already playing. If it is, then change the pitch.
					{
						//If the change in velocity is really big, then signal the change to the output device
						outputDev.Send(new ChannelMessage(ChannelCommand.PolyPressure, noteInfos[noteIndex].channelNumber, currentNoteValue, noteInfos[noteIndex].Velocity));
						//outputDev.Send(new ChannelMessage(ChannelCommand.NoteOff, noteInfos[noteIndex].channelNumber, noteInfos[noteIndex].Velocity, 127));
						//outputDev.Send(new ChannelMessage(ChannelCommand.NoteOn, noteInfos[noteIndex].channelNumber, currentNoteValue, noteInfos[noteIndex].Velocity));
					}
					noteInfos[noteIndex].zeroCounter = 0;
				}
				else //If the user "released" a note
				{
					//Flicker check
					if(noteInfos[noteIndex].zeroCounter <= 2)
					{
						noteInfos[noteIndex].zeroCounter++;
					} else
					{
						//If the note is already playing, send a message to the output device to stop playing the note.
						if (noteInfos[noteIndex].isPlaying)
						{
							outputDev.Send(new ChannelMessage(ChannelCommand.NoteOff, noteInfos[noteIndex].channelNumber, currentNoteValue, 127));
							noteInfos[noteIndex].isPlaying = false;
						}
					}
				}
			}
		}
	}
}