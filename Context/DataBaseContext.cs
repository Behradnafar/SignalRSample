using Microsoft.EntityFrameworkCore;
using SignalRSample.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRSample.Context
{


    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions options):base (options)
        {

        }
        public DbSet<ChatRoom> ChatRooms{ get; set; }
        public DbSet<User> Users{ get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

    }
}
