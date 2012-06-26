using System.Linq;
using NUnit.Framework;
using tinyOS;

namespace ClassLibrary1
{
    [TestFixture]
    public class RamTests
    {
        [Test]
        public void Create()
        {
            var ram = new Ram(1024, 256);
            Assert.That(ram.Size, Is.EqualTo(1024));
            Assert.That(ram.FrameSize, Is.EqualTo(256));
            Assert.That(ram.FrameCount, Is.EqualTo(4));
        }

        [Test]
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
            Assert.That(page, Is.Null);
        }

        [Test]
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

        [Test]
        public void ReadFromStream()
        {
            var buffer = new byte[] {1, 2, 3, 4, 5, 6};
            const uint frameSize = 3;
            var ram = new Ram(buffer, frameSize);

            var s = ram.GetStream(0);
            Assert.That(s.Length, Is.EqualTo(frameSize));
            Assert.That(s.ToArray(), Is.EquivalentTo(new[] {1, 2, 3}));

            var s2 = ram.GetStream(1);
            Assert.That(s2.Length, Is.EqualTo(frameSize));
            Assert.That(s2.ToArray(), Is.EquivalentTo(new[] { 4, 5, 6 }));
        }

        [Test]
        public void WriteToStream()
        {
            var buffer = new byte[] {1, 2, 3, 4, 5, 6};
            const uint frameSize = 3;
            var ram = new Ram(buffer, frameSize);

            var s = ram.GetStream(0);
            s.WriteByte(99);
            Assert.That(s.ToArray(), Is.EquivalentTo(new[] {99, 2, 3}));
        }
    }
}