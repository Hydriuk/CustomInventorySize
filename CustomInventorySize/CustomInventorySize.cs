using CustomInventorySize.Events;
using CustomInventorySize.Services;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomInventorySize
{
    public class CustomInventorySize : RocketPlugin<Configuration>
    {
        public static CustomInventorySize Instance { get; private set; }

        private InventoryModifier _inventoryModifier;

        private PlayerConnectedEvent _playerConnectedEvent;
        private PlayerClothingEquippedEvent _playerClothingEquippedEvent;
        private PlayerLifeUpdatedEvent _playerLifeUpdatedEvent;

        protected override void Load()
        {
            Instance = this;

            _inventoryModifier = new InventoryModifier(Configuration.Instance);

            _playerConnectedEvent = new PlayerConnectedEvent(_inventoryModifier);
            _playerClothingEquippedEvent = new PlayerClothingEquippedEvent(_inventoryModifier);
            _playerLifeUpdatedEvent = new PlayerLifeUpdatedEvent(_inventoryModifier);
        }

        protected override void Unload()
        {
            _playerConnectedEvent.Dispose();
            _playerClothingEquippedEvent.Dispose();
            _playerLifeUpdatedEvent.Dispose();
        }
    }
}
