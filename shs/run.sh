#!/bin/bash

# VK ORD API Wrapper - Запуск на Unix системах
# Автор: AI Assistant
# Дата: 23 сентября 2025
# Поддерживаемые дистрибутивы: Ubuntu (включая 24.04 LTS), Debian, CentOS/RHEL/AlmaLinux/Rocky, Fedora, Alpine
# Особенности: Ubuntu 24.04 использует репозиторий Ubuntu 22.04 для совместимости

set -e  # Остановить скрипт при первой ошибке

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Функции для красивого вывода
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Установка .NET локально без sudo
install_dotnet_locally() {
    log_info "Установка .NET 8 локально в домашнюю папку..."
    
    # Создаем папку для .NET
    DOTNET_DIR="$HOME/.dotnet"
    mkdir -p "$DOTNET_DIR"
    
    # Скачиваем скрипт установки
    curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 8.0 --runtime dotnet --install-dir "$DOTNET_DIR"
    
    # Добавляем в PATH
    export PATH="$DOTNET_DIR:$PATH"
    
    # Добавляем в .bashrc для постоянного использования
    if ! grep -q "export PATH=\"\$HOME/.dotnet:\$PATH\"" ~/.bashrc; then
        echo 'export PATH="$HOME/.dotnet:$PATH"' >> ~/.bashrc
        log_info "Добавлен PATH в ~/.bashrc"
    fi
    
    # Проверяем установку
    if "$DOTNET_DIR/dotnet" --version &> /dev/null; then
        DOTNET_VERSION=$("$DOTNET_DIR/dotnet" --version)
        log_success ".NET 8 успешно установлен локально (версия: $DOTNET_VERSION)"
        
        # Создаем символическую ссылку для удобства
        if [ ! -f "$HOME/bin/dotnet" ]; then
            mkdir -p "$HOME/bin"
            ln -sf "$DOTNET_DIR/dotnet" "$HOME/bin/dotnet"
            export PATH="$HOME/bin:$PATH"
        fi
        return 0
    else
        log_error "Не удалось установить .NET 8 локально"
        return 1
    fi
}

