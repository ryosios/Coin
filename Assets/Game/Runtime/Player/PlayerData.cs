using System;

namespace Coin.Game.Player
{
    [Serializable]
    public sealed class PlayerData
    {
        public WalletData Wallet = new WalletData();
        public InventoryData Inventory = new InventoryData();
    }
}
