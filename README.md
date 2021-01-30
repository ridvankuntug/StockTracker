# StockTracker

Ufak ve orta büyüklükte işletmelerin kullanabileceği stok takip yazılımı.

-Depo büyüklüğüne ve düzenine göre özelleştirilebilir veri tabanı (Daha sonra depo büyütülmesine uygun)
-İşlem geçmişini görüntülemek için kayıt tutar, bu veriler varsayılan olarak iki yıldan eski ise silinir.(Süre değiştirilebilir)
-Yeni ürün eklenebilir, düzenlenebilir ama kayıp veri oluşmaması için silinemez.
-Stok bilgisi ve hangi ürünün hangi bölmelerde olduğu görülebilir.
-Veriler Excel çıktısı olarak alınabilir.
-Yazdırmak için barkod çıktısı alınabilir.
-Ürün giriş çıkışları sırasında ürünün hangi bölmelerde olduğu görülerek yerleşim kolay yapılabilir.
-Veri tabanı yedeği alınabilir, veri tabanı silinebilir.(yedek almadan silmeye izin vermez), daha önce alınan bir yedek yüklenip onunla devam edilebilir.

Platform: Windows
Dil: C#

Kütüphaneler:
SQLite
ExcelLibrary https://code.google.com/archive/p/excellibrary
Entity Framework
Threading

Veri tabanı: SQlite3
