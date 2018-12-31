using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace GiveNTake.Model
{
    public class User : IdentityUser
    {
        public IList<Product> Products { get; set; }
    }
}