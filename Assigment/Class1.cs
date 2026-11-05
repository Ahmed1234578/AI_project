using Microsoft.EntityFrameworkCore;

public class BankDbContext : DbContext
{
    public DbSet<Branch> Branches { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<CustomerAccount> CustomerAccounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=NationalBankDb;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CustomerAccount>()
            .HasKey(ca => new { ca.CustomerId, ca.AccountId });

        modelBuilder.Entity<CustomerAccount>()
            .HasOne(ca => ca.Customer)
            .WithMany(c => c.CustomerAccounts)
            .HasForeignKey(ca => ca.CustomerId);

        modelBuilder.Entity<CustomerAccount>()
            .HasOne(ca => ca.Account)
            .WithMany(a => a.CustomerAccounts)
            .HasForeignKey(ca => ca.AccountId);

        modelBuilder.Entity<Branch>()
            .HasOne(b => b.Manager)
            .WithOne(m => m.Branch)
            .HasForeignKey<Manager>(m => m.BranchId);

        modelBuilder.Entity<Branch>().HasData(
            new Branch { Id = 1, Code = "CAI-01", Name = "Cairo Main Branch", Address = "Tahrir Square, Cairo", PhoneNumber = "0223456789" },
            new Branch { Id = 2, Code = "ALX-02", Name = "Alexandria Branch", Address = "Corniche St, Alexandria", PhoneNumber = "0334567890" }
        );

        modelBuilder.Entity<Manager>().HasData(
            new Manager { Id = 1, FullName = "Ibrahim Khaled", Email = "ibrahim@bank.com", PhoneNumber = "01012345678", HireDate = new DateTime(2018, 5, 1), Address = "Cairo", BranchId = 1 },
            new Manager { Id = 2, FullName = "Mohamed Tarek", Email = "tarek@bank.com", PhoneNumber = "01212345678", HireDate = new DateTime(2020, 8, 15), Address = "Alexandria", BranchId = 2 }
        );
    }
}