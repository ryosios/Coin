using System;

namespace Coin.Game.Economy
{
    [Serializable]
    public sealed class RewardDefinition
    {
        public RewardType Type;
        public string ContentId;
        public int Amount;

        public RewardDefinition()
        {
        }

        public RewardDefinition(RewardType type, int amount, string contentId = "")
        {
            Type = type;
            ContentId = contentId;
            Amount = amount;
        }
    }
}
