# AdLawyer API

ASP.NET Core API для работы с рекламными данными и интеграцией с внешними сервисами.

## 📋 Описание

AdLawyer API состоит из двух основных компонентов:

- **WebApp** - REST API сервис для работы с рекламными данными
- **Jobs** - фоновые задачи для синхронизации данных с внешними системами (Hangfire)

## 🚀 Быстрый старт

### Локальная разработка

```bash
# Клонировать репозиторий
git clone <repository_url> AdLawyerApi
cd AdLawyerApi

# Восстановить зависимости
dotnet restore

# Запустить WebApp
cd src/WebApp
dotnet run

# В другом терминале запустить Jobs
cd src/Jobs
dotnet run
```

### Docker

```bash
# Собрать и запустить через Docker Compose
docker-compose up -d
```

## 📦 Развертывание на сервере

### Автоматический деплой через GitLab CI/CD

При пуше в ветку `main` автоматически происходит развертывание на сервер.

### Ручной деплой

```bash
# На сервере
cd /root/AdLawyerApi
git pull origin main
sudo ./deploy.sh
```

### Подробная документация

- [📖 Руководство по развертыванию](DEPLOYMENT-GUIDE.md) - полная инструкция по деплою
- [⚙️ Конфигурация окружений](DEPLOYMENT-CONFIG.md) - настройка для разных окружений
- [🔧 API документация](API_DOCUMENTATION.md) - документация API

## 🛠 Технологии

- **.NET 8.0** - основной фреймворк
- **ASP.NET Core** - веб-фреймворк
- **Entity Framework Core** - ORM для работы с БД
- **PostgreSQL** - база данных
- **Redis** - кэширование
- **Hangfire** - фоновые задачи
- **Serilog** - логирование
- **JWT** - аутентификация
- **Swagger/OpenAPI** - документация API

## 📁 Структура проекта

```
AdLawyerApi/
├── src/
│   ├── Domain/              # Общий слой домена, сущности, контекст БД
│   ├── WebApp/              # REST API сервис
│   └── Jobs/                # Фоновые задачи (Hangfire)
├── deploy.sh                # Скрипт развертывания
├── deploy.config.example.sh # Пример конфигурации
├── docker-compose.yml       # Docker Compose конфигурация
├── Dockerfile               # Dockerfile для WebApp
├── Dockerfile.jobs          # Dockerfile для Jobs
└── .gitlab-ci.yml          # GitLab CI/CD пайплайн
```

## ⚙️ Конфигурация

### Переменные окружения

Основные настройки задаются в `appsettings.json` и `appsettings.Production.json`.

Для локальной разработки можно создать `appsettings.Development.json` (не коммитить в git).

### Настройка деплоя

Скопируйте пример конфигурации:

```bash
cp deploy.config.example.sh deploy.config.sh
# Отредактируйте deploy.config.sh под ваше окружение
```

## 🔐 Безопасность

- Не храните секреты в репозитории
- Используйте переменные окружения для чувствительных данных
- Настройте firewall на сервере
- Используйте HTTPS через reverse proxy (nginx)
- Регулярно обновляйте зависимости

## 📊 Мониторинг

### Логи служб

```bash
# WebApp логи
sudo journalctl -u adlawyer-webapp -f

# Jobs логи
sudo journalctl -u adlawyer-jobs -f
```

### Статус служб

```bash
sudo systemctl status adlawyer-webapp
sudo systemctl status adlawyer-jobs
```

### Hangfire Dashboard

Jobs сервис предоставляет Hangfire Dashboard для мониторинга фоновых задач:
- URL: `http://localhost:5001/hangfire` (или через настроенный домен)

## 🧪 Тестирование

```bash
# Запустить тесты
dotnet test
```

## 📝 GitLab CI/CD переменные

Для автоматического деплоя настройте в Settings → CI/CD → Variables:

- `SSH_PRIVATE_KEY` - приватный SSH ключ для доступа к серверу
- `SSH_HOST` - адрес сервера (IP или домен)
- `SSH_USER` - пользователь SSH (обычно `root`)

## 🤝 Участие в разработке

1. Создайте feature branch от `develop`
2. Внесите изменения
3. Создайте Merge Request в `develop`
4. После ревью и тестирования изменения попадут в `main`

## 📄 Лицензия

Proprietary - все права защищены

## 📞 Контакты

Для вопросов и предложений обращайтесь к команде разработки.

Every project is different, so consider which of these sections apply to yours. The sections used in the template are suggestions for most open source projects. Also keep in mind that while a README can be too long and detailed, too long is better than too short. If you think your README is too long, consider utilizing another form of documentation rather than cutting out information.

## Name
Choose a self-explaining name for your project.

## Description
Let people know what your project can do specifically. Provide context and add a link to any reference visitors might be unfamiliar with. A list of Features or a Background subsection can also be added here. If there are alternatives to your project, this is a good place to list differentiating factors.

## Badges
On some READMEs, you may see small images that convey metadata, such as whether or not all the tests are passing for the project. You can use Shields to add some to your README. Many services also have instructions for adding a badge.

## Visuals
Depending on what you are making, it can be a good idea to include screenshots or even a video (you'll frequently see GIFs rather than actual videos). Tools like ttygif can help, but check out Asciinema for a more sophisticated method.

## Installation
Within a particular ecosystem, there may be a common way of installing things, such as using Yarn, NuGet, or Homebrew. However, consider the possibility that whoever is reading your README is a novice and would like more guidance. Listing specific steps helps remove ambiguity and gets people to using your project as quickly as possible. If it only runs in a specific context like a particular programming language version or operating system or has dependencies that have to be installed manually, also add a Requirements subsection.

## Usage
Use examples liberally, and show the expected output if you can. It's helpful to have inline the smallest example of usage that you can demonstrate, while providing links to more sophisticated examples if they are too long to reasonably include in the README.

## Support
Tell people where they can go to for help. It can be any combination of an issue tracker, a chat room, an email address, etc.

## Roadmap
If you have ideas for releases in the future, it is a good idea to list them in the README.

## Contributing
State if you are open to contributions and what your requirements are for accepting them.

For people who want to make changes to your project, it's helpful to have some documentation on how to get started. Perhaps there is a script that they should run or some environment variables that they need to set. Make these steps explicit. These instructions could also be useful to your future self.

You can also document commands to lint the code or run tests. These steps help to ensure high code quality and reduce the likelihood that the changes inadvertently break something. Having instructions for running tests is especially helpful if it requires external setup, such as starting a Selenium server for testing in a browser.

## Authors and acknowledgment
Show your appreciation to those who have contributed to the project.

## License
For open source projects, say how it is licensed.

## Project status
If you have run out of energy or time for your project, put a note at the top of the README saying that development has slowed down or stopped completely. Someone may choose to fork your project or volunteer to step in as a maintainer or owner, allowing your project to keep going. You can also make an explicit request for maintainers.
