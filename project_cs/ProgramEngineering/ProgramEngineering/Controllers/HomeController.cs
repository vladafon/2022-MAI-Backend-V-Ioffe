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
using ProgramEngineering.Logic;

namespace ProgramEngineering.Controllers
{
    [Route("api/v1")]
    public class HomeController : Controller
    {
        private readonly ApiDbContext _dbContext;
        private readonly S3Repository _s3Repository;

        public HomeController(ApiDbContext dbContext, S3Repository s3Repository)
        {
            _dbContext = dbContext;
            _s3Repository = s3Repository;
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

        [HttpPut("files/upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadDocumentToS3(IFormFile file)
        {
            if (file is null || file.Length <= 0)
                return BadRequest();

            using (var stream = file.OpenReadStream())
            {
                await _s3Repository.PutFile(stream, file.FileName);
            }
            return Ok();
        }

        [HttpGet("files/get/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDocumentsFromS3()
        {
            var files = await _s3Repository.GetFiles();
            return ConvertToJsonResponse(files);
        }

        [HttpGet("files/get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDocumentFromS3(string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest();

            var document = await _s3Repository.GetFile(name);

            return File(document, "application/octet-stream", name);
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
