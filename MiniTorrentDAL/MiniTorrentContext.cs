using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentDAL
{

    public class MiniTorrentContext : DbContext
    {
        public static readonly string CONNECTION_STRING = "MiniTorrentDBString";
        public DbSet<UserEntity> UserEntiries { get; set; }
        public DbSet<LoggedInUserEntity> LoggedInUserEntities { get; set; }
        public DbSet<FileEntity> FileEntities { get; set; }

        public MiniTorrentContext() : base(nameOrConnectionString: CONNECTION_STRING)
        {
            Database.SetInitializer<MiniTorrentContext>(new CreateDatabaseIfNotExists<MiniTorrentContext>());
        }
    }
}