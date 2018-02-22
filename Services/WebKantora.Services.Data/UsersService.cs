﻿using System.Linq;

using WebKantora.Data.Common.Contracts;
using WebKantora.Data.Models;
using WebKantora.Services.Data.Contracts;

namespace WebKantora.Services.Data
{
    public class UsersService : IUsersService
    {
        private IUserDbRepository users;
        private IUnitOfWork unitOfWork;

        public UsersService(IUserDbRepository users, IUnitOfWork unitOfWork)
        {
            this.users = users;
            this.unitOfWork = unitOfWork;
        }

        public User GetByUserName(string userName)
        {
            var user = this.users.All()
                .Where(u => u.UserName == userName)
                .FirstOrDefault();
            return user;
        }
    }
}
