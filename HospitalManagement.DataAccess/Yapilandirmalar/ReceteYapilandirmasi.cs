using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.DataAccess.Yapilandirmalar
{
    public class ReceteYapilandirmasi
        : IEntityTypeConfiguration<Recete>
    {
        public void Configure(EntityTypeBuilder<Recete> builder)
        {
            builder.ToTable("tbl_Receteler");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReceteTarihi)
                .IsRequired();

            builder.Property(x => x.GenelNotlar)
                .HasColumnType("text");

            builder.Property(x => x.OlusturulmaTarihi)
                .IsRequired();

            builder.HasIndex(x => x.MuayeneId)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.HastaId,
                x.ReceteTarihi
            });

            builder.HasIndex(x => new
            {
                x.DoktorId,
                x.ReceteTarihi
            });

            builder.HasOne(x => x.Muayene)
                .WithOne()
                .HasForeignKey<Recete>(x => x.MuayeneId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Hasta)
                .WithMany()
                .HasForeignKey(x => x.HastaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Doktor)
                .WithMany()
                .HasForeignKey(x => x.DoktorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}