# Kullanım kılavuzu kaynağı

`../CvPortal-Kullanim-Kilavuzu.pdf` bu klasördeki dosyalardan üretilir.

| Dosya | İçerik |
|---|---|
| `kilavuz.html` | Kılavuzun metni ve A4 baskı tasarımı (kapak, bölümler, tablolar, notlar) |
| `img/` | Ekran görüntüleri (1280×800 tarayıcı görünümünden alınmıştır) |
| `build.ps1` | HTML'i Microsoft Edge ile PDF'e çevirir |

## PDF'i yeniden üretmek

Proje kökünden:

```bash
powershell -ExecutionPolicy Bypass -File docs\kilavuz-kaynak\build.ps1
```

Betik sonunda dosya yolunu, boyutu ve sayfa sayısını yazar.

## Güncelleme ipuçları

- **Metin değişikliği:** `kilavuz.html` içinde ilgili `<section>` bölümünü düzenleyin.
- **Ekran görüntüsü:** `img/` altındaki dosyayı aynı adla değiştirin. Uygulamayı veritabanına dokunmadan demo verisiyle açmak için
  `Database__Provider=InMemory` ortam değişkeniyle çalıştırabilirsiniz.
- **Sayfa düzeni:** Görseller sayfa ortasında bölünmez; bir bölüm sonraki sayfaya birkaç satır taşarsa o bölümdeki
  `<figure>` etiketine `class="sm"` (%68) veya `class="xs"` (%50) ekleyerek görseli küçültün.
