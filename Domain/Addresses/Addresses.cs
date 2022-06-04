using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain.Addresses
{
    //for data coming from the database 
    public class Address : BaseAddress
    {
        public bool IsActive { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
    }
}
