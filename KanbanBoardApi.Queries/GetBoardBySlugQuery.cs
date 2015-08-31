namespace KanbanBoardApi.Queries
{
    public class GetBoardBySlugQuery : IQuery
    {
        public string BoardSlug { get; set; }
    }

    public interface IQuery
    {
    }
}