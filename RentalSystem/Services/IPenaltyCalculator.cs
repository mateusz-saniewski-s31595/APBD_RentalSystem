namespace RentalSystem.Services
{
    public interface IPenaltyCalculator
    {
        decimal Calculate(DateTime returnedAt, DateTime dueDate);
    }
}