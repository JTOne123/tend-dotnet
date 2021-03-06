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
using Piot.Tend.Client;

using Xunit;

namespace Tests
{
    public static class OutgoingLogicTest
    {
        static OutgoingLogic SetupLogic()
        {
            var logic = new OutgoingLogic();

            return logic;
        }

        static Header SetupHeader(byte sequence, uint mask)
        {
            var sequenceId = new SequenceId(sequence);
            var bitMask = new ReceiveMask(mask);
            var header = new Header(sequenceId, bitMask);

            return header;
        }

        [Fact]
        public static void FirstReceive()
        {
            var l = SetupLogic();
            var h = SetupHeader(0, 0);

            l.ReceivedByRemote(h);

            Assert.Equal(1, l.Count);
            Assert.False(l.Dequeue().WasDelivered);
        }

        [Fact]
        public static void DroppedReceive()
        {
            var l = SetupLogic();
            var h = SetupHeader(2, 0xffffffff);

            l.ReceivedByRemote(h);

            Assert.Equal(3, l.Count);
            Assert.True(l.Dequeue().WasDelivered);
            Assert.True(l.Dequeue().WasDelivered);
            Assert.True(l.Dequeue().WasDelivered);
        }

        [Fact]
        public static void MultipleReceive()
        {
            var l = SetupLogic();

            var h = SetupHeader(2, 0xffffffff);

            l.ReceivedByRemote(h);
            Assert.Equal(3, l.Count);

            var h2 = SetupHeader(4, 0xffffffff);
            l.ReceivedByRemote(h2);

            Assert.Equal(5, l.Count);
            Assert.True(l.Dequeue().WasDelivered);
            Assert.True(l.Dequeue().WasDelivered);
            Assert.True(l.Dequeue().WasDelivered);
            Assert.True(l.Dequeue().WasDelivered);
            Assert.True(l.Dequeue().WasDelivered);
        }

        [Fact]
        public static void CheckOutgoingSequenceId()
        {
            var l = SetupLogic();

            for (var i = 0; i < ReceiveMask.Range; ++i)
            {
                l.IncreaseOutgoingSequenceId();
            }

            Assert.False(l.CanIncrementOutgoingSequence);

            var h = SetupHeader(2, 0xffffffff);
            l.ReceivedByRemote(h);
            Assert.True(l.CanIncrementOutgoingSequence);
            l.IncreaseOutgoingSequenceId();
            Assert.True(l.CanIncrementOutgoingSequence);
            l.IncreaseOutgoingSequenceId();
            Assert.True(l.CanIncrementOutgoingSequence);
            l.IncreaseOutgoingSequenceId();
            Assert.False(l.CanIncrementOutgoingSequence);

            Assert.Equal(3, l.Count);
        }

        [Fact]
        public static void NoNewInfo()
        {
            var l = SetupLogic();
            var h = SetupHeader(SequenceId.MaxValue, 0xffffffff);

            Assert.Equal(0, l.Count);
        }

        [Fact]
        public static void SomeDropped()
        {
            var l = SetupLogic();
            var h = SetupHeader(3, 0x2);

            l.ReceivedByRemote(h);

            Assert.Equal(4, l.Count);
            Assert.False(l.Dequeue().WasDelivered);
            var info = l.Dequeue();
            Assert.True(info.WasDelivered);
            Assert.Equal(1u, info.PacketSequenceId.Value);
            Assert.False(l.Dequeue().WasDelivered);
            var info2 = l.Dequeue();
            Assert.False(info2.WasDelivered);
            Assert.Equal(3u, info2.PacketSequenceId.Value);
        }
    }
}
