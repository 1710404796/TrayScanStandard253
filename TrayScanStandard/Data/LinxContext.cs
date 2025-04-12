 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinxUniverse.Auth;
using Microsoft.EntityFrameworkCore;
using TrayScanStandard.Data.Models;

namespace TrayScanStandard.Data
{
    public class LinxContext : LinxUserDBContext<LinxUser>
    {
        //private const string connectString = @"Server=(localdb)\MSSQLLocalDB;Database=LXDB_TrayScanStandard;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true;Connect Timeout=500";
        private const string connectString = @"Data Source=LXDB_TrayScanStandardsl.db";


        public LinxContext(DbContextOptions<LinxContext> options) : base(options)
        {

        }

        public LinxContext()
        {

        }
        /// <summary>
        /// 配置数据库连接
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(connectString); // 这句在迁移时启用可消除迁移时的报错
            //optionsBuilder.UseSqlite(connectString); // 这句在迁移时启用可消除迁移时的报错
            base.OnConfiguring(optionsBuilder);

        }
        /// <summary>
        /// 配置数据库模型
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PalletLog>().OwnsMany(
                  poke => poke.BatteryInfo,
                  onNav =>
                  {
                      onNav.ToJson();

                  });


            base.OnModelCreating(modelBuilder);

        }
        /// <summary>
        /// 警告日志
        /// </summary>
        public DbSet<WarningLog> WarningLogs { get; set; }
        /// <summary>
        /// WCS日志
        /// </summary>
        public DbSet<WcsLog> WcsLogs { get; set; }
        /// <summary>
        /// OKNG统计
        /// </summary>
        public DbSet<OKNGCnt> OKNGCnts { get; set; }
        /// <summary>
        /// 托盘日志
        /// </summary>
        public DbSet<PalletLog> PalletLogs { get; set; }

    }
}