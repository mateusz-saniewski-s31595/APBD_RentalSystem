namespace RentalSystem.Services
{
    public class OperationResult<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Value { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;

        private OperationResult() { }

        public static OperationResult<T> Success(T value)
        {
            return new OperationResult<T>
            {
                IsSuccess = true,
                Value = value
            };
        }

        public static OperationResult<T> Failure(string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
                throw new ArgumentException("Error message cannot be empty");

            return new OperationResult<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }
}