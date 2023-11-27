// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
//

using Microsoft.EntityFrameworkCore;

namespace aspnetapp.Models
{
    public class LecturerContext : DbContext
    {
        public DbSet<DbLecturer> lecturers { get; set; } = null!;
        public DbSet<Lecturer.Tag> tags { get; set; } = null!;

        public LecturerContext(DbContextOptions<LecturerContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new LecturerConfiguration());
        }
    }
}
