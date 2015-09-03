namespace KanbanBoardApi.Commands
{
    public class DeleteBoardColumnCommand : ICommand
    {
        public string BoardSlug { get; set; }

        public string BoardColumnSlug { get; set; }
    }
}