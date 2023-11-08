using Microsoft.Extensions.DependencyInjection;
using Shop.Application.Interfaces;
using Shop.Application.Services;
using Shop.Domain.Interfaces;
using Shop.Infra.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Infra.IoC
{
    public class DependencyContainer
    {
        public static void RejosterService(IServiceCollection services)
        {
            #region services

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<ISmsService, SmsService>();

            #endregion


            #region repositories

            services.AddScoped<IUserRepository, UserRepository>();

            #endregion


            #region tools

            services.AddScoped<IPasswordHelper, PasswordHelper>();

            #endregion
        }
    }
}
