﻿namespace MoviesApi.DTos
{
    public class MoveDto
    {
        [MaxLength(255)]
        public string Title { get; set; }

        public int Year { get; set; }

        public double Rate { get; set; }


        [MaxLength(2500)]
        public string StoreLine { get; set; }


        public IFormFile? Poster { get; set; }


        public byte GenreId { get; set; }
    }
}
