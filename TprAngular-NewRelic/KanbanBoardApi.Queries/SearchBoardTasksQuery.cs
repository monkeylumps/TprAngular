namespace KanbanBoardApi.Queries
{
    public class SearchBoardTasksQuery : IQuery
    {
        public string BoardSlug { get; set; }

        public string BoardColumnSlug { get; set; }
    }
}