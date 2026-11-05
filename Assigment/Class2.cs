using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Program
{
    static void Main(string[] args)
    {
        using (var db = new BankDbContext())
        {
            db.Database.EnsureCreated();
        }

        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("National Bank Management System");
            Console.WriteLine("===============================");
            Console.WriteLine("1) Add a new Customer");
            Console.WriteLine("2) Open a new Account for a Customer");
            Console.WriteLine("3) Update Account Status (Active / Closed)");
            Console.WriteLine("4) Remove an Account from a Customer");
            Console.WriteLine("5) List all Customers (with accounts)");
            Console.WriteLine("0) Exit");
            Console.Write("\nEnter choice: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddNewCustomer();
                    break;
                case "2":
                    OpenNewAccount();
                    break;
                case "3":
                    UpdateAccountStatus();
                    break;
                case "4":
                    RemoveAccountFromCustomer();
                    break;
                case "5":
                    ListAllCustomers();
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("\n[!] Invalid choice. Please enter a number from 0 to 5.");
                    break;
            }

            if (!exit)
            {
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
            }
        }
    }

    static void AddNewCustomer()
    {
        Console.Clear();
        Console.WriteLine("=== Add New Customer ===");

        Console.Write("Full Name: ");
        string name = Console.ReadLine();

        Console.Write("National ID: ");
        string nationalId = Console.ReadLine();

        DateTime dob;
        while (true)
        {
            Console.Write("Date of Birth (yyyy-MM-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out dob)) break;
            Console.WriteLine("Invalid date format. Try again.");
        }

        Console.Write("Email: ");
        string email = Console.ReadLine();

        Console.Write("Phone: ");
        string phone = Console.ReadLine();

        Console.Write("Address: ");
        string address = Console.ReadLine();

        Console.WriteLine("Customer Type:\n1) Individual\n2) Business");
        string typeChoice = Console.ReadLine();
        string customerType = (typeChoice == "2") ? "Business" : "Individual";

        using (var db = new BankDbContext())
        {
            var customer = new Customer
            {
                FullName = name,
                NationalId = nationalId,
                DateOfBirth = dob,
                Email = email,
                PhoneNumber = phone,
                Address = address,
                CustomerType = customerType
            };

            db.Customers.Add(customer);
            db.SaveChanges();

            Console.WriteLine($"\nCustomer created successfully. CustomerId = {customer.Id}");
        }
    }

    static void OpenNewAccount()
    {
        Console.Clear();
        Console.WriteLine("=== Open New Account ===");

        Console.Write("Account Number: ");
        string accNumber = Console.ReadLine();

        Console.WriteLine("Account Type:\n1) Savings\n2) Current\n3) Business");
        string tChoice = Console.ReadLine();
        string accType = tChoice == "2" ? "Current" : (tChoice == "3" ? "Business" : "Savings");

        Console.Write("Branch Code: ");
        string branchCode = Console.ReadLine();

        int customerId;
        while (true)
        {
            Console.Write("Customer Id: ");
            if (int.TryParse(Console.ReadLine(), out customerId)) break;
            Console.WriteLine("Invalid ID. Enter a numeric value.");
        }

        Console.WriteLine("Ownership Role:\n1) Primary\n2) CoHolder");
        string roleChoice = Console.ReadLine();
        string role = roleChoice == "2" ? "CoHolder" : "Primary";

        using (var db = new BankDbContext())
        {
            Console.WriteLine($"\nValidating branch '{branchCode}' and customer #{customerId}...");

            var branch = db.Branches.FirstOrDefault(b => b.Code == branchCode);
            var customer = db.Customers.Find(customerId);

            if (branch == null || customer == null)
            {
                Console.WriteLine("[Error] Branch code or Customer ID does not exist!");
                return;
            }

            var account = new Account
            {
                AccountNumber = accNumber,
                AccountType = accType,
                BranchId = branch.Id,
                CurrentBalance = 0.00m
            };

            db.Accounts.Add(account);
            db.SaveChanges();

            var customerAccount = new CustomerAccount
            {
                CustomerId = customer.Id,
                AccountId = account.Id,
                OwnershipType = role
            };

            db.CustomerAccounts.Add(customerAccount);
            db.SaveChanges();

            Console.WriteLine($"\nAccount {accNumber} created and linked to customer {customerId} as {role} owner.");
        }
    }

    static void UpdateAccountStatus()
    {
        Console.Clear();
        Console.WriteLine("=== Update Account Status ===");

        Console.Write("Account Number: ");
        string accNumber = Console.ReadLine();

        int customerId;
        while (true)
        {
            Console.Write("Customer Id: ");
            if (int.TryParse(Console.ReadLine(), out customerId)) break;
            Console.WriteLine("Invalid ID. Enter a numeric value.");
        }

        Console.WriteLine("New Status:\n1) Active\n2) Closed");
        string statusChoice = Console.ReadLine();
        string newStatus = statusChoice == "2" ? "Closed" : "Active";

        using (var db = new BankDbContext())
        {
            var link = db.CustomerAccounts
                .Include(ca => ca.Account)
                .FirstOrDefault(ca => ca.Account.AccountNumber == accNumber && ca.CustomerId == customerId);

            if (link == null)
            {
                Console.WriteLine("[Error] Account and Customer relationship not found!");
                return;
            }

            link.Account.AccountStatus = newStatus;
            db.SaveChanges();

            Console.WriteLine($"\nStatus updated to {newStatus}.");
        }
    }

    static void RemoveAccountFromCustomer()
    {
        Console.Clear();
        Console.WriteLine("=== Remove Account From Customer ===");

        Console.Write("Account Number: ");
        string accNumber = Console.ReadLine();

        int customerId;
        while (true)
        {
            Console.Write("Customer Id: ");
            if (int.TryParse(Console.ReadLine(), out customerId)) break;
            Console.WriteLine("Invalid ID. Enter a numeric value.");
        }

        using (var db = new BankDbContext())
        {
            var link = db.CustomerAccounts
                .Include(ca => ca.Account)
                .ThenInclude(a => a.CustomerAccounts)
                .FirstOrDefault(ca => ca.Account.AccountNumber == accNumber && ca.CustomerId == customerId);

            if (link == null)
            {
                Console.WriteLine("[Error] Relationship not found!");
                return;
            }

            var account = link.Account;
            db.CustomerAccounts.Remove(link);
            db.SaveChanges();

            Console.WriteLine("Ownership link deleted.");

            if (!account.CustomerAccounts.Any())
            {
                db.Accounts.Remove(account);
                db.SaveChanges();
                Console.WriteLine($"That was the last owner; account '{accNumber}' was also removed.");
            }
        }
    }

    static void ListAllCustomers()
    {
        Console.Clear();
        Console.WriteLine("=== All Customers ===");

        using (var db = new BankDbContext())
        {
            var customers = db.Customers
                .Include(c => c.CustomerAccounts)
                    .ThenInclude(ca => ca.Account)
                        .ThenInclude(a => a.Branch)
                .ToList();

            if (!customers.Any())
            {
                Console.WriteLine("No customers found.");
                return;
            }

            foreach (var c in customers)
            {
                Console.WriteLine($"\n#{c.Id} {c.FullName} ({c.CustomerType})");

                if (!c.CustomerAccounts.Any())
                {
                    Console.WriteLine("\t(no accounts)");
                }
                else
                {
                    foreach (var ca in c.CustomerAccounts)
                    {
                        var acc = ca.Account;
                        Console.WriteLine($"\t{acc.AccountNumber} Current Balance: {acc.CurrentBalance:N2} {ca.OwnershipType} {acc.AccountStatus} @ {acc.Branch.Name}");
                    }
                }
            }
        }
    }
}