const API_ADRESI = "/api/Yetkilendirme";

const girisSekmesi = document.getElementById("girisSekmesi");
const kayitSekmesi = document.getElementById("kayitSekmesi");
const girisFormu = document.getElementById("girisFormu");
const kayitFormu = document.getElementById("kayitFormu");
const mesajAlani = document.getElementById("mesajAlani");

girisSekmesi.addEventListener("click", girisFormunuGoster);
kayitSekmesi.addEventListener("click", kayitFormunuGoster);

function girisFormunuGoster() {
    girisSekmesi.classList.add("aktif");
    kayitSekmesi.classList.remove("aktif");

    girisFormu.classList.remove("gizli");
    kayitFormu.classList.add("gizli");

    mesajiTemizle();
}

function kayitFormunuGoster() {
    kayitSekmesi.classList.add("aktif");
    girisSekmesi.classList.remove("aktif");

    kayitFormu.classList.remove("gizli");
    girisFormu.classList.add("gizli");

    mesajiTemizle();
}

// Giriş işlemi
girisFormu.addEventListener("submit", async (event) => {
    event.preventDefault();

    const eposta =
        document.getElementById("girisEposta").value.trim();

    const parola =
        document.getElementById("girisParola").value;

    const buton =
        girisFormu.querySelector("button[type='submit']");

    butonuYukleniyorYap(buton, "Giriş yapılıyor...");

    try {
        const cevap = await fetch(`${API_ADRESI}/giris`, {
            method: "POST",

            headers: {
                "Content-Type": "application/json"
            },

            body: JSON.stringify({
                eposta: eposta,
                parola: parola
            })
        });

        const veri = await cevabiOku(cevap);

        if (!cevap.ok) {
            throw new Error(
                hataMesajiniGetir(
                    veri,
                    "E-posta veya parola hatalı."
                )
            );
        }

        sessionStorage.setItem("token", veri.token);
        sessionStorage.setItem("kullaniciId", veri.kullaniciId);
        sessionStorage.setItem("eposta", veri.eposta);
        sessionStorage.setItem("rol", veri.rol);

        sessionStorage.setItem(
            "tokenBitisTarihi",
            veri.tokenBitisTarihi
        );

        mesajGoster(
            "Giriş başarılı. Yönlendiriliyorsunuz...",
            "basarili"
        );

        setTimeout(() => {
            window.location.href = "/pages/panel.html";
        }, 800);
    }
    catch (hata) {
        mesajGoster(hata.message, "hata");
    }
    finally {
        butonuNormalYap(buton, "Giriş Yap");
    }
});

// Kayıt işlemi
kayitFormu.addEventListener("submit", async (event) => {
    event.preventDefault();

    const eposta =
        document.getElementById("kayitEposta").value.trim();

    const parola =
        document.getElementById("kayitParola").value;

    const parolaTekrar =
        document.getElementById("parolaTekrar").value;

    if (parola !== parolaTekrar) {
        mesajGoster(
            "Parolalar birbiriyle aynı olmalıdır.",
            "hata"
        );

        return;
    }

    const buton =
        kayitFormu.querySelector("button[type='submit']");

    butonuYukleniyorYap(
        buton,
        "Kayıt oluşturuluyor..."
    );

    try {
        const cevap = await fetch(`${API_ADRESI}/kayit`, {
            method: "POST",

            headers: {
                "Content-Type": "application/json"
            },

            body: JSON.stringify({
                eposta: eposta,
                parola: parola
            })
        });

        const veri = await cevabiOku(cevap);

        if (!cevap.ok) {
            throw new Error(
                hataMesajiniGetir(
                    veri,
                    "Kayıt işlemi gerçekleştirilemedi."
                )
            );
        }

        kayitFormu.reset();
        girisFormunuGoster();

        document.getElementById("girisEposta").value =
            eposta;

        mesajGoster(
            "Kaydınız oluşturuldu. Şimdi giriş yapabilirsiniz.",
            "basarili"
        );
    }
    catch (hata) {
        mesajGoster(hata.message, "hata");
    }
    finally {
        butonuNormalYap(buton, "Kayıt Ol");
    }
});

// API cevabını okur
async function cevabiOku(cevap) {
    const metin = await cevap.text();

    if (!metin) {
        return {};
    }

    try {
        return JSON.parse(metin);
    }
    catch {
        return {
            mesaj: metin
        };
    }
}

function hataMesajiniGetir(veri, varsayilanMesaj) {
    return veri.detail ||
        veri.mesaj ||
        veri.title ||
        varsayilanMesaj;
}

function mesajGoster(mesaj, tur) {
    mesajAlani.textContent = mesaj;
    mesajAlani.className = `mesaj ${tur}`;
}

function mesajiTemizle() {
    mesajAlani.textContent = "";
    mesajAlani.className = "mesaj gizli";
}

function butonuYukleniyorYap(buton, metin) {
    buton.disabled = true;
    buton.textContent = metin;
}

function butonuNormalYap(buton, metin) {
    buton.disabled = false;
    buton.textContent = metin;
}