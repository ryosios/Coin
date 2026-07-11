using System;

namespace Coin.Game.Player
{
    [Serializable]
    public sealed class InventoryEntry
    {
        public string ContentId;
        public int Amount;

        public InventoryEntry(string contentId, int amount)
        {
            ContentId = contentId;
            Amount = amount;
        }
    }
}
