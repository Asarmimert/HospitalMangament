using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.DataAccess.Yapilandirmalar
{
    public class MuayeneTeshisiYapilandirmasi
        : IEntityTypeConfiguration<MuayeneTeshisi>
    {
        public void Configure(EntityTypeBuilder<MuayeneTeshisi> builder)
        {
            builder.ToTable("tbl_MuayeneTeshisleri");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DoktorNotu)
                .HasColumnType("text");

            builder.Property(x => x.OlusturulmaTarihi)
                .IsRequired();
            builder.Property(x => x.GuncellenmeTarihi)
             .IsRequired(false);
            builder.HasIndex(x => new
            {
                x.MuayeneId,
                x.TeshisId
            })
            .IsUnique();

            builder.HasOne(x => x.Muayene)
                .WithMany()
                .HasForeignKey(x => x.MuayeneId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Teshis)
                .WithMany()
                .HasForeignKey(x => x.TeshisId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}