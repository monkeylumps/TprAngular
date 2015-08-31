namespace KanbanBoardApi.Commands.Services
{
    public interface ISlugService
    {
        string Slugify(string input);
    }
}