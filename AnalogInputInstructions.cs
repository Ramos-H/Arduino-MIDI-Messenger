using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sanford.Multimedia.Midi;

namespace ArduinoMIDIMessenger
{
	public class AnalogInputInstructions
	{
		public int previousValue = 0;
		private int currentValue = 0;

		public int zeroCounter = 0;

		public int Value
		{
			get { return currentValue; }
			set
			{
				previousValue = currentValue;
				Value = value;
			}
		}

		public bool isPlaying = false;


	}
}
