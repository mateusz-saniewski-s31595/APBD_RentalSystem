namespace RentalSystem.Services
{
    public class DailyRatePenaltyCalculator : IPenaltyCalculator
    {
        private readonly decimal _dailyRate;

        public DailyRatePenaltyCalculator(decimal dailyRate = 5.00m)
        {
            if (dailyRate < 0)
                throw new ArgumentException("Daily penalty rate cannot be negative", nameof(dailyRate));
            _dailyRate = dailyRate;
        }

        public decimal Calculate(DateTime returnedAt, DateTime dueDate)
        {
            if (returnedAt <= dueDate)
                return 0m;

            var daysOverdue = (int)Math.Ceiling((returnedAt - dueDate).TotalDays);

            return daysOverdue * _dailyRate;
        }
    }
}