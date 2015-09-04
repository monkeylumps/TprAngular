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

            var boardEntity = new BoardEntity
            {
                Name = "Work List",
                Slug = "work-list",
            };

            context.Set<BoardEntity>().AddOrUpdate(x => x.Slug, boardEntity);

            var todoBoardColumnEntity = new BoardColumnEntity
            {
                BoardEntity = boardEntity,
                Slug = "to-do",
                Name = "To Do",
                Order = 1
            };

            var doingBoardColumnEntity = new BoardColumnEntity
            {
                BoardEntity = boardEntity,
                Slug = "doing",
                Name = "Doing",
                Order = 2
            };

            var doneBoardColumnEntity = new BoardColumnEntity
            {
                BoardEntity = boardEntity,
                Slug = "done",
                Name = "Done",
                Order = 3
            };

            context.Set<BoardColumnEntity>().AddOrUpdate(x => x.Slug, todoBoardColumnEntity);
            context.Set<BoardColumnEntity>().AddOrUpdate(x => x.Slug, doingBoardColumnEntity);
            context.Set<BoardColumnEntity>().AddOrUpdate(x => x.Slug, doneBoardColumnEntity);

            var boardTaskEntity = new BoardTaskEntity
            {
                Id = 1,
                BoardColumnEntity = todoBoardColumnEntity,
                Name = "Implement UI"
            };
            context.Set<BoardTaskEntity>().AddOrUpdate(x => x.Id, boardTaskEntity);
        }
    }
}