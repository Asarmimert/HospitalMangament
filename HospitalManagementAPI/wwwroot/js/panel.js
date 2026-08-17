const token = sessionStorage.getItem("token");
const kullaniciId = sessionStorage.getItem("kullaniciId");
const eposta = sessionStorage.getItem("eposta");
const rol = sessionStorage.getItem("rol");
const tokenBitisTarihi =
    sessionStorage.getItem("tokenBitisTarihi");

// Kullanıcı giriş yapmamışsa giriş sayfasına gönder.
if (!token) {
    window.location.href = "/index.html";
}

// Token süresi dolmuşsa oturumu kapat.
if (
    tokenBitisTarihi &&
    new Date(tokenBitisTarihi) <= new Date()
) {
    oturumuKapat();
}

const kullaniciEposta =
    document.getElementById("kullaniciEposta");

const kullaniciRol =
    document.getElementById("kullaniciRol");

const rolBilgisi =
    document.getElementById("rolBilgisi");

const hosgeldinMetni =
    document.getElementById("hosgeldinMetni");

const kullaniciSimgesi =
    document.querySelector(".kullanici-simgesi");

const tarihAlani =
    document.getElementById("tarihAlani");

const sayfaBasligi =
    document.getElementById("sayfaBasligi");

const icerikAlani =
    document.getElementById("icerikAlani");

const anaSayfaIcerigi = icerikAlani.innerHTML;


// Kullanıcı bilgilerini göster.
kullaniciEposta.textContent = eposta || "Kullanıcı";
kullaniciRol.textContent = rol || "Rol";
rolBilgisi.textContent = rol || "-";

hosgeldinMetni.textContent =
    `${rol || "Kullanıcı"} paneline hoş geldiniz`;

kullaniciSimgesi.textContent =
    eposta ? eposta.charAt(0).toUpperCase() : "K";


// Bugünün tarihini göster.
tarihAlani.textContent =
    new Intl.DateTimeFormat(
        "tr-TR",
        {
            day: "numeric",
            month: "long",
            year: "numeric"
        }
    ).format(new Date());


// Kullanıcının rolüne uygun olmayan menüleri gizle.
document
    .querySelectorAll("[data-roller]")
    .forEach((menuOgesi) => {
        const izinliRoller =
            menuOgesi.dataset.roller.split(",");

        if (!izinliRoller.includes(rol)) {
            menuOgesi.classList.add("gizli");
        }
    });


// Menü tıklama işlemleri.
document
    .querySelectorAll(".menu-ogesi")
    .forEach((menuOgesi) => {
        menuOgesi.addEventListener("click", () => {
            document
                .querySelectorAll(".menu-ogesi")
                .forEach((oge) => {
                    oge.classList.remove("aktif");
                });

            menuOgesi.classList.add("aktif");

            const sayfa = menuOgesi.dataset.sayfa;
            const baslik = menuOgesi.textContent.trim();

            sayfaBasligi.textContent = baslik;

            if (sayfa === "anasayfa") {
                icerikAlani.innerHTML = anaSayfaIcerigi;

                document.getElementById(
                    "rolBilgisi"
                ).textContent = rol || "-";

                document.getElementById(
                    "hosgeldinMetni"
                ).textContent =
                    `${rol || "Kullanıcı"} paneline hoş geldiniz`;

                return;
            }

            if (sayfa === "departmanlar") {
                departmanlariGetir();
                return;
            }

            if (sayfa === "doktorlar") {
                doktorlariGetir();
                return;
            }
            if (sayfa === "hastalar") {
                hastalariGetir();
                return;
            }

            if (sayfa === "randevular") {
                randevulariGetir();
                return;
            }


            if (sayfa === "muayeneler") {
                muayeneleriGetir();
                return;
            }

            if (sayfa === "receteler") {
                receteleriGetir();
                return;
            }

            icerikAlani.innerHTML = `
                <div class="icerik-karti">
                    <h3>${baslik}</h3>

                    <p>
                        ${baslik} ekranı hazırlanıyor.
                        Bir sonraki adımda bu bölüm API'ye
                        bağlanacaktır.
                    </p>
                </div>
            `;
        });
    });


// Çıkış işlemi.
document
    .getElementById("cikisButonu")
    .addEventListener("click", () => {
        oturumuKapat();
    });


function oturumuKapat() {
    sessionStorage.clear();
    window.location.href = "/index.html";
}


async function departmanlariGetir() {
    icerikAlani.innerHTML = `
        <div class="yukleniyor">
            Departmanlar yükleniyor...
        </div>
    `;

    try {
        const cevap = await apiIstegi(
            "/api/Departments"
        );

        if (!cevap.ok) {
            if (cevap.status === 403) {
                throw new Error(
                    "Departmanları görüntüleme yetkiniz bulunmuyor."
                );
            }

            throw new Error(
                "Departmanlar alınamadı. " +
                `Hata kodu: ${cevap.status}`
            );
        }

        const departmanlar = await cevap.json();

        if (!departmanlar || departmanlar.length === 0) {
            icerikAlani.innerHTML = `
                <div class="bos-kayit">
                    Aktif departman bulunamadı.
                </div>
            `;

            return;
        }

        const satirlar = departmanlar
            .map((departman) => {
                return `
                    <tr>
                        <td>
                            ${departman.departmentId}
                        </td>

                        <td>
                            ${htmlGuvenli(departman.name)}
                        </td>

                        <td class="metin-hucresi">
                            ${htmlGuvenli(
                    departman.description
                )}
                        </td>

                        <td>
                            <span class="durum-etiketi">
                                Aktif
                            </span>
                        </td>
                    </tr>
                `;
            })
            .join("");

        icerikAlani.innerHTML = `
            <div class="icerik-karti">
                <div class="icerik-basligi">
                    <div>
                        <h3>Departmanlar</h3>

                        <p>
                            Aktif hastane departmanları
                        </p>
                    </div>

                    <button
                        type="button"
                        class="yenile-butonu"
                        onclick="departmanlariGetir()">
                        Yenile
                    </button>
                </div>

                <div class="tablo-kapsayici">
                    <table class="veri-tablosu">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Departman</th>
                                <th>Açıklama</th>
                                <th>Durum</th>
                            </tr>
                        </thead>

                        <tbody>
                            ${satirlar}
                        </tbody>
                    </table>
                </div>
            </div>
        `;
    }
    catch (hata) {
        icerikAlani.innerHTML = `
            <div class="hata-kutusu">
                ${htmlGuvenli(hata.message)}
            </div>
        `;
    }
}


async function doktorlariGetir() {
    icerikAlani.innerHTML = `
        <div class="yukleniyor">
            Doktorlar yükleniyor...
        </div>
    `;

    try {
        const cevap = await apiIstegi(
            "/api/Doctors" +
            "?SayfaNo=1" +
            "&SayfaBoyutu=100" +
            "&AktifMi=true"
        );

        if (!cevap.ok) {
            throw new Error(
                "Doktorlar alınamadı. " +
                `Hata kodu: ${cevap.status}`
            );
        }

        const veri = await cevap.json();

        const doktorlar =
            veri.kayitlar ?? veri;

        if (!doktorlar || doktorlar.length === 0) {
            icerikAlani.innerHTML = `
                <div class="bos-kayit">
                    Kayıtlı aktif doktor bulunamadı.
                </div>
            `;

            return;
        }

        const satirlar = doktorlar
            .map((doktor) => {
                return `
                    <tr>
                        <td>
                            ${htmlGuvenli(
                    doktor.doktorAd
                )}
                        </td>

                        <td>
                            ${htmlGuvenli(
                    doktor.doktorSoyad
                )}
                        </td>

                        <td>
                            ${htmlGuvenli(
                    doktor.departmanAdi
                )}
                        </td>

                        <td>
                            ${htmlGuvenli(
                    doktor.uzmanlikAlani
                )}
                        </td>

                        <td>
                            ${htmlGuvenli(
                    doktor.telefonNumarasi
                )}
                        </td>

                        <td>
                            <span class="durum-etiketi">
                                Aktif
                            </span>
                        </td>
                    </tr>
                `;
            })
            .join("");

        icerikAlani.innerHTML = `
            <div class="icerik-karti">
                <div class="icerik-basligi">
                    <div>
                        <h3>Doktorlar</h3>

                        <p>
                            Aktif doktorların listesi
                        </p>
                    </div>

                    <button
                        type="button"
                        class="yenile-butonu"
                        onclick="doktorlariGetir()">
                        Yenile
                    </button>
                </div>

                <div class="tablo-kapsayici">
                    <table class="veri-tablosu">
                        <thead>
                            <tr>
                                <th>Ad</th>
                                <th>Soyad</th>
                                <th>Departman</th>
                                <th>Uzmanlık</th>
                                <th>Telefon</th>
                                <th>Durum</th>
                            </tr>
                        </thead>

                        <tbody>
                            ${satirlar}
                        </tbody>
                    </table>
                </div>
            </div>
        `;
    }
    catch (hata) {
        icerikAlani.innerHTML = `
            <div class="hata-kutusu">
                ${htmlGuvenli(hata.message)}
            </div>
        `;
    }
}
async function hastalariGetir() {
    icerikAlani.innerHTML = `
        <div class="yukleniyor">
            Hastalar yükleniyor...
        </div>
    `;

    try {
        const cevap = await apiIstegi(
            "/api/Hasta" +
            "?SayfaNo=1" +
            "&SayfaBoyutu=100" +
            "&AktifMi=true"
        );

        if (!cevap.ok) {
            if (cevap.status === 403) {
                throw new Error(
                    "Hastaları görüntüleme yetkiniz bulunmuyor."
                );
            }

            throw new Error(
                `Hastalar alınamadı. Hata kodu: ${cevap.status}`
            );
        }

        const veri = await cevap.json();
        const hastalar = veri.kayitlar ?? veri;

        if (!hastalar || hastalar.length === 0) {
            icerikAlani.innerHTML = `
                <div class="bos-kayit">
                    Kayıtlı aktif hasta bulunmuyor.
                </div>
            `;

            return;
        }

        const satirlar = hastalar
            .map((hasta) => {
                return `
                    <tr>
                        <td>${hasta.id}</td>

                        <td>
                            ${htmlGuvenli(hasta.ad)}
                        </td>

                        <td>
                            ${htmlGuvenli(hasta.soyad)}
                        </td>

                        <td>
                            ${htmlGuvenli(hasta.eposta)}
                        </td>

                        <td>
                            ${htmlGuvenli(
                    hasta.kimlikNumarasi
                )}
                        </td>

                        <td>
                            ${tarihYaz(hasta.dogumTarihi)}
                        </td>

                        <td>
                            ${htmlGuvenli(
                    hasta.telefonNumarasi
                )}
                        </td>

                        <td>
                            <span class="durum-etiketi">
                                ${hasta.aktifMi
                        ? "Aktif"
                        : "Pasif"
                    }
                            </span>
                        </td>
                    </tr>
                `;
            })
            .join("");

        icerikAlani.innerHTML = `
            <div class="icerik-karti">
                <div class="icerik-basligi">
                    <div>
                        <h3>Hastalar</h3>

                        <p>
                            Sistemde kayıtlı aktif hastalar
                        </p>
                    </div>

                    <button
                        type="button"
                        class="yenile-butonu"
                        onclick="hastalariGetir()">
                        Yenile
                    </button>
                </div>

                <div class="tablo-kapsayici">
                    <table class="veri-tablosu">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Ad</th>
                                <th>Soyad</th>
                                <th>E-posta</th>
                                <th>Kimlik numarası</th>
                                <th>Doğum tarihi</th>
                                <th>Telefon</th>
                                <th>Durum</th>
                            </tr>
                        </thead>

                        <tbody>
                            ${satirlar}
                        </tbody>
                    </table>
                </div>
            </div>
        `;
    }
    catch (hata) {
        icerikAlani.innerHTML = `
            <div class="hata-kutusu">
                ${htmlGuvenli(hata.message)}
            </div>
        `;
    }
}

