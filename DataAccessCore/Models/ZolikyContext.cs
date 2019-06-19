using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataAccessCore.Models
{
	public partial class ZolikyContext : DbContext
	{
		public ZolikyContext() { }

		public ZolikyContext(DbContextOptions<ZolikyContext> options)
			: base(options) { }

		public virtual DbSet<Achievement> Achievements { get; set; }
		public virtual DbSet<AchievementUnlock> AchievementUnlocks { get; set; }
		public virtual DbSet<AnonymTicket> AnonymTickets { get; set; }
		public virtual DbSet<AnonymTicketComment> AnonymTicketComments { get; set; }
		public virtual DbSet<Changelog> Changelogs { get; set; }
		public virtual DbSet<Class> Classes { get; set; }
		public virtual DbSet<Consent> Consents { get; set; }
		public virtual DbSet<Crash> Crashes { get; set; }
		public virtual DbSet<FaQ> FaQs { get; set; }
		public virtual DbSet<FaqCategory> FaqCategories { get; set; }
		public virtual DbSet<Image> Images { get; set; }
		public virtual DbSet<News> News { get; set; }
		public virtual DbSet<Notification> Notifications { get; set; }
		public virtual DbSet<Path> Paths { get; set; }
		public virtual DbSet<Post> Posts { get; set; }
		public virtual DbSet<Price> Prices { get; set; }
		public virtual DbSet<Project> Projects { get; set; }
		public virtual DbSet<Rank> Ranks { get; set; }
		public virtual DbSet<ReadNotification> ReadNotifications { get; set; }
		public virtual DbSet<Role> Roles { get; set; }
		public virtual DbSet<SomeHash> SomeHashes { get; set; }
		public virtual DbSet<Subject> Subjects { get; set; }
		public virtual DbSet<Term> Terms { get; set; }
		public virtual DbSet<Ticket> Tickets { get; set; }
		public virtual DbSet<TicketCategory> TicketCategories { get; set; }
		public virtual DbSet<TicketComment> TicketComments { get; set; }
		public virtual DbSet<Token> Tokens { get; set; }
		public virtual DbSet<Transaction> Transactions { get; set; }
		public virtual DbSet<Unavailability> Unavailabilities { get; set; }
		public virtual DbSet<Url> Urls { get; set; }
		public virtual DbSet<User> Users { get; set; }
		public virtual DbSet<UserLogin> UserLogins { get; set; }
		public virtual DbSet<UserLoginToken> UserLoginTokens { get; set; }
		public virtual DbSet<UserRole> UserRoles { get; set; }
		public virtual DbSet<UserSetting> UserSettings { get; set; }
		public virtual DbSet<WebEvent> WebEvents { get; set; }
		public virtual DbSet<Zolik> Zoliky { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured) { }
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

			modelBuilder.Entity<Achievement>(entity => {
				entity.Property(e => e.Enabled).HasDefaultValueSql("((1))");

				entity.HasOne(d => d.ImageLocked)
					  .WithMany(p => p.AchievementImageLockeds)
					  .HasForeignKey(d => d.ImageLockedID)
					  .HasConstraintName("FK_Achievements_LockedImage");

				entity.HasOne(d => d.UnlockedImage)
					  .WithMany(p => p.AchievementUnlockedImages)
					  .HasForeignKey(d => d.UnlockedImageID)
					  .HasConstraintName("FK_Achievements_UnlockedImage");
			});

			modelBuilder.Entity<AchievementUnlock>(entity => {
				entity.HasKey(e => new {e.UserID, e.AchievementID});

				entity.HasOne(d => d.Achievement)
					  .WithMany(p => p.AchievementUnlocks)
					  .HasForeignKey(d => d.AchievementID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_AchievementUnlocks_Achievements");

				entity.HasOne(d => d.User)
					  .WithMany(p => p.AchievementUnlocks)
					  .HasForeignKey(d => d.UserID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_AchievementUnlocks_Users");
			});

			modelBuilder.Entity<AnonymTicket>(entity => {
				entity.Property(e => e.Email).IsUnicode(false);

				entity.Property(e => e.Ip).IsUnicode(false);

				entity.HasOne(d => d.Admin)
					  .WithMany(p => p.AnonymTickets)
					  .HasForeignKey(d => d.AdminID)
					  .HasConstraintName("FK_AnonymTickets_Users");

				entity.HasOne(d => d.Category)
					  .WithMany(p => p.AnonymTickets)
					  .HasForeignKey(d => d.CategoryID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_AnonymTickets_TicketCategories");
			});

			modelBuilder.Entity<AnonymTicketComment>(entity => {
				entity.Property(e => e.Ip).IsUnicode(false);

				entity.HasOne(d => d.Ticket)
					  .WithMany(p => p.AnonymTicketComments)
					  .HasForeignKey(d => d.TicketID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_AnonymTicketComments_AnonymTicketComments");

				entity.HasOne(d => d.User)
					  .WithMany(p => p.AnonymTicketComments)
					  .HasForeignKey(d => d.UserID)
					  .HasConstraintName("FK_AnonymTicketComments_Admin");
			});

			modelBuilder.Entity<Changelog>(entity => {
				entity.Property(e => e.Version).IsUnicode(false);

				entity.HasOne(d => d.Project)
					  .WithMany(p => p.Changelogs)
					  .HasForeignKey(d => d.ProjectID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Changelogs_Projects");
			});

			modelBuilder.Entity<Class>(entity => {
				entity.Property(e => e.Enabled).HasDefaultValueSql("((1))");

				entity.Property(e => e.Name).IsUnicode(false);
			});

			modelBuilder.Entity<Consent>(entity => {
				entity.HasKey(e => new {e.UserID, e.TermID});

				entity.HasOne(d => d.Term)
					  .WithMany(p => p.Consents)
					  .HasForeignKey(d => d.TermID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Consents_Terms");

				entity.HasOne(d => d.User)
					  .WithMany(p => p.Consents)
					  .HasForeignKey(d => d.UserID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Consents_Users");
			});

			modelBuilder.Entity<Crash>(entity => {
				entity.Property(e => e.AppVersion).IsUnicode(false);

				entity.Property(e => e.Email).IsUnicode(false);

				entity.Property(e => e.Enabled).HasDefaultValueSql("((1))");

				entity.HasOne(d => d.Project)
					  .WithMany(p => p.Crashes)
					  .HasForeignKey(d => d.ProjectID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Crashes_Projects");

				entity.HasOne(d => d.Screenshot)
					  .WithMany(p => p.Crashes)
					  .HasForeignKey(d => d.ScreenshotID)
					  .HasConstraintName("FK_Crashes_Images");

				entity.HasOne(d => d.User)
					  .WithMany(p => p.Crashes)
					  .HasForeignKey(d => d.UserID)
					  .HasConstraintName("FK_Crashes_Users");
			});

			modelBuilder.Entity<FaQ>(entity => {
				entity.HasOne(d => d.Category)
					  .WithMany(p => p.FaQS)
					  .HasForeignKey(d => d.CategoryID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_FaQ_FaqCategories");
			});

			modelBuilder.Entity<FaqCategory>(entity => {
				entity.HasIndex(e => e.Name)
					  .HasName("IX_Names")
					  .IsUnique();

				entity.HasIndex(e => e.Type)
					  .HasName("IX_Types")
					  .IsUnique();

				entity.Property(e => e.Type).IsUnicode(false);
			});

			modelBuilder.Entity<Image>(entity => {
				entity.Property(e => e.Base64).IsUnicode(false);

				entity.Property(e => e.Mime).IsUnicode(false);

				entity.Property(e => e.Name).IsUnicode(false);

				entity.Property(e => e.Size).HasDefaultValueSql("((1))");

				entity.HasOne(d => d.Owner)
					  .WithMany(p => p.Images)
					  .HasForeignKey(d => d.OwnerID)
					  .HasConstraintName("FK_Images_Owner");
			});

			modelBuilder.Entity<News>(entity => {
				entity.Property(e => e.Title).IsUnicode(false);

				entity.HasOne(d => d.Project)
					  .WithMany(p => p.News)
					  .HasForeignKey(d => d.ProjectID)
					  .HasConstraintName("FK_News_Projects");
			});

			modelBuilder.Entity<Notification>(entity => {
				entity.HasOne(d => d.From)
					  .WithMany(p => p.NotificationFroms)
					  .HasForeignKey(d => d.FromID)
					  .HasConstraintName("FK_Notifications_FromUser");

				entity.HasOne(d => d.Project)
					  .WithMany(p => p.Notifications)
					  .HasForeignKey(d => d.ProjectID)
					  .HasConstraintName("FK_Notifications_Projects");

				entity.HasOne(d => d.To)
					  .WithMany(p => p.NotificationTos)
					  .HasForeignKey(d => d.ToID)
					  .HasConstraintName("FK_Notifications_ToUser");
			});

			modelBuilder.Entity<Post>(entity => {
				entity.HasOne(d => d.Author)
					  .WithMany(p => p.Posts)
					  .HasForeignKey(d => d.AuthorID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_User_Posts");
			});

			modelBuilder.Entity<Project>(entity => {
				entity.Property(e => e.Active).HasDefaultValueSql("((1))");

				entity.Property(e => e.Version)
					  .IsUnicode(false)
					  .HasDefaultValueSql("('1.0.0')");
			});

			modelBuilder.Entity<ReadNotification>(entity => {
				entity.HasKey(e => new {e.UserID, e.NotificationID});

				entity.HasOne(d => d.Notification)
					  .WithMany(p => p.ReadNotifications)
					  .HasForeignKey(d => d.NotificationID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_ReadNotifications_Notifications");

				entity.HasOne(d => d.User)
					  .WithMany(p => p.ReadNotifications)
					  .HasForeignKey(d => d.UserID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_ReadNotifications_Users");
			});

			modelBuilder.Entity<Role>(entity => {
				entity.HasIndex(e => e.Name)
					  .HasName("UK_Roles")
					  .IsUnique();
			});

			modelBuilder.Entity<SomeHash>(entity => {
				entity.Property(e => e.Hash).IsUnicode(false);

				entity.Property(e => e.Version)
					  .IsUnicode(false)
					  .HasDefaultValueSql("('1.0.2')");

				entity.HasOne(d => d.Owner)
					  .WithMany(p => p.SomeHashes)
					  .HasForeignKey(d => d.OwnerID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_SomeHashes_Users");
			});

			modelBuilder.Entity<Subject>(entity => {
				entity.HasIndex(e => e.Shortcut)
					  .HasName("IX_Subject_Shortcut")
					  .IsUnique();

				entity.Property(e => e.Shortcut).IsUnicode(false);
			});

			modelBuilder.Entity<Term>(entity => {
				entity.HasIndex(e => e.Shortcut)
					  .HasName("IX_Terms")
					  .IsUnique();

				entity.Property(e => e.Shortcut).IsUnicode(false);
			});

			modelBuilder.Entity<Ticket>(entity => {
				entity.HasOne(d => d.Admin)
					  .WithMany(p => p.TicketAdmins)
					  .HasForeignKey(d => d.AdminID)
					  .HasConstraintName("FK_Tickets_Admin");

				entity.HasOne(d => d.Category)
					  .WithMany(p => p.Tickets)
					  .HasForeignKey(d => d.CategoryID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Tickets_TicketCategories");

				entity.HasOne(d => d.User)
					  .WithMany(p => p.TicketUsers)
					  .HasForeignKey(d => d.UserID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Tickets_Users");
			});

			modelBuilder.Entity<TicketComment>(entity => {
				entity.HasOne(d => d.Ticket)
					  .WithMany(p => p.TicketComments)
					  .HasForeignKey(d => d.TicketID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_TicketComments_Ticket");

				entity.HasOne(d => d.User)
					  .WithMany(p => p.TicketComments)
					  .HasForeignKey(d => d.UserID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_TicketComments_Users");
			});

			modelBuilder.Entity<Token>(entity => {
				entity.HasIndex(e => e.Code)
					  .HasName("UK_Guids")
					  .IsUnique();

				entity.HasOne(d => d.User)
					  .WithMany(p => p.Tokens)
					  .HasForeignKey(d => d.UserID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Tokens_Users");
			});

			modelBuilder.Entity<Transaction>(entity => {
				entity.HasOne(d => d.From)
					  .WithMany(p => p.TransactionFroms)
					  .HasForeignKey(d => d.FromID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Transactions_Users");

				entity.HasOne(d => d.To)
					  .WithMany(p => p.TransactionTos)
					  .HasForeignKey(d => d.ToID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Transactions_Users1");

				entity.HasOne(d => d.Zolik)
					  .WithMany(p => p.Transactions)
					  .HasForeignKey(d => d.ZolikID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Transactions_Zoliky");
			});

			modelBuilder.Entity<Unavailability>(entity => {
				entity.HasOne(d => d.Project)
					  .WithMany(p => p.Unavailabilities)
					  .HasForeignKey(d => d.ProjectID)
					  .HasConstraintName("FK_Unavailabilities_Projects");
			});

			modelBuilder.Entity<Url>(entity => {
				entity.HasIndex(e => e.Name)
					  .HasName("IX_Urls")
					  .IsUnique();

				entity.Property(e => e.Enabled).HasDefaultValueSql("((1))");

				entity.Property(e => e.Name).IsUnicode(false);

				entity.Property(e => e.ProjectID).HasDefaultValueSql("((1))");

				entity.HasOne(d => d.Path)
					  .WithMany(p => p.Urls)
					  .HasForeignKey(d => d.PathID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Urls_Paths");

				entity.HasOne(d => d.Project)
					  .WithMany(p => p.Urls)
					  .HasForeignKey(d => d.ProjectID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Urls_Projects");
			});

			modelBuilder.Entity<User>(entity => {
				entity.HasIndex(e => e.Email)
					  .IsUnique();

				entity.HasIndex(e => e.Uqid)
					  .IsUnique();

				entity.HasIndex(e => e.Username)
					  .HasName("IX_Users_UName")
					  .IsUnique();

				entity.Property(e => e.Email).IsUnicode(false);

				entity.Property(e => e.Lastname).IsUnicode(false);

				entity.Property(e => e.Name).IsUnicode(false);

				entity.Property(e => e.ProfilePhotoID).HasDefaultValueSql("((4))");

				entity.Property(e => e.RegistrationIp)
					  .IsUnicode(false)
					  .HasDefaultValueSql("('127.0.0.1')");

				entity.Property(e => e.Username).IsUnicode(false);

				entity.Property(e => e.Version)
					  .IsUnicode(false)
					  .HasDefaultValueSql("('1.0.0')");

				entity.HasOne(d => d.Class)
					  .WithMany(p => p.Users)
					  .HasForeignKey(d => d.ClassID)
					  .HasConstraintName("FK_Users_Classes");

				entity.HasOne(d => d.Password)
					  .WithMany(p => p.Users)
					  .HasForeignKey(d => d.PasswordID)
					  .HasConstraintName("FK_Users_SomeHashes");

				entity.HasOne(d => d.ProfilePhoto)
					  .WithMany(p => p.Users)
					  .HasForeignKey(d => d.ProfilePhotoID)
					  .HasConstraintName("FK_Users_Images");
			});

			modelBuilder.Entity<UserLogin>(entity => {
				entity.Property(e => e.Ip).IsUnicode(false);

				entity.HasOne(d => d.Project)
					  .WithMany(p => p.UserLogins)
					  .HasForeignKey(d => d.ProjectID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Logins_Projects");

				entity.HasOne(d => d.User)
					  .WithMany(p => p.UserLogins)
					  .HasForeignKey(d => d.UserID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Logins_Users");
			});

			modelBuilder.Entity<UserLoginToken>(entity => {
				entity.HasKey(e => new {e.LoginProvider, e.ProviderKey});

				entity.HasIndex(e => e.UserID)
					  .HasName("IX_UserId");

				entity.HasOne(d => d.User)
					  .WithMany(p => p.UserLoginTokens)
					  .HasForeignKey(d => d.UserID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_UserLoginTokens_Users");
			});

			modelBuilder.Entity<UserRole>(entity => {
				entity.HasKey(e => new {e.UserID, e.RoleID});

				entity.HasOne(d => d.Role)
					  .WithMany(p => p.UserRoles)
					  .HasForeignKey(d => d.RoleID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_UserRoles_Roles");

				entity.HasOne(d => d.User)
					  .WithMany(p => p.UserRoles)
					  .HasForeignKey(d => d.UserID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_UserRoles_Users");
			});

			modelBuilder.Entity<UserSetting>(entity => {
				entity.HasKey(e => new {e.ID, e.Key});

				entity.HasIndex(e => new {e.ID, e.ProjectID, e.Key})
					  .HasName("UK_UserSettings")
					  .IsUnique();

				entity.Property(e => e.Key).IsUnicode(false);

				entity.HasOne(d => d.IdNavigation)
					  .WithMany(p => p.UserSettings)
					  .HasForeignKey(d => d.ID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_UserSettings_Users");

				entity.HasOne(d => d.Project)
					  .WithMany(p => p.UserSettings)
					  .HasForeignKey(d => d.ProjectID)
					  .HasConstraintName("FK_UserSettings_Projects");
			});

			modelBuilder.Entity<WebEvent>(entity => {
				entity.HasOne(d => d.From)
					  .WithMany(p => p.WebEventFroms)
					  .HasForeignKey(d => d.FromID)
					  .HasConstraintName("FK_Events_FromUser");

				entity.HasOne(d => d.FromProject)
					  .WithMany(p => p.WebEvents)
					  .HasForeignKey(d => d.FromProjectID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Events_Projects");

				entity.HasOne(d => d.To)
					  .WithMany(p => p.WebEventTos)
					  .HasForeignKey(d => d.ToID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Events_ToUser");
			});

			modelBuilder.Entity<Zolik>(entity => {
				entity.Property(e => e.Type).HasConversion<byte>();

				entity.Property(e => e.SubjectID).HasDefaultValueSql("((1))");

				entity.Property(e => e.TeacherID).HasDefaultValueSql("((1))");

				entity.HasOne(d => d.OriginalOwner)
					  .WithMany(p => p.ZolikyOriginalOwners)
					  .HasForeignKey(d => d.OriginalOwnerID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Zoliky_OriginalOwner");

				entity.HasOne(d => d.Owner)
					  .WithMany(p => p.ZolikyOwners)
					  .HasForeignKey(d => d.OwnerID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Zoliky_Owner");

				entity.HasOne(d => d.Subject)
					  .WithMany(p => p.Zolikies)
					  .HasForeignKey(d => d.SubjectID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Zoliky_Subject");

				entity.HasOne(d => d.Teacher)
					  .WithMany(p => p.ZolikyTeachers)
					  .HasForeignKey(d => d.TeacherID)
					  .OnDelete(DeleteBehavior.ClientSetNull)
					  .HasConstraintName("FK_Zoliky_Teacher");
			});

			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
	}
}