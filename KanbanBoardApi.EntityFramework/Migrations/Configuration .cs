using System.Data.Entity.Migrations;
using KanbanBoardApi.Domain;

namespace KanbanBoardApi.EntityFramework.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(DataContext context)
        {
            //  This method will be called after migrating to the latest version.
            context.Set<BoardEntity>().AddOrUpdate(x => x.Slug, new BoardEntity
            {
                Name = "Work List",
                Slug = "work-list"
            });
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}