async function randevulariGetir() {
    icerikAlani.innerHTML = `
        <div class="yukleniyor">
            Randevular yükleniyor...
        </div>
    `;

    try {
        const cevap = await apiIstegi(
            "/api/Randevu" +
            "?SayfaNo=1" +
            "&SayfaBoyutu=100"
        );

        if (!cevap.ok) {
            if (cevap.status === 403) {
                throw new Error(
                    "Randevuları görüntüleme yetkiniz bulunmuyor " +
                    "veya hasta profiliniz henüz oluşturulmamış."
                );
            }

            throw new Error(
                "Randevular alınamadı. " +
                `Hata kodu: ${cevap.status}`
            );
        }

        const veri = await cevap.json();

        const randevular =
            veri.kayitlar ?? veri;

        if (!randevular || randevular.length === 0) {
            icerikAlani.innerHTML = `
                <div class="bos-kayit">
                    Henüz randevunuz bulunmuyor.
                </div>
            `;

            return;
        }

        const satirlar = randevular
            .map((randevu) => {
                return `
                    <tr>
                        <td>
                            ${htmlGuvenli(
                    randevu.doktorAdiSoyadi
                )}
                        </td>

                        <td>
                            ${tarihYaz(
                    randevu.baslangicZamani
                )}
                        </td>

                        <td>
                            ${saatYaz(
                    randevu.baslangicZamani
                )}
                        </td>

                        <td>
                            ${saatYaz(
                    randevu.bitisZamani
                )}
                        </td>

                        <td>
                            <span class="durum-etiketi">
                                ${htmlGuvenli(
                    randevu.durumAdi
                )}
                            </span>
                        </td>
                        ${(rol ?? "").trim().toLowerCase() === "sekreter"
                        ? `
        <td>
           ${randevu.durum === 2
                        ? `
        <div class="islem-butonlari">
            <button
                type="button"
                class="duzenle-butonu"
                onclick="randevuDuzenlemeFormunuGoster(${randevu.id})">
                Düzenle
            </button>

            <button
                type="button"
                class="iptal-butonu"
                onclick="randevuIptalEt(${randevu.id})">
                İptal Et
            </button>
        </div>
    `
                        : "-"
}
        </td>
    `
                        : ""
                    }${(rol ?? "").trim().toLowerCase() === "doktor"
                        ? `
        <td>
            ${randevu.durum === 2
                            ? `
                    <div class="doktor-islem-butonlari">
                        <button
                            type="button"
                            class="tamamla-butonu"
                            onclick="randevuTamamla(${randevu.id})">
                            Tamamla
                        </button>

                        <button
                            type="button"
                            class="gelmedi-butonu"
                            onclick="randevuHastaGelmedi(${randevu.id})">
                            Gelmedi
                        </button>

                        <button
                            type="button"
                            class="iptal-butonu"
                            onclick="randevuDoktorIptalEt(${randevu.id})">
                            İptal
                        </button>
                    </div>
                `
                            : "-"
                        }
        </td>
    `
                        : ""
}

                    </tr>
                `;
            })
            .join("");

        icerikAlani.innerHTML = `
            <div class="icerik-karti">
                <div class="icerik-basligi">
                    <div>
                        <h3>Randevularım</h3>

                        <p>
                            Size ait randevu kayıtları
                        </p>
                    </div>

                    <div class="baslik-butonlari">
${(rol ?? "").trim().toLowerCase() === "sekreter"                ? `
            <button
                type="button"
                class="yenile-butonu"
                onclick="randevuOlusturmaFormunuGoster()">
                Yeni Randevu
            </button>
        `
            : ""
    }
    <button
    type="button"
    class="yenile-butonu"
    onclick="randevuFiltreFormunuGoster()">
    Randevu Ara
</button>
    <button
        type="button"
        class="yenile-butonu"
        onclick="randevulariGetir()">
        Yenile
    </button>
</div>
                </div>

                <div class="tablo-kapsayici">
                    <table class="veri-tablosu">
                        <thead>
                            <tr>
                                <th>Doktor</th>
                                <th>Tarih</th>
                                <th>Başlangıç</th>
                                <th>Bitiş</th>
                                <th>Durum</th>
                              ${["sekreter", "doktor"].includes(
                                  (rol ?? "").trim().toLowerCase()
                              )
                ? "<th>İşlem</th>"
                : ""
}

                            </tr>
                        </thead>

                        <tbody>
                            ${satirlar}
                        </tbody>
                    </table>
                </div>
            </div>
        `;
    }
    catch (hata) {
        icerikAlani.innerHTML = `
            <div class="hata-kutusu">
                ${htmlGuvenli(hata.message)}
            </div>
        `;
    }
}
async function randevuOlusturmaFormunuGoster() {
    icerikAlani.innerHTML = `
        <div class="yukleniyor">
            Randevu formu hazırlanıyor...
        </div>
    `;

    try {
        const [doktorCevabi, hastaCevabi] =
            await Promise.all([
                apiIstegi(
                    "/api/Doctors" +
                    "?SayfaNo=1" +
                    "&SayfaBoyutu=100" +
                    "&AktifMi=true"
                ),

                apiIstegi(
                    "/api/Hasta" +
                    "?SayfaNo=1" +
                    "&SayfaBoyutu=100" +
                    "&AktifMi=true"
                )
            ]);

        if (!doktorCevabi.ok) {
            throw new Error(
                "Doktor listesi alınamadı."
            );
        }

        if (!hastaCevabi.ok) {
            throw new Error(
                "Hasta listesi alınamadı."
            );
        }

        const doktorVerisi =
            await doktorCevabi.json();

        const hastaVerisi =
            await hastaCevabi.json();

        const doktorlar =
            doktorVerisi.kayitlar ?? doktorVerisi;

        const hastalar =
            hastaVerisi.kayitlar ?? hastaVerisi;

        const doktorSecenekleri = doktorlar
            .map((doktor) => {
                return `
                    <option value="${doktor.id}">
                        ${htmlGuvenli(
                    doktor.doktorAd + " " +
                    doktor.doktorSoyad
                )}
                    </option>
                `;
            })
            .join("");

        const hastaSecenekleri = hastalar
            .map((hasta) => {
                return `
                    <option value="${hasta.id}">
                        ${htmlGuvenli(
                    hasta.ad + " " +
                    hasta.soyad
                )}
                    </option>
                `;
            })
            .join("");

        icerikAlani.innerHTML = `
            <div class="icerik-karti">
                <div class="icerik-basligi">
                    <div>
                        <h3>Yeni Randevu</h3>

                        <p>
                            Doktor, hasta ve randevu
                            saatini seçiniz.
                        </p>
                    </div>

                    <button
                        type="button"
                        class="yenile-butonu"
                        onclick="randevulariGetir()">
                        Listeye Dön
                    </button>
                </div>

                <form
                    id="randevuOlusturmaFormu"
                    class="randevu-formu">

                    <div class="form-grid">
                        <div class="form-grubu">
                            <label for="randevuDoktorId">
                                Doktor
                            </label>

                            <select
                                id="randevuDoktorId"
                                required>

                                <option value="">
                                    Doktor seçiniz
                                </option>

                                ${doktorSecenekleri}
                            </select>
                        </div>

                        <div class="form-grubu">
                            <label for="randevuHastaId">
                                Hasta
                            </label>

                            <select
                                id="randevuHastaId"
                                required>

                                <option value="">
                                    Hasta seçiniz
                                </option>

                                ${hastaSecenekleri}
                            </select>
                        </div>

                        <div class="form-grubu">
                            <label for="randevuBaslangic">
                                Başlangıç zamanı
                            </label>

                            <input
                                type="datetime-local"
                                id="randevuBaslangic"
                                required>
                        </div>

                        <div class="form-grubu">
                            <label for="randevuBitis">
                                Bitiş zamanı
                            </label>

                            <input
                                type="datetime-local"
                                id="randevuBitis"
                                required>
                        </div>
                    </div>

                    <div class="form-islemleri">
                        <button
                            type="submit"
                            class="yenile-butonu">
                            Randevuyu Kaydet
                        </button>
                    </div>

                    <div
                        id="randevuMesajAlani"
                        class="mesaj gizli">
                    </div>
                </form>
            </div>
        `;

        document
            .getElementById("randevuOlusturmaFormu")
            .addEventListener(
                "submit",
                randevuOlustur
            );
    }
    catch (hata) {
        icerikAlani.innerHTML = `
            <div class="hata-kutusu">
                ${htmlGuvenli(hata.message)}
            </div>
        `;
    }
}


