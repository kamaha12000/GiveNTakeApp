using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GiveNTake.Model;
using GiveNTake.Model.DTO;
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
            User owner = _context.Users.SingleOrDefault(u => u.Id == "seller1@seller.com");
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

        [HttpGet("categories")]
        public async Task<ActionResult<CategoryDTO[]>> GetCategoriesAsync()
        {
            var categories = await _context.Categories
                                .Where(c => c.ParentCategory == null)
                                .Include(c => c.SubCategories)
                                .ToArrayAsync();
            return _productsMapper.Map<CategoryDTO[]>(categories);
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


    }
}
