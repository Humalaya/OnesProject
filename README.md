# ONES To-Do List Project (.NET Core Web API & Angular)

Bu proje, ONES Stajyer Programı kapsamında geliştirilmiş tam donanımlı bir **To-Do List** uygulamasıdır. Backend tarafında **.NET Core 8 Web API**, **CQRS Mimarisi**, **Repository Pattern**, **Entity Framework Core** ve **MSSQL** kullanılırken; Frontend tarafında **Angular 18**, **TypeScript**, **HTML** ve **SCSS** kullanılmıştır.

---

## 🛠️ Kullanılan Teknolojiler

### Backend (.NET Core)
- **.NET 8.0 Web API**
- **CQRS Pattern** (MediatR kütüphanesi ile Command & Query ayrımı)
- **Repository Pattern** (`IToDoRepository` ve `ToDoRepository`)
- **Entity Framework Core 8** (Code-First yaklaşımı, MSSQL, Guid/UNIQUEIDENTIFIER ID'ler)
- **Swagger / OpenAPI** (API dokümantasyonu ve test araçları)

### Frontend (Angular)
- **Angular 18** (Standalone Components mimarisi)
- **TypeScript**
- **HTML5 & SCSS** (Responsive, modern ve şık kullanıcı arayüzü)
- **HttpClient** (RESTful API entegrasyonu)

---

## 🏗️ Proje Mimarisi ve Katmanlar

Proje katmanlı mimari ve **CQRS** prensiplerine uygun olarak tasarlanmıştır:
- **Domain Katmanı (`Domain/Entities/ToDo.cs`)**: `ID` (Guid), `Title`, `Description`, `IsCompleted`, `CreatedAt` özelliklerini içeren temel varlık (Entity).
- **Application Katmanı (`Application/`)**:
  - `Repositories/IToDoRepository.cs`: Veri erişim sözleşmesi.
  - `Features/ToDoFeature/Commands/`: Create, Update, Delete Command ve Handler sınıfları.
  - `Features/ToDoFeature/Queries/`: GetAll ve GetById Query ve Handler sınıfları.
- **Persistence Katmanı (`Persistence/`)**:
  - `Contexts/TodoDbContext.cs`: EF Core DbContext, MSSQL ve Fluent API konfigürasyonları.
  - `Repositories/ToDoRepository.cs`: Repository implementasyonu.
- **API Katmanı (`Controllers/ToDoController.cs`)**: CQRS Handler'larını `Mediator` vasıtasıyla tetikleyen REST API endpoint'leri.
- **Frontend Katmanı (`TodoListClient/`)**: Angular standalone bileşenler ve servisler ile oluşturulmuş arayüz.

---

## 📋 Veritabanı Tablo Şeması (MSSQL)

```sql
CREATE TABLE ToDo (
    ID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    IsCompleted BIT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
```

---

## 🚀 Kurulum ve Çalıştırma Adımları (Setup Instructions)

Projenin kök dizininde yer alan **`start.sh`** script dosyası sayesinde hem Backend (.NET API) hem de Frontend (Angular) tek komutla aynı anda başlatılabilir.

### Tek Komutla Çalıştırma (Önerilen)
Kök dizinde terminali açın ve şu komutu çalıştırın:
```bash
./start.sh
```
*(Bu script hem API'yi `http://localhost:5000` adresinde hem de Angular UI'yi `http://localhost:4200` adresinde aynı anda başlatır. `Ctrl + C` tuşlarına bastığınızda her iki süreç de güvenle kapatılır).*

### Manuel Çalıştırma Adımları

#### 1. Veritabanı ve Backend Setup
1. **MSSQL Server** çalışır durumda olmalıdır.
2. `TodoListApi/appsettings.json` içerisindeki connection string'i kendi MSSQL sunucu ayarlarınıza göre güncelleyin.
3. API'yi başlatın:
   ```bash
   cd TodoListApi
   dotnet run
   ```
   *API varsayılan olarak `http://localhost:5000` adresinde çalışacaktır. Swagger arayüzüne `http://localhost:5000/swagger` adresinden erişebilirsiniz.*

#### 2. Frontend (Angular) Setup
1. Terminalde `TodoListClient` klasörüne gidin:
   ```bash
   cd TodoListClient
   ```
2. Bağımlılıkları yükleyin ve Angular uygulamasını başlatın:
   ```bash
   npm install
   npm start
   ```
   *Uygulama tarayıcınızda `http://localhost:4200` adresinde açılacaktır.*

---

## 🔐 Giriş (Login)

Uygulama artık email/şifre + JWT token ile giriş gerektiriyor. Her kullanıcının kendi görev listesi vardır.

İlk çalıştırmada (`./start.sh` veya `dotnet run`), veritabanı boşsa otomatik olarak bir demo hesap oluşturulur ve konsola yazdırılır:

```
email:    demo@example.com
password: Demo123!
```

Yeni kullanıcı eklemek için sayfa yok — Swagger üzerinden `POST /api/auth/register` çağırarak (`username`, `email`, `password`, isteğe bağlı `fullName`) hesap oluşturabilirsiniz.

## 📡 API Endpoints ve Örnek İstekler

| İşlem | HTTP Metodu | Endpoint | Açıklama |
| :--- | :--- | :--- | :--- |
| **Register** | POST | `/api/auth/register` | Yeni kullanıcı oluşturur, JWT döner. |
| **Login** | POST | `/api/auth/login` | Email/şifre ile giriş yapar, JWT döner. |
| **GetAll** | GET | `/api/todo?pageNumber=1&pageSize=10` | Giriş yapan kullanıcının görevlerini sayfalı listeler (pageSize: 5/10/20). |
| **GetById** | GET | `/api/todo/{id}` | ID'ye göre spesifik görevi getirir. |
| **Create** | POST | `/api/todo` | Yeni görev oluşturur. |
| **Update** | PUT | `/api/todo/{id}` | Belirtilen ID'ye sahip görevi günceller. |
| **Delete** | DELETE | `/api/todo/{id}` | Belirtilen ID'ye sahip görevi siler. |
| **Profile** | GET | `/api/profile` | Giriş yapan kullanıcının profilini getirir. |
| **Update Username** | PUT | `/api/profile/username` | Kullanıcı adını değiştirir. |
| **Change Password** | PUT | `/api/profile/password` | Şifre değiştirir (mevcut + yeni şifre). |
| **Upload Picture** | POST | `/api/profile/picture` | Profil fotoğrafı yükler (multipart/form-data, `file`). |

`/api/todo` ve `/api/profile` altındaki tüm endpoint'ler `Authorization: Bearer <token>` header'ı gerektirir.

### Örnek İstekler (cURL / JSON)

#### 1. Yeni Görev Ekle (POST `/api/todo`)
```json
{
  "title": "Staj Projesini Tamamla",
  "description": ".NET Core API, CQRS ve Angular arayüzü bitirilecek.",
  "isCompleted": false
}
```

#### 2. Görev Güncelle (PUT `/api/todo/{id}`)
```json
{
  "title": "Staj Projesini Tamamla ve Gönder",
  "description": "Tüm gereksinimler kontrol edildi ve mail atıldı.",
  "isCompleted": true
}
```

---

## 📧 İletişim & Stajyer Bilgileri
- **Ad Soyad:** Emir [Soyadınız]
- **Okul:** [Üniversiteniz / Bölümünüz]
- **Ünvan:** Yazılım Mühendisliği Stajyeri (Software Engineering Intern)
- **LinkedIn:** [linkedin.com/in/emir-profiliniz](https://linkedin.com)
- **E-posta Teslim Adresi:** ismail@ones.com.tr

