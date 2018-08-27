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
	public class IncomingLogic
	{
		SequenceId lastReceivedToUs = SequenceId.Max;
		uint receiveMask;

		public void ReceivedToUs(SequenceId nextId)
		{
			if (!lastReceivedToUs.IsValidSuccessor(nextId))
			{
				throw new UnorderedPacketException("Unordered packets. Duplicates and old packets should be filtered in other layers.");
			}

			var distance = lastReceivedToUs.Distance(nextId);

			if (distance == 0)
			{
				throw new Exception("Distance should not be zero");
			}

			if (distance > ReceiveMask.Range)
			{
				throw new Exception("too big gap in sequence." + distance);
			}

			for (var i = 0; i < distance - 1; ++i)
			{
				Append(false);
			}
			Append(true);
			lastReceivedToUs = nextId;
		}

		public Header ReceivedHeader
		{
			get
			{
				return new Header(lastReceivedToUs, new ReceiveMask(receiveMask));
			}
		}

		void Append(bool bit)
		{
			receiveMask <<= 1;
			receiveMask |= bit ? (uint)0x1 : (uint)0x0;
		}
	}
}
