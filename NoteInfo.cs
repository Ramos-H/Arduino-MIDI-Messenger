using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoMIDIMessenger
{
	public class NoteInfo
	{
		public int channelNumber;
		public int octave;
		public int semitone;

		public int previousVelocity = 0;
		private int currentVelocity = 0;

		public int zeroCounter = 0;

		public int Velocity
		{
			get { return currentVelocity; }
			set
			{
				previousVelocity = currentVelocity;
				currentVelocity = value;
			}
		}

		public bool isPlaying = false;

		public NoteInfo(int newChannelNumber = 0, int newOctave = 0, int newSemitone = 0)
		{
			channelNumber = newChannelNumber;
			octave = newOctave;
			semitone = newSemitone;
		}
	}
}
