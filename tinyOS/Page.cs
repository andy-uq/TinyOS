using System.IO;

namespace tinyOS
{
    public class Page
    {
        public uint PageNumber { get; set; }
        public uint Size { get; set; }
        public uint FrameNumber { get; set; }

        public VirtualAddress VirtualAddress { get { return new VirtualAddress(0, (int )PageNumber); } }
    }
}