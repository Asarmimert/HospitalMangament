# Hastane Yönetim Sistemi API

## 1. Projenin Amacı

Bu proje; hastane içerisindeki departman, doktor, hasta, randevu, muayene, teşhis ve reçete işlemlerinin yönetilmesi amacıyla geliştirilmiş bir ASP.NET Core Web API uygulamasıdır.

Sistem; kullanıcıların rollerine göre yetkilendirilmesini, kişisel sağlık kayıtlarına yalnızca ilgili kullanıcıların erişmesini ve hastane işlemlerinin katmanlı bir mimari üzerinden yürütülmesini sağlar.

## 2. Kullanılan Teknolojiler

* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* PostgreSQL
* Npgsql
* JWT kimlik doğrulama
* Swagger/OpenAPI
* Memory Cache
* Fluent API
* Dependency Injection
* Git

## 3. Katmanlı Mimari

Proje dört ana katmandan oluşmaktadır:

### HospitalManagement.Entity

Veritabanı tablolarını temsil eden entity sınıfları ve enum değerleri bu katmanda bulunur.

Başlıca entity sınıfları:

* `KullaniciHesabi`
* `Department`
* `Doctor`
* `Hasta`
* `Sekreter`
* `Randevu`
* `Muayene`
* `Teshis`
* `MuayeneTeshisi`
* `Recete`
* `Ilac`
* `ReceteIcerik`

### HospitalManagement.DataAccess

Veritabanı bağlantısı ve veri erişim işlemleri bu katmanda gerçekleştirilir.

Bu katmanda:

* `HospitalDbContext`
* Entity yapılandırmaları
* Genel repository
* Entitylere özel repository sınıfları
* Entity Framework Core migration dosyaları

bulunur.

Salt okunur sorgularda `AsNoTracking`, ilişkili verilerin alınmasında `Include`, filtreleme ve sayfalama işlemlerinde veritabanı taraflı LINQ sorguları kullanılmaktadır.

### HospitalManagement.Business

İş kuralları ve doğrulamalar bu katmanda bulunur.

Örneğin:

* Aynı saatte doktor için çakışan randevu oluşturulamaması
* Pasif doktor, hasta veya departmanla işlem yapılamaması
* Bir muayeneye yalnızca bir reçete yazılması
* Aynı teşhisin aynı muayeneye tekrar eklenememesi
* Kullanıcı hesabının doğru role sahip olması
* Silme işlemlerinin uygun yerlerde pasifleştirme olarak uygulanması

Soyut klasöründeki arayüzler yapılacak işlemleri tanımlar. Somut klasöründeki sınıflar ise bu işlemlerin nasıl gerçekleştirileceğini içerir.

### HospitalManagementAPI

HTTP isteklerini karşılayan controller sınıfları, DTO’lar, JWT işlemleri, merkezi hata yönetimi, filtreler ve uygulama yapılandırmaları bu katmanda bulunur.

Controller sınıfları doğrudan veritabanına erişmez. İşlemler Business katmanındaki servisler üzerinden gerçekleştirilir.

## 4. Veritabanı Tasarım Kararları

Kullanıcı hesabı bilgileri ortak bir `KullaniciHesabi` tablosunda tutulmaktadır. Doktor, hasta ve sekreter kayıtları kullanıcı hesabına bire bir ilişkiyle bağlanmıştır.

Başlıca ilişkiler:

* Kullanıcı hesabı ile doktor: bire bir
* Kullanıcı hesabı ile hasta: bire bir
* Kullanıcı hesabı ile sekreter: bire bir
* Departman ile doktor: bire çok
* Doktor ile randevu: bire çok
* Hasta ile randevu: bire çok
* Randevu ile muayene: bire bir
* Muayene ile reçete: bire bir
* Muayene ile teşhis: ara tablo üzerinden çoka çok
* Reçete ile ilaç: reçete içeriği üzerinden çoka çok

Silme işlemlerinde ilişkili verilerin yanlışlıkla silinmesini engellemek amacıyla önemli foreign key ilişkilerinde `DeleteBehavior.Restrict` kullanılmıştır.

Doktor ve hasta randevularında zaman çakışmalarını engellemek için indeksler ve Business katmanında zaman aralığı kontrolü uygulanmıştır.

