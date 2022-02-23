using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace Timensit_API.Models.Process
{
    public partial class NGUOICOCONGContext : DbContext
    {
        public NGUOICOCONGContext()
        {
        }

        public NGUOICOCONGContext(DbContextOptions<NGUOICOCONGContext> options)
            : base(options)
        {
        }

        public virtual DbSet<QuytrinhCapquanlyduyet> QuytrinhCapquanlyduyet { get; set; }
        public virtual DbSet<QuytrinhDieukien> QuytrinhDieukien { get; set; }
        public virtual DbSet<QuytrinhDieukienCapquanly> QuytrinhDieukienCapquanly { get; set; }
        public virtual DbSet<QuytrinhLichsu> QuytrinhLichsu { get; set; }
        public virtual DbSet<QuytrinhLoaiphieuduyet> QuytrinhLoaiphieuduyet { get; set; }
        public virtual DbSet<QuytrinhLoaiphieuduyetMaildatareplace> QuytrinhLoaiphieuduyetMaildatareplace { get; set; }
        public virtual DbSet<QuytrinhQuatrinhduyet> QuytrinhQuatrinhduyet { get; set; }
        public virtual DbSet<QuytrinhQuatrinhduyetSs> QuytrinhQuatrinhduyetSs { get; set; }
        public virtual DbSet<QuytrinhQuytrinhduyet> QuytrinhQuytrinhduyet { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
            //            if (!optionsBuilder.IsConfigured)
            //            {
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            //                optionsBuilder.UseSqlServer("Data Source=202.143.110.154,1443;Initial Catalog=NGUOICOCONG;User ID=ncc;Password=Dps@123456");
            //            }
            //dotnet ef dbcontext scaffold "Data Source=DESKTOP-CVKT2IU\MSSQLSERVER17;Initial Catalog=NGUOICOCONG_210128;User ID=ncc_local;Password=Ncc@123" Microsoft.EntityFrameworkCore.SqlServer --table quytrinh_dieukien  --table quytrinh_dieukien_capquanly --project Timensit_API --force --output-dir Models/Process
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<QuytrinhCapquanlyduyet>(entity =>
            {
                entity.HasKey(e => e.Rowid)
                    .HasName("pk_quytrinh_capquanlyduyet");

                entity.ToTable("quytrinh_capquanlyduyet");

                entity.Property(e => e.Rowid).HasColumnName("rowid");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnName("createddate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Deletedby).HasColumnName("deletedby");

                entity.Property(e => e.Deleteddate)
                    .HasColumnName("deleteddate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Disable)
                    .HasColumnName("disable")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DuyetSs)
                    .HasColumnName("duyetSS")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Icon)
                    .HasColumnName("icon")
                    .HasMaxLength(50);

                entity.Property(e => e.SoNgayXuLy).HasColumnName("SoNgayXuLy");

                entity.Property(e => e.IdBack).HasColumnName("id_back");

                entity.Property(e => e.IdCapquanly).HasColumnName("id_capquanly");

                entity.Property(e => e.IdChucdanh).HasColumnName("id_chucdanh");

                entity.Property(e => e.IdQuytrinh).HasColumnName("id_quytrinh");

                entity.Property(e => e.Lastmodfied)
                    .HasColumnName("lastmodfied")
                    .HasColumnType("datetime");

                entity.Property(e => e.Maxlevel).HasColumnName("maxlevel");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasMaxLength(500);

                entity.Property(e => e.Notifyto)
                    .HasColumnName("notifyto")
                    .HasMaxLength(500);

                entity.Property(e => e.Priority).HasColumnName("priority");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(200);

                entity.HasOne(d => d.IdQuytrinhNavigation)
                    .WithMany(p => p.QuytrinhCapquanlyduyet)
                    .HasForeignKey(d => d.IdQuytrinh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_quytrinh_capquanlyduyet_quytrinh_quytrinhduyet");
            });

            modelBuilder.Entity<QuytrinhDieukien>(entity =>
            {
                entity.ToTable("quytrinh_dieukien");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnName("createddate")
                    .HasColumnType("datetime");

                entity.Property(e => e.DieuKien).HasMaxLength(500);

                entity.Property(e => e.Disabled).HasColumnName("disabled");

                entity.Property(e => e.IdKey).HasColumnName("id_key");

                entity.Property(e => e.IdQuyTrinh).HasColumnName("Id_QuyTrinh");

                entity.Property(e => e.Lastmodfied)
                    .HasColumnName("lastmodfied")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Operator)
                    .HasColumnName("operator")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<QuytrinhDieukienCapquanly>(entity =>
            {
                entity.ToTable("quytrinh_dieukien_capquanly");

                entity.Property(e => e.IdDieuKien).HasColumnName("Id_DieuKien");

                entity.Property(e => e.IdQuyTrinhCapQuanLy).HasColumnName("Id_QuyTrinh_CapQuanLy");
            });

            modelBuilder.Entity<QuytrinhLichsu>(entity =>
            {
                entity.HasKey(e => e.IdRow)
                    .HasName("pk_quytrinh_lichsu");

                entity.ToTable("quytrinh_lichsu");

                entity.Property(e => e.IdRow).HasColumnName("id_row");

                entity.Property(e => e.Approved).HasColumnName("approved");

                entity.Property(e => e.IdQuatrinh).HasColumnName("id_quatrinh");

                entity.Property(e => e.IdQuatrinhReturn).HasColumnName("id_quatrinh_return");

                entity.Property(e => e.IsFinal).HasColumnName("is_final");

                entity.Property(e => e.NgayTao)
                    .HasColumnName("ngay_tao")
                    .HasColumnType("datetime");

                entity.Property(e => e.NguoiTao).HasColumnName("nguoi_tao");

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasMaxLength(500);

                entity.Property(e => e.FileDinhKem)
                    .HasColumnName("FileDinhKem")
                    .HasMaxLength(500);

                entity.Property(e => e.src)
                    .HasColumnName("src")
                    .HasMaxLength(500);

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasMaxLength(100);

                entity.Property(e => e.Deadline)
                    .HasColumnName("deadline")
                    .HasColumnType("datetime");

                entity.Property(e => e.Checkers).HasColumnName("checkers");

                entity.Property(e => e.NguoiNhan).HasColumnName("nguoi_nhan");

                entity.HasOne(d => d.IdQuatrinhNavigation)
                    .WithMany(p => p.QuytrinhLichsu)
                    .HasForeignKey(d => d.IdQuatrinh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_quytrinh_lichsu_quytrinh_quatrinhduyet");
            });

            modelBuilder.Entity<QuytrinhLoaiphieuduyet>(entity =>
            {
                entity.HasKey(e => e.Rowid)
                    .HasName("pk_quytrinh_loaiphieuduyet");

                entity.ToTable("quytrinh_loaiphieuduyet");

                entity.Property(e => e.Rowid)
                    .HasColumnName("rowid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Acceptmailtitle)
                    .HasColumnName("acceptmailtitle")
                    .HasMaxLength(200);

                entity.Property(e => e.AllowDevChecker)
                    .HasColumnName("allowDevChecker")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.AppaddressList)
                    .HasColumnName("appaddress_list")
                    .HasMaxLength(500);

                entity.Property(e => e.Appnotifyicon)
                    .HasColumnName("appnotifyicon")
                    .HasMaxLength(50);

                entity.Property(e => e.Checkeddatefield)
                    .HasColumnName("checkeddatefield")
                    .HasMaxLength(50);

                entity.Property(e => e.Checkerfield)
                    .HasColumnName("checkerfield")
                    .HasMaxLength(50);

                entity.Property(e => e.Checknotefield)
                    .HasColumnName("checknotefield")
                    .HasMaxLength(50);

                entity.Property(e => e.Completemailtemplate)
                    .HasColumnName("completemailtemplate")
                    .HasMaxLength(200);

                entity.Property(e => e.Dataviewname)
                    .HasColumnName("dataviewname")
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(500);

                entity.Property(e => e.Invalidfield)
                    .HasColumnName("invalidfield")
                    .HasMaxLength(50);

                entity.Property(e => e.Isupdateprocesstext)
                    .HasColumnName("isupdateprocesstext")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.LsGhichu)
                    .HasColumnName("ls_ghichu")
                    .HasMaxLength(50);

                entity.Property(e => e.LsKhoangoai)
                    .HasColumnName("ls_khoangoai")
                    .HasMaxLength(50);

                entity.Property(e => e.LsNgay)
                    .HasColumnName("ls_ngay")
                    .HasMaxLength(50);

                entity.Property(e => e.LsNguoi)
                    .HasColumnName("ls_nguoi")
                    .HasMaxLength(50);

                entity.Property(e => e.LsTable)
                    .HasColumnName("ls_table")
                    .HasMaxLength(50);

                entity.Property(e => e.LsTinhtrang)
                    .HasColumnName("ls_tinhtrang")
                    .HasMaxLength(50);

                entity.Property(e => e.Notifyicon)
                    .HasColumnName("notifyicon")
                    .HasMaxLength(30);

                entity.Property(e => e.Notifylangkey)
                    .HasColumnName("notifylangkey")
                    .HasMaxLength(50);

                entity.Property(e => e.Notifymailtemplate)
                    .HasColumnName("notifymailtemplate")
                    .HasMaxLength(200);

                entity.Property(e => e.Notifymailtitle)
                    .HasColumnName("notifymailtitle")
                    .HasMaxLength(200);

                entity.Property(e => e.Outputmailtemplate)
                    .HasColumnName("outputmailtemplate")
                    .HasMaxLength(200);

                entity.Property(e => e.Outputmailtitle)
                    .HasColumnName("outputmailtitle")
                    .HasMaxLength(200);

                entity.Property(e => e.PageaddressList)
                    .HasColumnName("pageaddress_list")
                    .HasMaxLength(500);

                entity.Property(e => e.Primarykeyfield)
                    .HasColumnName("primarykeyfield")
                    .HasMaxLength(50);

                entity.Property(e => e.Processmethod)
                    .HasColumnName("processmethod")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Processtextfield)
                    .HasColumnName("processtextfield")
                    .HasMaxLength(50);

                entity.Property(e => e.Rejectmailtitle)
                    .HasColumnName("rejectmailtitle")
                    .HasMaxLength(200);

                entity.Property(e => e.Senderfield)
                    .HasColumnName("senderfield")
                    .HasMaxLength(50);

                entity.Property(e => e.Statusfield)
                    .HasColumnName("statusfield")
                    .HasMaxLength(50);

                entity.Property(e => e.Tablename)
                    .HasColumnName("tablename")
                    .HasMaxLength(50);

                entity.Property(e => e.Validfield)
                    .HasColumnName("validfield")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<QuytrinhLoaiphieuduyetMaildatareplace>(entity =>
            {
                entity.HasKey(e => e.Rowid)
                    .HasName("pk_dm_loaiphieuduyet_maildatareplace");

                entity.ToTable("quytrinh_loaiphieuduyet_maildatareplace");

                entity.Property(e => e.Rowid).HasColumnName("rowid");

                entity.Property(e => e.Columnname)
                    .HasColumnName("columnname")
                    .HasMaxLength(200);

                entity.Property(e => e.Custemerid).HasColumnName("custemerid");

                entity.Property(e => e.Format)
                    .HasColumnName("format")
                    .HasMaxLength(50);

                entity.Property(e => e.Loaimail).HasColumnName("loaimail");

                entity.Property(e => e.Loaiphieuduyetid).HasColumnName("loaiphieuduyetid");

                entity.Property(e => e.Replacestring)
                    .HasColumnName("replacestring")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<QuytrinhQuatrinhduyet>(entity =>
            {
                entity.HasKey(e => e.IdRow)
                    .HasName("pk_quytrinh_quatrinhduyet");

                entity.ToTable("quytrinh_quatrinhduyet");

                entity.HasIndex(e => new { e.IdPhieu, e.Loai, e.IdQuytrinhCapquanly })
                    .HasName("unique_quytrinh_quatrinhduyet")
                    .IsUnique();

                entity.Property(e => e.IdRow).HasColumnName("id_row");

                entity.Property(e => e.Checkeddate)
                    .HasColumnName("checkeddate")
                    .HasMaxLength(1);

                entity.Property(e => e.Deadline)
                    .HasColumnName("deadline")
                    .HasMaxLength(1);

                entity.Property(e => e.Checker).HasColumnName("checker");

                entity.Property(e => e.Checkers)
                    .HasColumnName("checkers")
                    .HasMaxLength(500);

                entity.Property(e => e.Checknote)
                    .HasColumnName("checknote")
                    .HasMaxLength(2000);

                entity.Property(e => e.IdPhieu).HasColumnName("id_phieu");

                entity.Property(e => e.IdQuytrinhCapquanly).HasColumnName("id_quytrinh_capquanly");

                entity.Property(e => e.Loai).HasColumnName("loai");

                entity.Property(e => e.Notifyto)
                    .HasColumnName("notifyto")
                    .HasMaxLength(500);

                entity.Property(e => e.Priority).HasColumnName("priority");

                entity.Property(e => e.Ss)
                    .HasColumnName("ss")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Valid).HasColumnName("valid");

                entity.HasOne(d => d.IdQuytrinhCapquanlyNavigation)
                    .WithMany(p => p.QuytrinhQuatrinhduyet)
                    .HasForeignKey(d => d.IdQuytrinhCapquanly)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_quytrinh_quatrinhduyet_dm_quytrinh_capquanlyduyet");

                entity.HasOne(d => d.LoaiNavigation)
                    .WithMany(p => p.QuytrinhQuatrinhduyet)
                    .HasForeignKey(d => d.Loai)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_quytrinh_quatrinhduyet_dm_loaiphieuduyet");
            });

            modelBuilder.Entity<QuytrinhQuatrinhduyetSs>(entity =>
            {
                entity.HasKey(e => e.IdRow)
                    .HasName("pk_quytrinh_quatrinhduyet_ss");

                entity.ToTable("quytrinh_quatrinhduyet_ss");

                entity.Property(e => e.IdRow).HasColumnName("id_row");

                entity.Property(e => e.Checkeddate)
                    .HasColumnName("checkeddate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Deadline)
                    .HasColumnName("deadline")
                    .HasColumnType("datetime");

                entity.Property(e => e.Checker).HasColumnName("checker");

                entity.Property(e => e.Checkers)
                    .HasColumnName("checkers")
                    .HasMaxLength(500);

                entity.Property(e => e.Checknote)
                    .HasColumnName("checknote")
                    .HasMaxLength(2000);

                entity.Property(e => e.IdCt).HasColumnName("id_ct");

                entity.Property(e => e.IdQuatrinh).HasColumnName("id_quatrinh");

                entity.Property(e => e.Passed)
                    .HasColumnName("passed")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Valid).HasColumnName("valid");

                entity.HasOne(d => d.IdQuatrinhNavigation)
                    .WithMany(p => p.QuytrinhQuatrinhduyetSs)
                    .HasForeignKey(d => d.IdQuatrinh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_quytrinh_quatrinhduyet_quytrinh_quatrinhduyet_ss");
            });

            modelBuilder.Entity<QuytrinhQuytrinhduyet>(entity =>
            {
                entity.HasKey(e => e.Rowid)
                    .HasName("pk_quytrinh_quytrinhduyet");

                entity.ToTable("quytrinh_quytrinhduyet");

                entity.Property(e => e.Rowid).HasColumnName("rowid");

                entity.Property(e => e.Checkernotfoundsendto)
                    .HasColumnName("checkernotfoundsendto")
                    .HasMaxLength(200);

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnName("createddate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Custemerid).HasColumnName("custemerid");

                entity.Property(e => e.Deletedby).HasColumnName("deletedby");

                entity.Property(e => e.Deleteddate)
                    .HasColumnName("deleteddate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Disable)
                    .HasColumnName("disable")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IdCapquanly).HasColumnName("id_capquanly");

                entity.Property(e => e.IdChucdanh).HasColumnName("id_chucdanh");

                entity.Property(e => e.IdQuytrinhthaythe).HasColumnName("id_quytrinhthaythe");

                entity.Property(e => e.Isdefault)
                    .HasColumnName("isdefault")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Lastmodified)
                    .HasColumnName("lastmodified")
                    .HasColumnType("datetime");

                entity.Property(e => e.Listccwhenaccept)
                    .HasColumnName("listccwhenaccept")
                    .HasMaxLength(50);

                entity.Property(e => e.Listccwhenreject)
                    .HasColumnName("listccwhenreject")
                    .HasMaxLength(50);

                entity.Property(e => e.Loai).HasColumnName("loai");

                entity.Property(e => e.Macdinh)
                    .HasColumnName("macdinh")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Processmethod)
                    .HasColumnName("processmethod")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Summary)
                    .HasColumnName("summary")
                    .HasMaxLength(200);

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(200);
            });
        }
    }
}
