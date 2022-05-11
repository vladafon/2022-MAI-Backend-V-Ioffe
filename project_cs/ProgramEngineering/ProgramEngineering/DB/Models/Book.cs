using System;

namespace ProgramEngineering.DB.Models
{
    public class Book
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public Author Author { get; set; }
        public long AuthorId { get; set; }
        public DateTime PublicationDate { get; set; }
    }
}