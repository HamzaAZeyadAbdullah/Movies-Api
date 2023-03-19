using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using MoviesApi.Models;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;


        private new List<string> _allowedExtention=new List<string> { ".jpg",".png"};
        private long _maxAllowedPosterSize = 1048576 ;


        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }


     



        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _context.Movies.OrderByDescending(x=>x.Rate).Include(m=>m.Genre).Select(m=> new MovieDetailsDTO
            {
             GenreId =m.GenreId,
             GenreName=m.Genre.Name,
             //Poster=m.Poster,
             Rate=m.Rate,
             StoreLine=m.StoreLine,
             Title=m.Title,
             Year=m.Year
            
            }).ToListAsync();

            return Ok(movies);
            
            
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movies = await _context.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m=>m.Id==id);

            if (movies == null)
                return NotFound();


            var dto = new MovieDetailsDTO
            {
                GenreId = movies.GenreId,
                GenreName = movies.Genre?.Name,
              Poster=movies.Poster,
                Rate = movies.Rate,
                StoreLine = movies.StoreLine,
                Title = movies.Title,
                Year = movies.Year
            };
            return Ok(dto);
        }



        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreIdAsync(byte genreId)
        {
            var movies = await _context.Movies.Where(m=>m.GenreId==genreId).OrderByDescending(x => x.Rate).Include(m => m.Genre).Select(m => new MovieDetailsDTO
            {
                GenreId = m.GenreId,
                GenreName = m.Genre.Name,
                //Poster=m.Poster,
                Rate = m.Rate,
                StoreLine = m.StoreLine,
                Title = m.Title,
                Year = m.Year

            }).ToListAsync();

            return Ok(movies);
        }



        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm]MoveDto dto)
        {
            if (dto.Poster == null)
                return BadRequest("Poster is required");

            if (!_allowedExtention.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("Only png or jpg Images");

            if (dto.Poster.Length > _maxAllowedPosterSize)
                return BadRequest("Max allowed size for poster is 1MB ");


            var isValidGenre = await _context.Genres.AnyAsync(g =>g.Id==dto.GenreId);
            if (!isValidGenre)
                return BadRequest("Invalid Genre Id");


            using var dataStream =new MemoryStream();

            await dto.Poster.CopyToAsync(dataStream);

            var movie = new Movie
            {
                GenreId = dto.GenreId,  
                Title = dto.Title,
                Poster= dataStream.ToArray(),  
                Rate=dto.Rate,
                StoreLine=dto.StoreLine,
                Year = dto.Year
            };
            await _context.AddAsync(movie);
            _context.SaveChanges();
            return Ok(movie);

        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] MoveDto dto)
        {
            var mpvie = await _context.Movies.FindAsync(id);

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound($"No Movie ID {id}");

            var isValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if (!isValidGenre)
                return BadRequest("Invalid Genre Id");



            if(dto.Poster != null)
            {
                if (!_allowedExtention.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("Only png or jpg Images");

                if (dto.Poster.Length > _maxAllowedPosterSize)
                    return BadRequest("Max allowed size for poster is 1MB ");


                using var dataStream = new MemoryStream();

                await dto.Poster.CopyToAsync(dataStream);

                movie.Poster = dataStream.ToArray();
            }

            movie.Title = dto.Title;
            movie.GenreId = dto.GenreId;
            movie.Year = dto.Year;
            movie.Rate = dto.Rate;
            movie.StoreLine = dto.StoreLine;

            _context.SaveChanges();
            return Ok(movie);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {



            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound($"No Movie ID {id}");
            _context.Remove(movie);
            _context.SaveChanges();
            return Ok(movie);
        }

    }
}
