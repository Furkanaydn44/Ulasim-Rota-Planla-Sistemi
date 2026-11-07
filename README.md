# 🚌 Şehir İçi Ulaşım Rota Planlama Sistemi

Bu proje, şehir içi ulaşım ağlarını (Otobüs, Tramvay ve Taksi) analiz ederek, kullanıcının belirlediği başlangıç ve hedef konumlarına göre en uygun rotayı hesaplayan, maliyet ve süre optimizasyonu yapan kapsamlı bir masaüstü uygulamasıdır.

Uygulama, **Nesne Yönelimli Programlama (OOP)** prensiplerine ve **SOLID** kurallarına (özellikle Open/Closed Principle) uygun olarak geliştirilmiş olup, genişletilebilir bir mimariye sahiptir.

## 📖 Proje Hakkında

Sistem, kullanıcıdan alınan koordinat verilerini, yolcu tipini ve ödeme yöntemini işleyerek en verimli ulaşım senaryosunu oluşturur. JSON tabanlı veri setleri üzerinden durak konumlarını ve tarife bilgilerini çeker, **Dijkstra** ve **Haversine** algoritmalarını kullanarak gerçekçi rota ve maliyet analizleri sunar.

## Proje Katılımcıları
<a href="https://github.com/numankhy28">
  <img src="https://avatars.githubusercontent.com/u/219421415?v=4" width="128" height="128" alt="numankhy28 GitHub Profile" />
</a>
<a href="https://github.com/Furkanaydn44">
  <img src="https://avatars.githubusercontent.com/u/174454978?v=4" width="128" height="128" alt="Furkanaydn44 GitHub Profile" />
</a>

## ✨ Temel Özellikler

* **Çok Modlu Ulaşım Desteği:**
    * **Otobüs:** Duraklar arası en kısa mesafe hesaplaması.
    * **Tramvay (Trambüs):** Tramvay hattı öncelikli rota oluşturma ve gerektiğinde otobüs aktarması (transfer) önerisi.
    * **Taksi:** Açılış ücreti ve KM başına maliyet hesaplaması.
* **Kişiselleştirilmiş Hesaplamalar:**
    * **Yolcu Tipleri:** Öğrenci, Yaşlı (65+) ve Genel kullanıcılar için dinamik indirim oranları.
    * **Ödeme Yöntemleri:** Nakit, Kredi Kartı ve Şehir Kartı seçeneklerine göre fiyatlandırma.
* **Akıllı Karar Destek Sistemi:**
    * Kullanıcı durağa yakınsa (2 km altı) yürüme, uzaksa taksi ile durağa gitme önerisi sunar.
    * Bakiye kontrolü yaparak yetersiz bakiye durumunda uyarı verir.
* **Görselleştirme:** Harita üzerinde başlangıç, bitiş ve durak noktalarının interaktif gösterimi.

## 🛠️ Teknik Altyapı ve Mimarisi

Bu proje **C#** dili ve **.NET Framework (Windows Forms)** kullanılarak geliştirilmiştir. Veri kalıcılığı ve konfigürasyon için **JSON** formatı tercih edilmiştir.

### 🏗️ OOP ve Yazılım Mimarisi
Proje, **Açık/Kapalı Prensibi (Open/Closed Principle)** gözetilerek tasarlanmıştır. Bu sayede sisteme yeni bir araç (örn. Scooter) veya yeni bir yolcu tipi eklendiğinde mevcut kodlarda değişiklik yapılmasına gerek kalmaz, sadece yeni sınıfların türetilmesi yeterlidir.

* **Soyut Sınıflar (Abstract Classes):**
    * `Arac`: Rota hesaplama (`CalculateRoute`) ve yazdırma işlemlerini yönetir.
    * `Yolcu`: İndirim mantığını yönetir.
    * `Odeme`: Ödeme tiplerine göre maliyet değişimlerini yönetir.
* **Sınıf Hiyerarşisi:** Tüm araçlar, yolcular ve ödeme yöntemleri bu soyut sınıflardan türetilerek (Inheritance) özelleştirilmiştir.

### 🧮 Kullanılan Algoritmalar
1.  **Dijkstra Algoritması:** Ulaşım ağındaki (Graph yapısı) duraklar arasındaki en kısa yolu bulmak için kullanılır.
2.  **Haversine Formülü:** Koordinatlar (enlem-boylam) arasındaki kuş uçuşu gerçek mesafeyi hesaplamak için kullanılır.

## 📂 Veri Yapısı (JSON)

Proje, dışarıdan beslenen bir `data.json` dosyası ile çalışır. Bu dosya şunları içerir:
* Şehir ve tarife bilgileri (Taksi açılış ücreti vb.)
* Durak listesi, koordinatları ve komşu durak bilgileri.
* Aktarma (Transfer) noktaları.

## 🚀 Kurulum ve Çalıştırma

Projeyi yerel makinenizde çalıştırmak için:

1.  Bu depoyu (repository) klonlayın.
2.  Projeyi **Visual Studio** veya uygun bir C# IDE'si ile açın.
3.  `bin/Debug` klasörü içerisinde gerekli `.json` veri dosyalarının ve harita görsellerinin (assets) bulunduğundan emin olun.
4.  Projeyi derleyin ve `Start` diyerek çalıştırın.
5.  Açılan arayüzde koordinatları ve tercihleri girerek rotaları analiz edin.

## 📄 Lisans

Bu proje açık kaynaklıdır ve eğitim/geliştirme amaçlı kullanıma uygundur.
