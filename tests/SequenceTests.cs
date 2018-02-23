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

namespace NUnit.Tests
{
	using System;
	using NUnit.Framework;

	[TestFixture]
	public class SequenceTest
	{
		[Test]
		public static void Setting()
		{
			var s = new SequenceId(0);

			Assert.That(s.Value, Is.EqualTo(0) );
		}

		[Test]
		public static void NextWrap()
		{
			var current = SequenceId.Max;
			var next = current.Next();

			Assert.That(current.Value, Is.EqualTo(SequenceId.MaxValue));
			Assert.That(next.Value, Is.EqualTo(0) );
		}

		[Test]
		public static void NormalNext()
		{
			var current = new SequenceId(12);
			var next = current.Next();

			Assert.That(current.Value, Is.EqualTo(12) );
			Assert.That(next.Value, Is.EqualTo(13) );
		}

		[Test]
		public static void DistanceWrap()
		{
			var current = SequenceId.Max;
			var next = new SequenceId(0);

			Assert.That(current.Distance(next), Is.EqualTo(1));
		}

		[Test]
		public static void DistanceNormal()
		{
			var current = new SequenceId(0);
			var next = new SequenceId(10);

			Assert.That(current.Distance(next), Is.EqualTo(10));
		}

		[Test]
		public static void DistancePassed()
		{
			var current = new SequenceId(10);
			var next = new SequenceId(9);

			Assert.That(current.Distance(next), Is.EqualTo(SequenceId.MaxValue));
		}

		[Test]
		public static void DistancePassedAgain()
		{
			var current = new SequenceId(10);
			var next =  SequenceId.Max;

			Assert.That(current.Distance(next), Is.EqualTo(SequenceId.MaxValue - 10));
		}

		[Test]
		public static void DistanceSame()
		{
			var current = new SequenceId(10);
			var next = new SequenceId(10);

			Assert.That(current.Distance(next), Is.EqualTo(0) );
		}

		[Test]
		public static void IllegalValue()
		{
			Assert.Throws<Exception>(() => new SequenceId(SequenceId.MaxValue + 11));
		}
	}
}
