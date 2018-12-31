using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GiveNTake.Model;
using GiveNTake.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GiveNTake.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly GiveNTakeContext _context;
        private static readonly IMapper _productsMapper;

        static ProductsController()
        {
            var config = new MapperConfiguration(cfg =>
            {
            cfg.CreateMap<Product, ProductDTO>()
                .ForMember(dto => dto.City, opt => opt.MapFrom(product => product.City))
                .ForMember(dto => dto.Category, opt => opt.MapFrom(product => product.Category.ParentCategory.Name))
                .ForMember(dto => dto.SubCategory, opt => opt.MapFrom(product => product.Category.Name));

            cfg.CreateMap<User, OwnerDTO>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(product => product.Id));

            cfg.CreateMap<City, CityDTO>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(city => city.CityId));

            cfg.CreateMap<ProductMedia, MediaDTO>();

                cfg.CreateMap<Category, CategoryDTO>();

                cfg.CreateMap<Category, SubCategoryDTO>();
            });

            _productsMapper = config.CreateMapper();
        }
        public ProductsController(GiveNTakeContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        [HttpGet]
        [HttpGet("all")]
        public async Task<ActionResult<ProductDTO[]>> GetProducts()
        {
            var products = await _context.Products
                 .Include(p => p.Owner)
                 .Include(p => p.City)
                 .Include(p => p.Category)
                 .ThenInclude(c => c.ParentCategory)
                 .ToArrayAsync();
            return _productsMapper.Map<ProductDTO[]>(products);
        }

        [AllowAnonymous]
        [HttpGet("{productId}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int productId)
        {
            var product = await _context.Products
                .Include(p => p.Owner)
                .Include(p => p.City)
                .Include(p => p.Media)
                .Include(c => c.Category)
                .ThenInclude(c => c.ParentCategory)
                .SingleOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return NotFound();
            }

            return _productsMapper.Map<ProductDTO>(product);
        }

        
        [HttpPost("")]
        public async Task<ActionResult<ProductDTO>> AddNewProduct([FromBody] NewProductDTO newProductDTO)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();//(ModelState);
            }

            Category category = _context.Categories
                                    .Include(c => c.ParentCategory)
                                    .SingleOrDefault(c => c.Name == newProductDTO.SubCategory || c.ParentCategory.Name == newProductDTO.Category);
            if (category == null)
            {
                return new BadRequestObjectResult("The provided category and sub category doesnt exist");
            }

            City city = _context.Cities.SingleOrDefault(c => c.Name == newProductDTO.City);

            if (city == null)
            {
                return new BadRequestObjectResult("The provided category and sub category doesnt exist");
            }
            User owner = await _context.Users.FindAsync(User.Identity.Name);
            var product = new Product()
            {
                Owner = owner,
                Title = newProductDTO.Title,
                Description = newProductDTO.Description,
                Category = category,
                City = city,
                Media = null,
                PublishDate = DateTime.Now
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(GetProduct), 
                new { productId = product.ProductId }, 
                _productsMapper.Map<ProductDTO>(product)
                );
        }

        [HttpDelete("{productId}")]
        public async Task<ActionResult> DeleteProduct(int productId)
        {
            var product = await _context.Products.SingleOrDefaultAsync(p => p.ProductId == productId);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{productId}")]
        public async Task<ActionResult> UpdateProduct(int productId, [FromBody] NewProductDTO newProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Category category = _context.Categories
                .SingleOrDefault(c => c.Name == newProduct.SubCategory || c.ParentCategory.Name == newProduct.Category);
            if (category == null)
            {
                return new BadRequestObjectResult("The provided category and sub category doesnt exist");
            }
            City city = _context.Cities.SingleOrDefault(c => c.Name == newProduct.City);
            if (city == null)
            {
                return new BadRequestObjectResult("The provided city doesnt exist");
            }
            var product = new Product()
            {
                ProductId = productId,
                Category = category,
                Title = newProduct.Title,
                Description = newProduct.Description,
                City = city,
                PublishDate = DateTime.UtcNow
            };
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [HttpGet("categories")]
        public async Task<ActionResult<CategoryDTO[]>> GetCategoriesAsync()
        {
            var categories = await _context.Categories
                                .Where(c => c.ParentCategory == null)
                                .Include(c => c.SubCategories)
                                .ToArrayAsync();
            return _productsMapper.Map<CategoryDTO[]>(categories);
        }

        [AllowAnonymous]
        [HttpGet("cities")]
        public async Task<ActionResult<string[]>> GetCities()
        {
            var cities = await _context.Cities
                .Select(c => c.Name)
                .ToArrayAsync();
            return cities;
        }

        [Authorize(Policy ="ExperiencedUser")]
        [HttpPost("categories")]
        public async Task<ActionResult> AddCategory([FromBody] NewCategoryDTO newCategoryDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _context.Categories
                .Include(c => c.SubCategories)
                .SingleOrDefaultAsync(c => c.Name == newCategoryDTO.CategoryName && c.ParentCategory == null);

            if (!string.IsNullOrWhiteSpace(newCategoryDTO.SubCategoryName))         //Adding a subCategory
            {
                if (category == null)
                {
                    return NotFound(new SerializableError() { { nameof(newCategoryDTO.CategoryName), "Category not Found"} });
                }
                if(category.SubCategories.Any(c=>c.Name == newCategoryDTO.CategoryName)) //Subcategorie already exists
                {
                    return Ok();
                }
                category.SubCategories.Add(new Category { Name = newCategoryDTO.SubCategoryName, ParentCategory = category });
                
            }
            else if (category == null)    //Adding a new category
            {
                _context.Categories.Add(new Category { Name = newCategoryDTO.CategoryName });
            }


            await _context.SaveChangesAsync();
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("search/{keyword}")]
        public async Task<ActionResult<ProductDTO[]>> SearchProducts(string keyword)
        {
            var products = await _context.Products
                            .Include(p => p.Owner)
                            .Include(p => p.City)
                            .Include(p => p.Category)
                            .ThenInclude(c => c.SubCategories)
                            .Where(p => p.Title.Contains(keyword))
                            .ToListAsync();

            return _productsMapper.Map<ProductDTO[]>(products);
        }

        [AllowAnonymous]
        [HttpGet("searchcategory/{category}/{subcategory=all}/")]
        public async Task<ActionResult<ProductDTO[]>> SearchByProducts(string category, string subcategory, string location = "all", bool imageOnly = false)
        {
            if (string.IsNullOrEmpty(category))
            {
                return BadRequest();
            }

            IQueryable<Product> productsQuery = _context.Products
                .Include(p => p.Owner)
                .Include(p => p.City)
                .Include(p => p.Media)
                .Include(p => p.Category)
                .ThenInclude(c => c.ParentCategory);

            if (location != "all")
            {
                productsQuery = productsQuery.Where(p => p.City.Name == location);
            }

            if (subcategory != "all")
            {
                productsQuery = productsQuery.Where(p => p.Category.Name == subcategory)
                    .Where(p => p.Category.ParentCategory.Name == category);
            }
            else
            {
                productsQuery = productsQuery.Where(p => p.Category.Name == category || p.Category.ParentCategory.Name == category);
            }
            var products = await productsQuery.ToListAsync();

            return Ok(_productsMapper.Map<ProductDTO[]>(products));
        }

        [AllowAnonymous]
        [HttpGet("search/{date:datetime}/{keyword}/")]
        public async Task<ActionResult<ProductDTO[]>> Search(DateTime date, string keyword)
        {
            var products = await _context.Products
                .Include(p => p.Owner)
                .Include(p => p.City)
                .Include(p => p.Category)
                .ThenInclude(c => c.ParentCategory)
                .Where(p => p.Title.Contains(keyword))
                .Where(p => p.PublishDate.Date == date.Date)
                .ToListAsync();

            return _productsMapper.Map<ProductDTO[]>(products);
        }

    }
}
