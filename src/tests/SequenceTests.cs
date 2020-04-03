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
using Piot.Tend.Client;
using Xunit;

namespace Tests
{
    public static class SequenceTest
    {
        [Fact]
        public static void Setting()
        {
            var s = new SequenceId(0);

            Assert.Equal(0, s.Value);
        }

        [Fact]
        public static void NextWrap()
        {
            var current = SequenceId.Max;
            var next = current.Next();

            Assert.Equal(SequenceId.MaxValue, current.Value);
            Assert.Equal(0, next.Value);
        }

        [Fact]
        public static void NormalNext()
        {
            var current = new SequenceId(12);
            var next = current.Next();

            Assert.Equal(12, current.Value);
            Assert.Equal(13, next.Value);
        }

        [Fact]
        public static void DistanceWrap()
        {
            var current = SequenceId.Max;
            var next = new SequenceId(0);

            Assert.Equal(1, current.Distance(next));
        }

        [Fact]
        public static void DistanceNormal()
        {
            var current = new SequenceId(0);
            var next = new SequenceId(10);

            Assert.Equal(10, current.Distance(next));
        }

        [Fact]
        public static void DistancePassed()
        {
            var current = new SequenceId(10);
            var next = new SequenceId(9);

            Assert.Equal(SequenceId.MaxValue, current.Distance(next));
        }

        [Fact]
        public static void DistancePassedAgain()
        {
            var current = new SequenceId(10);
            var next = SequenceId.Max;

            Assert.Equal(SequenceId.MaxValue - 10, current.Distance(next));
        }

        [Fact]
        public static void DistanceSame()
        {
            var current = new SequenceId(10);
            var next = new SequenceId(10);

            Assert.Equal(0, current.Distance(next));
        }

        [Fact]
        public static void IllegalValue()
        {
            Assert.Throws<Exception>(() => new SequenceId(SequenceId.MaxValue + 11));
        }
    }
}
