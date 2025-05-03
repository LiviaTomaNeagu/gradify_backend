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
        public DbSet<Student_Coordinator> Student_Coordinators { get; set; }
        public DbSet<StudentDetails> StudentDetails { get; set; }
        public DbSet<UserTopics> UsersTopics { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Group> Groups { get; set; }


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

            modelBuilder.Entity<Student_Coordinator>()
                .ToTable("student_coordinator")
                .HasKey(sc => new { sc.StudentId, sc.CoordinatorId });

            modelBuilder.Entity<Student_Coordinator>()
                .HasOne(sc => sc.Student)
                .WithMany(u => u.Students)
                .HasForeignKey(sc => sc.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Student_Coordinator>()
                .HasOne(sc => sc.Coordinator)
                .WithMany(u => u.Coordinators)
                .HasForeignKey(sc => sc.CoordinatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentDetails>().ToTable("student_details")
                .HasOne(sd => sd.User)
                .WithOne(u => u.StudentDetails)
                .HasForeignKey<StudentDetails>(sd => sd.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentDetails>()
                .HasOne(sd => sd.Group)
                .WithMany()
                .HasForeignKey(sd => sd.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserTopics>()
                .ToTable("user_topics")
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTopics)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChatMessage>()
                .ToTable("chat_messages");

            modelBuilder.Entity<Note>().ToTable("notes")
                .HasOne(n => n.Student)
                .WithMany()
                .HasForeignKey(n => n.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Event>()
                .ToTable("events");

            modelBuilder.Entity<Group>()
                .ToTable("groups");
        }

    }
}