async function randevuOlustur(event) {
    event.preventDefault();

    const doktorId = Number(
        document
            .getElementById("randevuDoktorId")
            .value
    );

    const hastaId = Number(
        document
            .getElementById("randevuHastaId")
            .value
    );

    const baslangicDegeri =
        document
            .getElementById("randevuBaslangic")
            .value;

    const bitisDegeri =
        document
            .getElementById("randevuBitis")
            .value;

    const baslangicZamani =
        new Date(baslangicDegeri);

    const bitisZamani =
        new Date(bitisDegeri);

    const mesajAlani =
        document.getElementById(
            "randevuMesajAlani"
        );

    if (bitisZamani <= baslangicZamani) {
        mesajAlani.textContent =
            "Bitiş zamanı başlangıç zamanından " +
            "sonra olmalıdır.";

        mesajAlani.className = "mesaj hata";
        return;
    }

    const buton =
        event.currentTarget.querySelector(
            "button[type='submit']"
        );

    buton.disabled = true;
    buton.textContent = "Kaydediliyor...";

    try {
        const cevap = await apiIstegi(
            "/api/Randevu",
            {
                method: "POST",

                body: JSON.stringify({
                    doktorId: doktorId,
                    hastaId: hastaId,

                  

                    baslangicZamani:
                        baslangicZamani.toISOString(),

                    bitisZamani:
                        bitisZamani.toISOString()
                })
            }
        );

        if (!cevap.ok) {
            const hataMetni =
                await cevap.text();

            let hataMesaji =
                "Randevu oluşturulamadı.";

            if (hataMetni) {
                try {
                    const hataVerisi =
                        JSON.parse(hataMetni);

                    hataMesaji =
                        hataVerisi.detail ??
                        hataVerisi.mesaj ??
                        hataVerisi.title ??
                        hataMesaji;
                }
                catch {
                    const ilkSatir =
                        hataMetni.split(/\r?\n/)[0];

                    hataMesaji = ilkSatir.replace(
                        /^System\.[^:]+:\s*/,
                        ""
                    );
                }
            }

            throw new Error(hataMesaji);
        }

        mesajAlani.textContent =
            "Randevu başarıyla oluşturuldu.";

        mesajAlani.className =
            "mesaj basarili";

        setTimeout(() => {
            randevulariGetir();
        }, 700);
    }
    catch (hata) {
        mesajAlani.textContent =
            hata.message;

        mesajAlani.className =
            "mesaj hata";
    }
    finally {
        buton.disabled = false;
        buton.textContent =
            "Randevuyu Kaydet";
    }
}
async function randevuIptalEt(randevuId) {
    const iptalNedeni = prompt(
        "Randevunun iptal nedenini yazınız:"
    );

    // Kullanıcı İptal butonuna bastıysa işlemi durdur.
    if (iptalNedeni === null) {
        return;
    }

    if (!iptalNedeni.trim()) {
        alert("İptal nedeni boş bırakılamaz.");
        return;
    }

    const onaylandiMi = confirm(
        "Randevuyu iptal etmek istediğinize emin misiniz?"
    );

    if (!onaylandiMi) {
        return;
    }

    try {
        const cevap = await apiIstegi(
            `/api/Randevu/${randevuId}/durum`,
            {
                method: "PATCH",

                body: JSON.stringify({
                    durum: 0,
                    iptalNedeni: iptalNedeni.trim()
                })
            }
        );

        if (!cevap.ok) {
            const hataMetni = await cevap.text();

            let hataMesaji =
                "Randevu iptal edilemedi.";

            if (hataMetni) {
                try {
                    const hataVerisi =
                        JSON.parse(hataMetni);

                    hataMesaji =
                        hataVerisi.detail ??
                        hataVerisi.mesaj ??
                        hataVerisi.title ??
                        hataMesaji;
                }
                catch {
                    hataMesaji = hataMetni;
                }
            }

            throw new Error(hataMesaji);
        }

        alert("Randevu başarıyla iptal edildi.");

        await randevulariGetir();
    }
    catch (hata) {
        alert(hata.message);
    }
}
async function randevuDuzenlemeFormunuGoster(randevuId) {
    icerikAlani.innerHTML = `
        <div class="yukleniyor">
            Randevu bilgileri hazırlanıyor...
        </div>
    `;

    try {
        const [randevuCevabi, doktorCevabi, hastaCevabi] =
            await Promise.all([
                apiIstegi(`/api/Randevu/${randevuId}`),

                apiIstegi(
                    "/api/Doctors" +
                    "?SayfaNo=1" +
                    "&SayfaBoyutu=100" +
                    "&AktifMi=true"
                ),

                apiIstegi(
                    "/api/Hasta" +
                    "?SayfaNo=1" +
                    "&SayfaBoyutu=100" +
                    "&AktifMi=true"
                )
            ]);

        if (!randevuCevabi.ok) {
            throw new Error("Randevu bulunamadı.");
        }

        if (!doktorCevabi.ok) {
            throw new Error("Doktor listesi alınamadı.");
        }

        if (!hastaCevabi.ok) {
            throw new Error("Hasta listesi alınamadı.");
        }

        const randevu = await randevuCevabi.json();
        const doktorVerisi = await doktorCevabi.json();
        const hastaVerisi = await hastaCevabi.json();

        const doktorlar =
            doktorVerisi.kayitlar ?? doktorVerisi;

        const hastalar =
            hastaVerisi.kayitlar ?? hastaVerisi;

        const doktorSecenekleri = doktorlar
            .map((doktor) => {
                const seciliMi =
                    doktor.id === randevu.doktorId
                        ? "selected"
                        : "";

                return `
                    <option
                        value="${doktor.id}"
                        ${seciliMi}>
                        ${htmlGuvenli(
                    doktor.doktorAd + " " +
                    doktor.doktorSoyad
                )}
                    </option>
                `;
            })
            .join("");

        const hastaSecenekleri = hastalar
            .map((hasta) => {
                const seciliMi =
                    hasta.id === randevu.hastaId
                        ? "selected"
                        : "";

                return `
                    <option
                        value="${hasta.id}"
                        ${seciliMi}>
                        ${htmlGuvenli(
                    hasta.ad + " " + hasta.soyad
                )}
                    </option>
                `;
            })
            .join("");

        icerikAlani.innerHTML = `
            <div class="icerik-karti">
                <div class="icerik-basligi">
                    <div>
                        <h3>Randevuyu Düzenle</h3>

                        <p>
                            Randevu ${randevuId} bilgilerini güncelleyin.
                        </p>
                    </div>

                    <button
                        type="button"
                        class="yenile-butonu"
                        onclick="randevulariGetir()">
                        Listeye Dön
                    </button>
                </div>

                <form
                    id="randevuGuncellemeFormu"
                    class="randevu-formu">

                    <div class="form-grid">
                        <div class="form-grubu">
                            <label for="duzenleDoktorId">
                                Doktor
                            </label>

                            <select
                                id="duzenleDoktorId"
                                required>
                                ${doktorSecenekleri}
                            </select>
                        </div>

                        <div class="form-grubu">
                            <label for="duzenleHastaId">
                                Hasta
                            </label>

                            <select
                                id="duzenleHastaId"
                                required>
                                ${hastaSecenekleri}
                            </select>
                        </div>

                        <div class="form-grubu">
                            <label for="duzenleBaslangic">
                                Başlangıç zamanı
                            </label>

                            <input
                                type="datetime-local"
                                id="duzenleBaslangic"
                                value="${tarihSaatInputDegeri(
            randevu.baslangicZamani
        )}"
                                required>
                        </div>

                        <div class="form-grubu">
                            <label for="duzenleBitis">
                                Bitiş zamanı
                            </label>

                            <input
                                type="datetime-local"
                                id="duzenleBitis"
                                value="${tarihSaatInputDegeri(
            randevu.bitisZamani
        )}"
                                required>
                        </div>
                    </div>

                    <div class="form-islemleri">
                        <button
                            type="submit"
                            class="yenile-butonu">
                            Değişiklikleri Kaydet
                        </button>
                    </div>

                    <div
                        id="randevuGuncellemeMesaji"
                        class="mesaj gizli">
                    </div>
                </form>
            </div>
        `;

        document
            .getElementById("randevuGuncellemeFormu")
            .addEventListener("submit", (event) => {
                randevuGuncelle(event, randevuId);
            });
    }
    catch (hata) {
        icerikAlani.innerHTML = `
            <div class="hata-kutusu">
                ${htmlGuvenli(hata.message)}
            </div>
        `;
    }
}


