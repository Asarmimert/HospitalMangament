using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders; // EntityTypeBuilder için gerekli
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagement.DataAccess.Yapilandirmalar
{
    public class KullaniciHesabiYapilandirmasi : IEntityTypeConfiguration<KullaniciHesabi>
    {
        public void Configure(EntityTypeBuilder<KullaniciHesabi> builder)
        {
            builder.ToTable("tbl_KullaniciHesaplari");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Eposta)
                .IsRequired()
                .HasMaxLength(150);


            builder.HasIndex(x=> x.Eposta)
                .IsUnique();


            builder.Property(x => x.ParolaHash)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Rol)
                .IsRequired();

            builder.Property(x=>x.AktifMi)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x=>x.OlusturulmaTarihi)
                .IsRequired();




















        }
    }
}