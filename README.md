# TMK MiniApp - Telegram Mini App Backend

Backend для Telegram Mini App каталога товаров TMK.

## Функциональность

- **Каталог товаров** с фильтрацией по диаметру, толщине стенки, марке стали, ГОСТу
- **Корзина покупок** с поддержкой тонн и метров  
- **Оформление заказов** с сохранением данных клиента
- **Динамические цены** с объемными скидками в зависимости от количества
- **Мульти-складская система** с разными ценами и остатками

## Технологии

- .NET 9.0
- Entity Framework Core
- SQLite
- Swagger/OpenAPI

## API Documentation

### Основные endpoints:

#### Каталог
- `GET /api/Catalog` - получение каталога с фильтрацией
  - Параметры: `diameterMin`, `diameterMax`, `wallMin`, `wallMax`, `steelGrade`, `gost`, `productionType`, `stockId`

#### Корзина
- `GET /api/Cart/{userId}` - получить корзину пользователя
- `POST /api/Cart/add` - добавить товар в корзину
- `PUT /api/Cart/update` - изменить количество товара
- `DELETE /api/Cart/remove/{itemId}` - удалить товар из корзины

#### Заказы
- `POST /api/Order/create` - создать заказ из корзины
- `GET /api/Order/{userId}` - список заказов пользователя
- `GET /api/Order/details/{orderId}` - детали конкретного заказа

## Локальный запуск

### Требования:
- .NET 9.0 SDK
- Git

### Шаги:

1. **Клонирование репозитория:**
```bash
git clone https://github.com/kdsuniq/tmk-miniapp.git
cd tmk-miniapp
```
2. **Запуск приложения**
```dotnet run```