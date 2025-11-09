# Конфигурация для разных окружений

## Структура файлов

Создайте файл `deploy.config.sh` рядом с `deploy.sh` для переопределения настроек:

```bash
# deploy.config.sh - не коммитить в git!
# Этот файл будет автоматически загружен, если существует

# Пример для тестового сервера
REPO_ROOT="/home/deploy/AdLawyerApi"
WEBAPP_PORT="5100"
JOBS_PORT="5101"
ENABLE_CLO_TUNNEL=false
ENVIRONMENT="Staging"
```

## Примеры конфигураций

### Production сервер (с CLO туннелями)

```bash
# deploy.config.sh для Production
REPO_ROOT="/var/apps/AdLawyerApi"
ENVIRONMENT="Production"

WEBAPP_SERVICE_NAME="adlawyer-webapp"
WEBAPP_PORT="5000"
WEBAPP_INSTALL_DIR="/var/www/adlawyer-webapp"
WEBAPP_USER="www-data"

JOBS_SERVICE_NAME="adlawyer-jobs"
JOBS_PORT="5001"
JOBS_INSTALL_DIR="/var/www/adlawyer-jobs"
JOBS_USER="www-data"

ENABLE_CLO_TUNNEL=true
CLO_BIN="/usr/local/bin/clo"
CLO_USER="www-data"
```

### Staging сервер (без CLO)

```bash
# deploy.config.sh для Staging
REPO_ROOT="/home/staging/AdLawyerApi"
ENVIRONMENT="Staging"

WEBAPP_SERVICE_NAME="adlawyer-webapp-staging"
WEBAPP_PORT="6000"
WEBAPP_INSTALL_DIR="/var/www/staging/adlawyer-webapp"
WEBAPP_USER="www-data"

JOBS_SERVICE_NAME="adlawyer-jobs-staging"
JOBS_PORT="6001"
JOBS_INSTALL_DIR="/var/www/staging/adlawyer-jobs"
JOBS_USER="www-data"

ENABLE_CLO_TUNNEL=false
```

### Development (локальная разработка)

```bash
# deploy.config.sh для Development
REPO_ROOT="/home/developer/AdLawyerApi"
ENVIRONMENT="Development"

WEBAPP_SERVICE_NAME="adlawyer-webapp-dev"
WEBAPP_PORT="7000"
WEBAPP_INSTALL_DIR="/home/developer/deployed/adlawyer-webapp"
WEBAPP_USER="developer"

JOBS_SERVICE_NAME="adlawyer-jobs-dev"
JOBS_PORT="7001"
JOBS_INSTALL_DIR="/home/developer/deployed/adlawyer-jobs"
JOBS_USER="developer"

ENABLE_CLO_TUNNEL=false
```

## Использование

### Автоматическое чтение конфига

Обновите `deploy.sh`, добавив в начало функции `main()`:

```bash
main() {
    # Загрузка пользовательской конфигурации, если существует
    if [ -f "${REPO_ROOT}/deploy.config.sh" ]; then
        echo "📝 Загрузка конфигурации из deploy.config.sh..."
        source "${REPO_ROOT}/deploy.config.sh"
    fi
    
    # ... остальной код
}
```

### Или через параметры командной строки

```bash
#!/bin/bash
# В начале main() можно добавить парсинг аргументов

parse_args() {
    while [[ $# -gt 0 ]]; do
        case $1 in
            --env)
                ENVIRONMENT="$2"
                shift 2
                ;;
            --webapp-port)
                WEBAPP_PORT="$2"
                shift 2
                ;;
            --jobs-port)
                JOBS_PORT="$2"
                shift 2
                ;;
            --no-clo)
                ENABLE_CLO_TUNNEL=false
                shift
                ;;
            --help)
                show_help
                exit 0
                ;;
            *)
                echo "Unknown option: $1"
                exit 1
                ;;
        esac
    done
}

show_help() {
    cat <<EOF
Usage: deploy.sh [OPTIONS]

Options:
    --env <environment>      Set environment (Production/Staging/Development)
    --webapp-port <port>     Set WebApp port
    --jobs-port <port>       Set Jobs port
    --no-clo                 Disable CLO tunnels
    --help                   Show this help message

Examples:
    ./deploy.sh --env Staging --webapp-port 6000
    ./deploy.sh --no-clo
EOF
}
```

## GitLab CI/CD для разных окружений

### .gitlab-ci.yml с множественными окружениями

