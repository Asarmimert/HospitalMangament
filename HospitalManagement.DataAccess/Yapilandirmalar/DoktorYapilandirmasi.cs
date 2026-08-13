using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.DataAccess.Yapilandirmalar
{
    public class DoktorYapilandirmasi
        : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.ToTable("tbl_Doktorlar");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DoktorAd)
                .HasColumnName("Ad")
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(x => x.DoktorSoyad)
                .HasColumnName("Soyad")
                .IsRequired()
                .HasMaxLength(40);

            builder.Property(x => x.TelefonNumarasi)
                .HasMaxLength(11);

            builder.Property(x => x.UzmanlikAlani)
                .HasMaxLength(35);

            builder.Property(x => x.AktifMi)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.OlusturulmaTarihi)
                .IsRequired();

            builder.HasIndex(x => x.KullaniciHesabiId)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.DepartmentId,
                x.AktifMi
            });

            builder.HasOne(x => x.KullaniciHesabi)
               .WithOne()
               .HasForeignKey<Doctor>(x => x.KullaniciHesabiId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Department)
                .WithMany(x => x.Doctors)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.DepartmentId)
            .HasColumnName("DepartmanId");



            builder.Property(x => x.GuncellenmeTarihi)
            .IsRequired(false);

        }
    }
}