namespace KanbanBoardApi.Domain
{
    public class BoardColumn : EntityBase
    {
        public string Name { get; set; }

        public int Order { get; set; }
    }
}