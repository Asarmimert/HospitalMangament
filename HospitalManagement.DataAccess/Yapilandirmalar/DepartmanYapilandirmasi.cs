using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.DataAccess.Yapilandirmalar
{
    public class DepartmanYapilandirmasi
        : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("tbl_Departmanlar");

            builder.HasKey(x => x.DepartmentId);

            builder.Property(x => x.DepartmentId)
                .HasColumnName("Id");

            builder.Property(x => x.Name)
                .HasColumnName("Ad")
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.Name)
                .IsUnique();

            builder.Property(x => x.Description)
                .HasColumnName("Aciklama")
                .HasMaxLength(200);

            builder.Property(x => x.AktifMi)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.OlusturulmaTarihi)
                .IsRequired();
        }
    }
}