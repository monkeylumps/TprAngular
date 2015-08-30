namespace KanbanBoardApi.Domain
{
    public class Board : EntityBase
    {
        public string Name { get; set; }

        public string Slug { get; set; }
    }
}