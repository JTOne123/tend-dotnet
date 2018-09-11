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
using System.Collections.Generic;

namespace Piot.Tend.Client
{
	public class OutgoingLogic
	{
		SequenceId lastReceivedByRemoteSequenceId = SequenceId.Max;
		readonly Queue<DeliveryInfo> receivedQueue = new Queue<DeliveryInfo>();
		SequenceId outgoingSequenceId = SequenceId.Max;

		public void ReceivedByRemote(Header header)
		{
			var nextId = header.SequenceId;

			var distance = lastReceivedByRemoteSequenceId.Distance(nextId);
			if (distance == 0)
			{
				return;
			}

			if (!lastReceivedByRemoteSequenceId.IsValidSuccessor(nextId))
			{
				throw new UnorderedPacketException("Outgoing Unordered packets. Duplicates and old packets should be filtered in other layers.", lastReceivedByRemoteSequenceId, nextId);
			}

			var receivedId = new SequenceId(lastReceivedByRemoteSequenceId.Value);
			receivedId = receivedId.Next();
			var bits = new MutableReceiveMask(header.ReceivedBits, distance);
			for (var i=0; i<distance; ++i)
			{
				var wasReceived = bits.ReadNextBit();
				Append(receivedId, wasReceived.IsOn);
				receivedId = receivedId.Next();
			}
			lastReceivedByRemoteSequenceId = nextId;
		}

		public bool CanIncrementOutgoingSequence
		{
			get
			{
				return lastReceivedByRemoteSequenceId.Distance(outgoingSequenceId) < ReceiveMask.Range;
			}
		}

		public SequenceId IncreaseOutgoingSequenceId()
		{
			if (!CanIncrementOutgoingSequence)
			{
				throw new Exception("Can not increase sequence ID. Range:" + outgoingSequenceId.Distance(lastReceivedByRemoteSequenceId) + " id:" + outgoingSequenceId);
			}
			outgoingSequenceId = outgoingSequenceId.Next();

			return outgoingSequenceId;
		}

		public SequenceId OutgoingSequenceId
		{
			get
			{
				return outgoingSequenceId;
			}
		}

		public int Count
		{
			get
			{
				return receivedQueue.Count;
			}
		}

		public DeliveryInfo Dequeue()
		{
			return receivedQueue.Dequeue();
		}

		void Append(SequenceId receivedId, bool bit)
		{
			var info = new DeliveryInfo
			{
				PacketSequenceId = receivedId,
				WasDelivered = bit
			};

			receivedQueue.Enqueue(info);
		}
	}
}
