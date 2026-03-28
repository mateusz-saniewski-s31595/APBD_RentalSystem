namespace RentalSystem.Domain
{
    public class Rental
    {
        public Guid Id { get; private set; }

        public User User { get; private set; }
        public Equipment Equipment { get; private set; }

        public DateTime RentedAt { get; private set; }
        public DateTime DueDate { get; private set; }
        public DateTime? ReturnedAt { get; private set; }

        public decimal? PenaltyAmount { get; private set; }

        public bool IsReturned => ReturnedAt.HasValue;

        public bool IsActive => !IsReturned;

        public bool IsOverdue => IsActive && DateTime.Now > DueDate;

        public Rental(User user, Equipment equipment, DateTime rentedAt, int rentalDays)
        {
            if (user == null) 
                throw new ArgumentNullException(nameof(user));
            if (equipment == null) 
                throw new ArgumentNullException(nameof(equipment));
            if (rentalDays <= 0)
                throw new ArgumentException("Number of days must be grater than 0", nameof(rentalDays));

            Id = Guid.NewGuid();
            User = user;
            Equipment = equipment;
            RentedAt = rentedAt;
            DueDate = rentedAt.AddDays(rentalDays);
        }
        
        public void RegisterReturn(DateTime returnedAt, decimal penaltyAmount)
        {
            if (IsReturned)
                throw new InvalidOperationException("This equipment has been already returned");
            if (returnedAt < RentedAt)
                throw new ArgumentException("Return date cannot be earlier than the rental date");

            ReturnedAt = returnedAt;
            PenaltyAmount = penaltyAmount;
        }

        public override string ToString()
        {
            var status = IsReturned
                ? $"Returned: {ReturnedAt:dd.MM.yyyy}" + (PenaltyAmount > 0 ? $" , Penalty: {PenaltyAmount:C}" : " , No penalty")
                : IsOverdue
                    ? $"Overdue (deadline: {DueDate:dd.MM.yyyy})"
                    : $"Active (deadline: {DueDate:dd.MM.yyyy})";

            return $"[{Id.ToString()[..8]}] {User.FullName} → {Equipment.Name} , {RentedAt:dd.MM.yyyy} , {status}";
        }
    } }