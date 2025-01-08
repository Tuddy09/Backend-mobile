using Backend_mobile.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_mobile.Context
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<CarMaintenanceRecord> MaintenanceRecords { get; set; } = null!;
    }
}
