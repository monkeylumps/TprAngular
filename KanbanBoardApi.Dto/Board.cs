using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KanbanBoardApi.Dto.Validators;
using FluentValidation.Attributes;

namespace KanbanBoardApi.Dto
{
    [Validator(typeof(BoardValidator))]
    public class Board : IHyperMediaItem
    {
        [StringLength(100)]
        public string Slug { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public IList<Link> Links { get; set; }
    }
}