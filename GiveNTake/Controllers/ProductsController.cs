using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GiveNTake.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GiveNTake.Controllers
{   [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : Controller
    {
       [HttpGet]
       [HttpGet("all")]
        public string[] GetProducts()
        {
            return new[]
            {
                "1 - Microwave",
                "2 - Washing Machine",
                "3 - Mirror"
            };
        }
        [HttpGet("searchcategory/{category}/{subcategory=all}/")]
        public string[] SearchByProducts(string category, string subcategory, string location = "all", bool imageOnly = false)
        {
            return new[] { $"Category: {category}, Subcategory: {subcategory}, Location: {location}, Only with Images: {imageOnly}" };
        }

        [HttpGet("search/{date:datetime}/{keyword}/")]
        public string[] Search(string date, string keyword)
        {
            return new[]
            {
                $"Date: {date}, keyword: {keyword}"
            };
        }
        [HttpPost("")]
        public async Task<ActionResult<NewProductDTO>> AddNewProduct([FromBody] NewProductDTO newProduct)
        {
            await Task.Delay(1000);
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();//(ModelState);
            }
            return new ObjectResult(newProduct);
        }

    }
}
