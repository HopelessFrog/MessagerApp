using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messager.Models;

namespace Messager.Services.Interfaces
{
    public interface ILoginService
    {
        Task<ConnectedUser> Login(UserInfo userData);
    }
}