async function randevuGuncelle(event, randevuId) {
    event.preventDefault();

    const doktorId = Number(
        document.getElementById("duzenleDoktorId").value
    );

    const hastaId = Number(
        document.getElementById("duzenleHastaId").value
    );

    const baslangicZamani = new Date(
        document.getElementById("duzenleBaslangic").value
    );

    const bitisZamani = new Date(
        document.getElementById("duzenleBitis").value
    );

    const mesajAlani = document.getElementById(
        "randevuGuncellemeMesaji"
    );

    if (bitisZamani <= baslangicZamani) {
        mesajAlani.textContent =
            "Bitiş zamanı başlangıçtan sonra olmalıdır.";

        mesajAlani.className = "mesaj hata";
        return;
    }

    const buton = event.currentTarget.querySelector(
        "button[type='submit']"
    );

    buton.disabled = true;
    buton.textContent = "Güncelleniyor...";

    try {
        const cevap = await apiIstegi(
            `/api/Randevu/${randevuId}`,
            {
                method: "PUT",

                body: JSON.stringify({
                    doktorId: doktorId,
                    hastaId: hastaId,
                    baslangicZamani:
                        baslangicZamani.toISOString(),
                    bitisZamani:
                        bitisZamani.toISOString()
                })
            }
        );

        if (!cevap.ok) {
            const hataMetni = await cevap.text();

            if (
                hataMetni.includes(
                    "Doktorun seçilen zaman aralığında başka bir randevusu var"
                )
            ) {
                throw new Error(
                    "Doktorun seçilen zaman aralığında başka bir randevusu var."
                );
            }

            throw new Error(
                "Randevu güncellenirken bir hata oluştu."
            );
        }

        mesajAlani.textContent =
            "Randevu başarıyla güncellendi.";

        mesajAlani.className = "mesaj basarili";

        setTimeout(() => {
            randevulariGetir();
        }, 700);
    }
    catch (hata) {
        mesajAlani.textContent = hata.message;
        mesajAlani.className = "mesaj hata";
    }
    finally {
        buton.disabled = false;
        buton.textContent = "Değişiklikleri Kaydet";
    }
}


function tarihSaatInputDegeri(tarihDegeri) {
    const tarih = new Date(tarihDegeri);

    const saatFarki =
        tarih.getTimezoneOffset() * 60000;

    return new Date(tarih.getTime() - saatFarki)
        .toISOString()
        .slice(0, 16);
}
async function randevuFiltreFormunuGoster() {
    icerikAlani.innerHTML = `
        <div class="yukleniyor">
            Arama formu hazırlanıyor...
        </div>
    `;

    try {
        const cevap = await apiIstegi(
            "/api/Doctors" +
            "?SayfaNo=1" +
            "&SayfaBoyutu=100" +
            "&AktifMi=true"
        );

        if (!cevap.ok) {
            throw new Error(
                "Doktor listesi alınamadı."
            );
        }

        const veri = await cevap.json();
        const doktorlar = veri.kayitlar ?? veri;

        const doktorSecenekleri = doktorlar
            .map((doktor) => {
                return `
                    <option value="${doktor.id}">
                        ${htmlGuvenli(
                    doktor.doktorAd + " " +
                    doktor.doktorSoyad
                )}
                    </option>
                `;
            })
            .join("");

        icerikAlani.innerHTML = `
            <div class="icerik-karti">
                <div class="icerik-basligi">
                    <div>
                        <h3>Randevu Ara</h3>

                        <p>
                            Doktor ve tarih aralığına göre
                            randevuları görüntüleyin.
                        </p>
                    </div>

                    <button
                        type="button"
                        class="yenile-butonu"
                        onclick="randevulariGetir()">
                        Listeye Dön
                    </button>
                </div>

                <form
                    id="randevuFiltreFormu"
                    class="randevu-formu">

                    <div class="form-grid">
                        <div class="form-grubu">
                            <label for="filtreDoktorId">
                                Doktor
                            </label>

                            <select
                                id="filtreDoktorId"
                                required>

                                <option value="">
                                    Doktor seçiniz
                                </option>

                                ${doktorSecenekleri}
                            </select>
                        </div>

                        <div class="form-grubu">
                            <label for="filtreBaslangic">
                                Başlangıç tarihi
                            </label>

                            <input
                                type="date"
                                id="filtreBaslangic"
                                required>
                        </div>

                        <div class="form-grubu">
                            <label for="filtreBitis">
                                Bitiş tarihi
                            </label>

                            <input
                                type="date"
                                id="filtreBitis"
                                required>
                        </div>
                    </div>

                    <div class="form-islemleri">
                        <button
                            type="submit"
                            class="yenile-butonu">
                            Randevuları Göster
                        </button>
                    </div>
                </form>

                <div id="randevuFiltreSonuclari"></div>
            </div>
        `;

        document
            .getElementById("randevuFiltreFormu")
            .addEventListener(
                "submit",
                randevulariFiltrele
            );
    }
    catch (hata) {
        icerikAlani.innerHTML = `
            <div class="hata-kutusu">
                ${htmlGuvenli(hata.message)}
            </div>
        `;
    }
}


async function randevulariFiltrele(event) {
    event.preventDefault();

    const doktorId =
        document.getElementById(
            "filtreDoktorId"
        ).value;

    const baslangicDegeri =
        document.getElementById(
            "filtreBaslangic"
        ).value;

    const bitisDegeri =
        document.getElementById(
            "filtreBitis"
        ).value;

    const sonucAlani =
        document.getElementById(
            "randevuFiltreSonuclari"
        );

    if (bitisDegeri < baslangicDegeri) {
        sonucAlani.innerHTML = `
            <div class="hata-kutusu">
                Bitiş tarihi başlangıç tarihinden
                önce olamaz.
            </div>
        `;

        return;
    }

    sonucAlani.innerHTML = `
        <div class="yukleniyor">
            Randevular aranıyor...
        </div>
    `;

    try {
        const baslangicTarihi =
            new Date(
                `${baslangicDegeri}T00:00:00`
            ).toISOString();

        const bitisTarihi =
            new Date(
                `${bitisDegeri}T23:59:59`
            ).toISOString();

        const adres =
            "/api/Randevu" +
            "?SayfaNo=1" +
            "&SayfaBoyutu=100" +
            `&DoktorId=${doktorId}` +
            `&BaslangicTarihi=${encodeURIComponent(
                baslangicTarihi
            )}` +
            `&BitisTarihi=${encodeURIComponent(
                bitisTarihi
            )}`;

        const cevap = await apiIstegi(adres);

        if (!cevap.ok) {
            throw new Error(
                "Randevular alınamadı."
            );
        }

        const veri = await cevap.json();
        const randevular =
            veri.kayitlar ?? veri;

        if (!randevular.length) {
            sonucAlani.innerHTML = `
                <div class="bos-kayit">
                    Seçilen kriterlere uygun
                    randevu bulunamadı.
                </div>
            `;

            return;
        }

        const satirlar = randevular
            .map((randevu) => {
                return `
                    <tr>
                        <td>
                            ${htmlGuvenli(
                    randevu.doktorAdiSoyadi
                )}
                        </td>

                        <td>
                            ${htmlGuvenli(
                    randevu.hastaAdiSoyadi
                )}
                        </td>

                        <td>
                            ${tarihYaz(
                    randevu.baslangicZamani
                )}
                        </td>

                        <td>
                            ${saatYaz(
                    randevu.baslangicZamani
                )}
                        </td>

                        <td>
                            ${saatYaz(
                    randevu.bitisZamani
                )}
                        </td>

                        <td>
                            <span class="durum-etiketi">
                                ${htmlGuvenli(
                    randevu.durumAdi
                )}
                            </span>
                        </td>
                    </tr>
                `;
            })
            .join("");

        sonucAlani.innerHTML = `
            <div class="filtre-sonuc-basligi">
                <h3>Arama sonuçları</h3>

                <span>
                    ${randevular.length} randevu bulundu
                </span>
            </div>

            <div class="tablo-kapsayici">
                <table class="veri-tablosu">
                    <thead>
                        <tr>
                            <th>Doktor</th>
                            <th>Hasta</th>
                            <th>Tarih</th>
                            <th>Başlangıç</th>
                            <th>Bitiş</th>
                            <th>Durum</th>
                        </tr>
                    </thead>

                    <tbody>
                        ${satirlar}
                    </tbody>
                </table>
            </div>
        `;
    }
    catch (hata) {
        sonucAlani.innerHTML = `
            <div class="hata-kutusu">
                ${htmlGuvenli(hata.message)}
            </div>
        `;
    }
} window.randevuTamamla = async function (randevuId) {
    if (!confirm("Randevu tamamlandı olarak işaretlensin mi?")) {
        return;
    }

    await randevuDurumGuncelle(
        randevuId,
        1,
        null,
        "Randevu tamamlandı."
    );
};


window.randevuHastaGelmedi = async function (randevuId) {
    if (!confirm("Hasta randevuya gelmedi olarak işaretlensin mi?")) {
        return;
    }

    await randevuDurumGuncelle(
        randevuId,
        3,
        null,
        "Hasta gelmedi olarak işaretlendi."
    );
};


window.randevuDoktorIptalEt = async function (randevuId) {
    const iptalNedeni = prompt(
        "Randevunun iptal nedenini yazınız:"
    );

    if (iptalNedeni === null) {
        return;
    }

    if (!iptalNedeni.trim()) {
        alert("İptal nedeni boş bırakılamaz.");
        return;
    }

    await randevuDurumGuncelle(
        randevuId,
        0,
        iptalNedeni.trim(),
        "Randevu iptal edildi."
    );
};


