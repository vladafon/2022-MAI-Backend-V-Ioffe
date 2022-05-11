using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgramEngineering.DB.Models;

namespace ProgramEngineering.DB
{
    public class AuthorMap
    {
        public AuthorMap(EntityTypeBuilder<Author> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
            entityBuilder.ToTable("authors");

            entityBuilder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entityBuilder.Property(x => x.Name).HasColumnName("name");
        }
    }
}