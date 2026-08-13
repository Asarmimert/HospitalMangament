using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.DataAccess.Yapilandirmalar
{
    public class ReceteIcerikYapilandirmasi : IEntityTypeConfiguration<ReceteIcerik>
    {

        public void Configure(EntityTypeBuilder<ReceteIcerik> builder)
        {

            builder.ToTable("tbl_ReceteIcerik");


            builder.HasKey(x => x.Id);


            builder.HasIndex(x => x.ReceteId);

            builder.HasIndex(x => x.IlacId);



            builder.Property(x => x.KullanimTalimatlari)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.KullanimSuresi)
                .IsRequired()
                .HasMaxLength(100);



            builder.Property(x => x.Miktar)
                .IsRequired();

            builder.Property(x => x.OlusturulmaTarihi)
                .IsRequired();




            builder.HasOne(x => x.Recete)
                .WithMany()
                .HasForeignKey(x => x.ReceteId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(x => x.Ilac)
             .WithMany()
            .HasForeignKey(x => x.IlacId)
            .OnDelete(DeleteBehavior.Restrict);




        }






    }
}
