namespace CollaBotFramework.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class UserKeywords : DbContext
    {
        public UserKeywords()
            : base("name=CollaAzure")
        {
        }

        public virtual DbSet<RepoText> RepoTexts { get; set; }
        public virtual DbSet<User_Keyword_Score> User_Keyword_Score { get; set; }
        public virtual DbSet<User_Keywords> User_Keywords { get; set; }
        public virtual DbSet<UserEmail> UserEmails { get; set; }
        public virtual DbSet<User_Star_Repo> User_Star_Repo { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RepoText>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<RepoText>()
                .Property(e => e.RepoName)
                .IsUnicode(false);

            modelBuilder.Entity<User_Keyword_Score>()
                .Property(e => e.UserLogin)
                .IsUnicode(false);

            modelBuilder.Entity<User_Keyword_Score>()
                .Property(e => e.Keyword)
                .IsUnicode(false);

            modelBuilder.Entity<User_Keywords>()
                .Property(e => e.UserLogin)
                .IsUnicode(false);

            modelBuilder.Entity<User_Keywords>()
                .Property(e => e.KeywordsList)
                .IsUnicode(false);

            modelBuilder.Entity<UserEmail>()
                .Property(e => e.Login)
                .IsUnicode(false);

            modelBuilder.Entity<UserEmail>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<User_Star_Repo>()
                .Property(e => e.UserLogin)
                .IsUnicode(false);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.userID)
                .IsUnicode(false);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.GitHubLogin)
                .IsUnicode(false);

            modelBuilder.Entity<UserInfo>()
                .Property(e => e.Email)
                .IsUnicode(false);
        }
    }
}