async function randevuDurumGuncelle(
    randevuId,
    durum,
    iptalNedeni,
    basariMesaji
) {
    try {
        const cevap = await apiIstegi(
            `/api/Randevu/${randevuId}/durum`,
            {
                method: "PATCH",
                body: JSON.stringify({
                    durum: durum,
                    iptalNedeni: iptalNedeni
                })
            }
        );

        if (!cevap.ok) {
            const hataMetni = await cevap.text();

            let hataMesaji =
                "Randevu durumu güncellenemedi.";

            if (hataMetni) {
                try {
                    const hataVerisi = JSON.parse(hataMetni);

                    hataMesaji =
                        hataVerisi.detail ??
                        hataVerisi.mesaj ??
                        hataVerisi.title ??
                        hataMesaji;
                }
                catch {
                    hataMesaji = hataMetni
                        .split(/\r?\n|\s+at\s+/)[0]
                        .replace(/^System\.[^:]+:\s*/, "");
                }
            }

            throw new Error(hataMesaji);
        }

        alert(basariMesaji);
        await randevulariGetir();
    }
    catch (hata) {
        alert(hata.message);
    }
}
function tarihYaz(tarihDegeri) {
    if (!tarihDegeri) {
        return "-";
    }

    return new Date(tarihDegeri)
        .toLocaleDateString("tr-TR");
}


function saatYaz(tarihDegeri) {
    if (!tarihDegeri) {
        return "-";
    }

    return new Date(tarihDegeri)
        .toLocaleTimeString(
            "tr-TR",
            {
                hour: "2-digit",
                minute: "2-digit"
            }
        );
}


async function muayeneleriGetir() {
    icerikAlani.innerHTML = `
        <div class="yukleniyor">
            Muayeneler yükleniyor...
        </div>
    `;

    try {
        const cevap = await apiIstegi(
            "/api/Muayene" +
            "?SayfaNo=1" +
            "&SayfaBoyutu=100"
        );

        if (!cevap.ok) {
            if (cevap.status === 403) {
                throw new Error(
                    "Muayene kayıtlarını görüntüleme " +
                    "yetkiniz bulunmuyor."
                );
            }

            throw new Error(
                "Muayeneler alınamadı. " +
                `Hata kodu: ${cevap.status}`
            );
        }

        const veri = await cevap.json();

        const muayeneler =
            veri.kayitlar ?? veri;

        if (!muayeneler || muayeneler.length === 0) {
            icerikAlani.innerHTML = `
                <div class="bos-kayit">
                    Henüz muayene kaydınız bulunmuyor.
                </div>
            `;

            return;
        }

        const satirlar = muayeneler
            .map((muayene) => {
                return `
                    <tr>
                        <td>
                            ${htmlGuvenli(
                    muayene.doktorAdiSoyadi
                )}
                        </td>

                        <td>
                            ${htmlGuvenli(
                    muayene.hastaAdiSoyadi
                )}
                        </td>

                        <td>
                            ${tarihYaz(
                    muayene.muayeneTarihi
                )}
                        </td>

                        <td class="metin-hucresi">
                            ${htmlGuvenli(
                    muayene.hastaSikayeti
                )}
                        </td>

                        <td class="metin-hucresi">
                            ${htmlGuvenli(
                    muayene.doktorDegerlendirmesi
                )}
                        </td>
                        ${(rol ?? "").trim().toLowerCase() === "doktor"
                        ? `
        <td>
            <button
                type="button"
                class="yenile-butonu"
                onclick="muayeneTeshisleriniGoster(${muayene.id})">
                Teşhisler
            </button>
        </td>
    `
                        : ""
}

                    </tr>
                `;
            })
            .join("");

        icerikAlani.innerHTML = `
            <div class="icerik-karti">
                <div class="icerik-basligi">
                    <div>
                        <h3>Muayeneler</h3>
                        <p>Muayene geçmişiniz</p>
                    </div>
<div class="baslik-butonlari">
    ${(rol ?? "").trim().toLowerCase() === "doktor"
                ? `
            <button
                type="button"
                class="yenile-butonu"
                onclick="muayeneOlusturmaFormunuGoster()">
                Yeni Muayene
            </button>
        `
                : ""
    }

    <button
        type="button"
        class="yenile-butonu"
        onclick="muayeneleriGetir()">
        Yenile
    </button>
</div>
                </div>

                <div class="tablo-kapsayici">
                    <table class="veri-tablosu">
                        <thead>
                            <tr>
                                <th>Doktor</th>
                                <th>Hasta</th>
                                <th>Tarih</th>
                                <th>Şikâyet</th>
                                <th>Değerlendirme</th>
                                ${(rol ?? "").trim().toLowerCase() === "doktor"
                ? "<th>İşlem</th>"
                : ""
}
                            </tr>
                        </thead>

                        <tbody>
                            ${satirlar}
                        </tbody>
                    </table>
                </div>
            </div>
        `;
    }
    catch (hata) {
        icerikAlani.innerHTML = `
            <div class="hata-kutusu">
                ${htmlGuvenli(hata.message)}
            </div>
        `;
    }
}

window.muayeneOlusturmaFormunuGoster =
    async function () {
        icerikAlani.innerHTML = `
            <div class="yukleniyor">
                Muayene formu hazırlanıyor...
            </div>
        `;

        try {
            const [randevuCevabi, muayeneCevabi] =
                await Promise.all([
                    apiIstegi(
                        "/api/Randevu" +
                        "?SayfaNo=1" +
                        "&SayfaBoyutu=100" +
                        "&Durum=1"
                    ),

                    apiIstegi(
                        "/api/Muayene" +
                        "?SayfaNo=1" +
                        "&SayfaBoyutu=100"
                    )
                ]);

            if (!randevuCevabi.ok) {
                throw new Error(
                    "Tamamlanan randevular alınamadı."
                );
            }

            if (!muayeneCevabi.ok) {
                throw new Error(
                    "Muayene kayıtları alınamadı."
                );
            }

            const randevuVerisi =
                await randevuCevabi.json();

            const muayeneVerisi =
                await muayeneCevabi.json();

            const randevular =
                randevuVerisi.kayitlar ??
                randevuVerisi;

            const muayeneler =
                muayeneVerisi.kayitlar ??
                muayeneVerisi;

            const kullanilanRandevuIdleri =
                new Set(
                    muayeneler.map(
                        (muayene) =>
                            muayene.randevuId
                    )
                );

            const uygunRandevular =
                randevular.filter(
                    (randevu) =>
                        !kullanilanRandevuIdleri.has(
                            randevu.id
                        )
                );

            if (uygunRandevular.length === 0) {
                icerikAlani.innerHTML = `
                    <div class="icerik-karti">
                        <div class="icerik-basligi">
                            <div>
                                <h3>Yeni Muayene</h3>

                                <p>
                                    Muayene oluşturulabilecek
                                    tamamlanmış randevu bulunamadı.
                                </p>
                            </div>

                            <button
                                type="button"
                                class="yenile-butonu"
                                onclick="muayeneleriGetir()">
                                Listeye Dön
                            </button>
                        </div>
                    </div>
                `;

                return;
            }

            const randevuSecenekleri =
                uygunRandevular
                    .map((randevu) => {
                        return `
                            <option value="${randevu.id}">
                                ${htmlGuvenli(
                            randevu.hastaAdiSoyadi
                        )} -
                                ${tarihYaz(
                            randevu.baslangicZamani
                        )}
                                ${saatYaz(
                            randevu.baslangicZamani
                        )}
                            </option>
                        `;
                    })
                    .join("");

            icerikAlani.innerHTML = `
                <div class="icerik-karti">
                    <div class="icerik-basligi">
                        <div>
                            <h3>Yeni Muayene</h3>

                            <p>
                                Tamamlanan randevu için
                                muayene bilgilerini giriniz.
                            </p>
                        </div>

                        <button
                            type="button"
                            class="yenile-butonu"
                            onclick="muayeneleriGetir()">
                            Listeye Dön
                        </button>
                    </div>

                    <form
                        id="muayeneOlusturmaFormu"
                        class="randevu-formu">

                        <div class="form-grid">
                            <div class="form-grubu">
                                <label for="muayeneRandevuId">
                                    Randevu
                                </label>

                                <select
                                    id="muayeneRandevuId"
                                    required>

                                    <option value="">
                                        Randevu seçiniz
                                    </option>

                                    ${randevuSecenekleri}
                                </select>
                            </div>

                            <div class="form-grubu">
                                <label for="muayeneTarihi">
                                    Muayene tarihi
                                </label>

                                <input
                                    type="datetime-local"
                                    id="muayeneTarihi"
                                    required>
                            </div>

                            <div class="form-grubu">
                                <label for="hastaSikayeti">
                                    Hasta şikâyeti
                                </label>

                                <textarea
                                    id="hastaSikayeti"
                                    rows="4"
                                    required>
                                </textarea>
                            </div>

                            <div class="form-grubu">
                                <label for="doktorDegerlendirmesi">
                                    Doktor değerlendirmesi
                                </label>

                                <textarea
                                    id="doktorDegerlendirmesi"
                                    rows="4"
                                    required>
                                </textarea>
                            </div>

                            <div class="form-grubu">
                                <label for="doktorNotlari">
                                    Doktor notları
                                </label>

                                <textarea
                                    id="doktorNotlari"
                                    rows="4">
                                </textarea>
                            </div>
                        </div>

                        <div class="form-islemleri">
                            <button
                                type="submit"
                                class="yenile-butonu">
                                Muayeneyi Kaydet
                            </button>
                        </div>

                        <div
                            id="muayeneMesajAlani"
                            class="mesaj gizli">
                        </div>
                    </form>
                </div>
            `;

            document
                .getElementById(
                    "muayeneOlusturmaFormu"
                )
                .addEventListener(
                    "submit",
                    muayeneOlustur
                );
        }
        catch (hata) {
            icerikAlani.innerHTML = `
                <div class="hata-kutusu">
                    ${htmlGuvenli(hata.message)}
                </div>
            `;
        }
    };


