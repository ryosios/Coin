using System;
using Coin.Game.Player;

namespace Coin.Game.Economy
{
    public sealed class EconomyService
    {
        private readonly PlayerData playerData;

        public EconomyService(PlayerData playerData)
        {
            this.playerData = playerData ?? throw new ArgumentNullException(nameof(playerData));
        }

        public event Action Changed;

        public int GetAmount(RewardDefinition resource)
        {
            ValidateResource(resource);
            return GetAmount(resource.Type, resource.ContentId);
        }

        public int GetAmount(RewardType type, string contentId = "")
        {
            switch (type)
            {
                case RewardType.GachaStone:
                    return playerData.Wallet.GachaStone;
                case RewardType.Coin:
                    return playerData.Wallet.Coin;
                case RewardType.SeasonPoint:
                    return playerData.Wallet.SeasonPoint;
                case RewardType.Item:
                case RewardType.GachaTicket:
                case RewardType.TimeExtensionTicket:
                    ValidateContentId(contentId, type);
                    InventoryEntry entry = FindEntry(type, contentId);
                    return entry == null ? 0 : entry.Amount;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public bool CanAfford(RewardDefinition cost)
        {
            ValidateResource(cost);
            return GetAmount(cost) >= cost.Amount;
        }

        public bool TrySpend(RewardDefinition cost)
        {
            ValidateResource(cost);

            if (!CanAfford(cost))
            {
                return false;
            }

            ChangeAmount(cost.Type, cost.ContentId, -cost.Amount);
            Changed?.Invoke();
            return true;
        }

        public void Add(RewardDefinition reward)
        {
            ValidateResource(reward);
            ChangeAmount(reward.Type, reward.ContentId, reward.Amount);
            Changed?.Invoke();
        }

        private void ChangeAmount(RewardType type, string contentId, int delta)
        {
            switch (type)
            {
                case RewardType.GachaStone:
                    playerData.Wallet.GachaStone += delta;
                    return;
                case RewardType.Coin:
                    playerData.Wallet.Coin += delta;
                    return;
                case RewardType.SeasonPoint:
                    playerData.Wallet.SeasonPoint += delta;
                    return;
                case RewardType.Item:
                case RewardType.GachaTicket:
                case RewardType.TimeExtensionTicket:
                    InventoryEntry entry = FindEntry(type, contentId);

                    if (entry == null)
                    {
                        entry = new InventoryEntry(CreateInventoryKey(type, contentId), 0);
                        playerData.Inventory.Entries.Add(entry);
                    }

                    entry.Amount += delta;
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private InventoryEntry FindEntry(RewardType type, string contentId)
        {
            string key = CreateInventoryKey(type, contentId);
            return playerData.Inventory.Entries.Find(entry => entry.ContentId == key);
        }

        private static string CreateInventoryKey(RewardType type, string contentId)
        {
            return $"{type}:{contentId}";
        }

        private static void ValidateResource(RewardDefinition resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (resource.Amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(resource.Amount), "Amount must be greater than zero.");
            }

            if (RequiresContentId(resource.Type))
            {
                ValidateContentId(resource.ContentId, resource.Type);
            }
        }

        private static bool RequiresContentId(RewardType type)
        {
            return type == RewardType.Item ||
                   type == RewardType.GachaTicket ||
                   type == RewardType.TimeExtensionTicket;
        }

        private static void ValidateContentId(string contentId, RewardType type)
        {
            if (string.IsNullOrWhiteSpace(contentId))
            {
                throw new ArgumentException($"ContentId is required for {type}.", nameof(contentId));
            }
        }
    }
}
