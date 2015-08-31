using FluentValidation;

namespace KanbanBoardApi.Dto.Validators
{
    public class BoardValidator : AbstractValidator<Board>
    {
        public BoardValidator()
        {
            RuleFor(x => x.Name).Length(1, 100);
        }
    }
}