using RentalSystem.Domain;

namespace RentalSystem.Services
{
    public class RentalService
    {
        private readonly List<Equipment> _equipment = new();
        private readonly List<User> _users = new();
        private readonly List<Rental> _rentals = new();

        private readonly IPenaltyCalculator _penaltyCalculator;

        public RentalService(IPenaltyCalculator penaltyCalculator)
        {
            _penaltyCalculator = penaltyCalculator ?? throw new ArgumentNullException(nameof(penaltyCalculator));
        }

        public void AddEquipment(Equipment equipment)
        {
            if (equipment == null) throw new ArgumentNullException(nameof(equipment));

            if (_equipment.Any(e => e.Id == equipment.Id))
                throw new InvalidOperationException($"Equipment with ID {equipment.Id} already exists");

            _equipment.Add(equipment);
        }

        public IReadOnlyList<Equipment> GetAllEquipment()
        {
            return _equipment.AsReadOnly();
        }

        public IReadOnlyList<Equipment> GetAvailableEquipment()
        {
            return _equipment.Where(e => e.Status == EquipmentStatus.Available).ToList().AsReadOnly();
        }

        public OperationResult<bool> MarkEquipmentUnavailable(Guid equipmentId)
        {
            var equipment = _equipment.FirstOrDefault(e => e.Id == equipmentId);
            if (equipment == null)
                return OperationResult<bool>.Failure($"No equipment with ID: {equipmentId} has been found");

            try
            {
                equipment.MarkAsUnavailable();
                return OperationResult<bool>.Success(true);
            }
            catch (InvalidOperationException ex)
            {
                return OperationResult<bool>.Failure(ex.Message);
            }
        }


        public void AddUser(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (_users.Any(u => u.Id == user.Id))
                throw new InvalidOperationException($"User with ID {user.Id} already exists");

            _users.Add(user);
        }

        public IReadOnlyList<User> GetAllUsers()
        {
            return _users.AsReadOnly();
        }


        public OperationResult<Rental> RentEquipment(Guid userId, Guid equipmentId, int rentalDays)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return OperationResult<Rental>.Failure($"User with ID: {userId} has not been found");

            var equipment = _equipment.FirstOrDefault(e => e.Id == equipmentId);
            if (equipment == null)
                return OperationResult<Rental>.Failure($"NEquipment with ID: {equipmentId} has not been found");

            if (rentalDays <= 0)
                return OperationResult<Rental>.Failure("Number of days must be greater than 0");

            if (equipment.Status != EquipmentStatus.Available)
                return OperationResult<Rental>.Failure(
                    $"Equipment '{equipment.Name}' is not available (status: {equipment.Status}).");


            var activeRentalsCount = GetActiveRentalsForUser(userId).Count;
            if (activeRentalsCount >= user.MaxActiveRentals)
                return OperationResult<Rental>.Failure(
                    $"{user.UserType} {user.FullName} reached the limit {user.MaxActiveRentals} of active rentals");
            
            try
            {
                equipment.MarkAsRented();

                var rental = new Rental(user, equipment, DateTime.Now, rentalDays);
                _rentals.Add(rental);

                return OperationResult<Rental>.Success(rental);
            }
            catch (Exception ex)
            {
                return OperationResult<Rental>.Failure($"Error while renting: {ex.Message}");
            }
        }

        public OperationResult<Rental> ReturnEquipment(Guid rentalId)
        {
            var rental = _rentals.FirstOrDefault(r => r.Id == rentalId);
            if (rental == null)
                return OperationResult<Rental>.Failure($"No rental ID: {rentalId} has been found");

            if (rental.IsReturned)
                return OperationResult<Rental>.Failure("This rental has already been completed.");

            var returnedAt = DateTime.Now;

            var penalty = _penaltyCalculator.Calculate(returnedAt, rental.DueDate);

            rental.RegisterReturn(returnedAt, penalty);

            rental.Equipment.MarkAsAvailable();

            return OperationResult<Rental>.Success(rental);
        }
        

        public IReadOnlyList<Rental> GetActiveRentalsForUser(Guid userId)
        {
            return _rentals
                .Where(r => r.User.Id == userId && r.IsActive)
                .ToList()
                .AsReadOnly();
        }

        public IReadOnlyList<Rental> GetOverdueRentals()
        {
            return _rentals
                .Where(r => r.IsOverdue)
                .ToList()
                .AsReadOnly();
        }

        public IReadOnlyList<Rental> GetAllRentals()
        {
            return _rentals.AsReadOnly();
        }


        public RentalReport GenerateReport()
        {
            return new RentalReport
            {
                TotalEquipment = _equipment.Count,
                AvailableEquipment = _equipment.Count(e => e.Status == EquipmentStatus.Available),
                RentedEquipment = _equipment.Count(e => e.Status == EquipmentStatus.Rented),
                UnavailableEquipment = _equipment.Count(e => e.Status == EquipmentStatus.Unavailable),
                TotalUsers = _users.Count,
                ActiveRentals = _rentals.Count(r => r.IsActive),
                OverdueRentals = _rentals.Count(r => r.IsOverdue),
                CompletedRentals = _rentals.Count(r => r.IsReturned),
                TotalPenaltiesCollected = _rentals
                    .Where(r => r.IsReturned && r.PenaltyAmount > 0)
                    .Sum(r => r.PenaltyAmount ?? 0m)
            };
        }
    }
    
    public record RentalReport
    {
        public int TotalEquipment { get; init; }
        public int AvailableEquipment { get; init; }
        public int RentedEquipment { get; init; }
        public int UnavailableEquipment { get; init; }
        public int TotalUsers { get; init; }
        public int ActiveRentals { get; init; }
        public int OverdueRentals { get; init; }
        public int CompletedRentals { get; init; }
        public decimal TotalPenaltiesCollected { get; init; }
    }
}