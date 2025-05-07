using FluentValidation;
using SmartTaskAPI.Dtos;

namespace SmartTaskAPI.Validators
{
    public class TaskItemCreateDtoValidator : AbstractValidator<TaskItemCreateDto>
    {
        public TaskItemCreateDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Başlık boş olamaz.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Açıklama boş olamaz.");
        }
    }
}
