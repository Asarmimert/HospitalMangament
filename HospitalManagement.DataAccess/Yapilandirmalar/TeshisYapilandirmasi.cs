using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.DataAccess.Yapilandirmalar
{
    public class TeshisYapilandirmasi
        : IEntityTypeConfiguration<Teshis>
    {
        public void Configure(EntityTypeBuilder<Teshis> builder)
        {
            builder.ToTable("tbl_Teshisler");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TeshisKodu)
                .HasMaxLength(100);

            builder.Property(x => x.TeshisAdi)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Aciklama)
                .HasColumnType("text");

            builder.Property(x => x.AktifMi)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.OlusturulmaTarihi)
                .IsRequired();
            builder.Property(x => x.GuncellenmeTarihi)
            .IsRequired(false);

            // Aynı teşhis kodu iki defa kaydedilemez.
            builder.HasIndex(x => x.TeshisKodu)
                .IsUnique();

            // Aktif teşhisleri listeleyen sorguları hızlandırır.
            builder.HasIndex(x => x.AktifMi);
        }
    }
}