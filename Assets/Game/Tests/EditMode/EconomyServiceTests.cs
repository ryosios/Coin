using System;
using System.Collections.Generic;
using Coin.Game.Economy;
using Coin.Game.Player;
using NUnit.Framework;

namespace Coin.Game.Tests
{
    public sealed class EconomyServiceTests
    {
        private PlayerData playerData;
        private EconomyService economy;
        private RewardService rewards;

        [SetUp]
        public void SetUp()
        {
            playerData = new PlayerData();
            economy = new EconomyService(playerData);
            rewards = new RewardService(economy);
        }

        [Test]
        public void GrantAll_AddsWalletAndInventoryRewards()
        {
            rewards.GrantAll(new List<RewardDefinition>
            {
                new RewardDefinition(RewardType.GachaStone, 100),
                new RewardDefinition(RewardType.Coin, 500),
                new RewardDefinition(RewardType.Item, 2, "iron_sword"),
                new RewardDefinition(RewardType.GachaTicket, 1, "normal_ticket")
            });

            Assert.That(playerData.Wallet.GachaStone, Is.EqualTo(100));
            Assert.That(playerData.Wallet.Coin, Is.EqualTo(500));
            Assert.That(economy.GetAmount(RewardType.Item, "iron_sword"), Is.EqualTo(2));
            Assert.That(economy.GetAmount(RewardType.GachaTicket, "normal_ticket"), Is.EqualTo(1));
        }

        [Test]
        public void TrySpend_WhenAffordable_RemovesCostAndReturnsTrue()
        {
            RewardDefinition stones = new RewardDefinition(RewardType.GachaStone, 100);
            rewards.Grant(stones);

            bool succeeded = economy.TrySpend(new RewardDefinition(RewardType.GachaStone, 30));

            Assert.That(succeeded, Is.True);
            Assert.That(playerData.Wallet.GachaStone, Is.EqualTo(70));
        }

        [Test]
        public void TrySpend_WhenNotAffordable_DoesNotChangeBalance()
        {
            rewards.Grant(new RewardDefinition(RewardType.Coin, 20));

            bool succeeded = economy.TrySpend(new RewardDefinition(RewardType.Coin, 30));

            Assert.That(succeeded, Is.False);
            Assert.That(playerData.Wallet.Coin, Is.EqualTo(20));
        }

        [Test]
        public void SameContentId_DoesNotMixDifferentInventoryTypes()
        {
            rewards.Grant(new RewardDefinition(RewardType.Item, 3, "starter"));
            rewards.Grant(new RewardDefinition(RewardType.GachaTicket, 2, "starter"));

            Assert.That(economy.GetAmount(RewardType.Item, "starter"), Is.EqualTo(3));
            Assert.That(economy.GetAmount(RewardType.GachaTicket, "starter"), Is.EqualTo(2));
        }

        [Test]
        public void InvalidAmount_IsRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                rewards.Grant(new RewardDefinition(RewardType.GachaStone, 0)));
        }

        [Test]
        public void InventoryRewardWithoutContentId_IsRejected()
        {
            Assert.Throws<ArgumentException>(() =>
                rewards.Grant(new RewardDefinition(RewardType.Item, 1)));
        }
    }
}
