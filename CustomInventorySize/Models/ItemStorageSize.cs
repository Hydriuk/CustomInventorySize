using System.Xml.Serialization;

namespace CustomInventorySize.Models
{
    public class ItemStorageSize : StorageSize
    {
        [XmlAttribute]
        public ushort Id { get; set; }

        public ItemStorageSize()
        { }

        public ItemStorageSize(ushort itemId, byte width, byte height) : base(width, height)
        {
            Id = itemId;
        }
    }
}