namespace RentalSystem.Domain
{
    public abstract class User
    {
        public Guid Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public string FullName => $"{FirstName} {LastName}";

        protected User(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("Name cannot be empty", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Surname cannot be empty", nameof(lastName));

            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
        }
        
        public abstract int MaxActiveRentals { get; }

        public abstract string UserType { get; }

        public override string ToString()
        {
            return $"[{Id.ToString()[..8]}] {FullName} ({UserType})";
        }
    }


    public class Student : User
    {
        public override int MaxActiveRentals => 2;
        public override string UserType => "Stud";

        public string IndexNumber { get; private set; }

        public Student(string firstName, string lastName, string indexNumber) : base(firstName, lastName)
        {
            if (string.IsNullOrWhiteSpace(indexNumber))
                throw new ArgumentException("Index number cannot be empty", nameof(indexNumber));
            IndexNumber = indexNumber;
        }

        public override string ToString()
        {
            return $"{base.ToString()} , Index no: {IndexNumber}";
        }
    }

    public class Employee : User
    {
        public override int MaxActiveRentals => 5;
        public override string UserType => "Emp";

        public string Department { get; private set; }

        public Employee(string firstName, string lastName, string department) : base(firstName, lastName)
        {
            if (string.IsNullOrWhiteSpace(department))
                throw new ArgumentException("Dept cannot be empty", nameof(department));
            Department = department;
        }

        public override string ToString()
        {
            return $"{base.ToString()} , Dept: {Department}";
        }
    }
}