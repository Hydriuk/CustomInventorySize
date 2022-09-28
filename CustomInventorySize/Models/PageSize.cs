using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CustomInventorySize.Models
{
    public class PageSize : StorageSize
    {
        [XmlAttribute("Index")]
        public byte PageIndex { get; set; }

        public PageSize() { }

        public PageSize(byte pageIndex, byte width, byte height) : base(width, height)
        {
            PageIndex = pageIndex;
        }
    }
}
