namespace Application.Utility
{
    internal static class GiftPointsFormula
    {
        const decimal REWARD_MULTIPLIER = 0.01M;
        const int AMOUNT_MULTIPLIER = 100;
        public static int CalculateRewardPoints(decimal amount)
        {
            return Convert.ToInt32(Math.Round(amount * REWARD_MULTIPLIER));
        }

        public static int CalculateRewardAmount(int points)
        {
            return points * AMOUNT_MULTIPLIER;
        }
    }
}
