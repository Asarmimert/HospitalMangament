using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.DataAccess.Yapilandirmalar
{
    public class IlacYapilandirmasi
        : IEntityTypeConfiguration<Ilac>
    //Ilac entitynin veritabanı kurallarını belirleyen sınıftır.
    {
        public void Configure(EntityTypeBuilder<Ilac> builder)
        {
            //Entity Framework bu metodu çalıştırır. İçine tablo adı, primary key, alan uzunlukları ve ilişkiler yazılır. builder, bu kuralları tanımlamak için kullanılır.
            builder.ToTable("tbl_Ilaclar");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Ad)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(x => x.Ad)
                .IsUnique();

            builder.Property(x => x.AktifMi)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.OlusturulmaTarihi)
                .IsRequired();
        }
    }
}