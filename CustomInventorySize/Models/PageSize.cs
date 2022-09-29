using System.Xml.Serialization;

namespace CustomInventorySize.Models
{
    public class PageSize : StorageSize
    {
        [XmlAttribute()]
        public byte Index { get; set; }

        public PageSize()
        { }

        public PageSize(byte pageIndex, byte width, byte height) : base(width, height)
        {
            Index = pageIndex;
        }
    }
}