```yaml
stages:
  - deploy

# Production деплой
deploy_production:
  image: mcr.microsoft.com/dotnet/sdk:8.0
  stage: deploy
  before_script:
    - apt-get update -y && apt-get install -y openssh-client
    - eval $(ssh-agent -s)
    - echo "$SSH_PRIVATE_KEY_PROD" | tr -d "\r" | ssh-add -
    - mkdir -p ~/.ssh
    - chmod 700 ~/.ssh
    - ssh-keyscan $SSH_HOST_PROD >> ~/.ssh/known_hosts
    - chmod 644 ~/.ssh/known_hosts
  script:
    - ssh $SSH_USER_PROD@$SSH_HOST_PROD "cd /root/AdLawyerApi && git pull origin main"
    - ssh $SSH_USER_PROD@$SSH_HOST_PROD "chmod +x /root/AdLawyerApi/deploy.sh && /root/AdLawyerApi/deploy.sh"
  only:
    - main
  environment:
    name: production
    url: https://api.adlawyer.com

# Staging деплой
deploy_staging:
  image: mcr.microsoft.com/dotnet/sdk:8.0
  stage: deploy
  before_script:
    - apt-get update -y && apt-get install -y openssh-client
    - eval $(ssh-agent -s)
    - echo "$SSH_PRIVATE_KEY_STAGING" | tr -d "\r" | ssh-add -
    - mkdir -p ~/.ssh
    - chmod 700 ~/.ssh
    - ssh-keyscan $SSH_HOST_STAGING >> ~/.ssh/known_hosts
    - chmod 644 ~/.ssh/known_hosts
  script:
    - ssh $SSH_USER_STAGING@$SSH_HOST_STAGING "cd /home/staging/AdLawyerApi && git pull origin develop"
    - ssh $SSH_USER_STAGING@$SSH_HOST_STAGING "chmod +x /home/staging/AdLawyerApi/deploy.sh && /home/staging/AdLawyerApi/deploy.sh --env Staging"
  only:
    - develop
  environment:
    name: staging
    url: https://staging-api.adlawyer.com

# Manual deploy для Development
deploy_development:
  image: mcr.microsoft.com/dotnet/sdk:8.0
  stage: deploy
  before_script:
    - apt-get update -y && apt-get install -y openssh-client
    - eval $(ssh-agent -s)
    - echo "$SSH_PRIVATE_KEY_DEV" | tr -d "\r" | ssh-add -
    - mkdir -p ~/.ssh
    - chmod 700 ~/.ssh
    - ssh-keyscan $SSH_HOST_DEV >> ~/.ssh/known_hosts
    - chmod 644 ~/.ssh/known_hosts
  script:
    - ssh $SSH_USER_DEV@$SSH_HOST_DEV "cd /home/developer/AdLawyerApi && git pull origin $CI_COMMIT_REF_NAME"
    - ssh $SSH_USER_DEV@$SSH_HOST_DEV "chmod +x /home/developer/AdLawyerApi/deploy.sh && /home/developer/AdLawyerApi/deploy.sh --env Development --no-clo"
  when: manual
  environment:
    name: development
```

## Nginx конфигурация для reverse proxy

### Production (с SSL)

```nginx
# /etc/nginx/sites-available/adlawyer-api

upstream webapp_backend {
    server 127.0.0.1:5000;
}

upstream jobs_backend {
    server 127.0.0.1:5001;
}

# WebApp (основное API)
server {
    listen 80;
    listen [::]:80;
    server_name api.adlawyer.com;
    
    # Redirect to HTTPS
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    listen [::]:443 ssl http2;
    server_name api.adlawyer.com;

    # SSL certificates (Let's Encrypt)
    ssl_certificate /etc/letsencrypt/live/api.adlawyer.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/api.adlawyer.com/privkey.pem;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;

    # Logging
    access_log /var/log/nginx/adlawyer-webapp-access.log;
    error_log /var/log/nginx/adlawyer-webapp-error.log;

    # Max body size
    client_max_body_size 50M;

    location / {
        proxy_pass http://webapp_backend;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
        
        # Timeouts
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }
}

# Jobs Dashboard (только для администраторов)
server {
    listen 443 ssl http2;
    listen [::]:443 ssl http2;
    server_name jobs.adlawyer.com;

    ssl_certificate /etc/letsencrypt/live/jobs.adlawyer.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/jobs.adlawyer.com/privkey.pem;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;

    access_log /var/log/nginx/adlawyer-jobs-access.log;
    error_log /var/log/nginx/adlawyer-jobs-error.log;

    # Basic Auth для защиты Hangfire Dashboard
    auth_basic "Restricted Access";
    auth_basic_user_file /etc/nginx/.htpasswd;

    location / {
        proxy_pass http://jobs_backend;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### Активация конфигурации

```bash
# Создать .htpasswd для Jobs dashboard
sudo apt install apache2-utils
sudo htpasswd -c /etc/nginx/.htpasswd admin

# Проверить конфигурацию
sudo nginx -t

# Активировать сайт
sudo ln -s /etc/nginx/sites-available/adlawyer-api /etc/nginx/sites-enabled/

# Перезапустить Nginx
sudo systemctl restart nginx

# Получить SSL сертификат (Let's Encrypt)
sudo apt install certbot python3-certbot-nginx
sudo certbot --nginx -d api.adlawyer.com -d jobs.adlawyer.com
```

## Мониторинг и алерты

### Простой health check скрипт

```bash
#!/bin/bash
# /usr/local/bin/adlawyer-healthcheck.sh

WEBAPP_URL="http://localhost:5000/health"
JOBS_URL="http://localhost:5001/health"
ALERT_EMAIL="admin@adlawyer.com"

check_service() {
    local name=$1
    local url=$2
    
    if ! curl -f -s -o /dev/null -w "%{http_code}" "$url" | grep -q "200"; then
        echo "❌ $name is DOWN!"
        echo "$name is not responding at $url" | mail -s "Alert: $name Down" "$ALERT_EMAIL"
        return 1
    else
        echo "✅ $name is UP"
        return 0
    fi
}

check_service "WebApp" "$WEBAPP_URL"
check_service "Jobs" "$JOBS_URL"
```

### Добавить в crontab

```bash
# Проверять каждые 5 минут
*/5 * * * * /usr/local/bin/adlawyer-healthcheck.sh >> /var/log/adlawyer-healthcheck.log 2>&1
```

---

**Совет**: Храните `deploy.config.sh` в безопасном месте и не коммитьте в git. Добавьте в `.gitignore`:

```
deploy.config.sh
*.local.sh
```

