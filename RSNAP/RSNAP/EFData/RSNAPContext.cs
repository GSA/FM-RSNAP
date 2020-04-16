using System;
using GSA.FM.Utility.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RSNAP.EFModels;
using RSNAP.Models;

namespace RSNAP.EFData
{
    public partial class RSNAPContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RSNAPContext> _logger;
        private readonly IFMUtilityPasswordService _utilityPasswordService;
        private readonly IFMUtilityConfigService _utilityConfigService;

        public RSNAPContext(IConfiguration configuration, ILogger<RSNAPContext> logger,
            IFMUtilityPasswordService utilityPasswordService, IFMUtilityConfigService utilityConfigService)
        {
            _configuration = configuration;
            _logger = logger;
            _utilityPasswordService = utilityPasswordService;
            _utilityConfigService = utilityConfigService;
        }

        public RSNAPContext(DbContextOptions<RSNAPContext> options, IConfiguration configuration, ILogger<RSNAPContext> logger,
            IFMUtilityPasswordService utilityPasswordService, IFMUtilityConfigService utilityConfigService)
            : base(options)
        {
            _configuration = configuration;
            _logger = logger;
            _utilityPasswordService = utilityPasswordService;
            _utilityConfigService = utilityConfigService;
        }

        public virtual DbSet<PendingroActions> PendingroActions { get; set; }
        public virtual DbSet<PendingroApprovalLog> PendingroApprovalLog { get; set; }
        public virtual DbSet<PendingroCommentLog> PendingroCommentLog { get; set; }
        public virtual DbSet<Pendingros> Pendingros { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var success = true;
                var connectionString = "";

                var databasesEntry = _configuration.GetSection("Databases");
                if (databasesEntry.Exists())
                {
                    // Confirm database name.
                    var databaseEntry = databasesEntry.GetSection("RSNAP");
                    if (databaseEntry.Exists())
                    {
                        // Get database name.
                        var databaseName = databaseEntry.GetValue<string>("Database");
                        if (string.IsNullOrEmpty(databaseName)) success = false;

                        // Get username.
                        var username = databaseEntry.GetValue<string>("Username");
                        if (string.IsNullOrEmpty(username)) success = false;

                        // Get the connection string from the config service.
                        if (success)
                        {
                            connectionString = _utilityConfigService.GetConnectionString(databaseName);
                            if (string.IsNullOrEmpty(connectionString)) success = false;
                        }

                        // Get the current username from the config service.
                        var currentUsername = "";
                        if (success)
                        {
                            currentUsername = _utilityConfigService.GetDatabaseUsername(databaseName, username);
                            if (string.IsNullOrEmpty(currentUsername)) success = false;
                        }

                        // Resolve the password for this database/username. Cast to upper.
                        var password = "";
                        if (success)
                        {
                            password = _utilityPasswordService.RetrievePassword(databaseName.ToUpper(), currentUsername.ToUpper()).Result;
                            if (string.IsNullOrEmpty(password)) success = false;
                        }

                        // Format the final connection string.
                        if (success)
                        {
                            connectionString = _utilityConfigService.FormatMySQLConnectionStringWithSSL(connectionString, currentUsername, password,
                                "certificates", "RSNAP");
                            if (connectionString == "") success = false;
                        }
                    }
                }

                if (success)
                {
                    optionsBuilder.UseMySql(connectionString);
                }
                else
                {
                    _logger.LogError("Unable to retrieve connection string for RSNAP.");
                }
            }
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PendingroActions>(entity =>
            {
                entity.HasKey(e => e.ProActId)
                    .HasName("PRIMARY");

                entity.ToTable("pendingro_actions");

                entity.Property(e => e.ProActId)
                    .HasColumnName("pro_act_id")
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ContractingApprovalStatus)
                    .HasColumnName("contracting_approval_status")
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ContractingApprover)
                    .HasColumnName("contracting_approver")
                    .HasColumnType("varchar(120)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ContractingApproverNote)
                    .HasColumnName("contracting_approver_note")
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.FundApprovalStatus)
                    .HasColumnName("fund_approval_status")
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.FundApprover)
                    .HasColumnName("fund_approver")
                    .HasColumnType("varchar(120)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.FundApproverNote)
                    .HasColumnName("fund_approver_note")
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.NotificationDate)
                    .HasColumnName("notification_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.NotificationStatus)
                    .HasColumnName("notification_status")
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ProId)
                    .IsRequired()
                    .HasColumnName("pro_id")
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");
            });

            modelBuilder.Entity<PendingroApprovalLog>(entity =>
            {
                entity.HasKey(e => e.ProApprId)
                    .HasName("PRIMARY");

                entity.ToTable("pendingro_approval_log");

                entity.Property(e => e.ProApprId)
                    .HasColumnName("pro_appr_id")
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ActionDate)
                    .HasColumnName("action_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.ActionTaken)
                    .HasColumnName("action_taken")
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ContractingApprover)
                    .HasColumnName("contracting_approver")
                    .HasColumnType("varchar(120)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.FundApprover)
                    .HasColumnName("fund_approver")
                    .HasColumnType("varchar(120)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ProId)
                    .IsRequired()
                    .HasColumnName("pro_id")
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");
            });

            modelBuilder.Entity<PendingroCommentLog>(entity =>
            {
                entity.HasKey(e => e.ProCommentId)
                    .HasName("PRIMARY");

                entity.ToTable("pendingro_comment_log");

                entity.Property(e => e.ProCommentId)
                    .HasColumnName("pro_comment_id")
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.CommentDate)
                    .HasColumnName("comment_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.ProComment)
                    .HasColumnName("pro_comment")
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ProId)
                    .IsRequired()
                    .HasColumnName("pro_id")
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("varchar(120)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");
            });

            modelBuilder.Entity<Pendingros>(entity =>
            {
                entity.HasKey(e => e.ProId)
                    .HasName("PRIMARY");

                entity.ToTable("pendingros");

                entity.Property(e => e.ProId)
                    .HasColumnName("pro_id")
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.AddrNm)
                    .HasColumnName("addr_nm")
                    .HasColumnType("varchar(120)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.CoEmail)
                    .HasColumnName("co_email")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ContractEfctEndt)
                    .HasColumnName("contract_efct_endt")
                    .HasColumnType("datetime");

                entity.Property(e => e.ContractEfctStdt)
                    .HasColumnName("contract_efct_stdt")
                    .HasColumnType("datetime");

                entity.Property(e => e.CtrcNum)
                    .HasColumnName("ctrc_num")
                    .HasColumnType("varchar(30)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.EtlUpdateFl)
                    .HasColumnName("etl_update_fl")
                    .HasColumnType("varchar(1)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.Idv)
                    .HasColumnName("idv")
                    .HasColumnType("varchar(61)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.LastGnrdDt)
                    .HasColumnName("last_gnrd_dt")
                    .HasColumnType("datetime");

                entity.Property(e => e.MaterialChangeDate)
                    .HasColumnName("material_change_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.MfIoFrmUidy)
                    .HasColumnName("mf_io_frm_uidy")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.NextExpdDt)
                    .HasColumnName("next_expd_dt")
                    .HasColumnType("datetime");

                entity.Property(e => e.OrddAm)
                    .HasColumnName("ordd_am")
                    .HasColumnType("decimal(20,2)");

                entity.Property(e => e.Pdocno)
                    .IsRequired()
                    .HasColumnName("pdocno")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.Piid)
                    .HasColumnName("piid")
                    .HasColumnType("varchar(30)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.PopEndt)
                    .HasColumnName("pop_endt")
                    .HasColumnType("datetime");

                entity.Property(e => e.PopStdt)
                    .HasColumnName("pop_stdt")
                    .HasColumnType("datetime");

                entity.Property(e => e.Region)
                    .HasColumnName("region")
                    .HasColumnType("varchar(2)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.VendorEmail)
                    .HasColumnName("vendor_email")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");
            });
            
            modelBuilder.Entity<ApprovalsModel>().HasNoKey(); 
            modelBuilder.Entity<PagerCount>().HasNoKey();
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
