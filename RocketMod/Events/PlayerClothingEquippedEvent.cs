using CustomInventorySize.API;
using SDG.Unturned;
using System;

namespace CustomInventorySize.RocketMod.Events
{
    public class PlayerClothingEquippedEvent : IDisposable
    {
        private readonly IInventoryModifier _inventoryModifier;

        private readonly bool _enabled = Plugin.Instance.Configuration.Instance.Enabled;

        public PlayerClothingEquippedEvent(IInventoryModifier inventoryModifier)
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

        private void OnBackpackChanged(PlayerClothing clothing)
        {
            if (_enabled)
                _inventoryModifier.ModifyPageByRoles(clothing.player, PlayerInventory.BACKPACK);
        }

        private void OnVestChanged(PlayerClothing clothing)
        {
            if (_enabled)
                _inventoryModifier.ModifyPageByRoles(clothing.player, PlayerInventory.VEST);
        }

        private void OnShirtChanged(PlayerClothing clothing)
        {
            if (_enabled)
                _inventoryModifier.ModifyPageByRoles(clothing.player, PlayerInventory.SHIRT);
        }

        private void OnPantsChanged(PlayerClothing clothing)
        {
            if (_enabled)
                _inventoryModifier.ModifyPageByRoles(clothing.player, PlayerInventory.PANTS);
        }
    }
}