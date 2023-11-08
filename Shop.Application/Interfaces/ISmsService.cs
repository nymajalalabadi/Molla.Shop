using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Interfaces
{
    public interface ISmsService
    {
        Task SendVerificationCode(string mobile, string activeCode);
    }
}
