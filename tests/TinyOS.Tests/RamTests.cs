using System.Linq;
using Andy.TinyOS;
using FluentAssertions;
using Xunit;

namespace ClassLibrary1
{
    public class RamTests
    {
        [Fact]
        public void Create()
        {
            var ram = new Ram(1024, 256);
            ram.Size.Should().Be(1024);
            ram.FrameSize.Should().Be(256);
            ram.FrameCount.Should().Be(4);
        }

		[Fact]
		public void CreateShared()
		{
			var ram = new Ram(1024, 256);
			var offset = ram.AllocateShared(512);
			offset.Should().Be(0);

			var pcb = new ProcessContextBlock {Id = 10};
			var p1 = ram.Allocate(pcb);
			var p2 = ram.Allocate(pcb);

			var p1Offset = ram.ToPhysicalAddress(10, p1.VirtualAddress);
			var p2Offset = ram.ToPhysicalAddress(10, p2.VirtualAddress);
			p1Offset.Should().BeGreaterThan(offset);
			p2Offset.Should().BeGreaterThan(offset);
			p2Offset.Should().BeGreaterThan(p1Offset);
		}

    	[Fact]
        public void Allocate()
        {
            var ram = new Ram(2, 1);

            var pcb = new ProcessContextBlock { Id = 1 };
            var page = ram.Allocate(pcb);
            Assert.That(page, Is.Not.Null);
            Assert.That(page.PageNumber, Is.Not.EqualTo(0));
            
            page = ram.Allocate(pcb);
            Assert.That(page, Is.Not.Null);
            Assert.That(page.PageNumber, Is.Not.EqualTo(0));

            page = ram.Allocate(pcb);
            page.Should().BeNull();
        }

        [Fact]
        public void Free()
        {
            var ram = new Ram(2, 1);

            var pcb = new ProcessContextBlock { Id = 1 };
            var frame = ram.Allocate(pcb);
            Assert.That(frame, Is.Not.Null);
            
            frame = ram.Allocate(pcb);
            Assert.That(frame, Is.Not.Null);

            ram.Free(frame);

            frame = ram.Allocate(pcb);
            Assert.That(frame, Is.Not.Null);
        }

        [Fact]
        public void ReadFromStream()
        {
            var buffer = new byte[] {1, 2, 3, 4, 5, 6};
            const uint frameSize = 3;
            var ram = new Ram(buffer, frameSize);

            var s = ram.GetStream(0);
            Assert.That(s.Length, Is.EqualTo<long>(frameSize));
            Assert.That(s.ToArray(), Is.EquivalentTo(new byte[] {1, 2, 3}));

            var s2 = ram.GetStream(1);
            Assert.That(s2.Length, Is.EqualTo<long>(frameSize));
            Assert.That(s2.ToArray(), Is.EquivalentTo(new byte[] { 4, 5, 6 }));
        }

        [Fact]
        public void WriteToStream()
        {
            var buffer = new byte[] {1, 2, 3, 4, 5, 6};
            const uint frameSize = 3;
            var ram = new Ram(buffer, frameSize);

            var s = ram.GetStream(0);
            s.WriteByte(99);
            Assert.That(s.ToArray(), Is.EquivalentTo(new byte[] {99, 2, 3}));
        }

		[Fact]
		public void VirtualAddressCalculations()
		{
			var c = new VirtualAddressCalculator(16);
			c.PageSize.Should().Be(16);
			c.MaxPages.Should().Be(1 << 28);
		}
    }
}