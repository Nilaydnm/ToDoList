using Business.Interfaces;
using DataAccess.Interfaces;
using Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules
{
    public class UserValidator : AbstractValidator<User>
    {
        private readonly IUserRepository _userRepository;
        public UserValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(user => user.Username)
                .NotEmpty().WithMessage("Kullanıcı adı boş bırakılamaz!")
                .Length(3, 20).WithMessage("Kullanıcı adı 3 ila 20 karakter uzunluğunda olmalıdır.")
                .Matches("^[a-zA-Z0-9]+$").WithMessage("Kullanıcı adı yalnızca harf ve rakamlardan oluşabilir.")
                .Must((user) => 
                {
                    var existingUser =  _userRepository.GetByUsernameAsync(user).Result;
                    return existingUser == null;
                }).WithMessage("Bu kullanıcı adı zaten alınmış. Lütfen farklı bir kullanıcı adı girin.");
            
            
            RuleFor(user => user.Password).NotEmpty()
                .WithMessage("Şifre boş bırakılamaz!")
                .Length(6, 100).WithMessage("Şifre 6 ila 100 karakter uzunluğunda olmalıdır.")
                .Matches("^[a-zA-Z0-9]+$").WithMessage("Şifre yalnızca harf ve rakamlardan oluşabilir.");
            

        }
    }
}
