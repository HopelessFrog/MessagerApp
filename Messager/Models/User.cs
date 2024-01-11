using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messager.Models
{
     public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public byte[] AvatarSource { get; set; } = null!;
        public bool IsOnline { get; set; }
        public DateTime LastLogonTime { get; set; }
        public bool IsAway { get; set; }
        public string AwayDuration { get; set; } = null!;
    }
}