async function muayeneOlustur(event) {
    event.preventDefault();

    const randevuId = Number(
        document
            .getElementById("muayeneRandevuId")
            .value
    );

    const muayeneTarihiDegeri =
        document
            .getElementById("muayeneTarihi")
            .value;

    const hastaSikayeti =
        document
            .getElementById("hastaSikayeti")
            .value
            .trim();

    const doktorDegerlendirmesi =
        document
            .getElementById(
                "doktorDegerlendirmesi"
            )
            .value
            .trim();

    const doktorNotlari =
        document
            .getElementById("doktorNotlari")
            .value
            .trim();

    const mesajAlani =
        document.getElementById(
            "muayeneMesajAlani"
        );

    const buton =
        event.currentTarget.querySelector(
            "button[type='submit']"
        );

    buton.disabled = true;
    buton.textContent = "Kaydediliyor...";

    try {
        const cevap = await apiIstegi(
            "/api/Muayene",
            {
                method: "POST",

                body: JSON.stringify({
                    randevuId: randevuId,
                    hastaSikayeti: hastaSikayeti,

                    doktorDegerlendirmesi:
                        doktorDegerlendirmesi,

                    doktorNotlari:
                        doktorNotlari || null,

                    muayeneTarihi:
                        new Date(
                            muayeneTarihiDegeri
                        ).toISOString()
                })
            }
        );

        if (!cevap.ok) {
            const hataMetni = await cevap.text();

            let hataMesaji =
                "Muayene oluşturulamadı.";

            if (hataMetni) {
                try {
                    const hataVerisi =
                        JSON.parse(hataMetni);

                    hataMesaji =
                        hataVerisi.detail ??
                        hataVerisi.mesaj ??
                        hataVerisi.title ??
                        hataMesaji;
                }
                catch {
                    hataMesaji = hataMetni
                        .split(/\r?\n|\s+at\s+/)[0]
                        .replace(
                            /^System\.[^:]+:\s*/,
                            ""
                        );
                }
            }

            throw new Error(hataMesaji);
        }

        mesajAlani.textContent =
            "Muayene başarıyla oluşturuldu.";

        mesajAlani.className =
            "mesaj basarili";

        setTimeout(() => {
            muayeneleriGetir();
        }, 700);
    }
    catch (hata) {
        mesajAlani.textContent =
            hata.message;

        mesajAlani.className =
            "mesaj hata";
    }
    finally {
        buton.disabled = false;
        buton.textContent =
            "Muayeneyi Kaydet";
    }
}
window.muayeneTeshisleriniGoster =
    async function (muayeneId) {
        icerikAlani.innerHTML = `
            <div class="yukleniyor">
                Teşhisler yükleniyor...
            </div>
        `;

        try {
            const [kayitCevabi, teshisCevabi] =
                await Promise.all([
                    apiIstegi(
                        `/api/MuayeneTeshisi/muayene/${muayeneId}`
                    ),
                    apiIstegi("/api/Teshis")
                ]);

            if (!kayitCevabi.ok) {
                throw new Error(
                    "Muayeneye ait teşhisler alınamadı."
                );
            }

            if (!teshisCevabi.ok) {
                throw new Error(
                    "Teşhis listesi alınamadı."
                );
            }

            const kayitVerisi =
                await kayitCevabi.json();

            const teshisVerisi =
                await teshisCevabi.json();

            const mevcutKayitlar =
                kayitVerisi.kayitlar ??
                kayitVerisi;

            const tumTeshisler =
                teshisVerisi.kayitlar ??
                teshisVerisi;

            const kullanilanTeshisIdleri =
                new Set(
                    mevcutKayitlar.map(
                        (kayit) => kayit.teshisId
                    )
                );

            const eklenebilirTeshisler =
                tumTeshisler.filter(
                    (teshis) =>
                        !kullanilanTeshisIdleri.has(
                            teshis.id
                        )
                );

            const satirlar =
                mevcutKayitlar.length > 0
                    ? mevcutKayitlar
                        .map((kayit) => `
                            <tr>
                                <td>
                                    ${htmlGuvenli(
                            kayit.teshisKodu || "-"
                        )}
                                </td>

                                <td>
                                    ${htmlGuvenli(
                            kayit.teshisAdi
                        )}
                                </td>

                                <td class="metin-hucresi">
                                    ${htmlGuvenli(
                            kayit.doktorNotu || "-"
                        )}
                                </td>
                            </tr>
                        `)
                        .join("")
                    : `
                        <tr>
                            <td colspan="3">
                                Bu muayeneye henüz
                                teşhis eklenmemiş.
                            </td>
                        </tr>
                    `;

            const secenekler =
                eklenebilirTeshisler
                    .map((teshis) => `
                        <option value="${teshis.id}">
                            ${htmlGuvenli(
                        teshis.teshisKodu || "-"
                    )}
                            -
                            ${htmlGuvenli(
                        teshis.teshisAdi
                    )}
                        </option>
                    `)
                    .join("");

            const formAlani =
                eklenebilirTeshisler.length > 0
                    ? `
                        <form
                            id="muayeneTeshisiFormu"
                            class="randevu-formu">

                            <div class="form-grid">
                                <div class="form-grubu">
                                    <label for="teshisSecimi">
                                        Teşhis
                                    </label>

                                    <select
                                        id="teshisSecimi"
                                        required>

                                        <option value="">
                                            Teşhis seçiniz
                                        </option>

                                        ${secenekler}
                                    </select>
                                </div>

                                <div class="form-grubu">
                                    <label for="teshisDoktorNotu">
                                        Doktor notu
                                    </label>

                                    <textarea
                                        id="teshisDoktorNotu"
                                        rows="3"></textarea>
                                </div>
                            </div>

                            <div class="form-islemleri">
                                <button
                                    type="submit"
                                    class="yenile-butonu">
                                    Teşhisi Ekle
                                </button>
                            </div>

                            <div
                                id="teshisMesajAlani"
                                class="mesaj gizli">
                            </div>
                        </form>
                    `
                    : `
                        <div class="bos-kayit">
                            Eklenebilecek başka teşhis bulunmuyor.
                        </div>
                    `;

            icerikAlani.innerHTML = `
                <div class="icerik-karti">
                    <div class="icerik-basligi">
                        <div>
                            <h3>Muayene Teşhisleri</h3>

                            <p>
                                Muayene numarası:
                                ${muayeneId}
                            </p>
                        </div>

                        <button
                            type="button"
                            class="yenile-butonu"
                            onclick="muayeneleriGetir()">
                            Muayenelere Dön
                        </button>
                    </div>

                    <div class="tablo-kapsayici">
                        <table class="veri-tablosu">
                            <thead>
                                <tr>
                                    <th>Teşhis kodu</th>
                                    <th>Teşhis adı</th>
                                    <th>Doktor notu</th>
                                </tr>
                            </thead>

                            <tbody>
                                ${satirlar}
                            </tbody>
                        </table>
                    </div>

                    ${formAlani}
                </div>
            `;

            const form =
                document.getElementById(
                    "muayeneTeshisiFormu"
                );

            if (form) {
                form.addEventListener(
                    "submit",
                    (event) => {
                        muayeneTeshisiEkle(
                            event,
                            muayeneId
                        );
                    }
                );
            }
        }
        catch (hata) {
            icerikAlani.innerHTML = `
                <div class="hata-kutusu">
                    ${htmlGuvenli(hata.message)}
                </div>
            `;
        }
    };


