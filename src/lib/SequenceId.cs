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
	public class SequenceId
	{
		byte id;

		const byte maxRange = 128;
		public const byte MaxValue = 127;

		public static SequenceId Max = new SequenceId(MaxValue);

		public SequenceId(byte id)
		{
			if (!IsValid(id))
			{
				throw new Exception("Illegal SequenceID:" + id);
			}
			this.id = id;
		}

		public byte Value
		{
			get
			{
				return id;
			}
		}

		public SequenceId Next()
		{
			var nextValue = (byte)((id + 1) % maxRange);

			return new SequenceId(nextValue);
		}

		static bool IsValid(byte id)
		{
			return id < maxRange;
		}

		public int Distance(SequenceId otherId)
		{
			var nextValue = (int) otherId.id;
			var idValue = (int) id;

			if (nextValue < idValue)
			{
				nextValue += maxRange;
			}
			var diff = nextValue - idValue;

			return diff;
		}

		public bool IsValidSuccessor(SequenceId nextId)
		{
			var distance = Distance(nextId);

			return (distance != 0) && (distance < (maxRange / 2));
		}
	}
}
