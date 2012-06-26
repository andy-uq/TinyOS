using NUnit.Framework;
using tinyOS;

namespace ClassLibrary1
{
    [TestFixture]
    public class PageStreamTests
    {
        [Test]
        public void Read()
        {
            var buffer = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            const uint frameSize = 3;
            var ram = new Ram(buffer, frameSize);
            var pageSet = new PageInfo();

            var pcb = new ProcessContextBlock {Id = 1};
            ram.Allocate(pcb);
            pageSet.Append(ram.Allocate(pcb));
            pageSet.Append(ram.Allocate(pcb));

            var pageStream = new PageStream(ram, pageSet);
            Assert.That(pageStream.Length, Is.EqualTo(6));

            var b = new byte[6];
            pageStream.Read(b, 0, 6);

            Assert.That(b, Is.EquivalentTo(new[] { 4, 5, 6, 7, 8, 9 }));
        }
    }
}