# Проверка и установка .NET 8
check_and_install_dotnet() {
    log_info "Проверка наличия .NET 8..."

    if command -v dotnet &> /dev/null; then
        DOTNET_VERSION=$(dotnet --version 2>/dev/null || echo "")
        if [[ $DOTNET_VERSION == 8.* ]]; then
            log_success ".NET 8 уже установлен (версия: $DOTNET_VERSION)"
            return 0
        else
            log_warning "Найдена версия .NET: $DOTNET_VERSION. Нужна .NET 8."
        fi
    else
        log_warning ".NET не найден. Начинаем установку..."
    fi

    # Определение дистрибутива Linux
    if [ -f /etc/os-release ]; then
        . /etc/os-release
        DISTRO=$ID
        VERSION=$VERSION_ID
    else
        log_error "Не удалось определить дистрибутив Linux"
        exit 1
    fi

    log_info "Обнаружен дистрибутив: $DISTRO $VERSION"

    case $DISTRO in
        ubuntu)
            log_info "Установка .NET 8 для Ubuntu $VERSION..."

            # Для новых версий Ubuntu используем репозиторий предыдущей LTS
            case $VERSION in
                24.*)
                    UBUNTU_REPO="22.04"
                    log_info "Ubuntu 24.xx - используем репозиторий Ubuntu 22.04"
                    ;;
                22.*)
                    UBUNTU_REPO="22.04"
                    ;;
                20.*)
                    UBUNTU_REPO="20.04"
                    ;;
                18.*)
                    UBUNTU_REPO="18.04"
                    ;;
                *)
                    UBUNTU_REPO="22.04"
                    log_warning "Неизвестная версия Ubuntu $VERSION, используем репозиторий 22.04"
                    ;;
            esac

            # Установка необходимых пакетов для HTTPS репозиториев
            log_info "Установка необходимых пакетов..."
            sudo apt-get update
            sudo apt-get install -y wget apt-transport-https ca-certificates

            # Добавление Microsoft repository
            log_info "Добавление Microsoft repository для Ubuntu $UBUNTU_REPO..."
            wget https://packages.microsoft.com/config/ubuntu/$UBUNTU_REPO/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
            sudo dpkg -i packages-microsoft-prod.deb
            rm packages-microsoft-prod.deb

            # Обновление пакетов и установка .NET 8
            log_info "Установка .NET 8 Runtime..."
            sudo apt-get update
            sudo apt-get install -y dotnet-runtime-8.0

            # Проверка установки
            if dotnet --list-runtimes | grep -q "Microsoft.NETCore.App 8.0"; then
                DOTNET_VERSION=$(dotnet --version 2>/dev/null || echo "Unknown")
                log_success ".NET 8 Runtime успешно установлен (текущая версия хоста/SDK: $DOTNET_VERSION)"
                return 0
            else
                log_error "Установка .NET 8 Runtime завершилась с ошибкой или .NET 8 Runtime не найден после установки."
                return 1
            fi
            ;;

        debian)
            log_info "Установка .NET 8 для Debian $VERSION..."

            # Установка необходимых пакетов для HTTPS репозиториев
            log_info "Установка необходимых пакетов..."
            sudo apt-get update
            sudo apt-get install -y wget apt-transport-https ca-certificates gnupg

            # Для Debian используем репозиторий Ubuntu 22.04 как совместимый
            log_info "Добавление Microsoft repository (используем Ubuntu 22.04 репозиторий)..."
            wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
            sudo dpkg -i packages-microsoft-prod.deb
            rm packages-microsoft-prod.deb

            # Обновление пакетов и установка .NET 8
            log_info "Установка .NET 8 Runtime..."
            sudo apt-get update
            sudo apt-get install -y dotnet-runtime-8.0

            # Проверка установки
            if dotnet --list-runtimes | grep -q "Microsoft.NETCore.App 8.0"; then
                DOTNET_VERSION=$(dotnet --version 2>/dev/null || echo "Unknown")
                log_success ".NET 8 Runtime успешно установлен (текущая версия хоста/SDK: $DOTNET_VERSION)"
                return 0
            else
                log_error "Установка .NET 8 Runtime завершилась с ошибкой или .NET 8 Runtime не найден после установки."
                return 1
            fi
            ;;

        centos|rhel|fedora|almalinux|rocky)
            log_info "Установка .NET 8 для CentOS/RHEL/Fedora/AlmaLinux/Rocky..."

            # Проверка наличия sudo
            if ! command -v sudo &> /dev/null || ! sudo -n true 2>/dev/null; then
                log_warning "Sudo недоступен. Устанавливаем .NET 8 локально в домашнюю папку..."
                install_dotnet_locally
                return 0
            fi

            # Добавление Microsoft repository
            sudo rpm -Uvh https://packages.microsoft.com/config/rhel/8/packages-microsoft-prod.rpm

            # Установка .NET 8
            if command -v dnf &> /dev/null; then
                sudo dnf install -y dotnet-runtime-8.0
            else
                sudo yum install -y dotnet-runtime-8.0
            fi
            ;;

        alpine)
            log_info "Установка .NET 8 для Alpine Linux..."

            # Добавление Microsoft repository для Alpine
            sudo apk add --no-cache dotnet8-runtime
            ;;

        *)
            log_warning "Неизвестный дистрибутив: $DISTRO. Пробуем установить .NET 8 локально..."
            install_dotnet_locally
            if [ $? -ne 0 ]; then
                log_error "Не удалось установить .NET 8 автоматически"
                log_info "Пожалуйста, установите .NET 8 вручную с https://dotnet.microsoft.com/download/dotnet/8.0"
                exit 1
            fi
            ;;
    esac

    # Проверка установки
    if command -v dotnet &> /dev/null; then
        DOTNET_VERSION=$(dotnet --version)
        log_success ".NET 8 успешно установлен (версия: $DOTNET_VERSION)"
    else
        log_error "Не удалось установить .NET 8"
        exit 1
    fi
}

# Проверка зависимостей проекта
check_dependencies() {
    log_info "Проверка зависимостей проекта..."

    # Проверка наличия appsettings.json
    if [ ! -f "appsettings.json" ]; then
        log_error "Файл appsettings.json не найден!"
        exit 1
    fi

    log_success "Конфигурационные файлы найдены"
}

