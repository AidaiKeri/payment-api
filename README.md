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

### Logout

- **POST /api/Auth/logout** – завершает текущую сессию пользователя.
Требует заголовок авторизации:

curl -X POST "http://localhost:5000/api/Auth/logout" \
  -H "Authorization: Bearer <your_jwt_token>"

### Платеж

- **POST /api/Payment/pay** – списывает деньги с баланса пользователя.
Требует авторизацию:

curl -X POST "http://localhost:5000/api/Payment/pay" \
  -H "Authorization: Bearer <your_jwt_token>" \
  -H "Content-Type: application/json"

Запуск

Клонировать репозиторий:

git clone <url>
cd PaymentApi


Запустить через Docker Compose:

docker-compose up --build


После запуска:

API доступно по адресу: http://localhost:5000/

PostgreSQL поднимается автоматически в контейнере