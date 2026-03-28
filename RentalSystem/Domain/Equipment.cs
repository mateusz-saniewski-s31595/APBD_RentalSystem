namespace RentalSystem.Domain
{
    public enum EquipmentStatus
    {
        Available,
        Rented,
        Unavailable
    }

    public abstract class Equipment
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public EquipmentStatus Status { get; private set; } = EquipmentStatus.Available;

        protected Equipment(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Equipment name cannot be empty", nameof(name));

            Id = Guid.NewGuid();
            Name = name;
        }

        public void MarkAsRented()
        {
            if (Status != EquipmentStatus.Available)
                throw new InvalidOperationException($"Equipment '{Name}' is not available to rent (status: {Status}).");
            Status = EquipmentStatus.Rented;
        }

        public void MarkAsAvailable()
        {
            Status = EquipmentStatus.Available;
        }

        public void MarkAsUnavailable()
        {
            if (Status == EquipmentStatus.Rented)
                throw new InvalidOperationException($"You cannot mark '{Name}'as unavailable if it is currently rented");
            Status = EquipmentStatus.Unavailable;
        }

        public abstract string GetDetails();

        public override string ToString()
        {
            return $"[{Id.ToString()[..8]}] {Name} ({Status})";
        }
    }

    public class Laptop : Equipment
    {
        public string Processor { get; private set; }
        public int RamGb { get; private set; }

        public Laptop(string name, string processor, int ramGb) : base(name)
        {
            if (string.IsNullOrWhiteSpace(processor))
                throw new ArgumentException("Processor cannot be empty", nameof(processor));
            if (ramGb <= 0)
                throw new ArgumentException("RAM must be greater than 0", nameof(ramGb));

            Processor = processor;
            RamGb = ramGb;
        }
        
        public override string GetDetails()
        {
            return $"Laptop - Processor: {Processor}, RAM: {RamGb} GB";
        }
    }

    public class Projector : Equipment
    {
        public int ResolutionWidth { get; private set; }
        public int LumensOutput { get; private set; }

        public Projector(string name, int resolutionWidth, int lumensOutput) : base(name)
        {
            if (resolutionWidth <= 0)
                throw new ArgumentException("Resolution must be greater than 0", nameof(resolutionWidth));
            if (lumensOutput <= 0)
                throw new ArgumentException("Brightness must be greater than 0", nameof(lumensOutput));

            ResolutionWidth = resolutionWidth;
            LumensOutput = lumensOutput;
        }

        public override string GetDetails()
        {
            return $"Projector - Resolution: {ResolutionWidth}p, Brightness: {LumensOutput} lm";
        }
    }

    public class Camera : Equipment
    {
        public double SensorSizeInch { get; private set; }
        public int MaxIso { get; private set; }

        public Camera(string name, double sensorSizeInch, int maxIso) : base(name)
        {
            if (sensorSizeInch <= 0)
                throw new ArgumentException("Sensor size must be greater than 0", nameof(sensorSizeInch));
            if (maxIso <= 0)
                throw new ArgumentException("Max ISO must be greater than 0", nameof(maxIso));

            SensorSizeInch = sensorSizeInch;
            MaxIso = maxIso;
        }

        public override string GetDetails()
        {
            return $"Camera - Sensor size: {SensorSizeInch} in, Max ISO: {MaxIso}";
        }
    }
}