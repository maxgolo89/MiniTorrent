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
        public DbSet<User> Users { get; set; }
        public DbSet<LoggedInUser> LoggedInUsers { get; set; }
        public DbSet<FileInformation> FilesInformation { get; set; }

        public MiniTorrentContext() : base(nameOrConnectionString: "MiniTorrentDBString")
        {
            Database.SetInitializer<MiniTorrentContext>(new CreateDatabaseIfNotExists<MiniTorrentContext>());
        }
    }
}