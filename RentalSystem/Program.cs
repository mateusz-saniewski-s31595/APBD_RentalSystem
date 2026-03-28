using RentalSystem.Domain;
using RentalSystem.Services;
using RentalSystem.UI;

var penaltyCalculator = new DailyRatePenaltyCalculator(dailyRate: 5.00m);

var rentalService = new RentalService(penaltyCalculator);

var ui = new ConsoleUI(rentalService);


// Adding equipment of different types

var laptop1 = new Laptop("Dell XPS 15", "Intel Core i7-13700H", 32);
var laptop2 = new Laptop("MacBook Pro M2", "Apple M2 Pro", 16);
var projector1 = new Projector("Epson EB-2250U", 1920, 5000);
var projector2 = new Projector("BenQ MH550", 1080, 3500);
var camera1 = new Camera("Sony A7 IV", 0.95, 51200);
var camera2 = new Camera("Canon EOS R6", 0.95, 102400);

rentalService.AddEquipment(laptop1);
rentalService.AddEquipment(laptop2);
rentalService.AddEquipment(projector1);
rentalService.AddEquipment(projector2);
rentalService.AddEquipment(camera1);
rentalService.AddEquipment(camera2);

// Adding users

var student1 = new Student("Anna", "Kowalska", "123456");
var student2 = new Student("Piotr", "Nowak", "654321");
var employee1 = new Employee("Marek", "Wisniewski", "Computer Science");
var employee2 = new Employee("Ewa", "Zielinska", "Physics");

rentalService.AddUser(student1);
rentalService.AddUser(student2);
rentalService.AddUser(employee1);
rentalService.AddUser(employee2);

// Display all equipment with statuses

ui.DisplayAllEquipment();

// Display only available equipment

ui.DisplayAvailableEquipment();

// Valid rentals

Console.WriteLine($"\n  {student1.FullName} rents {laptop1.Name} for 7 days:");
var rent1 = rentalService.RentEquipment(student1.Id, laptop1.Id, 7);
ui.DisplayRentResult(rent1);

Console.WriteLine($"\n  {student1.FullName} rents {camera1.Name} for 3 days:");
var rent2 = rentalService.RentEquipment(student1.Id, camera1.Id, 3);
ui.DisplayRentResult(rent2);

Console.WriteLine($"\n  {employee1.FullName} rents {projector1.Name} for 14 days:");
var rent3 = rentalService.RentEquipment(employee1.Id, projector1.Id, 14);
ui.DisplayRentResult(rent3);

Console.WriteLine($"\n  {student2.FullName} rents {laptop2.Name} for 5 days:");
var rent4 = rentalService.RentEquipment(student2.Id, laptop2.Id, 5);
ui.DisplayRentResult(rent4);

// Invalid operations

Console.WriteLine($"\n  {employee1.FullName} tries to rent {laptop1.Name} (already rented):");
var badRent1 = rentalService.RentEquipment(employee1.Id, laptop1.Id, 7);
ui.DisplayRentResult(badRent1);

Console.WriteLine($"\n  {student1.FullName} tries to rent {projector2.Name} (limit of 2 exceeded):");
var badRent2 = rentalService.RentEquipment(student1.Id, projector2.Id, 7);
ui.DisplayRentResult(badRent2);

Console.WriteLine($"\n  Marking {projector2.Name} as unavailable (maintenance):");
var markResult = rentalService.MarkEquipmentUnavailable(projector2.Id);
ui.DisplayMarkUnavailableResult(markResult, projector2.Name);

Console.WriteLine($"\n  {employee2.FullName} tries to rent {projector2.Name} (unavailable):");
var badRent3 = rentalService.RentEquipment(employee2.Id, projector2.Id, 3);
ui.DisplayRentResult(badRent3);

// Active rentals for a user

ui.DisplayActiveRentalsForUser(student1.Id);

// On-time return

if (rent2.IsSuccess)
{
    Console.WriteLine($"\n  Return: {camera1.Name}");
    var returnResult = rentalService.ReturnEquipment(rent2.Value!.Id);
    ui.DisplayReturnResult(returnResult);
}

// Simulated late return

Console.WriteLine("  (Simulating that the laptop was rented 10 days ago)");

if (rent4.IsSuccess)
{
    Console.WriteLine($"\n  Normal return: {laptop2.Name} by {student2.FullName}");
    var returnNormal = rentalService.ReturnEquipment(rent4.Value!.Id);
    ui.DisplayReturnResult(returnNormal);
}

var demoCalculator = new DailyRatePenaltyCalculator(5.00m);
var demoReturnDate = DateTime.Now;
var demoDueDate = DateTime.Now.AddDays(-5);
var demoPenalty = demoCalculator.Calculate(demoReturnDate, demoDueDate);

// Overdue rentals

ui.DisplayOverdueRentals();

// Employee returns projector

if (rent3.IsSuccess)
{
    Console.WriteLine($"\n  Return: {projector1.Name} by {employee1.FullName}");
    var returnEmp = rentalService.ReturnEquipment(rent3.Value!.Id);
    ui.DisplayReturnResult(returnEmp);
}

// Final equipment status

ui.DisplayAllEquipment();

// Final report

ui.DisplayReport();