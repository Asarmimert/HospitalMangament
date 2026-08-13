using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.DataAccess.Yapilandirmalar
{
    public class SekreterYapilandirmasi
        : IEntityTypeConfiguration<Sekreter>
    {
        public void Configure(
            EntityTypeBuilder<Sekreter> builder)
        {
            builder.ToTable("tbl_Sekreterler");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Ad)
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(x => x.Soyad)
                .IsRequired()
                .HasMaxLength(35);

            builder.Property(x => x.TelefonNumarasi)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.AktifMi)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.OlusturulmaTarihi)
                .IsRequired();

            builder.Property(x => x.GuncellenmeTarihi)
                .IsRequired(false);

            builder.HasIndex(x => x.KullaniciHesabiId)
                .IsUnique();

            builder.HasIndex(x => x.AktifMi);

            builder.HasOne(x => x.KullaniciHesabi)
                .WithOne()
                .HasForeignKey<Sekreter>(
                    x => x.KullaniciHesabiId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}