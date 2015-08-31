using System.Data.Entity;
using KanbanBoardApi.EntityFramework.EntityMapping;
using KanbanBoardApi.EntityFramework.Migrations;

namespace KanbanBoardApi.EntityFramework
{
    public class DataContext : DbContext, IDataContext
    {
        public DataContext() : base("ValidationPoc")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Configuration>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new BoardMapping());
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
    }
}