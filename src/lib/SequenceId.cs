/*

MIT License

Copyright (c) 2017 Peter Bjorklund

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/
using System;

namespace Piot.Tend.Client
{
	/// <summary>
	/// Represents an auto incrementing sequence identifier. Usually a value between 0 and 127 (7 bits). After 127 it wraps around to 0.
	/// </summary>
	public class SequenceId
	{
		const byte maxRange = 128;
		public const byte MaxValue = 127;

		public static SequenceId Max = new SequenceId(MaxValue);

		/// <summary>
		/// Constructing a SequenceId
		/// </summary>
		/// <exception cref="System.Exception">Thrown when the provided id is not between 0 and 127.</exception>
		public SequenceId(byte id)
		{
			if (!IsValid(id))
			{
				throw new Exception("Illegal SequenceID:" + id);
			}
			Value = id;
		}

		public byte Value	
		{
			get;
		}

		/// <summary>
		/// Returns the next SequenceId. Note that the value wraps around 127.
		/// </summary>
		public SequenceId Next()
		{
			var nextValue = (byte)((Value + 1) % maxRange);

			return new SequenceId(nextValue);
		}

		static bool IsValid(byte id)
		{
			return id < maxRange;
		}

		/// <summary>
		/// Returns the closest distance between the otherId and this SequenceId.
		/// </summary>
		public int Distance(SequenceId otherId)
		{
			var nextValue = (int) otherId.Value;
			var idValue = (int) Value;

			if (nextValue < idValue)
			{
				nextValue += maxRange;
			}
			var diff = nextValue - idValue;

			return diff;
		}

		/// <summary>
		/// Checks if the nextId comes after this SequenceId.
		/// </summary>
		public bool IsValidSuccessor(SequenceId nextId)
		{
			var distance = Distance(nextId);

			return (distance != 0) && (distance < (maxRange / 2));
		}

		public override string ToString()
		{
			return string.Format($"[SequenceId {Value}]");
		}
	}
}
