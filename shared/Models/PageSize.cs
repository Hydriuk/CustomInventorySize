using System.Xml.Serialization;

namespace CustomInventorySize.Models
{
    public class PageSize : StorageSize
    {
        [XmlAttribute("Index")]
        public byte PageIndex { get; set; }

        public PageSize()
        { }

        public PageSize(byte pageIndex, byte width, byte height) : base(width, height)
        {
            PageIndex = pageIndex;
        }
    }
}