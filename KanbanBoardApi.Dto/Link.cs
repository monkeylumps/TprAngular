namespace KanbanBoardApi.Dto
{
    public class Link
    {
        public const string SELF = "self";

        public string Rel { get; set; }

        public string Href { get; set; }
    }
}