# VK ORD API Wrapper - Production Deployment Guide

Этот документ описывает процесс развертывания приложения VK ORD API Wrapper в продакшен среде.

## Предварительные требования

- Docker и Docker Compose
- Минимум 2GB RAM
- Минимум 5GB дискового пространства
- Доступ к интернету для скачивания образов

## Быстрое развертывание

### 1. Клонирование репозитория

```bash
git clone <repository-url>
cd VkOrdApiWrapper
```

### 2. Настройка переменных окружения

Создайте файл `.env` на основе примера:

```bash
cp .env.example .env
```

Заполните необходимые переменные в файле `.env`:

```env
# Database Configuration
DB_PASSWORD=your_secure_database_password_here

# JWT Configuration
JWT_SECRET_KEY=your_256_bit_jwt_secret_key_here_minimum_32_characters

# OpenRouter API Configuration
OPENROUTER_API_KEY=sk-or-v1-your-openrouter-api-key-here

# DaData API Configuration
DADATA_API_TOKEN=your-dadata-api-token-here
```

### 3. Запуск приложения

```bash
# Сделать скрипт исполняемым (Linux/Mac)
chmod +x deploy.sh

# Запустить развертывание
./deploy.sh
```

При первом запуске выберите опцию "1) Build and start (fresh deployment)".

## Ручное развертывание

### Сборка и запуск через Docker Compose

```bash
# Сборка образов
docker-compose build

# Запуск всех сервисов
docker-compose up -d

# Проверка статуса
docker-compose ps
```

## Доступ к приложению

После успешного развертывания приложение будет доступно по адресам:

- **API**: http://localhost:8080
- **Health Check**: http://localhost:8080/health
- **Документация API**: http://localhost:8080/swagger (если включена)

## Управление приложением

### Просмотр логов

```bash
# Логи всех сервисов
docker-compose logs -f

# Логи только приложения
docker-compose logs -f vkord-api

# Логи базы данных
docker-compose logs -f postgres
```

### Перезапуск сервисов

```bash
# Перезапуск всех сервисов
docker-compose restart

# Перезапуск только приложения
docker-compose restart vkord-api
```

### Обновление приложения

```bash
# Обновление кода
git pull

# Пересборка и перезапуск
./deploy.sh
# Выберите опцию "2) Update application only"
```

### Остановка и очистка

```bash
# Остановка всех сервисов
docker-compose down

# Остановка с удалением volumes (данные БД будут удалены!)
docker-compose down -v
```

## Мониторинг

### Health Checks

Приложение имеет встроенные health checks:
- HTTP endpoint: `/health`
- Docker health checks для всех сервисов

### Метрики

Для продакшен мониторинга рекомендуется настроить:
- Prometheus + Grafana для метрик
- ELK Stack для логирования
- AlertManager для алертов

## Безопасность

### Переменные окружения

Никогда не коммитите файл `.env` в репозиторий. Он содержит чувствительные данные:
- API ключи
- Пароли базы данных
- JWT секреты

### SSL/TLS

Для продакшена рекомендуется настроить HTTPS:

1. Получите SSL сертификат (Let's Encrypt, коммерческий и т.д.)
2. Разкомментируйте SSL секцию в `nginx.conf`
3. Добавьте пути к сертификатам в переменные окружения

### Firewall

Убедитесь, что открыт только порт 80/443 для внешнего доступа.

## Архитектура

```
┌─────────────────┐    ┌─────────────────┐
│   Nginx Proxy   │    │   VK ORD API    │
│     (Port 80)   │◄──►│   (Port 8080)   │
└─────────────────┘    └─────────────────┘
                              │
                              ▼
                       ┌─────────────────┐
                       │   PostgreSQL    │
                       │   (Port 5432)   │
                       └─────────────────┘
                              │
                              ▼
                       ┌─────────────────┐
                       │     Redis       │
                       │   (Port 6379)   │
                       └─────────────────┘
```

## Troubleshooting

### Проблемы с запуском

1. **Порт уже занят**: Проверьте, что порты 8080, 5432, 6379 свободны
2. **Ошибка подключения к БД**: Проверьте переменную `DB_PASSWORD`
3. **Ошибка API ключей**: Проверьте переменные `OPENROUTER_API_KEY` и `DADATA_API_TOKEN`

### Логи контейнеров

```bash
# Детальные логи приложения
docker-compose logs vkord-api

# Логи базы данных
docker-compose logs postgres

# Логи Redis
docker-compose logs redis
```

### Доступ к базе данных

```bash
# Подключение к PostgreSQL
docker-compose exec postgres psql -U vkord_user -d vkord

# Проверка подключения из приложения
docker-compose exec vkord-api curl -f http://localhost:8080/health
```

## Производительность

### Рекомендуемые настройки сервера

- **CPU**: 2+ cores
- **RAM**: 4GB+
- **Disk**: SSD, 20GB+
- **Network**: 100Mbps+

### Оптимизация

1. Настройте connection pooling в PostgreSQL
2. Включите Redis для кэширования
3. Настройте nginx для статических файлов
4. Регулярно обновляйте Docker образы

## Резервное копирование

### База данных

```bash
# Создание бэкапа
docker-compose exec postgres pg_dump -U vkord_user vkord > backup_$(date +%Y%m%d_%H%M%S).sql

# Восстановление из бэкапа
docker-compose exec -T postgres psql -U vkord_user -d vkord < backup.sql
```

### Volumes

```bash
# Список volumes
docker volume ls

# Бэкап volume
docker run --rm -v vkordapi_postgres_data:/data -v $(pwd):/backup alpine tar czf /backup/postgres_backup.tar.gz -C /data .
```

## Обновление

### Стратегия обновления

1. Создайте бэкап базы данных
2. Обновите код: `git pull`
3. Пересоберите образы: `docker-compose build`
4. Запустите обновление: `docker-compose up -d`
5. Проверьте health checks
6. Если все OK - удалите старые образы

## Контакты

При проблемах с развертыванием проверьте:
1. Логи контейнеров
2. Статус сервисов
3. Переменные окружения
4. Сетевые подключения