# Сборка проекта
build_project() {
    log_info "Сборка проекта в режиме Release..."

    # Очистка предыдущей сборки
    if [ -d "bin" ]; then
        log_info "Очистка предыдущей сборки..."
        rm -rf bin obj
    fi

    # Восстановление зависимостей
    log_info "Восстановление NuGet пакетов..."
    dotnet restore

    # Сборка проекта
    log_info "Сборка проекта..."
    dotnet build --configuration Release --no-restore

    if [ $? -eq 0 ]; then
        log_success "Проект успешно собран"
    else
        log_error "Ошибка сборки проекта"
        exit 1
    fi
}

# Запуск приложения
run_application() {
    log_info "Запуск VK ORD API Wrapper..."

    # Определение порта (по умолчанию 8080)
    PORT=${PORT:-8080}
    ASPNETCORE_URLS="http://*:$PORT"

    log_info "Приложение будет доступно на порту: $PORT"
    log_info "Swagger UI: http://localhost:$PORT/swagger"
    log_info "Health check: http://localhost:$PORT/health"

    # Установка переменных окружения для продакшена
    export ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT:-Development}
    export ASPNETCORE_URLS=$ASPNETCORE_URLS

    # Запуск приложения
    log_info "Нажмите Ctrl+C для остановки приложения"
    dotnet run --configuration Release --urls="$ASPNETCORE_URLS"
}

# Настройка переменных окружения
setup_environment() {
    log_info "Настройка переменных окружения..."

    # Проверка наличия .env файла
    if [ -f ".env" ]; then
        log_info "Загрузка переменных из .env файла..."
        set -a
        source .env
        set +a
    else
        log_warning ".env файл не найден. Используются значения по умолчанию из appsettings.json"
        log_info "Для настройки переменных окружения создайте файл .env с необходимыми переменными:"
        cat << EOF
# Пример содержимого .env файла:
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://*:8080
JWT_SECRET_KEY=your_secure_jwt_secret_key_here_min_32_chars
OPENROUTER_API_KEY=sk-or-v1-your_openrouter_api_key
DADATA_API_TOKEN=your_dadata_api_token
REDIS_CONFIGURATION=localhost:6379
EOF
    fi
}

# Функция остановки приложения
cleanup() {
    log_info "Остановка приложения..."
    # Здесь можно добавить логику для graceful shutdown
    exit 0
}

# Обработка сигналов для graceful shutdown
trap cleanup SIGINT SIGTERM

# Главная функция
main() {
    log_info "🚀 Запуск VK ORD API Wrapper"
    log_info "================================="

    # Проверка и установка .NET
    check_and_install_dotnet

    # Настройка переменных окружения
    setup_environment

    # Проверка зависимостей
    check_dependencies

    # Сборка проекта
    build_project

    # Запуск приложения
    run_application
}

# Проверка аргументов командной строки
case "${1:-}" in
    --help|-h)
        echo "VK ORD API Wrapper - скрипт запуска"
        echo ""
        echo "Использование: $0 [опции]"
        echo ""
        echo "Опции:"
        echo "  --help, -h          Показать эту справку"
        echo "  --build-only        Только сборка проекта без запуска"
        echo "  --check-deps        Только проверка зависимостей"
        echo ""
        echo "Поддерживаемые системы:"
        echo "  Ubuntu 24.04 LTS    Автоматическая установка .NET 8 (использует Ubuntu 22.04 репозиторий)"
        echo "  Ubuntu 22.04/20.04  Полная поддержка"
        echo "  Debian              Поддержка через Ubuntu репозиторий"
        echo "  CentOS/RHEL/AlmaLinux/Rocky Полная поддержка"
        echo "  Fedora              Поддержка"
        echo "  Alpine Linux        Поддержка"
        echo "  Любая Unix система  Локальная установка .NET без sudo"
        echo ""
        echo "Переменные окружения:"
        echo "  PORT                Порт для запуска (по умолчанию: 8080)"
        echo "  ASPNETCORE_ENVIRONMENT  Среда выполнения (Development/Production)"
        echo ""
        exit 0
        ;;

    --build-only)
        check_and_install_dotnet
        setup_environment
        check_dependencies
        build_project
        log_success "Сборка завершена успешно"
        exit 0
        ;;

    --check-deps)
        check_and_install_dotnet
        check_dependencies
        log_success "Все зависимости проверены"
        exit 0
        ;;
esac

# Запуск основной функции
main