async function muayeneTeshisiEkle(
    event,
    muayeneId
) {
    event.preventDefault();

    const teshisId = Number(
        document
            .getElementById("teshisSecimi")
            .value
    );

    const doktorNotu =
        document
            .getElementById("teshisDoktorNotu")
            .value
            .trim();

    const mesajAlani =
        document.getElementById(
            "teshisMesajAlani"
        );

    const buton =
        event.currentTarget.querySelector(
            "button[type='submit']"
        );

    buton.disabled = true;
    buton.textContent = "Ekleniyor...";

    try {
        const cevap = await apiIstegi(
            "/api/MuayeneTeshisi",
            {
                method: "POST",

                body: JSON.stringify({
                    muayeneId: muayeneId,
                    teshisId: teshisId,
                    doktorNotu:
                        doktorNotu || null
                })
            }
        );

        if (!cevap.ok) {
            const hataMetni =
                await cevap.text();

            let hataMesaji =
                "Teşhis eklenemedi.";

            if (hataMetni) {
                try {
                    const hataVerisi =
                        JSON.parse(hataMetni);

                    hataMesaji =
                        hataVerisi.detail ??
                        hataVerisi.mesaj ??
                        hataVerisi.title ??
                        hataMesaji;
                }
                catch {
                    hataMesaji = hataMetni
                        .split(/\r?\n|\s+at\s+/)[0]
                        .replace(
                            /^System\.[^:]+:\s*/,
                            ""
                        );
                }
            }

            throw new Error(hataMesaji);
        }

        alert("Teşhis başarıyla eklendi.");

        await window.muayeneTeshisleriniGoster(
            muayeneId
        );
    }
    catch (hata) {
        mesajAlani.textContent =
            hata.message;

        mesajAlani.className =
            "mesaj hata";
    }
    finally {
        buton.disabled = false;
        buton.textContent = "Teşhisi Ekle";
    }
}
async function receteleriGetir() {
    icerikAlani.innerHTML = `
        <div class="yukleniyor">
            Reçeteler yükleniyor...
        </div>
    `;

    try {
        const cevap = await apiIstegi(
            "/api/Recete" +
            "?SayfaNo=1" +
            "&SayfaBoyutu=100"
        );

        if (!cevap.ok) {
            if (cevap.status === 403) {
                throw new Error(
                    "Reçeteleri görüntüleme yetkiniz bulunmuyor."
                );
            }

            throw new Error(
                "Reçeteler alınamadı. " +
                `Hata kodu: ${cevap.status}`
            );
        }

        const veri = await cevap.json();

        const receteler =
            veri.kayitlar ?? veri;

        if (!receteler || receteler.length === 0) {
            icerikAlani.innerHTML = `
                <div class="bos-kayit">
                    Henüz reçete kaydınız bulunmuyor.
                </div>
            `;

            return;
        }

        const satirlar = receteler
            .map((recete) => {
                return `
                    <tr>
                        <td>
                            ${recete.id}
                        </td>

                        <td>
                            ${tarihYaz(
                    recete.receteTarihi
                )}
                        </td>

                        <td>
                            ${htmlGuvenli(
                    recete.doktorAdiSoyadi
                )}
                        </td>

                        <td>
                            ${htmlGuvenli(
                    recete.hastaAdiSoyadi
                )}
                        </td>

                        <td class="metin-hucresi">
                            ${htmlGuvenli(
                    recete.genelNotlar
                )}
                        </td>

                        <td>
                            <button
                                type="button"
                                class="yenile-butonu"
                                onclick="receteIceriginiGetir(
                                    ${recete.id}
                                )">
                                İlaçları Gör
                            </button>
                        </td>
                    </tr>
                `;
            })
            .join("");

        icerikAlani.innerHTML = `
            <div class="icerik-karti">
                <div class="icerik-basligi">
                    <div>
                        <h3>Reçeteler</h3>
                        <p>Reçete geçmişiniz</p>
                    </div>

                    <div class="baslik-butonlari">
    ${(rol ?? "").trim().toLowerCase() === "doktor"
                ? `
            <button
                type="button"
                class="yenile-butonu"
                onclick="receteOlusturmaFormunuGoster()">
                Yeni Reçete
            </button>
        `
                : ""
    }

    <button
        type="button"
        class="yenile-butonu"
        onclick="receteleriGetir()">
        Yenile
    </button>
</div>
                </div>

                <div class="tablo-kapsayici">
                    <table class="veri-tablosu">
                        <thead>
                            <tr>
                                <th>Reçete No</th>
                                <th>Tarih</th>
                                <th>Doktor</th>
                                <th>Hasta</th>
                                <th>Genel Not</th>
                                <th>İşlem</th>
                            </tr>
                        </thead>

                        <tbody>
                            ${satirlar}
                        </tbody>
                    </table>
                </div>
            </div>
        `;
    }
    catch (hata) {
        icerikAlani.innerHTML = `
            <div class="hata-kutusu">
                ${htmlGuvenli(hata.message)}
            </div>
        `;
    }
}
window.receteOlusturmaFormunuGoster =
    async function () {
        icerikAlani.innerHTML = `
            <div class="yukleniyor">
                Reçete formu hazırlanıyor...
            </div>
        `;

        try {
            const [muayeneCevabi, receteCevabi] =
                await Promise.all([
                    apiIstegi(
                        "/api/Muayene" +
                        "?SayfaNo=1" +
                        "&SayfaBoyutu=100"
                    ),

                    apiIstegi(
                        "/api/Recete" +
                        "?SayfaNo=1" +
                        "&SayfaBoyutu=100"
                    )
                ]);

            if (!muayeneCevabi.ok) {
                throw new Error(
                    "Muayene listesi alınamadı."
                );
            }

            if (!receteCevabi.ok) {
                throw new Error(
                    "Reçete listesi alınamadı."
                );
            }

            const muayeneVerisi =
                await muayeneCevabi.json();

            const receteVerisi =
                await receteCevabi.json();

            const muayeneler =
                muayeneVerisi.kayitlar ??
                muayeneVerisi;

            const receteler =
                receteVerisi.kayitlar ??
                receteVerisi;

            const kullanilanMuayeneIdleri =
                new Set(
                    receteler.map(
                        (recete) =>
                            recete.muayeneId
                    )
                );

            const uygunMuayeneler =
                muayeneler.filter(
                    (muayene) =>
                        !kullanilanMuayeneIdleri.has(
                            muayene.id
                        )
                );

            if (uygunMuayeneler.length === 0) {
                icerikAlani.innerHTML = `
                    <div class="icerik-karti">
                        <div class="icerik-basligi">
                            <div>
                                <h3>Yeni Reçete</h3>

                                <p>
                                    Reçete oluşturulabilecek
                                    muayene bulunamadı.
                                </p>
                            </div>

                            <button
                                type="button"
                                class="yenile-butonu"
                                onclick="receteleriGetir()">
                                Listeye Dön
                            </button>
                        </div>
                    </div>
                `;

                return;
            }

            const muayeneSecenekleri =
                uygunMuayeneler
                    .map((muayene) => `
                        <option value="${muayene.id}">
                            ${htmlGuvenli(
                        muayene.hastaAdiSoyadi
                    )}
                            -
                            ${tarihYaz(
                        muayene.muayeneTarihi
                    )}
                        </option>
                    `)
                    .join("");

            icerikAlani.innerHTML = `
                <div class="icerik-karti">
                    <div class="icerik-basligi">
                        <div>
                            <h3>Yeni Reçete</h3>

                            <p>
                                Muayene seçerek reçete
                                oluşturunuz.
                            </p>
                        </div>

                        <button
                            type="button"
                            class="yenile-butonu"
                            onclick="receteleriGetir()">
                            Listeye Dön
                        </button>
                    </div>

                    <form
                        id="receteOlusturmaFormu"
                        class="randevu-formu">

                        <div class="form-grid">
                            <div class="form-grubu">
                                <label for="receteMuayeneId">
                                    Muayene
                                </label>

                                <select
                                    id="receteMuayeneId"
                                    required>

                                    <option value="">
                                        Muayene seçiniz
                                    </option>

                                    ${muayeneSecenekleri}
                                </select>
                            </div>

                            <div class="form-grubu">
                                <label for="receteGenelNotlar">
                                    Genel notlar
                                </label>

                                <textarea
                                    id="receteGenelNotlar"
                                    rows="4"></textarea>
                            </div>
                        </div>

                        <div class="form-islemleri">
                            <button
                                type="submit"
                                class="yenile-butonu">
                                Reçeteyi Kaydet
                            </button>
                        </div>

                        <div
                            id="receteMesajAlani"
                            class="mesaj gizli">
                        </div>
                    </form>
                </div>
            `;

            document
                .getElementById(
                    "receteOlusturmaFormu"
                )
                .addEventListener(
                    "submit",
                    receteOlustur
                );
        }
        catch (hata) {
            icerikAlani.innerHTML = `
                <div class="hata-kutusu">
                    ${htmlGuvenli(hata.message)}
                </div>
            `;
        }
    };


async function receteOlustur(event) {
    event.preventDefault();

    const muayeneId = Number(
        document
            .getElementById("receteMuayeneId")
            .value
    );

    const genelNotlar =
        document
            .getElementById("receteGenelNotlar")
            .value
            .trim();

    const mesajAlani =
        document.getElementById(
            "receteMesajAlani"
        );

    const buton =
        event.currentTarget.querySelector(
            "button[type='submit']"
        );

    buton.disabled = true;
    buton.textContent = "Kaydediliyor...";

    try {
        const cevap = await apiIstegi(
            "/api/Recete",
            {
                method: "POST",

                body: JSON.stringify({
                    muayeneId: muayeneId,
                    genelNotlar:
                        genelNotlar || null
                })
            }
        );

        if (!cevap.ok) {
            const hataMetni =
                await cevap.text();

            let hataMesaji =
                "Reçete oluşturulamadı.";

            if (hataMetni) {
                try {
                    const hataVerisi =
                        JSON.parse(hataMetni);

                    hataMesaji =
                        hataVerisi.detail ??
                        hataVerisi.mesaj ??
                        hataVerisi.title ??
                        hataMesaji;
                }
                catch {
                    hataMesaji = hataMetni
                        .split(/\r?\n|\s+at\s+/)[0]
                        .replace(
                            /^System\.[^:]+:\s*/,
                            ""
                        );
                }
            }

            throw new Error(hataMesaji);
        }

        mesajAlani.textContent =
            "Reçete başarıyla oluşturuldu.";

        mesajAlani.className =
            "mesaj basarili";

        setTimeout(() => {
            receteleriGetir();
        }, 700);
    }
    catch (hata) {
        mesajAlani.textContent =
            hata.message;

        mesajAlani.className =
            "mesaj hata";
    }
    finally {
        buton.disabled = false;
        buton.textContent =
            "Reçeteyi Kaydet";
    }
}

