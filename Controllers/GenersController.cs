using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Models;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var geners=await _context.Genres.OrderBy(x=>x.Name).ToListAsync();

            return Ok(geners);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateGenreDto dto)
        {
            var genre = new Genre { Name = dto.Name };
            //await _context.Genres.AddAsync(genre);
            //same
            await _context.AddAsync(genre);
            _context.SaveChanges();
            return Ok(genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] CreateGenreDto dto)
        {
            var genre= await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
            if (genre == null)
                return NotFound($"No genre found with id :{id}");
            genre.Name=dto.Name;

            _context.SaveChanges(); 

            return Ok(genre.Id);    
        }


        [HttpDelete(template:"{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
            if (genre == null)
                return NotFound($"No genre found with id :{id}");
            //_context.Genres.Remove(genre);
            _context.Remove(genre);
            _context.SaveChanges();
            return Ok(genre);
        }
        
    }
}
