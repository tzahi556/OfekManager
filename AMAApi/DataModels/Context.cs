using FarmsApi.Migrations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace FarmsApi.DataModels
{
    public class Context : DbContext
    {

        public DbSet<Banks> Banks { get; set; }
        public DbSet<BanksBrunchs> BanksBrunchs { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Workers> Workers { get; set; }
        public DbSet<Cities> Cities { get; set; }
        public DbSet<Testpdfs> Testpdfs { get; set; }

        public DbSet<EsekConfiguraions> EsekConfiguraions { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<WorkerChilds> WorkerChilds { get; set; }
        


        public Context() : base("Farms") {
           Database.SetInitializer(new MigrateDatabaseToLatestVersion<Context, FarmsApi.Migrations.Configuration>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}