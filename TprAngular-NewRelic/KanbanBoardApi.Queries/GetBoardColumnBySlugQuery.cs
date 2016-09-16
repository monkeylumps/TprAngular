namespace KanbanBoardApi.Queries
{
    public class GetBoardColumnBySlugQuery : IQuery
    {
        public string BoardSlug { get; set; }

        public string BoardColumnSlug { get; set; }
    }
}