using System.Data.Entity;
using KanbanBoardApi.EntityFramework.EntityMapping;
using KanbanBoardApi.EntityFramework.Migrations;

namespace KanbanBoardApi.EntityFramework
{
    public class DataContext : DbContext, IDataContext
    {
        public DataContext() : base("KanbanBoard")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Configuration>());
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new BoardMapping());
            modelBuilder.Configurations.Add(new BoardColumnMapping());
            modelBuilder.Configurations.Add(new BoardTaskMapping());
        }
    }
}