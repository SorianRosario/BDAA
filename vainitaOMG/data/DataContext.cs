using Microsoft.EntityFrameworkCore;

public class DataContext : DbContext
{
    // DbSets for each of your entities
    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Articulos> Articulos { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
    public DbSet<Invoice> Invoices { get; set; } = null!;

    // Constructor that accepts DbContextOptions
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    // Configure relationships and additional configurations in this method
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Order-Employee relationship (One employee can have many orders)
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Employee)
            .WithMany(e => e.Orders)
            .HasForeignKey(o => o.EmployeeId);

        // OrderDetail-Order relationship (One order can have many order details)
        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId);

        // OrderDetail-Article relationship (One order detail can reference one article)
        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Article)
            .WithMany()
            .HasForeignKey(od => od.ArticleId);

        // Invoice-Order relationship (One order can generate one invoice)
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Order)
            .WithMany()
            .HasForeignKey(i => i.OrderId);
    }
}
