using Microsoft.EntityFrameworkCore;
using Misc.DC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misc.DC.Storage
{
    public class DcDbContext : DbContext
    {
        //使用两个上下文 一个用于api 一个用于系统服务
        public DcDbContext(DbContextOptions options) : base(options)
        {
            Database.Migrate();
            Database.EnsureCreated();
        }
        public DcDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("Server=localhost;Database=OASystem2;User Id=root;password=123456;port=3306;Charset=utf8;SslMode=none;");
            Database.Migrate();
            Database.EnsureCreated();
        }
        public DbSet<TempAndHumid> tempAndHumids { get; set; }

    }

}
