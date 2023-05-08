using System.Data.Entity;
using WarehouseServer.Properties;
using WarehouseServer.Server.Entities;
using WarehouseServer.Server.Entities.Warehouse;

namespace WarehouseServer.Server.Sql {
    public class ServerContext : DbContext {

        private static readonly string CONNECTION_STRING
            = $@"Data Source={Settings.Default.DbServer};
                 Initial Catalog={Settings.Default.DbDatabase};
                 User ID={Settings.Default.DbUser};
                 Password={Settings.Default.DbPassword};
                 Integrated Security=False";

        public ServerContext() : base(CONNECTION_STRING) {
            Configuration.LazyLoadingEnabled = true;
        }

        public virtual DbSet<User> Users {
            get; set;
        }
        public virtual DbSet<Log> Logs {
            get; set;
        }
        public virtual DbSet<Part> Parts {
            get; set;
        }
        public virtual DbSet<Group> Groups {
            get; set;
        }
        public virtual DbSet<Unit> Units {
            get; set;
        }
        public virtual DbSet<Repair> Repairs {
            get; set;
        }
        public virtual DbSet<Request> Requests {
            get; set;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Entity<User>().HasIndex(e => e.Login).IsUnique();
            modelBuilder.Entity<Part>().HasIndex(e => new { e.Name, e.Type, e.Description }).IsUnique();
            modelBuilder.Entity<Group>().HasIndex(e => e.Name).IsUnique();
            modelBuilder.Entity<Unit>().HasIndex(e => e.Name).IsUnique();
        }
    }
}
