using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messager.Models
{
    public class ConnectedUser
    {
        public StreamReader Sr { get; set; }
        public StreamWriter Sw { get; set; }
    }
}
