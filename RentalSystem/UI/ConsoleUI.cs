using RentalSystem.Domain;
using RentalSystem.Services;

namespace RentalSystem.UI
{
    public class ConsoleUI
    {
        private readonly RentalService _service;
        
        public ConsoleUI(RentalService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }


        public void DisplayAllEquipment()
        {
            var items = _service.GetAllEquipment();
            PrintHeader("All equipment in the system");

            if (!items.Any())
            {
                PrintInfo("No equipment in the system");
                return;
            }

            foreach (var item in items)
            {
                Console.WriteLine($"  {item}");
                Console.WriteLine($"      {item.GetDetails()}");
            }

            Console.WriteLine();
        }

        public void DisplayAvailableEquipment()
        {
            var items = _service.GetAvailableEquipment();
            PrintHeader("Available equipment");

            if (!items.Any())
            {
                PrintInfo("No equipment available");
                return;
            }

            foreach (var item in items)
            {
                Console.WriteLine($"  {item}");
                Console.WriteLine($"      {item.GetDetails()}");
            }
            Console.WriteLine();
        }


        public void DisplayActiveRentalsForUser(Guid userId)
        {
            var rentals = _service.GetActiveRentalsForUser(userId);
            var users = _service.GetAllUsers();
            var user = users.FirstOrDefault(u => u.Id == userId);

            PrintHeader($"Active rentals: {user?.FullName ?? "Unknown user"}");

            if (!rentals.Any())
            {
                PrintInfo("No active rentals");
                return;
            }

            foreach (var rental in rentals)
            {
                var overdueTag = rental.IsOverdue ? " Overdue" : "";
                Console.WriteLine($"  {rental}{overdueTag}");
            }
            Console.WriteLine();
        }

        public void DisplayOverdueRentals()
        {
            var rentals = _service.GetOverdueRentals();
            PrintHeader("Overdue rentals");

            if (!rentals.Any())
            {
                PrintInfo("No overdue rentals");
                return;
            }

            foreach (var rental in rentals)
            {
                Console.WriteLine($"  {rental}");
            }
            Console.WriteLine();
        }
        

        public void DisplayRentResult(OperationResult<Rental> result)
        {
            if (result.IsSuccess)
            {
                PrintSuccess("Rented successfully!");
                Console.WriteLine($"  {result.Value}");
            }
            else
            {
                PrintError($"Cannot rent: {result.ErrorMessage}");
            }
        }

        public void DisplayReturnResult(OperationResult<Rental> result)
        {
            if (result.IsSuccess)
            {
                var rental = result.Value!;
                if (rental.PenaltyAmount > 0)
                {
                    PrintWarning($"Return registered with penalty: {rental.PenaltyAmount}");
                }
                else
                {
                    PrintSuccess("Return registered on time. No penalty.");
                }
                Console.WriteLine($"  {rental}");
            }
            else
            {
                PrintError($"Cannot register return: {result.ErrorMessage}");
            }
        }

        public void DisplayMarkUnavailableResult(OperationResult<bool> result, string equipmentName)
        {
            if (result.IsSuccess)
                PrintSuccess($"Equipment '{equipmentName}' marked as unavailable.");
            else
                PrintError($"Error: {result.ErrorMessage}");
        }
        

        public void DisplayReport()
        {
            var report = _service.GenerateReport();
            PrintHeader("RENTAL SYSTEM SUMMARY REPORT");

            Console.WriteLine($"Equipment:");
            Console.WriteLine($"Total:       {report.TotalEquipment}");
            Console.WriteLine($"Available:   {report.AvailableEquipment}");
            Console.WriteLine($"Rented:      {report.RentedEquipment}");
            Console.WriteLine($"Unavailable: {report.UnavailableEquipment}");

            Console.WriteLine();
            Console.WriteLine($"Users:");
            Console.WriteLine($"Registered:  {report.TotalUsers}");

            Console.WriteLine();
            Console.WriteLine($"Rentals:");
            Console.WriteLine($"Active:      {report.ActiveRentals}");
            Console.WriteLine($"Overdue:     {report.OverdueRentals}");
            Console.WriteLine($"Completed:   {report.CompletedRentals}");

            Console.WriteLine();
            Console.WriteLine($"Total penalties collected: {report.TotalPenaltiesCollected}");
            Console.WriteLine();
        }

        public void PrintHeader(string text)
        {
            Console.WriteLine();
            Console.WriteLine($" === {text} === ");
            Console.WriteLine();
        }

        public void PrintSuccess(string text)
        {
            Console.WriteLine($"SUCCESS: {text}");
        }

        public void PrintError(string text)
        {
            Console.WriteLine($"ERROR: {text}");
        }

        public void PrintWarning(string text)
        {
            Console.WriteLine($"WARNING: {text}");
        }

        public void PrintInfo(string text)
        {
            Console.WriteLine($"INFO: {text}");
        }

    }
}
