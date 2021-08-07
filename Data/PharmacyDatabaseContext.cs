using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class PharmacyDatabaseContext : DbContext
    {
        public PharmacyDatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Drug>? Drugs { get; set; }
        public DbSet<User>? Users { get; set; }

        //timestamp management
        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries().Where(x =>
                x.Entity is PersistedEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow; // current datetime

                if (entity.State == EntityState.Added) ((PersistedEntity)entity.Entity).CreatedAt = now;
                ((PersistedEntity)entity.Entity).UpdatedAt = now;
            }
        }
    }
}