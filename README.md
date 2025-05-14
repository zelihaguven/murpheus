QuestManager.cs:
Görev Yönetimi:
10 farklı görevi yönetir
Her görevin başlığı, açıklaması ve tamamlanma adımları vardır
Görevler sırayla açılır (bir görev tamamlanmadan diğeri açılmaz)
UI Özellikleri:
Q tuşuna basınca görev paneli açılır/kapanır
Görev panelinde:
Görev başlığı
Görev açıklaması
İlerleme çubuğu
Tamamlanma yüzdesi
Animasyon ve Ses:
Panel açılıp kapanırken animasyon
Panel açılırken/kapanırken ses efekti
Görev tamamlandığında ses efekti

HintManager.cs:
İpucu Yönetimi:
Her görev için 3'er adet özel ipucu içerir
İpuçları daha gizemli ve atmosferik bir dille yazılmış
H tuşuna basınca ipucu paneli açılır/kapanır
Zamanlama Sistemi:
İpuçları 1 dakika boyunca görünür kalır
İpucu kullanıldıktan sonra 5 dakika kilitli kalır
Geri sayım sayacı ile kalan süre gösterilir
UI Özellikleri:
İpucu paneli
İpucu başlığı ve açıklaması
Geri sayım sayacı
Arka plan karartması
Animasyon ve Ses:
Panel açılıp kapanırken animasyon
İpucu gösterilirken ses efekti

HintManager Entegrasyonu:
Her görev için ipucu gösterimi
İpuçları HintManager üzerinden yönetilir
Oyuncu H tuşuna bastığında, mevcut görevin ilk ipucu gösterilecek
1 dakika sonra ipucu kapanacak
5 dakika sonra ipucu tekrar kullanılabilir olacak
Oyuncu tekrar H tuşuna bastığında, aynı görevin bir sonraki ipucu gösterilecek
Tüm ipuçları gösterildikten sonra, görev tamamlanana kadar ipuçları tekrar baştan gösterilecek
