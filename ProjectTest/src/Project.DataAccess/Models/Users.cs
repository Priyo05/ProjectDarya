using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DataAccess.Models
{
    public class Users : IdentityUser
    {
        public string Fullname { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
