using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.DataAccess.Yapilandirmalar
{
    public class RandevuYapilandirmasi
        : IEntityTypeConfiguration<Randevu>
    {
        public void Configure(
            EntityTypeBuilder<Randevu> builder)
        {
            builder.ToTable(
                "tbl_Randevular",
                tablo =>
                {
                    tablo.HasCheckConstraint(
                        "CK_Randevu_BitisBaslangictanSonra",
                        "\"BitisZamani\" > \"BaslangicZamani\"");
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.BaslangicZamani)
                .IsRequired();

            builder.Property(x => x.BitisZamani)
                .IsRequired();

            builder.Property(x => x.Durum)
                .IsRequired();

            builder.Property(x => x.IptalNedeni)
                .HasMaxLength(300);

            builder.Property(x => x.OlusturulmaTarihi)
                .IsRequired();

            builder.Property(x => x.GuncellenmeTarihi)
                .IsRequired(false);

            builder.HasIndex(x => new
            {
                x.DoktorId,
                x.BaslangicZamani
            });

            builder.HasIndex(x => new
            {
                x.HastaId,
                x.BaslangicZamani
            });

            builder.HasOne(x => x.Doktor)
                .WithMany()
                .HasForeignKey(x => x.DoktorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Hasta)
                .WithMany()
                .HasForeignKey(x => x.HastaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OlusturanSekreter)
                .WithMany()
                .HasForeignKey(x => x.OlusturanSekreterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}