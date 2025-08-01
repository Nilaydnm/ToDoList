using DataAccess.Interfaces;
using Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Repositories;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules
{
   
    public class ToDoValidator : AbstractValidator<ToDo>
    {
        private readonly IToDoRepository _toDoRepository;
        public ToDoValidator(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Yapılacaklar boş bırakılamaz")
                .MaximumLength(100).WithMessage("Başlık 100 karakteri geçmemeli")
                .MinimumLength(3).WithMessage("Başlık en az 3 karakter olmalı");
            RuleFor(x => x.Deadline)
                .GreaterThan(DateTime.Now).WithMessage("Son teslim tarihi gelecek bir tarih olmalı");

            RuleFor(x => x.Deadline)
                .LessThan(DateTime.Now.AddYears(50))
                .WithMessage("Deadline çok uzak bir tarih olamaz");

        }

    }
}
