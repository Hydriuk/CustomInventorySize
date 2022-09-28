using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInventorySize.Models
{
    public class PageSize : StorageSize
    {
        public byte PageIndex { get; set; }

        public PageSize() { }

        public PageSize(byte pageIndex, byte width, byte height) : base(width, height)
        {
            PageIndex = pageIndex;
        }
    }
}
