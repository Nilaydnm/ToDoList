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
        private readonly IToDoGroupRepository _toDoGroupRepository;

        

        public ToDoGroupValidator(IToDoGroupRepository toDoGroupRepository)
        {
            _toDoGroupRepository = toDoGroupRepository;
            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Başlık 100 karakteri geçmemeli")
                .MinimumLength(3).WithMessage("Başlık en az 3 karakter olmalı");

        }
    }
}
