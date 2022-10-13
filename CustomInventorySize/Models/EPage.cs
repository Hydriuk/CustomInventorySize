using System;

namespace CustomInventorySize.Models
{
    [Flags]
    public enum EPage
    {
        None      = 0,
        Primary   = 1,
        Secondary = 2,
        Hands     = 4,
        Backpack  = 8,
        Vest      = 16,
        Shirt     = 32,
        Pants     = 64,
        Storage   = 128,
        Nearby    = 256,

        Slots = Primary | Secondary,
        Inventory = Hands | Backpack | Vest | Shirt | Pants,
        All       = Slots | Inventory | Storage | Nearby
    }
}
