﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ZoliksEntities : DbContext
    {
        public ZoliksEntities()
            : base("name=ZoliksEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Zolik> Zoliky { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Url> Urls { get; set; }
        public virtual DbSet<Changelog> Changelogs { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Price> Prices { get; set; }
        public virtual DbSet<Crash> Crashes { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<WebEvent> WebEvents { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserLogin> UserLogins { get; set; }
        public virtual DbSet<Consent> Consents { get; set; }
        public virtual DbSet<Term> Terms { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
        public virtual DbSet<SomeHash> SomeHashes { get; set; }
        public virtual DbSet<Unavailability> Unavailabilities { get; set; }
        public virtual DbSet<Achievement> Achievements { get; set; }
        public virtual DbSet<Rank> Ranks { get; set; }
        public virtual DbSet<Path> Paths { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<UserSetting> UserSettings { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserLoginToken> UserLoginTokens { get; set; }
    
        public virtual ObjectResult<Notification> GetAllNotifications()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Notification>("GetAllNotifications");
        }
    
        public virtual ObjectResult<Notification> GetAllNotifications(MergeOption mergeOption)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Notification>("GetAllNotifications", mergeOption);
        }
    
        private ObjectResult<GetTopStudents_Result> GetTopStudents(Nullable<int> top, Nullable<int> imageMaxSize, Nullable<int> classId, string settingsKey, Nullable<int> defaultPhotoId)
        {
            var topParameter = top.HasValue ?
                new ObjectParameter("top", top) :
                new ObjectParameter("top", typeof(int));
    
            var imageMaxSizeParameter = imageMaxSize.HasValue ?
                new ObjectParameter("imageMaxSize", imageMaxSize) :
                new ObjectParameter("imageMaxSize", typeof(int));
    
            var classIdParameter = classId.HasValue ?
                new ObjectParameter("classId", classId) :
                new ObjectParameter("classId", typeof(int));
    
            var settingsKeyParameter = settingsKey != null ?
                new ObjectParameter("settingsKey", settingsKey) :
                new ObjectParameter("settingsKey", typeof(string));
    
            var defaultPhotoIdParameter = defaultPhotoId.HasValue ?
                new ObjectParameter("defaultPhotoId", defaultPhotoId) :
                new ObjectParameter("defaultPhotoId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetTopStudents_Result>("GetTopStudents", topParameter, imageMaxSizeParameter, classIdParameter, settingsKeyParameter, defaultPhotoIdParameter);
        }
    
        private ObjectResult<GetTopStudents_Result> GetTopStudentsXp(Nullable<int> top, Nullable<int> imageMaxSize, Nullable<int> classId, string settingsKey, Nullable<int> defaultPhotoId)
        {
            var topParameter = top.HasValue ?
                new ObjectParameter("top", top) :
                new ObjectParameter("top", typeof(int));
    
            var imageMaxSizeParameter = imageMaxSize.HasValue ?
                new ObjectParameter("imageMaxSize", imageMaxSize) :
                new ObjectParameter("imageMaxSize", typeof(int));
    
            var classIdParameter = classId.HasValue ?
                new ObjectParameter("classId", classId) :
                new ObjectParameter("classId", typeof(int));
    
            var settingsKeyParameter = settingsKey != null ?
                new ObjectParameter("settingsKey", settingsKey) :
                new ObjectParameter("settingsKey", typeof(string));
    
            var defaultPhotoIdParameter = defaultPhotoId.HasValue ?
                new ObjectParameter("defaultPhotoId", defaultPhotoId) :
                new ObjectParameter("defaultPhotoId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetTopStudents_Result>("GetTopStudentsXp", topParameter, imageMaxSizeParameter, classIdParameter, settingsKeyParameter, defaultPhotoIdParameter);
        }
    
        private ObjectResult<GetTopStudents_Result> GetStudents(Nullable<int> imageMaxSize, Nullable<int> classId, Nullable<int> defaultPhotoId, Nullable<bool> onlyActive)
        {
            var imageMaxSizeParameter = imageMaxSize.HasValue ?
                new ObjectParameter("imageMaxSize", imageMaxSize) :
                new ObjectParameter("imageMaxSize", typeof(int));
    
            var classIdParameter = classId.HasValue ?
                new ObjectParameter("classId", classId) :
                new ObjectParameter("classId", typeof(int));
    
            var defaultPhotoIdParameter = defaultPhotoId.HasValue ?
                new ObjectParameter("defaultPhotoId", defaultPhotoId) :
                new ObjectParameter("defaultPhotoId", typeof(int));
    
            var onlyActiveParameter = onlyActive.HasValue ?
                new ObjectParameter("onlyActive", onlyActive) :
                new ObjectParameter("onlyActive", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetTopStudents_Result>("GetStudents", imageMaxSizeParameter, classIdParameter, defaultPhotoIdParameter, onlyActiveParameter);
        }
    
        private ObjectResult<GetTopStudents_Result> GetFakeStudents(Nullable<int> imageMaxSize, Nullable<int> defaultPhotoId, Nullable<bool> onlyActive)
        {
            var imageMaxSizeParameter = imageMaxSize.HasValue ?
                new ObjectParameter("imageMaxSize", imageMaxSize) :
                new ObjectParameter("imageMaxSize", typeof(int));
    
            var defaultPhotoIdParameter = defaultPhotoId.HasValue ?
                new ObjectParameter("defaultPhotoId", defaultPhotoId) :
                new ObjectParameter("defaultPhotoId", typeof(int));
    
            var onlyActiveParameter = onlyActive.HasValue ?
                new ObjectParameter("onlyActive", onlyActive) :
                new ObjectParameter("onlyActive", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetTopStudents_Result>("GetFakeStudents", imageMaxSizeParameter, defaultPhotoIdParameter, onlyActiveParameter);
        }
    
        private ObjectResult<Unavailability> GetUnavailabilities(Nullable<int> projectId, Nullable<System.DateTime> date)
        {
            var projectIdParameter = projectId.HasValue ?
                new ObjectParameter("projectId", projectId) :
                new ObjectParameter("projectId", typeof(int));
    
            var dateParameter = date.HasValue ?
                new ObjectParameter("date", date) :
                new ObjectParameter("date", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Unavailability>("GetUnavailabilities", projectIdParameter, dateParameter);
        }
    
        private ObjectResult<Unavailability> GetUnavailabilities(Nullable<int> projectId, Nullable<System.DateTime> date, MergeOption mergeOption)
        {
            var projectIdParameter = projectId.HasValue ?
                new ObjectParameter("projectId", projectId) :
                new ObjectParameter("projectId", typeof(int));
    
            var dateParameter = date.HasValue ?
                new ObjectParameter("date", date) :
                new ObjectParameter("date", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Unavailability>("GetUnavailabilities", mergeOption, projectIdParameter, dateParameter);
        }
    }
}