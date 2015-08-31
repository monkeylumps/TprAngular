using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanBoardApi.Domain
{
    public class BoardEntity : EntityBase
    {
        public BoardEntity()
        {
            Columns = new List<BoardColumnEntity>();
        }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string Slug { get; set; }

        public IList<BoardColumnEntity> Columns { get; set; }
    }
}