# PaymentApi

API для авторизации пользователей и совершения платежей.  

## Endpoints

### Авторизация

- **POST /api/Auth/login** – логин пользователя, возвращает JWT.  
  ```json
  {
    "username": "test",
    "password": "123456"
  }

Запуск

Клонировать репозиторий:

git clone <URL>
cd PaymentApi


Запустить через Docker Compose:

docker-compose up --build


API доступно: http://localhost:5000/

БД PostgreSQL запускается автоматически через Docker Compose.

Баланс нового пользователя = 8 USD.

Все платежи сохраняются в базе.

JWT-токены поддерживают несколько сессий одновременно.