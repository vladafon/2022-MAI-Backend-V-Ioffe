using Newtonsoft.Json;
using System;

namespace ProgramEngineering.Models
{
    public class Book
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("publicationDate")]
        public DateTime PublicationDate { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Title)
                || string.IsNullOrWhiteSpace(Author))
            {
                return false;
            }

            return true;
        }
    }
}
