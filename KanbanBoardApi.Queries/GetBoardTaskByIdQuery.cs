namespace KanbanBoardApi.Queries
{
    public class GetBoardTaskByIdQuery : IQuery
    {
        public string BoardSlug { get; set; }

        public int TaskId { get; set; }
    }
}