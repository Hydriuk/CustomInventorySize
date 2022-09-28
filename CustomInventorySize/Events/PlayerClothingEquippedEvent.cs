using CustomInventorySize.Services;
using SDG.Unturned;
using System;

namespace CustomInventorySize.Events
{
    public class PlayerClothingEquippedEvent : IDisposable
    {
        private readonly InventoryModifier _inventoryModifier;

        public PlayerClothingEquippedEvent(InventoryModifier inventoryModifier)
        {
            _inventoryModifier = inventoryModifier;

            PlayerClothing.OnBackpackChanged_Global += OnBackpackChanged;
            PlayerClothing.OnVestChanged_Global += OnVestChanged;
            PlayerClothing.OnShirtChanged_Global += OnShirtChanged;
            PlayerClothing.OnPantsChanged_Global += OnPantsChanged;
        }

        public void Dispose()
        {
            PlayerClothing.OnBackpackChanged_Global -= OnBackpackChanged;
            PlayerClothing.OnVestChanged_Global -= OnVestChanged;
            PlayerClothing.OnShirtChanged_Global -= OnShirtChanged;
            PlayerClothing.OnPantsChanged_Global -= OnPantsChanged;
        }

        private void OnBackpackChanged(PlayerClothing clothing) => _inventoryModifier.ModifyPage(clothing.player, PlayerInventory.BACKPACK);

        private void OnVestChanged(PlayerClothing clothing) => _inventoryModifier.ModifyPage(clothing.player, PlayerInventory.VEST);

        private void OnShirtChanged(PlayerClothing clothing) => _inventoryModifier.ModifyPage(clothing.player, PlayerInventory.SHIRT);

        private void OnPantsChanged(PlayerClothing clothing) => _inventoryModifier.ModifyPage(clothing.player, PlayerInventory.PANTS);
    }
}