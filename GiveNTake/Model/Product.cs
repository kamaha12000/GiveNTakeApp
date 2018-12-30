﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GiveNTake.Model
{
    public class Product
    {
        //Primary Key
        public int ProductId { get; set; }

        //Value Properties
        public User Owner { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //Navigation properties - represent relationships
        public Category Category { get; set; }
        public City City { get; set;}
        public IList<ProductMedia> Media { get; set; }
        public DateTime PublishDate { get; set; }

    }
}