Departman, doktor, hasta, teşhis ve ilaç gibi kayıtlar fiziksel olarak silinmek yerine uygun işlemlerde `AktifMi` alanı değiştirilerek pasifleştirilmektedir.

## 5. Dependency Injection ve Servis Yaşam Döngüleri

Repository ve Business servisleri `Scoped` yaşam döngüsüyle kaydedilmiştir.

Örnek:

```csharp
builder.Services.AddScoped<
    IDepartmanServisi,
    DepartmanServisi>();

builder.Services.AddScoped(
    typeof(IGenelDepo<>),
    typeof(GenelDepo<>));
```

`HospitalDbContext` her HTTP isteği için ayrı oluşturulduğu için onu kullanan repository ve servislerin de `Scoped` olması tercih edilmiştir. Böylece aynı HTTP isteği boyunca aynı DbContext örneği güvenli şekilde kullanılır.

Memory Cache uygulama genelinde ortak bellek kullandığı için `AddMemoryCache` ile kaydedilmiştir.

## 6. Cache Stratejisi

Aktif departman listesi sık okunan ve seyrek değişen bir veri olduğu için bellekte önbelleğe alınmaktadır.

Departman listesi belirli bir süre boyunca cache üzerinden döndürülür. Departman ekleme, güncelleme veya pasifleştirme işlemlerinden sonra eski verilerin gösterilmemesi için ilgili cache kaydı temizlenir.

Hasta, randevu ve reçete gibi sık değişen ve kullanıcıya özel veriler cache içerisinde tutulmamaktadır.

## 7. Kimlik Doğrulama ve Yetkilendirme

Sistemde JWT tabanlı kimlik doğrulama kullanılmaktadır.

Kullanıcı rolleri:

* `Doktor`
* `Sekreter`
* `Hasta`

Kayıt ve giriş endpoint’leri anonim olarak kullanılabilir. Diğer endpoint’ler geçerli bir JWT token gerektirir.

Temel yetkiler:

| İşlem                                      | Yetkili roller            |
| ------------------------------------------ | ------------------------- |
| Kayıt ve giriş                             | Herkes                    |
| Departmanları görüntüleme                  | Giriş yapan kullanıcılar  |
| Departman ekleme, güncelleme ve silme      | Sekreter                  |
| Doktor ekleme, güncelleme ve pasifleştirme | Sekreter                  |
| Hasta listesini görüntüleme                | Doktor ve sekreter        |
| Hastanın kendi bilgilerini görüntülemesi   | İlgili hasta              |
| Randevu oluşturma ve güncelleme            | Sekreter                  |
| Randevu durumunu değiştirme                | Sekreter ve ilgili doktor |
| Muayene oluşturma ve güncelleme            | İlgili doktor             |
| Teşhis yönetimi                            | Doktor                    |
| Reçete oluşturma ve güncelleme             | İlgili doktor             |
| Reçeteyi görüntüleme                       | İlgili doktor veya hasta  |
| Reçete içeriği yönetimi                    | Reçetenin ilgili doktoru  |
| İlaç yönetimi                              | Doktor                    |

Sadece rol kontrolü yapılmamaktadır. Hasta ve doktor kullanıcıları için kaydın gerçekten giriş yapan kullanıcıya ait olup olmadığı da JWT içerisindeki kullanıcı kimliği üzerinden kontrol edilmektedir.

## 8. Bilinen Eksikler ve Geliştirme Önerileri

Projenin ileride geliştirilebilecek bölümleri:

* Refresh token desteği
* Parola sıfırlama
* E-posta doğrulama
* Otomatik birim ve entegrasyon testleri
* Docker Compose yapılandırması
* Sağlık kontrolü endpoint’i
* Web veya mobil kullanıcı arayüzü
* Ayrıntılı denetim kayıtları
* Üretim ortamı için merkezi loglama

Mevcut test kullanıcıları geliştirme amacıyla hazırlanmıştır. Üretim ortamında kullanıcı ve personel oluşturma işlemleri güvenli bir yönetim süreci üzerinden gerçekleştirilmelidir.

## 9. Projeyi Çalıştırma

### Gereksinimler

* .NET 10 SDK
* PostgreSQL
* Visual Studio 2022 veya uyumlu bir geliştirme ortamı
* Entity Framework Core araçları

### Gizli ayarların tanımlanması

Bağlantı bilgisi ve JWT anahtarı kaynak kod içerisinde tutulmamalıdır.

