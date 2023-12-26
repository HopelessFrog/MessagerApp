using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messager.Models;

namespace Messager.Services.Message
{
    class MessageInitializeReponse : BaseResponse
    {
        public User FriendInfo { get; set; }
        public IEnumerable<Models.Message> Messages { get; set; }
    }
}
