using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CustomInventorySize.Models
{
    public class ItemStorageSize : StorageSize
    {
        [XmlAttribute()]
        public ushort ItemId { get; set; }

        public ItemStorageSize() { }

        public ItemStorageSize(ushort itemId, byte width, byte height) : base(width, height)
        {
            ItemId = itemId;
        }
    }
}
