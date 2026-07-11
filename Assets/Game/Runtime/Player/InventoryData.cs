using System;
using System.Collections.Generic;

namespace Coin.Game.Player
{
    [Serializable]
    public sealed class InventoryData
    {
        public List<InventoryEntry> Entries = new List<InventoryEntry>();
    }
}
