# SmartTaskAPI

SmartTaskAPI, .NET 8 Web API kullanılarak geliştirilmiş bir görev yönetim sistemidir. Kullanıcılar görevlerini oluşturabilir, filtreleyebilir, tamamlanma durumunu yönetebilir ve aktiviteleri kaydedebilir. Uygulama aynı zamanda yetkilendirme, rol yönetimi ve JWT tabanlı kimlik doğrulama destekler.

---

## 🚀 Özellikler

- JWT Authentication & Authorization
- Kullanıcı rolleri (Admin, User)
- CRUD işlemleri (TaskItem)
- Tarihe göre filtreleme ve tamamlanma durumu
- Aktif / silinmiş görev yönetimi (Soft delete yok ama hazır altyapı)
- Görev aktivitelerinin loglanması (veritabanına ve Elasticsearch'e)
- Swagger üzerinden erişilebilir REST API
- DTO kullanımı ile temiz API tasarımı

---

## 🤝 Kullanılan Teknolojiler

- .NET 8
- Entity Framework Core
- SQL Server
- JWT Authentication
- Role-based Authorization
- Elasticsearch (görev aktivitelerini loglamak için)
- Swagger (API dökümantasyonu ve test arayüzü)
- Minimal API configuration (builder.Services vs. app.MapControllers)
- Clean Architecture (Controller > DTO > Model > DbContext)

---

## 👤 Kullanıcı Rolleri

| Rol   | Açıklama                  |
|--------|-----------------------------|
| Admin  | Tüm kullanıcıları ve görevleri görür |
| User   | Yalnızca kendi görevlerini görür        |

---

## 🔧 Kurulum Talimatları

1. **Projeyi klonla**
```bash
git clone <repo-url>
cd SmartTaskAPI
```

2. **Veritabanı ayarları**
`appsettings.json` içinde `ConnectionStrings:DefaultConnection` alanını kendi SQL Server adresine göre düzenle.

3. **Migration uygula**
```bash
dotnet ef database update
```

4. **Projeyi başlat**
```bash
dotnet run
```

---

## 🔢 API Uç Noktaları (Endpoints)

Tüm uç noktalar Swagger arayüzü üzerinden test edilebilir: 
📎 `/swagger/index.html`

### Auth
| Metot | Endpoint      | Açıklama          |
|--------|---------------|---------------------|
| POST   | /api/auth/register | Yeni kullanıcı kaydı      |
| POST   | /api/auth/login    | JWT ile giriş al     |

### TaskItems
| Metot | Endpoint        | Açıklama                         |
|--------|-----------------|----------------------------------|
| GET    | /api/taskitems  | Tüm görevleri getir (rol bazlı)     |
| GET    | /api/taskitems/{id} | ID'ye göre görev getir           |
| POST   | /api/taskitems  | Yeni görev oluştur               |
| PUT    | /api/taskitems/{id} | Görevi güncelle                   |
| DELETE | /api/taskitems/{id} | Görevi sil (tam silme)             |

---

## 🖊️ Kod Yapısı

```
SmartTaskAPI/
├── Controllers/
│   ├── AuthController.cs
│   └── TaskItemsController.cs
├── Models/
│   ├── User.cs
│   ├── TaskItem.cs
│   └── ActivityLog.cs
├── Dtos/
│   ├── UserDto.cs
│   └── TaskItemDto.cs
├── Data/
│   └── AppDbContext.cs
├── Program.cs
├── appsettings.json
└── SmartTaskAPI.csproj
```

---

## 📊 Aktiviteleri Loglama
Her görev işlemi (oluşma, güncelleme, silme) hem veritabanına hem de Elasticsearch'e `ActivityLog` formatında kaydedilir. 
Bu sayede görev aktiviteleri izlenebilir, aranabilir ve denetlenebilir hale gelir.

---

## ✨ Geliştirici Notları
- Docker-compose entegresi **isteğe bağlı** olarak düşünüldü ancak bu versiyonda **yer almıyor**.
- Proje sade ve ölçeklenebilir yapıda tasarlandı.
- Ekstra özellikler: etiketleme, bildirim sistemi, soft delete yapısı ilerleyen sürümler için planlandı.

