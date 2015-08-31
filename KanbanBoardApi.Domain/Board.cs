using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanBoardApi.Domain
{
    public class Board : EntityBase
    {
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        [Index(IsUnique=true)]
        public string Slug { get; set; }

        public IList<BoardColumn> Columns { get; set; }

        public Board()
        {
            Columns = new List<BoardColumn>();
        }
    }
}