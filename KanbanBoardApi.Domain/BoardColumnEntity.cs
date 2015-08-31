namespace KanbanBoardApi.Domain
{
    public class BoardColumnEntity : EntityBase
    {
        public string Name { get; set; }

        public string Slug { get; set; }

        public int Order { get; set; }

        public BoardEntity BoardEntity { get; set; }
    }
}