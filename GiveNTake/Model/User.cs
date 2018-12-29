using System.Collections.Generic;

namespace GiveNTake.Model
{
    public class User
    {
        public int UserId { get; set; }
        public IList<Product> Products { get; set; }
    }
}