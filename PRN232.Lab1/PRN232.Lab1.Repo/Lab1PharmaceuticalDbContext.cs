using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace PRN232.Lab1.Repo;

public partial class Lab1PharmaceuticalDbContext : DbContext
{
    public Lab1PharmaceuticalDbContext(DbContextOptions<Lab1PharmaceuticalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<MedicineInformation> MedicineInformations { get; set; }

    public virtual DbSet<StoreAccount> StoreAccounts { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        =>
    //        optionsBuilder.UseSqlServer(
    //            "Server=localhost,14333;Uid=sa;Pwd=123456As;Database=Lab1PharmaceuticalDB;TrustServerCertificate=True"
    //        );

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Manufact__357E5CA190862DE7");

            entity.ToTable("Manufacturer");

            entity.Property(e => e.Id)
                .HasMaxLength(20)
                .HasColumnName("ManufacturerID");
            entity.Property(e => e.ContactInformation).HasMaxLength(120);
            entity.Property(e => e.CountryofOrigin).HasMaxLength(250);
            entity.Property(e => e.ManufacturerName).HasMaxLength(100);
            entity.Property(e => e.ShortDescription).HasMaxLength(400);
        });

        modelBuilder.Entity<MedicineInformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Medicine__4F2128F03F63CFDF");

            entity.ToTable("MedicineInformation");

            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .HasColumnName("MedicineID");
            entity.Property(e => e.ActiveIngredients).HasMaxLength(200);
            entity.Property(e => e.DosageForm).HasMaxLength(400);
            entity.Property(e => e.ExpirationDate).HasMaxLength(120);
            entity.Property(e => e.ManufacturerId)
                .HasMaxLength(20)
                .HasColumnName("ManufacturerID");
            entity.Property(e => e.MedicineName).HasMaxLength(160);
            entity.Property(e => e.WarningsAndPrecautions).HasMaxLength(400);

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.MedicineInformations)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__MedicineI__Manuf__3C69FB99");
        });

        modelBuilder.Entity<StoreAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StoreAcc__42D52A6ADA8026DC");

            entity.ToTable("StoreAccount");

            entity.HasIndex(e => e.EmailAddress, "UQ__StoreAcc__49A147400E49C1AF").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("StoreAccountID");
            entity.Property(e => e.EmailAddress).HasMaxLength(90);
            entity.Property(e => e.StoreAccountDescription).HasMaxLength(140);
            entity.Property(e => e.StoreAccountPassword).HasMaxLength(90);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
