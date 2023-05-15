using EmployeeManager.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManager.Data
{
    public class EmployeeDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public EmployeeDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
    }
}