async function receteIceriginiGetir(receteId) {
    icerikAlani.innerHTML = `
        <div class="yukleniyor">
            Reçete içeriği yükleniyor...
        </div>
    `;

    try {
        const cevap = await apiIstegi(
            `/api/ReceteIcerik/recete/${receteId}`
        );

        if (!cevap.ok) {
            if (cevap.status === 403) {
                throw new Error(
                    "Bu reçetenin içeriğini görüntüleme " +
                    "yetkiniz bulunmuyor."
                );
            }

            if (cevap.status === 404) {
                throw new Error(
                    "Reçete bulunamadı."
                );
            }

            throw new Error(
                "Reçete içeriği alınamadı. " +
                `Hata kodu: ${cevap.status}`
            );
        }

        const icerikler = await cevap.json();

        if (!icerikler || icerikler.length === 0) {
            icerikAlani.innerHTML = `
        <div class="icerik-karti">
            <div class="icerik-basligi">
                <div>
                    <h3>Reçete ${receteId}</h3>

                    <p>
                        Bu reçeteye henüz ilaç
                        eklenmemiş.
                    </p>
                </div>

                <div class="baslik-butonlari">
                    ${rol === "Doktor"
                    ? `
                            <button
                                type="button"
                                class="yenile-butonu"
                                onclick="receteIcerikOlusturmaFormunuGoster(${receteId})">
                                İlaç Ekle
                            </button>
                        `
                    : ""
                }

                    <button
                        type="button"
                        class="yenile-butonu"
                        onclick="receteleriGetir()">
                        Geri Dön
                    </button>
                </div>
            </div>
        </div>
    `;

            return;
        }

        const satirlar = icerikler
            .map((icerik) => {
                return `
                    <tr>
                        <td>
                            ${htmlGuvenli(
                    icerik.ilacAdi
                )}
                        </td>

                        <td>
                            ${htmlGuvenli(
                    icerik.kullanimSuresi
                )}
                        </td>

                        <td>
                            ${icerik.miktar ?? "-"}
                        </td>

                        <td class="metin-hucresi">
                            ${htmlGuvenli(
                    icerik.kullanimTalimatlari
                )}
                        </td>
                    </tr>
                `;
            })
            .join("");

        icerikAlani.innerHTML = `
            <div class="icerik-karti">
                <div class="icerik-basligi">
                    <div>
                        <h3>Reçete ${receteId}</h3>

                        <p>
                            Reçetede bulunan ilaçlar
                        </p>
                    </div>

                    <div class="baslik-butonlari">
   ${rol === "Doktor"
                ? `
        <button
            type="button"
            class="yenile-butonu"
            onclick="receteIcerikOlusturmaFormunuGoster(${receteId})">
            İlaç Ekle
        </button>
    `
                : ""
}

    <button
        type="button"
        class="yenile-butonu"
        onclick="receteleriGetir()">
        Reçetelere Dön
    </button>
</div>
                </div>

                <div class="tablo-kapsayici">
                    <table class="veri-tablosu">
                        <thead>
                            <tr>
                                <th>İlaç</th>
                                <th>Kullanım Süresi</th>
                                <th>Miktar</th>
                                <th>Kullanım Talimatı</th>
                            </tr>
                        </thead>

                        <tbody>
                            ${satirlar}
                        </tbody>
                    </table>
                </div>
            </div>
        `;
    }
    catch (hata) {
        icerikAlani.innerHTML = `
            <div class="hata-kutusu">
                ${htmlGuvenli(hata.message)}

                <br><br>

                <button
                    type="button"
                    class="yenile-butonu"
                    onclick="receteleriGetir()">
                    Reçetelere Dön
                </button>
            </div>
        `;
    }
}

window.receteIcerikOlusturmaFormunuGoster =
    async function (receteId) {
        icerikAlani.innerHTML = `
            <div class="yukleniyor">
                İlaç ekleme formu hazırlanıyor...
            </div>
        `;

        try {
            const [ilacCevabi, icerikCevabi] =
                await Promise.all([
                    apiIstegi("/api/Ilac"),

                    apiIstegi(
                        `/api/ReceteIcerik/recete/${receteId}`
                    )
                ]);

            if (!ilacCevabi.ok) {
                throw new Error(
                    "İlaç listesi alınamadı."
                );
            }

            if (!icerikCevabi.ok) {
                throw new Error(
                    "Reçete içeriği alınamadı."
                );
            }

            const ilacVerisi =
                await ilacCevabi.json();

            const icerikVerisi =
                await icerikCevabi.json();

            const ilaclar =
                ilacVerisi.kayitlar ??
                ilacVerisi;

            const mevcutIcerikler =
                icerikVerisi.kayitlar ??
                icerikVerisi;

            const kullanilanIlacIdleri =
                new Set(
                    mevcutIcerikler.map(
                        (icerik) => icerik.ilacId
                    )
                );

            const eklenebilirIlaclar =
                ilaclar.filter(
                    (ilac) =>
                        ilac.aktifMi !== false &&
                        !kullanilanIlacIdleri.has(
                            ilac.id
                        )
                );

            if (eklenebilirIlaclar.length === 0) {
                icerikAlani.innerHTML = `
                    <div class="icerik-karti">
                        <div class="icerik-basligi">
                            <div>
                                <h3>Reçeteye İlaç Ekle</h3>

                                <p>
                                    Eklenebilecek başka aktif
                                    ilaç bulunmuyor.
                                </p>
                            </div>

                            <div class="baslik-butonlari">
    <button
        type="button"
        class="yenile-butonu"
        onclick="receteIceriginiGetir(${receteId})">
        Reçeteye Dön
    </button>
</div>
                        </div>
                    </div>
                `;

                return;
            }

            const ilacSecenekleri =
                eklenebilirIlaclar
                    .map((ilac) => `
                        <option value="${ilac.id}">
                            ${htmlGuvenli(ilac.ad)}
                        </option>
                    `)
                    .join("");

            icerikAlani.innerHTML = `
                <div class="icerik-karti">
                    <div class="icerik-basligi">
                        <div>
                            <h3>Reçeteye İlaç Ekle</h3>

                            <p>
                                Reçete numarası:
                                ${receteId}
                            </p>
                        </div>

                        <button
                            type="button"
                            class="yenile-butonu"
                            onclick="receteIceriginiGetir(${receteId})">
                            Reçeteye Dön
                        </button>
                    </div>

                    <form
                        id="receteIcerikOlusturmaFormu"
                        class="randevu-formu">

                        <div class="form-grid">
                            <div class="form-grubu">
                                <label for="receteIlacId">
                                    İlaç
                                </label>

                                <select
                                    id="receteIlacId"
                                    required>

                                    <option value="">
                                        İlaç seçiniz
                                    </option>

                                    ${ilacSecenekleri}
                                </select>
                            </div>

                            <div class="form-grubu">
                                <label for="kullanimSuresi">
                                    Kullanım süresi
                                </label>

                                <input
                                    type="text"
                                    id="kullanimSuresi"
                                    placeholder="Örneğin: 7 gün"
                                    maxlength="100"
                                    required>
                            </div>

                            <div class="form-grubu">
                                <label for="ilacMiktari">
                                    Miktar
                                </label>

                                <input
                                    type="number"
                                    id="ilacMiktari"
                                    min="1"
                                    value="1"
                                    required>
                            </div>

                            <div class="form-grubu">
                                <label for="kullanimTalimatlari">
                                    Kullanım talimatları
                                </label>

                                <textarea
                                    id="kullanimTalimatlari"
                                    rows="4"
                                    maxlength="500"
                                    required></textarea>
                            </div>
                        </div>

                        <div class="form-islemleri">
                            <button
                                type="submit"
                                class="yenile-butonu">
                                İlacı Kaydet
                            </button>
                        </div>

                        <div
                            id="receteIcerikMesajAlani"
                            class="mesaj gizli">
                        </div>
                    </form>
                </div>
            `;

            document
                .getElementById(
                    "receteIcerikOlusturmaFormu"
                )
                .addEventListener(
                    "submit",
                    (event) => {
                        receteyeIlacEkle(
                            event,
                            receteId
                        );
                    }
                );
        }
        catch (hata) {
            icerikAlani.innerHTML = `
                <div class="hata-kutusu">
                    ${htmlGuvenli(hata.message)}
                </div>
            `;
        }
    };


async function receteyeIlacEkle(
    event,
    receteId
) {
    event.preventDefault();

    const ilacId = Number(
        document
            .getElementById("receteIlacId")
            .value
    );

    const kullanimSuresi =
        document
            .getElementById("kullanimSuresi")
            .value
            .trim();

    const miktar = Number(
        document
            .getElementById("ilacMiktari")
            .value
    );

    const kullanimTalimatlari =
        document
            .getElementById(
                "kullanimTalimatlari"
            )
            .value
            .trim();

    const mesajAlani =
        document.getElementById(
            "receteIcerikMesajAlani"
        );

    const buton =
        event.currentTarget.querySelector(
            "button[type='submit']"
        );

    buton.disabled = true;
    buton.textContent = "Kaydediliyor...";

    try {
        const cevap = await apiIstegi(
            "/api/ReceteIcerik",
            {
                method: "POST",

                body: JSON.stringify({
                    receteId: receteId,
                    ilacId: ilacId,

                    kullanimTalimatlari:
                        kullanimTalimatlari,

                    kullanimSuresi:
                        kullanimSuresi,

                    miktar: miktar
                })
            }
        );

        if (!cevap.ok) {
            const hataMetni =
                await cevap.text();

            let hataMesaji =
                "İlaç reçeteye eklenemedi.";

            if (hataMetni) {
                try {
                    const hataVerisi =
                        JSON.parse(hataMetni);

                    hataMesaji =
                        hataVerisi.detail ??
                        hataVerisi.mesaj ??
                        hataVerisi.title ??
                        hataMesaji;
                }
                catch {
                    hataMesaji = hataMetni
                        .split(/\r?\n|\s+at\s+/)[0]
                        .replace(
                            /^System\.[^:]+:\s*/,
                            ""
                        );
                }
            }

            throw new Error(hataMesaji);
        }

        alert("İlaç reçeteye başarıyla eklendi.");

        await receteIceriginiGetir(
            receteId
        );
    }
    catch (hata) {
        mesajAlani.textContent =
            hata.message;

        mesajAlani.className =
            "mesaj hata";
    }
    finally {
        buton.disabled = false;
        buton.textContent = "İlacı Kaydet";
    }
}
function htmlGuvenli(deger) {
    const geciciAlan =
        document.createElement("div");

    geciciAlan.textContent =
        deger ?? "-";

    return geciciAlan.innerHTML;
}


// API isteklerini token ile gönderir.
async function apiIstegi(adres, secenekler = {}) {
    const guncelToken =
        sessionStorage.getItem("token");

    const headers = {
        ...secenekler.headers,
        "Authorization": `Bearer ${guncelToken}`
    };

    if (
        secenekler.body &&
        !headers["Content-Type"]
    ) {
        headers["Content-Type"] = "application/json";
    }

    const cevap = await fetch(adres, {
        ...secenekler,
        headers: headers
    });

    if (cevap.status === 401) {
        oturumuKapat();

        throw new Error(
            "Oturum süreniz doldu."
        );
    }

    return cevap;
}