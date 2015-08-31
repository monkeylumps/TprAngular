namespace KanbanBoardApi.Queries
{
    public class GetBoardBySlugQuery : IQuery
    {
        public string Slug { get; set; }
    }

    public interface IQuery
    {
    }
}