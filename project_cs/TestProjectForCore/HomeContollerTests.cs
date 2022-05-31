using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.EntityFrameworkCore;
using ProgramEngineering.Controllers;
using ProgramEngineering.DB;
using ProgramEngineering.DB.Models;
using ProgramEngineering.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProjectForCore
{
    public class HomeControllerTests
    {
        
        private static readonly Fixture Fixture = new Fixture();

        public HomeControllerTests()
        {
            Fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetBooks_WhenEmptyList_ReturnsOkResult()
        {
            // Arrange
            var books = GetEmptyBooks();

            var dbContextMock = new Mock<ApiDbContext>();
            dbContextMock.Setup(x => x.Books).ReturnsDbSet(books);

            var config = InitConfiguration();
            var mockRepo = new Mock<S3Repository>(config);

            var controller = new HomeController(dbContextMock.Object, mockRepo.Object);

            // Act
            var result = await controller.GetBooks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            //Assert.IsType<SerializableError>(okResult.Value);
        }

        [Fact]
        public async Task GetBooks_WhenListWithBooks_ReturnsOkResult()
        {
            // Arrange
            var books = GetFakeBooks();

            var dbContextMock = new Mock<ApiDbContext>();
            dbContextMock.Setup(x => x.Books).ReturnsDbSet(books);

            var config = InitConfiguration();
            var mockRepo = new Mock<S3Repository>(config);

            var controller = new HomeController(dbContextMock.Object, mockRepo.Object);

            // Act
            var result = await controller.GetBooks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            //Assert.IsType<SerializableError>(okResult.Value);
        }

        [Fact]
        public async Task FindBooks_SearchIsPresented_ReturnsOkResultWithData()
        {
            // Arrange
            var books = GetFakeBooks();

            var dbContextMock = new Mock<ApiDbContext>();
            dbContextMock.Setup(x => x.Books).ReturnsDbSet(books);

            var config = InitConfiguration();
            var mockRepo = new Mock<S3Repository>(config);

            var controller = new HomeController(dbContextMock.Object, mockRepo.Object);

            // Act
            var result = await controller.FindBooks(books[0].Title);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains(books[0].Title, okResult.Value.ToString());
        }

        [Fact]
        public async Task GetBook_GiveValidId_ReturnsOkResultWithData()
        {
            // Arrange
            var books = GetFakeBooks();

            var dbContextMock = new Mock<ApiDbContext>();
            dbContextMock.Setup(x => x.Books).ReturnsDbSet(books);

            var config = InitConfiguration();
            var mockRepo = new Mock<S3Repository>(config);

            var controller = new HomeController(dbContextMock.Object, mockRepo.Object);

            // Act
            var result = await controller.GetBook(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains(books[0].Title, okResult.Value.ToString());
        }

        [Fact]
        public async Task GetBook_GiveInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var books = GetFakeBooks();

            var dbContextMock = new Mock<ApiDbContext>();
            dbContextMock.Setup(x => x.Books).ReturnsDbSet(books);

            var config = InitConfiguration();
            var mockRepo = new Mock<S3Repository>(config);

            var controller = new HomeController(dbContextMock.Object, mockRepo.Object);

            // Act
            var result = await controller.GetBook(-1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task AddBook_GiveBook_ReturnsOkResult()
        {
            // Arrange
            var books = GetFakeBooks();
            var authors = GetFakeAuthors();

            var dbContextMock = new Mock<ApiDbContext>();
            dbContextMock.Setup(x => x.Books).ReturnsDbSet(books);
            dbContextMock.Setup(x => x.Authors).ReturnsDbSet(authors);

            var config = InitConfiguration();
            var mockRepo = new Mock<S3Repository>(config);

            var controller = new HomeController(dbContextMock.Object, mockRepo.Object);

            // Act
            var result = await controller.AddBook(new ProgramEngineering.Models.Book
            {
                Title = "TestBook",
                Author = "Faker F.F.",
                PublicationDate = new DateTime(2000, 01, 01)
            }
                ) ;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public async Task GetDocumentsFromS3_HasDocument_ReturnsOkResult()
        {
            // Arrange
            var dbContextMock = new Mock<ApiDbContext>();

            var config = InitConfiguration();
            var mockRepo = new Mock<S3Repository>(config);

            var controller = new HomeController(dbContextMock.Object, mockRepo.Object);

            // Act
            var result = await controller.GetDocumentsFromS3();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("1.png", okResult.Value.ToString());
        }

        [Fact]
        public async Task GetDocumentFromS3_HasDocument_ReturnsOkResult()
        {
            // Arrange
            var dbContextMock = new Mock<ApiDbContext>();

            var config = InitConfiguration();
            var mockRepo = new Mock<S3Repository>(config);

            var controller = new HomeController(dbContextMock.Object, mockRepo.Object);

            // Act
            var result = await controller.GetDocumentFromS3("1.png");

            // Assert
            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async Task UploadDocumentFromS3_HasDocument_ReturnsOkResult()
        {
            // Arrange
            var dbContextMock = new Mock<ApiDbContext>();

            var config = InitConfiguration();
            var mockRepo = new Mock<S3Repository>(config);

            var controller = new HomeController(dbContextMock.Object, mockRepo.Object);

            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.txt");

            // Act
            var result = await controller.UploadDocumentToS3(file);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        private IList<Book> GetFakeBooks()
        {
            var authors = new List<Author>
            { 
                new Author
                {
                    Id = 1,
                    Name = "Лукьяненко С.В."
                },
                new Author
                {
                    Id = 2,
                    Name = "Д. Оруэлл"
                }
            };



            var booksList = new List<Book>
            {
                new Book
                {
                    Id = 1,
                    Title = "Семь дней до Меггидо",
                    Author = authors[0],
                    AuthorId = 1,
                    PublicationDate = new DateTime(2021,11,1),
                },
                new Book
                {
                    Id = 2,
                    Title = "1984",
                    Author = authors[1],
                    AuthorId = 2,
                    PublicationDate = new DateTime(1948,6,8)
                },
            };


            IList<Book> books = new List<Book>
            {
                Fixture.Build<Book>()
                .With(u => u.Author , booksList[0].Author)
                .With(u => u.AuthorId , booksList[0].AuthorId)
                .With(u => u.Id , booksList[0].Id)
                .With(u => u.PublicationDate , booksList[0].PublicationDate)
                .With(u => u.Title , booksList[0].Title)
                .Create(),
                Fixture.Build<Book>()
                .With(u => u.Author , booksList[1].Author)
                .With(u => u.AuthorId , booksList[1].AuthorId)
                .With(u => u.Id , booksList[1].Id)
                .With(u => u.PublicationDate , booksList[0].PublicationDate)
                .With(u => u.Title , booksList[1].Title)
                .Create(),
            };

            return books;
        }

        private IList<Author> GetFakeAuthors()
        {
            var authorsList = new List<Author>
            {
                new Author
                {
                    Id = 1,
                    Name = "Лукьяненко С.В."
                },
                new Author
                {
                    Id = 2,
                    Name = "Д. Оруэлл"
                }
            };


            IList<Author> authors = new List<Author>
            {
                Fixture.Build<Author>()
                .With(u => u.Id , authorsList[0].Id)
                .With(u => u.Name , authorsList[0].Name)
                .Create(),
                Fixture.Build<Author>()
                .With(u => u.Id , authorsList[1].Id)
                .With(u => u.Name , authorsList[1].Name)
                .Create(),
            };

            return authors;
        }

        private IList<Book> GetEmptyBooks()
        {
            return new List<Book>();
        }

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.test.json")
                .AddEnvironmentVariables()
                .Build();
            return config;
        }

    }

}
