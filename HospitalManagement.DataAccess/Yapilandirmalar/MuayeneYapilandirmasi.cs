using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagement.DataAccess.Yapilandirmalar
{
    public class MuayeneYapilandirmasi
         : IEntityTypeConfiguration<Muayene>
    {


        public void Configure(EntityTypeBuilder<Muayene> builder)
        {

            builder.ToTable("tbl_Muayeneler");

            builder.HasKey(x => x.Id);


            builder.Property(x => x.HastaSikayeti)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(x => x.DoktorDegerlendirmesi)
                .IsRequired()
                .HasColumnType("text");


            builder.Property(x => x.DoktorNotlari)
                .HasColumnType("text");

            builder.Property(x => x.MuayeneTarihi)
                .IsRequired();

            builder.Property(x => x.OlusturulmaTarihi)
                .IsRequired();
            builder.Property(x => x.GuncellenmeTarihi)
                .IsRequired(false);

            builder.HasIndex(x => x.RandevuId)
                .IsUnique();

            builder.HasOne(x=>x.Randevu)
                .WithOne()
                //1-1 ise Ef core anlamıyor o yüzden tablo adını yazıyoruz!!! 
                .HasForeignKey<Muayene>(x=>x.RandevuId)
                .OnDelete(DeleteBehavior.Restrict);










        }







    }
}
