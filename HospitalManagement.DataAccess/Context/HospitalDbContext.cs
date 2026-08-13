using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.DataAccess.Context;

public class HospitalDbContext : DbContext
{
    //Veritabanı ayarlarını Db den alıyor sonra consructor oluşturuyor başlatırken direk çalışan kod topluluğu Alınan ayarları üst sınıfa yollar.
    //HospitalDbContext sınıfının constructor’ıdır (yapıcı metot). HospitalDbContext oluşturulurken veritabanı bağlantı ayarlarını alır.
    //HospitalDbContext’in PostgreSQL’e nasıl ve hangi bağlantı bilgileriyle bağlanacağını taşıyan ayar paketidir.
    //Veritabanına bağlanmak için gereken bilgileri HospitalDbContext sınıfına getirir.
    public HospitalDbContext(DbContextOptions<HospitalDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(HospitalDbContext).Assembly);
    }

    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<Doctor> Doctors { get; set; } = null!;

    public DbSet<KullaniciHesabi> KullaniciHesaplari { get; set; } = null!;

    public DbSet<Sekreter> Sekreterler { get; set; } = null!;

    public DbSet<Hasta> Hastalar {  get; set; } = null!;

    public DbSet<Ilac> Ilaclar { get; set; } = null!;

    public DbSet<Teshis> Teshisler { get; set; } = null!;

    public DbSet<Randevu> Randevular { get; set; } = null!;

    public DbSet<Muayene> Muayeneler { get; set; } = null!;

    public DbSet<MuayeneTeshisi> MuayeneTeshisleri { get; set; } = null!;

    public DbSet<Recete> Receteler { get; set; } = null!;

    public DbSet<ReceteIcerik> ReceteIcerikleri { get; set; } = null!;

}