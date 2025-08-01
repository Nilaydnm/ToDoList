using Business.Interfaces;
using DataAccess.Interfaces;
using Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Managers
{
    public class UserManager : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher = new();
        private readonly IValidator<User> _validator;


        public UserManager(IUserRepository userRepository, IValidator<User> validator)
        {
            _userRepository = userRepository;
            _validator = validator;
        }

        public async Task AddAsync(User user)
        {

            var validationResult = await _validator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ValidationException(validationResult.Errors);
            }

            //FLuentVlidation kullanarak kullanıcı doğrulama işlemleri yapılabilir
            user.Password = _passwordHasher.HashPassword(user, user.Password);
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(User user)
        {
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _userRepository.Remove(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public Task<List<User>> GetByUserIdAsync(int userId)
        {
            return _userRepository.GetByUserIdAsync(userId);
        }
        public async Task CheckIfUsernameExistsAsync(string username)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(username);
            if (existingUser != null)
            {
                throw new Exception("Bu kullanıcı adı zaten alınmış. Lütfen farklı bir kullanıcı adı girin.");
            }
        }

    }
}

