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
	using System;


	public static class IncomingToUsLogicTest
	{
		static IncomingLogic SetupLogic()
		{
			var logic = new IncomingLogic();

			return logic;
		}

		[Fact]
		public static void FirstReceive()
		{
			var l = SetupLogic();

			l.ReceivedToUs(new SequenceId(1));
			var h = l.ReceivedHeader;

			Assert.Equal(1u, h.ReceivedBits.Bits);
		}

		[Fact]
		public static void ReceivedDroppedReceived()
		{
			var l = SetupLogic();

			l.ReceivedToUs(new SequenceId(0));
			var h = l.ReceivedHeader;
			Assert.Equal(1u, h.ReceivedBits.Bits);

			l.ReceivedToUs(new SequenceId(2));
			var h2 = l.ReceivedHeader;
			Assert.Equal(5u, h2.ReceivedBits.Bits);
		}

		[Fact]
		public static void IllegalDistance()
		{
			var l = SetupLogic();
			var h = l.ReceivedHeader;

			Assert.Equal(SequenceId.Max, h.SequenceId);
			Assert.Equal(0u, h.ReceivedBits.Bits);
			Assert.Throws<Exception>(() => l.ReceivedToUs(new SequenceId(32)));
		}
	}
}
