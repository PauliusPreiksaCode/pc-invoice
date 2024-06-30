using Invoice_generator.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invoice_generator
{
    public class DBContext : DbContext
    {
        public DBContext() { }

        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<BaseClient> BaseClients { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<Seller> Sellers { get; set; }
        public virtual DbSet<InvoicePurchases> InvoicePurchases { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
    }
}
