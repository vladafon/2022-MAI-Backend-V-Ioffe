using ProgramEngineering.DB.Models;
using System;
using System.Linq;

namespace ProgramEngineering.DB
{
    public static class DbInitializer
    {
        public static void Initialize(ApiDbContext ctx)
        {
            ctx.Database.EnsureCreated();

            if (!ctx.Authors.Any())
            {
                ctx.Authors.Add(new Author
                {
                    Name = "Лукьяненко С.В."
                });
                ctx.Authors.Add(new Author
                {
                    Name = "Д. Оруэлл"
                });
            }

            if (!ctx.Books.Any())
            {
                ctx.Books.Add(new Book
                {
                    Title = "Семь дней до Меггидо",
                    AuthorId = 1,
                    PublicationDate = new DateTime(2021, 11, 1),
                });
                ctx.Books.Add(new Book
                {
                    Title = "1984",
                    AuthorId = 2,
                    PublicationDate = new DateTime(1948, 6, 8)
                });
                ctx.SaveChanges();
            }
        }
    }
}
