# Görev ve İpucu Yönetimi

## QuestManager.cs

### Görev Yönetimi

* 10 farklı görevi yönetir.
* Her görevin başlığı, açıklaması ve tamamlanma adımları bulunur.
* Görevler sırayla açılır (bir görev tamamlanmadan diğeri açılmaz).

### UI Özellikleri

* **Q** tuşuna basınca görev paneli açılır/kapanır.
* Görev panelinde aşağıdaki bilgiler bulunur:

  * Görev başlığı
  * Görev açıklaması
  * İlerleme çubuğu
  * Tamamlanma yüzdesi

### Animasyon ve Ses

* Panel açılıp kapanırken animasyon.
* Panel açılırken/kapanırken ses efekti.
* Görev tamamlandığında ses efekti.

---

## HintManager.cs

### İpucu Yönetimi

* Her görev için 3 adet özel ipucu içerir.
* İpuçları, gizemli ve atmosferik bir dille yazılmıştır.

### UI Özellikleri

* **H** tuşuna basınca ipucu paneli açılır/kapanır.
* İpucu paneli şu bileşenleri içerir:

  * İpucu başlığı
  * İpucu açıklaması
  * Geri sayım sayacı
  * Arka plan karartması

### Zamanlama Sistemi

* İpuçları 1 dakika boyunca görünür kalır.
* İpucu kullanıldıktan sonra 5 dakika kilitli kalır.
* Geri sayım sayacı ile kalan süre gösterilir.

### Animasyon ve Ses

* Panel açılıp kapanırken animasyon.
* İpucu gösterilirken ses efekti.

---

## HintManager Entegrasyonu

* Her görev için ipucu gösterimi yapılır.
* İpuçları, **HintManager** üzerinden yönetilir.
* Oyuncu **H** tuşuna bastığında, mevcut görevin ilk ipucu gösterilir.
* 1 dakika sonra ipucu kapanır.
* 5 dakika sonra ipucu tekrar kullanılabilir hale gelir.
* Oyuncu **H** tuşuna tekrar bastığında, aynı görevin bir sonraki ipucu gösterilir.
* Tüm ipuçları gösterildikten sonra, görev tamamlanana kadar ipuçları sırayla tekrar gösterilir.
