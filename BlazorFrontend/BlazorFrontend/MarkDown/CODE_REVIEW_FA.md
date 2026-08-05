# گزارش بررسی کد پروژه Blazor Frontend / Backend

## جمع‌بندی

این پروژه یک نمونه Full-Stack بر پایه .NET 9 است که از ASP.NET Core Web API در بک‌اند و Blazor WebAssembly در فرانت‌اند استفاده می‌کند. هسته احراز هویت با ASP.NET Core Identity و JWT پیاده‌سازی شده و ارتباط با SQL Server از طریق Entity Framework Core انجام می‌شود.

ساختار کلی برای یک پروژه آموزشی، نمونه‌کار یا Starter مناسب است، اما پیش از انتشار عمومی در GitHub یا استفاده در محیط Production چند اصلاح ضروری دارد.

> بررسی انجام‌شده از نوع Static Code Review بوده است. در محیط بررسی، .NET SDK نصب نبود و امکان اجرای `dotnet build` و تست عملی پروژه فراهم نشد.

## نقاط قوت

- تفکیک مناسب فرانت‌اند و بک‌اند
- استفاده از ASP.NET Core Identity به‌جای پیاده‌سازی دستی رمز عبور
- تولید و اعتبارسنجی JWT
- استفاده از Claims برای شناسه، ایمیل، نام و Role
- استفاده از EF Core Migration
- فعال‌سازی Swagger در محیط Development
- استفاده از API Versioning
- پیاده‌سازی `AuthenticationStateProvider` سفارشی در Blazor
- استفاده از Delegating Handler برای افزودن خودکار JWT
- مدیریت پاسخ 401 و انتقال کاربر به صفحه ورود
- وجود یک الگوی یکپارچه برای پاسخ API
- پشتیبانی اولیه از PWA و Service Worker

## ایرادهای بحرانی

### 1. اطلاعات محرمانه داخل Source Code

در فایل‌های تنظیمات بک‌اند، Connection String شامل حساب SQL Server و رمز عبور و همچنین JWT Signing Key قرار دارد.

اقدام ضروری:

- این مقادیر را قبل از Push حذف کنید.
- رمز SQL Server و JWT Key فعلی را تغییر دهید.
- از Environment Variables، .NET User Secrets، Azure Key Vault یا Secret Manager استفاده کنید.
- یک فایل `appsettings.Example.json` با مقادیر نمونه و بدون Secret قرار دهید.

### 2. دسترسی مدیریت کاربران فقط به Login وابسته است

Endpoint دریافت فهرست کاربران با `[Authorize]` محافظت شده، اما Role یا Policy ادمین ندارد. در نتیجه هر کاربر واردشده می‌تواند اطلاعات سایر کاربران را دریافت کند.

پیشنهاد:

```csharp
[Authorize(Roles = "Admin")]
```

یا استفاده از Authorization Policy اختصاصی.

### 3. ذخیره JWT در Local Storage

توکن در Local Storage مرورگر نگهداری می‌شود. این روش در صورت وجود XSS می‌تواند باعث سرقت توکن شود.

برای پروژه Demo قابل قبول است، ولی برای Production بهتر است یکی از این روش‌ها بررسی شود:

- Secure + HttpOnly + SameSite Cookie
- Backend for Frontend pattern
- Access Token کوتاه‌عمر همراه با Refresh Token امن و Rotation

## ایرادهای مهم

### 4. دو پیاده‌سازی تکراری برای Authentication

هم `AuthController` و هم `v1/UserController` عملیات Login و Register را انجام می‌دهند. این تکرار باعث ایجاد APIهای موازی، رفتار متفاوت پاسخ‌ها و افزایش هزینه نگهداری می‌شود.

پیشنهاد: فقط Controller نسخه‌بندی‌شده نگه داشته شود و Controller تکراری حذف شود.

### 5. عمر JWT فقط یک دقیقه است

`ExpiresInMinutes` برابر یک دقیقه تنظیم شده و Refresh Token نیز وجود ندارد. کاربر تقریباً بلافاصله از سیستم خارج می‌شود.

پیشنهاد:

- عمر Access Token بر اساس نیاز واقعی، مثلاً 15 تا 30 دقیقه
- پیاده‌سازی Refresh Token با Rotation و Revoke

### 6. UpdateUser هنوز پیاده‌سازی نشده است

متد `UpdateUser` فقط DTO دریافتی را برمی‌گرداند و هیچ تغییری در دیتابیس ایجاد نمی‌کند. صفحه ویرایش فرانت‌اند نیز فقط یک پیام Toast نشان می‌دهد.

این بخش باید در README با عنوان Scaffold یا In Progress معرفی شود و تا زمان تکمیل، بهتر است در Production در دسترس نباشد.

