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
    public class ToDoGroupValidator : AbstractValidator<ToDoGroup>
    {
        public ToDoGroupValidator()
        {
            
            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Başlık 100 karakteri geçmemeli")
                .MinimumLength(2).WithMessage("Başlık en az 2 karakter olmalı");
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Başlık boş bırakılamaz");
               
        }
    }
}
