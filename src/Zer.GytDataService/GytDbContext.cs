﻿using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Zer.Entities;
using Zer.Framework.Entities;

namespace Zer.GytDataService
{
    public class GytDbContext : DbContext
    {
        public GytDbContext()
            :base("GytDbContext")
        {
            //Database.SetInitializer<GytDbContext>(null);
            //Configuration.AutoDetectChangesEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            int result;

            using (var transacton = this.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    result = base.SaveChanges();
                    transacton.Commit();
                }
                catch (Exception ex)
                {
                    transacton.Rollback();
                    result = 0;
                    throw;
                }
            }
            return result;
        }

        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<TruckInfo> TruckInfos { get; set; }
        public DbSet<PeccancyInfo> OverloadInfos { get; set; }
        public DbSet<CompanyInfo> CompanyInfos { get; set; }
        public DbSet<UserLogInfo> Logs { get; set; }
        public DbSet<LngAllowanceInfo> LngAllowanceInfos { get; set; }
        public DbSet<SystemLogInfo> SystemLogInfos { get; set; }
        public DbSet<GYTInfo> GytInfos { get; set; }
    }
}