### 7. پاسخ API برای همه Status Codeها یکدست نیست

`ApiResultFilterAttribute` پاسخ‌های 200، 400 و 404 را پوشش می‌دهد، ولی پاسخ 401 و برخی خطاهای دیگر را به همان قالب استاندارد تبدیل نمی‌کند. در نتیجه فرانت‌اند همیشه نمی‌تواند یک قرارداد پاسخ ثابت داشته باشد.

پیشنهاد:

- استفاده از Middleware سراسری برای Exception Handling و API Result
- استانداردسازی 400، 401، 403، 404، 409 و 500
- ترجیح `ProblemDetails` استاندارد یا یک Contract واحد و مستند

### 8. سیاست رمز عبور برای Production ضعیف است

حداقل طول رمز 6 کاراکتر است و Uppercase و Non-Alphanumeric اجباری نیستند. همچنین Lockout و Rate Limit به‌صورت مشخص تنظیم نشده‌اند.

پیشنهاد:

- افزایش حداقل طول
- فعال‌سازی Lockout
- Rate Limiting برای Login/Register
- بررسی Password Breach در سامانه‌های حساس

### 9. EmailConfirmed به‌صورت دستی True می‌شود

هنگام ثبت‌نام، ایمیل بدون ارسال لینک تأیید، Confirmed در نظر گرفته می‌شود.

برای Production باید Token تأیید ایمیل ایجاد و لینک تأیید ارسال شود.

### 10. CORS به آدرس‌های localhost متصل است

Originها به‌صورت Hard-coded در `Program.cs` قرار دارند. بهتر است لیست Originها از Configuration خوانده شود و برای هر Environment مقدار جداگانه داشته باشد.

## ایرادهای متوسط و پاک‌سازی

- Endpoint آزمایشی `zaz` باید حذف شود.
- Controllerهای Test و Weather در نسخه Production حذف یا محدود شوند.
- متد `LoginAsync1` در فرانت‌اند تکراری و بلااستفاده است.
- دو نمونه `ILocalStorageService` در Constructor سرویس Auth تزریق شده‌اند؛ یکی کافی است.
- `async void` در `LoginDisplay.razor` بهتر است به `async Task` تبدیل شود.
- در Logout، `NotifyUserLogout` دو بار فراخوانی می‌شود.
- `UserManagementService.GetUsers()` بهتر است Async و با `ToListAsync()` باشد.
- در UI فهرست کاربران، حالت Loading، Empty و Error کامل نیست.
- لینک‌های منوی `users/list`، `users/create` و `users/roles` با Routeهای موجود همخوانی ندارند.
- AutoMapper در پروژه‌ها اضافه شده ولی در کد بررسی‌شده استفاده‌ای از آن دیده نشد.
- نسخه `Microsoft.Extensions.Http` در فرانت‌اند با Target Framework پروژه هم‌نسخه نیست؛ بهتر است وابستگی‌ها یکپارچه شوند.
- سورس کامل Blazored.Toast داخل Repository قرار گرفته است؛ اگر تغییر محلی خاصی ندارید، استفاده از NuGet Package تمیزتر است.
- کامنت‌ها و کدهای Comment شده در `Program.cs` و `CustomAuthStateProvider` پاک‌سازی شوند.
- نام فایل `UnauthorizedHandle.cs` با نام کلاس `UnauthorizedHandler` یکسان نیست؛ برای خوانایی بهتر هماهنگ شود.
- پیام‌های فارسی دارای غلط تایپی مانند «کمله عبور» هستند.

## ترتیب پیشنهادی اصلاحات قبل از Push

1. حذف و تعویض همه Secretها و Passwordها
2. ساخت `.gitignore` استاندارد برای .NET و Visual Studio
3. حذف `bin`، `obj`، فایل‌های user-specific و تنظیمات محرمانه
4. حذف Controllerها و Methodهای تکراری یا آزمایشی
5. اعمال Role/Policy روی مدیریت کاربران
6. یکپارچه‌سازی قرارداد پاسخ API
7. تکمیل یا غیرفعال‌سازی UpdateUser
8. اصلاح Token Lifetime و طراحی Refresh Token
9. انتقال CORS و URLها به Configuration
10. اجرای Build، Migration و تست End-to-End

## پیشنهاد نام Repository

```text
blazor-jwt-authentication-starter
```

## توضیح کوتاه پیشنهادی GitHub

```text
A full-stack authentication starter built with .NET 9, ASP.NET Core Identity, JWT, EF Core, SQL Server, and Blazor WebAssembly.
```

## Topics پیشنهادی

```text
dotnet
aspnet-core
blazor-webassembly
jwt-authentication
aspnet-core-identity
entity-framework-core
sql-server
web-api
swagger
pwa
```
