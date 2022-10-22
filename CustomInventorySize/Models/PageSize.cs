using System.Xml.Serialization;

namespace CustomInventorySize.Models
{
    public class PageSize : StorageSize
    {
        [XmlAttribute]
        public byte Page { get; set; }

        public PageSize()
        { }

        public PageSize(byte pageIndex, byte width, byte height) : base(width, height)
        {
            Page = pageIndex;
        }
    }
}