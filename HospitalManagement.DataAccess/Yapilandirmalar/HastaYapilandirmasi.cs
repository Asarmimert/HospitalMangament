using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.DataAccess.Yapilandirmalar
{
    public class HastaYapilandirmasi
        : IEntityTypeConfiguration<Hasta>
    {
        public void Configure(EntityTypeBuilder<Hasta> builder)
        {
            builder.ToTable("tbl_Hastalar");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Ad)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Soyad)
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(x => x.KimlikNumarasi)
                .IsRequired()
                .HasMaxLength(11);

            builder.HasIndex(x => x.KimlikNumarasi)
                .IsUnique();

            builder.Property(x => x.DogumTarihi)
                .IsRequired();

            builder.Property(x => x.TelefonNumarasi)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(x => x.Adres)
                .HasMaxLength(300);

            builder.Property(x => x.AktifMi)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.OlusturulmaTarihi)
                .IsRequired();
            builder.Property(x => x.GuncellenmeTarihi)
            .IsRequired(false);

            builder.HasIndex(x => x.KullaniciHesabiId)
                .IsUnique();

            builder.HasOne(x => x.KullaniciHesabi)
                .WithOne()
                .HasForeignKey<Hasta>(x => x.KullaniciHesabiId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.AktifMi);
        }
    }
}