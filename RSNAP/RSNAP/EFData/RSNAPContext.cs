using System;
using System.Collections.Generic;
using System.Linq;
using GSA.FM.Utility.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly List<String> _unauditedTables = new List<string>()
        {
            "SYS_DATA_AUDIT", "SysDataAudit"
        };

        public RSNAPContext(IConfiguration configuration, ILogger<RSNAPContext> logger,
            IFMUtilityPasswordService utilityPasswordService, IFMUtilityConfigService utilityConfigService,
            IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _logger = logger;
            _utilityPasswordService = utilityPasswordService;
            _utilityConfigService = utilityConfigService;
            _httpContextAccessor = httpContextAccessor;
        }

        public RSNAPContext(DbContextOptions<RSNAPContext> options, IConfiguration configuration, ILogger<RSNAPContext> logger,
            IFMUtilityPasswordService utilityPasswordService, IFMUtilityConfigService utilityConfigService,
            IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _configuration = configuration;
            _logger = logger;
            _utilityPasswordService = utilityPasswordService;
            _utilityConfigService = utilityConfigService;
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual DbSet<PendingroActions> PendingroActions { get; set; }
        public virtual DbSet<PendingroActionsSeq> PendingroActionsSeq { get; set; }
        public virtual DbSet<PendingroApprovalLog> PendingroApprovalLog { get; set; }
        public virtual DbSet<PendingroApprovalLogSeq> PendingroApprovalLogSeq { get; set; }
        public virtual DbSet<PendingroCommentLog> PendingroCommentLog { get; set; }
        public virtual DbSet<PendingroCommentLogSeq> PendingroCommentLogSeq { get; set; }
        public virtual DbSet<Pendingros> Pendingros { get; set; }
        public virtual DbSet<PendingrosSeq> PendingrosSeq { get; set; }
        public virtual DbSet<SysDataAudit> SysDataAudit { get; set; }

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
                            password = _utilityPasswordService.RetrievePassword(databaseName.ToUpper(), currentUsername.ToUpper()).Result; ;
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
                    .HasColumnName("CONTRACTING_APPROVAL_STATUS")
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("'Not Reviewed'")
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
                    .HasColumnName("FUND_APPROVAL_STATUS")
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("'Not Reviewed'")
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
                    .HasColumnName("NOTIFICATION_STATUS")
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("'Not Generated'")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ProId)
                    .IsRequired()
                    .HasColumnName("pro_id")
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");
            });

            modelBuilder.Entity<PendingroActionsSeq>(entity =>
            {
                entity.ToTable("pendingro_actions_seq");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");
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

            modelBuilder.Entity<PendingroApprovalLogSeq>(entity =>
            {
                entity.ToTable("pendingro_approval_log_seq");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");
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

            modelBuilder.Entity<PendingroCommentLogSeq>(entity =>
            {
                entity.ToTable("pendingro_comment_log_seq");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");
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

            modelBuilder.Entity<PendingrosSeq>(entity =>
            {
                entity.ToTable("pendingros_seq");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<SysDataAudit>(entity =>
            {
                entity.ToTable("SYS_DATA_AUDIT");

                entity.Property(e => e.SysDataAuditId)
                    .HasColumnName("SYS_DATA_AUDIT_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ChangingColumn)
                    .IsRequired()
                    .HasColumnName("CHANGING_COLUMN")
                    .HasColumnType("varchar(30)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ChangingTable)
                    .IsRequired()
                    .HasColumnName("CHANGING_TABLE")
                    .HasColumnType("varchar(30)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("CREATED_BY")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("CREATED_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.ExternalUserId)
                    .IsRequired()
                    .HasColumnName("EXTERNAL_USER_ID")
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.NewValue)
                    .HasColumnName("NEW_VALUE")
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.OldValue)
                    .HasColumnName("OLD_VALUE")
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.RecordPk)
                    .IsRequired()
                    .HasColumnName("RECORD_PK")
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.SystemOfRecord)
                    .IsRequired()
                    .HasColumnName("SYSTEM_OF_RECORD")
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");
            });

            modelBuilder.Entity<ApprovalsModel>().HasNoKey();
            modelBuilder.Entity<ExcelDataModel>().HasNoKey();
            modelBuilder.Entity<PagerCount>().HasNoKey();
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public override int SaveChanges()
        {
            var changes = from e in ChangeTracker.Entries()
                          where e.State != EntityState.Unchanged
                          select e;
            var addedEntities = new List<EntityEntry>();
            var changeRecords = new List<SysDataAudit>();
            int objectsCount;

            // Log each change for audit purposes.
            foreach (var change in changes)
            {
                if (change.State == EntityState.Added)
                {
                    // Don't audit this change until after we finish the actual insert. This way any
                    // auto-generated keys will be created.
                    addedEntities.Add(change);
                }
                else
                {
                    changeRecords.AddRange(CreateSysDataAuditForEntity(change));
                }
            }

            // Add all audit change records.
            changeRecords.ForEach(x => SysDataAudit.Add(x));

            // Now save all changes. This will commit any inserts, which we can then audit afterwards.
            objectsCount = base.SaveChanges();

            // Audit any inserts.
            changeRecords.Clear();
            foreach (var insert in addedEntities)
            {
                changeRecords.AddRange(CreateSysDataAuditForEntity(insert, true));
            }
            if (changeRecords.Count > 0)
            {
                changeRecords.ForEach(x => SysDataAudit.Add(x));
                objectsCount += base.SaveChanges();
            }

            return objectsCount;
        }

        private List<SysDataAudit> CreateSysDataAuditForEntity(EntityEntry change, bool insertSpecial = false)
        {
            var entityType = change.Entity.GetType();
            var contextEntityType = Model.FindEntityType(entityType);
            var tableName = contextEntityType.GetTableName();
            var columnNames = contextEntityType.GetProperties().Select(p => p.Name);
            var primaryKeyNames = contextEntityType.FindPrimaryKey().Properties.Select(x => x.Name);

            var username = _httpContextAccessor.HttpContext.User.Identity.Name;

            var changeRecords = new List<SysDataAudit>();

            // Some tables aren't audited.
            if (_unauditedTables.Contains(tableName)) return changeRecords;

            if (change.State == EntityState.Added || insertSpecial)
            {
                var recPkey = "";

                // Primary key is all new values.
                foreach (var primaryKeyName in primaryKeyNames)
                {
                    var value = change.Property(primaryKeyName).CurrentValue;
                    recPkey += (String.IsNullOrEmpty(recPkey) ? "" : ",") + primaryKeyName + "=" + value.ToString();
                }

                // Insert audit record for each column.
                foreach (var columnName in columnNames)
                {
                    var value = change.Property(columnName).CurrentValue;
                    var changeRecord = new SysDataAudit()
                    {
                        ExternalUserId = username,
                        SystemOfRecord = "RSNAP",
                        ChangingTable = tableName,
                        ChangingColumn = columnName,
                        RecordPk = recPkey,
                        NewValue = value?.ToString()
                    };
                    changeRecords.Add(changeRecord);
                }
            }
            else if (change.State == EntityState.Modified)
            {
                var recPkey = "";

                // Primary key is either new or original values.
                foreach (var primaryKeyName in primaryKeyNames)
                {
                    var value = change.Property(primaryKeyName).CurrentValue;
                    if (value == null) value = change.Property(primaryKeyName).OriginalValue;
                    recPkey += "," + primaryKeyName + "=" + value.ToString();
                }

                // Insert audit record for each modified column.
                foreach (var columnName in columnNames)
                {
                    var property = change.Property(columnName);

                    if (!property.IsModified ||
                        (property.CurrentValue == null && property.OriginalValue == null) ||
                        (property.CurrentValue != null && property.OriginalValue != null &&
                        property.CurrentValue.Equals(property.OriginalValue))) continue;

                    var changeRecord = new SysDataAudit()
                    {
                        ExternalUserId = username,
                        SystemOfRecord = "RSNAP",
                        ChangingTable = tableName,
                        ChangingColumn = columnName,
                        RecordPk = recPkey,
                        OldValue = property.OriginalValue?.ToString(),
                        NewValue = property.CurrentValue?.ToString()
                    };
                    changeRecords.Add(changeRecord);
                }
            }
            else if (change.State == EntityState.Deleted)
            {
                var recPkey = "";

                // Primary key is all original values.
                foreach (var primaryKeyName in primaryKeyNames)
                {
                    var value = change.Property(primaryKeyName).OriginalValue;
                    recPkey += "," + primaryKeyName + "=" + value.ToString();
                }

                // Insert audit record for each column.
                foreach (var columnName in columnNames)
                {
                    var value = change.Property(columnName).OriginalValue;
                    var changeRecord = new SysDataAudit()
                    {
                        ExternalUserId = username,
                        SystemOfRecord = "RSNAP",
                        ChangingTable = tableName,
                        ChangingColumn = columnName,
                        RecordPk = recPkey,
                        OldValue = value?.ToString()
                    };
                    changeRecords.Add(changeRecord);
                }
            }

            return changeRecords;
        }
    }
}
