using System.Xml.Serialization;

namespace CustomInventorySize.Models
{
    public abstract class StorageSize
    {
        [XmlAttribute]
        public byte Width { get; set; }

        [XmlAttribute]
        public byte Height { get; set; }

        public StorageSize()
        { }

        public StorageSize(byte width, byte height)
        {
            Width = width;
            Height = height;
        }
    }
}