using System.Collections.Generic;

namespace GiveNTake.Model
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public IList<Category> SubCategories { get; set; }
        public Category ParentCategory { get; set; }
       
    }
}