using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        //---使用两个上下文 一个用于api 一个用于系统服务
        //修改 只做api部分 
        public DcDbContext(DbContextOptions options) : base(options)
        {
            Database.Migrate();
            Database.EnsureCreated();
        }
        //public DcDbContext() { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    //读取json文件
        //    var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        //    var DbString = config.GetSection("ConnectionStrings:DBString").Value;
        //    optionsBuilder.UseMySQL(DbString);
        //}
        public DbSet<TempAndHumid> tempAndHumids { get; set; }
        public DbSet<ProcessInfo> processInfos { get; set; }

    }

}
