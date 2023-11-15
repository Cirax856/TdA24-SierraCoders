using aspnetapp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace aspnetapp
{
    public class LecturerConfiguration : IEntityTypeConfiguration<DbLecturer>
    {
        public void Configure(EntityTypeBuilder<DbLecturer> builder)
        {
            ValueConverter<string[], string> converter = new ValueConverter<string[], string>(
                x => string.Join(";", x),
                x => x.Split(';', StringSplitOptions.RemoveEmptyEntries));

            builder.Property(e => e.emails)
                .HasConversion(converter);
            builder.Property(e => e.telephone_numbers)
                .HasConversion(converter);
        }
    }
}
