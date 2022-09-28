using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInventorySize.Models
{
    public class ItemStorageSize : StorageSize
    {
        public ushort ItemId { get; set; }

        public ItemStorageSize() { }

        public ItemStorageSize(ushort itemId, byte width, byte height) : base(width, height)
        {
            ItemId = itemId;
        }
    }
}
