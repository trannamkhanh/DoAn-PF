using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WpfApp3.Models;

public partial class LibraryDbContext : DbContext
{
    public LibraryDbContext()
    {
    }

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Loan> Loans { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Books__3DE0C2278DE2F5A5");

            entity.HasIndex(e => e.Author, "IX_Books_Author");

            entity.HasIndex(e => e.Title, "IX_Books_Title");

            entity.HasIndex(e => e.Isbn, "UQ__Books__447D36EA063593CC").IsUnique();

            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.Author).HasMaxLength(100);
            entity.Property(e => e.Genre).HasMaxLength(50);
            entity.Property(e => e.Isbn)
                .HasMaxLength(20)
                .HasColumnName("ISBN");
            entity.Property(e => e.Publisher).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.LoanId).HasName("PK__Loans__4F5AD43785843F56");

            entity.Property(e => e.LoanId).HasColumnName("LoanID");
            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.BorrowDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FineAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Borrowing");

            entity.HasOne(d => d.Book).WithMany(p => p.Loans)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Loans__BookID__440B1D61");

            entity.HasOne(d => d.Member).WithMany(p => p.Loans)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Loans__MemberID__44FF419A");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK__Members__0CF04B388929873C");

            entity.HasIndex(e => e.FullName, "IX_Members_FullName");

            entity.HasIndex(e => e.MemberCode, "UQ__Members__84CA6377446EA523").IsUnique();

            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.JoinDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MemberCode).HasMaxLength(20);
            entity.Property(e => e.Phone).HasMaxLength(15);
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Reports__D5BD48E52A71927E");

            entity.HasIndex(e => e.ReportDate, "IX_Reports_ReportDate");

            entity.HasIndex(e => e.ReportType, "IX_Reports_ReportType");

            entity.HasIndex(e => new { e.ReportType, e.ReportDate }, "UQ_ReportType_Date").IsUnique();

            entity.Property(e => e.ReportId).HasColumnName("ReportID");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.OverdueLoans).HasDefaultValue(0);
            entity.Property(e => e.ReportDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ReportTitle).HasMaxLength(200);
            entity.Property(e => e.ReportType).HasMaxLength(50);
            entity.Property(e => e.TotalBooks).HasDefaultValue(0);
            entity.Property(e => e.TotalFine)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 2)");
            entity.Property(e => e.TotalLoans).HasDefaultValue(0);
            entity.Property(e => e.TotalMembers).HasDefaultValue(0);
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__Staff__96D4AAF755963398");

            entity.HasIndex(e => e.Username, "UQ__Staff__536C85E48C490C79").IsUnique();

            entity.Property(e => e.StaffId).HasColumnName("StaffID");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("Staff");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
