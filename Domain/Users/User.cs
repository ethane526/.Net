using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain.Users
{
    public class User : BaseUser
    {
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
