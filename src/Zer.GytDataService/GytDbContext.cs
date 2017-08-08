﻿using System.Data.Entity;
using Zer.Entities;

namespace Zer.GytDataService
{
    public class GytDbContext : DbContext
    {
        public GytDbContext()
            :base("GytDbContext")
        {
            
        }
        
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<TruckInfo> TruckInfos { get; set; }
        public DbSet<OverloadInfo> OverloadInfos { get; set; }
        public DbSet<CompanyInfo> CompanyInfos { get; set; }
        public DbSet<UserLogInfo> Logs { get; set; }
        public DbSet<LngAllowanceInfo> LngAllowanceInfos { get; set; }
    }
}
