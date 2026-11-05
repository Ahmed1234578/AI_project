using System;
using System.Collections.Generic;

public class Branch
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }

    public Manager Manager { get; set; }
    public ICollection<Account> Accounts { get; set; } = new HashSet<Account>();
}

public class Manager
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime HireDate { get; set; }
    public string Address { get; set; }

    public int BranchId { get; set; }
    public Branch Branch { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string NationalId { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string CustomerType { get; set; }

    public ICollection<CustomerAccount> CustomerAccounts { get; set; } = new HashSet<CustomerAccount>();
}

public class Account
{
    public int Id { get; set; }
    public string AccountNumber { get; set; }
    public decimal CurrentBalance { get; set; }
    public string AccountType { get; set; }
    public DateTime OpeningDate { get; set; } = DateTime.Now;
    public string AccountStatus { get; set; } = "Active";

    public int BranchId { get; set; }
    public Branch Branch { get; set; }

    public ICollection<CustomerAccount> CustomerAccounts { get; set; } = new HashSet<CustomerAccount>();
    public ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
}

public class CustomerAccount
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }

    public int AccountId { get; set; }
    public Account Account { get; set; }

    public string OwnershipType { get; set; }
    public DateTime OwnershipStartDate { get; set; } = DateTime.Now;
}

public class Transaction
{
    public int Id { get; set; }
    public string TransactionNumber { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.Now;
    public decimal Amount { get; set; }
    public string TransactionType { get; set; }
    public string Note { get; set; }

    public int AccountId { get; set; }
    public Account Account { get; set; }
}
