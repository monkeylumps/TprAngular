namespace KanbanBoardApi.HyperMedia.States
{
    public interface IHyperMediaState
    {
        bool IsAppliable(object obj);
        void Apply(object obj);
    }
}