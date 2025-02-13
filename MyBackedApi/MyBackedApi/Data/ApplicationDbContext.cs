// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using MyBackedApi.Models;

namespace MyBackedApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Occupation> Occupations { get; set; }
        public DbSet<UserAuthToken> UserAuthTokens { get; set; }
        public DbSet<ActivationCode> ActivationCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("users")
                .HasOne(u => u.Occupation)
                .WithMany()
                .HasForeignKey(u => u.OccupationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Question>().ToTable("questions")
                .HasOne(q => q.User)
                .WithMany(u => u.Questions)
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>().ToTable("answers")
               .HasOne<Question>()
               .WithMany(q => q.Answers)
               .HasForeignKey(a => a.QuestionId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Occupation>().ToTable("occupations");

            modelBuilder.Entity<UserAuthToken>().ToTable("user_auth_tokens");

            modelBuilder.Entity<ActivationCode>().ToTable("activation_codes");
        }

    }
}
