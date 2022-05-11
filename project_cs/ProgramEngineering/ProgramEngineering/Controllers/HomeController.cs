using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProgramEngineering.Models;
using ProgramEngineering.DB;
using Microsoft.EntityFrameworkCore;

namespace ProgramEngineering.Controllers
{
    [Route("api/v1")]
    public class HomeController : Controller
    {
        private readonly ApiDbContext _dbContext;
        public HomeController(ApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("profile/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProfile(long id)
        {
            //TODO: Get Info

            return ConvertToJsonResponse(new Profile 
            { 
                UserName = $"Vasya_{id}",
                BoughtBooksCount = 100500,
                Email = "vasya_pupkin1234@gmail.com",
            });
        }

        [HttpGet("books")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()
        {
            var booksList = await _dbContext.Books
                .Select(b => new Book
                {
                    Id = b.Id,
                    Author = b.Author.Name,
                    PublicationDate = b.PublicationDate,
                    Title = b.Title
                })
                .ToListAsync();

            //var booksList = new List<Book>
            //{
            //    new Book
            //    {
            //        Title = "Семь дней до Меггидо",
            //        Author = "Лукьяненко С.В.",
            //        PublicationDate = new DateTime(2021,11,1),
            //    },
            //    new Book
            //    {
            //        Title = "1984",
            //        Author = "Д. Оруэлл",
            //        PublicationDate = new DateTime(1948,6,8)
            //    },
            //};

            return ConvertToJsonResponse(booksList);
        }

        [HttpGet("books/search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindBooks(string search)
        {
            var booksList = await _dbContext.Books
                .Where(b=>b.Title.Contains(search))
                .Select(b => new Book
                {
                    Id = b.Id,
                    Author = b.Author.Name,
                    PublicationDate = b.PublicationDate,
                    Title = b.Title
                })
                .ToListAsync();

            return ConvertToJsonResponse(booksList);
        }

        [HttpGet("books/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBook(long id)
        {
            var book = await _dbContext.Books
                 .Where(b=>b.Id == id)
                 .Select(b => new Book
                 {
                     Id = b.Id,
                     Author = b.Author.Name,
                     PublicationDate = b.PublicationDate,
                     Title = b.Title
                 })
                 .FirstOrDefaultAsync();


            return ConvertToJsonResponse(book);
        }

        [HttpPost("books")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddBook([FromBody] Book book)
        {
            if (!book.IsValid())
            {
                return BadRequest(book);
            }

            var authorId = await _dbContext.Authors
                .Where(a => a.Name == book.Author)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();

            if (authorId <= 0)
            {
                var authorDb = new DB.Models.Author
                {
                    Name = book.Author
                };
                _dbContext.Add(authorDb);
                _dbContext.SaveChanges();
                authorId = authorDb.Id;
            }
            //Write to DB
            var bookDb = new DB.Models.Book
            {
                PublicationDate = book.PublicationDate,
                Title = book.Title,
                AuthorId = authorId
            };
            _dbContext.Add(bookDb);
            _dbContext.SaveChanges();
            return ConvertToJsonResponse(book);
        }

        private IActionResult ConvertToJsonResponse(object obj)
        {
            if (obj is null)
            {
                return NotFound(obj);
            }

            return Ok(JsonConvert.SerializeObject(obj));
        }
    }
}
