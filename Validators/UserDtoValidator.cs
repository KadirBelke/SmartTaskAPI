using FluentValidation;
using SmartTaskAPI.Dtos;
using SmartTaskAPI.Models;

namespace SmartTaskAPI.Validators
{
    public class UserDtoValidator : AbstractValidator<UserRegisterDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.")
                .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalı.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre boş olamaz.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalı.");

            RuleFor(x => x.Role)
                .Must(role => Enum.IsDefined(typeof(User.RoleType), role))
                .WithMessage("Role must be 'User' or 'Admin'.");
        }
    }
}
