using System;
using System.Collections.Generic;

namespace Coin.Game.Economy
{
    public sealed class RewardService
    {
        private readonly EconomyService economyService;

        public RewardService(EconomyService economyService)
        {
            this.economyService = economyService ?? throw new ArgumentNullException(nameof(economyService));
        }

        public void Grant(RewardDefinition reward)
        {
            economyService.Add(reward);
        }

        public void GrantAll(IEnumerable<RewardDefinition> rewards)
        {
            if (rewards == null)
            {
                throw new ArgumentNullException(nameof(rewards));
            }

            foreach (RewardDefinition reward in rewards)
            {
                Grant(reward);
            }
        }
    }
}
