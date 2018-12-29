using System.Collections.Generic;

namespace GiveNTake.Model
{
    public class User
    {
        public string Id { get; set; }
        public IList<Product> Products { get; set; }
    }
}