API projesi için User Secrets tanımlanabilir:

```bash
dotnet user-secrets init --project HospitalManagementAPI
```

Veritabanı bağlantısı:

```bash
dotnet user-secrets set \
"ConnectionStrings:DefaultConnection" \
"Host=localhost;Port=5432;Database=HospitalManagement;Username=postgres;Password=PAROLANIZ" \
--project HospitalManagementAPI
```

JWT anahtarı:

```bash
dotnet user-secrets set \
"Jwt:Anahtar" \
"EN-AZ-32-KARAKTER-UZUNLUGUNDA-GIZLI-BIR-ANAHTAR" \
--project HospitalManagementAPI
```

Ayarlar ortam değişkenleriyle de verilebilir:

```text
ConnectionStrings__DefaultConnection
Jwt__Anahtar
```

### Paketlerin yüklenmesi

```bash
dotnet restore
```

### Migrationların veritabanına uygulanması

```bash
dotnet ef database update \
--project HospitalManagement.DataAccess \
--startup-project HospitalManagementAPI
```

### Uygulamanın başlatılması

```bash
dotnet run --project HospitalManagementAPI
```

Development ortamında Swagger arayüzü aşağıdaki adres üzerinden açılabilir:

```text
https://localhost:7007/swagger
```

Önce `/api/Yetkilendirme/giris` endpoint’i üzerinden giriş yapılır. Dönen JWT token, Swagger’daki `Authorize` düğmesine girildikten sonra korumalı endpoint’ler kullanılabilir.

Randevu listeleme işleminde istek iptal edildiğinde veritabanı sorgusunun da durdurulabilmesi için Controller, Business ve DataAccess katmanları boyunca `CancellationToken` aktarılmaktadır.

### DepartmanServisi Yaşam Döngüsü

`DepartmanServisi`, `Scoped` yaşam döngüsüyle kaydedilmiştir. Bu servis
veritabanına erişen repository sınıfını kullandığı için her HTTP
isteğinde bir servis örneği oluşturulması uygundur.

`Singleton` kullanılsaydı uygulama boyunca aynı servis örneği
paylaşılırdı. Bu durumda `Scoped` olan DbContext ile yaşam döngüsü
uyuşmazlığı, eş zamanlı kullanım ve eski veri sorunları oluşabilirdi.

`Transient` kullanılsaydı aynı HTTP isteği içerisinde ihtiyaç duyulan
her çözümlemede gereksiz yere yeni servis örnekleri oluşturulabilirdi.

### RandevuServisi Yaşam Döngüsü

`RandevuServisi`, `Scoped` yaşam döngüsüyle kaydedilmiştir. Randevu
oluşturma, güncelleme ve çakışma kontrolü sırasında aynı HTTP isteği
boyunca repository ve DbContext örneklerinin tutarlı kullanılması
amaçlanmıştır.

`Singleton` kullanılması DbContext'in farklı HTTP istekleri arasında
paylaşılmasına neden olabileceğinden güvenli değildir. `Transient`
kullanılması ise aynı istek içerisinde gereksiz servis örnekleri
oluşturabilir. Bu nedenle `Scoped` yaşam döngüsü tercih edilmiştir.

## 6. Cache Stratejisi

Sistemde sık görüntülenen ve diğer verilere göre daha seyrek değişen
aktif departman listesi `IMemoryCache` kullanılarak önbelleğe
alınmaktadır.

Cache anahtarı:

```text
aktif-departmanlar


Cache kaydı için 10 dakikalık mutlak geçerlilik süresi
kullanılmaktadır. Süre dolduğunda aktif departmanlar PostgreSQL
veritabanından yeniden alınır ve cache tekrar oluşturulur.

Departman ekleme, güncelleme veya pasifleştirme işlemi başarıyla
tamamlandıktan sonra CacheTemizle metodu çalıştırılır. Bu metot
aktif-departmanlar anahtarını cache üzerinden kaldırır. Böylece bir
sonraki listeleme isteğinde güncel veriler veritabanından alınır.

Hasta, randevu, muayene ve reçete gibi kullanıcıya özel veya sık
değişen veriler ortak bellek cache'inde tutulmamaktadır. Böylece hassas
verilerin kullanıcılar arasında paylaşılması ve eski verilerin
gösterilmesi önlenmektedir.

