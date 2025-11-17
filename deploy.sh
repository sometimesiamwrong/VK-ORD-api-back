#!/bin/bash
# ==============================================================================
#   Скрипт для развертывания AdLawyer API на сервере
#   Создает и управляет службами systemd для WebApp и Jobs
# ==============================================================================

set -e  # Прервать выполнение при любой ошибке

# ==============================================================================
# КОНФИГУРАЦИЯ - редактируйте эти переменные под ваше окружение
# ==============================================================================

# --- Общие настройки ---
REPO_ROOT="/root/AdLawyerApi"
DOTNET_VERSION="net8.0"
ENVIRONMENT="Production"

# --- Конфигурация WebApp ---
WEBAPP_SERVICE_NAME="adlawyer-webapp"
WEBAPP_PROJECT_PATH="${REPO_ROOT}/src/WebApp"
WEBAPP_PUBLISH_DIR="${REPO_ROOT}/publish/WebApp"  # ← Изменено
WEBAPP_INSTALL_DIR="/var/www/adlawyer-webapp"
WEBAPP_EXEC_NAME="WebApp"
WEBAPP_PORT="5000"
WEBAPP_URL="http://*:${WEBAPP_PORT}"
WEBAPP_USER="www-data"

# --- Конфигурация Jobs ---
JOBS_SERVICE_NAME="adlawyer-jobs"
JOBS_PROJECT_PATH="${REPO_ROOT}/src/Jobs"
JOBS_PUBLISH_DIR="${REPO_ROOT}/publish/Jobs"  # ← Изменено
JOBS_INSTALL_DIR="/var/www/adlawyer-jobs"
JOBS_EXEC_NAME="Jobs"
JOBS_PORT="5101"
JOBS_URL="http://*:${JOBS_PORT}"
JOBS_USER="www-data"

# --- Конфигурация CLO туннеля (опционально) ---
ENABLE_CLO_TUNNEL=true
CLO_BIN="/root/clo"
CLO_WEBAPP_SERVICE_NAME="clo-webapp-tunnel"
CLO_WEBAPP_PORT="${WEBAPP_PORT}"
CLO_USER="root"

# ==============================================================================
# ФУНКЦИИ
# ==============================================================================

check_root() {
    if [ "$EUID" -ne 0 ]; then
        echo "❌ Ошибка: запустите скрипт с правами root (sudo ./deploy.sh)"
        exit 1
    fi
}

install_dotnet_if_needed() {
    if ! command -v dotnet &> /dev/null; then
        echo "⚠️  .NET SDK не найден. Установка..."
        wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
        chmod +x dotnet-install.sh
        ./dotnet-install.sh --channel 8.0
        export PATH="$PATH:$HOME/.dotnet"
        rm dotnet-install.sh
    else
        echo "✅ .NET SDK найден: $(dotnet --version)"
    fi
}

publish_project() {
    local project_name=$1
    local project_path=$2
    local publish_dir=$3
    
    echo ""
    echo "========================================="
    echo "📦 Публикация проекта: ${project_name}"
    echo "========================================="
    
    local project_file="${project_path}/${project_name}.csproj"
    
    if [ ! -f "$project_file" ]; then
        echo "❌ Ошибка: Файл проекта '${project_file}' не найден"
        exit 1
    fi
    
    # Очистка старой публикации
    if [ -d "$publish_dir" ]; then
        echo "🗑️  Удаление старой публикации..."
        rm -rf "$publish_dir"
    fi
    
    # Создание выходной директории
    mkdir -p "$publish_dir"
    
    # Публикация проекта с абсолютным путем
    echo "🔨 Выполняется dotnet publish..."
    dotnet publish "$project_file" -c Release -o "$publish_dir" --nologo
    
    if [ ! -d "$publish_dir" ]; then
        echo "❌ Ошибка: Публикация не создана в '${publish_dir}'"
        exit 1
    fi
    
    # Проверка наличия главного исполняемого файла
    if [ ! -f "${publish_dir}/${project_name}.dll" ]; then
        echo "❌ Ошибка: ${project_name}.dll не найден в '${publish_dir}'"
        exit 1
    fi
    
    echo "✅ Проект ${project_name} успешно опубликован в ${publish_dir}"
}

kill_processes_by_port() {
    local port=$1
    local port_desc=$2
    
    echo "🔍 Проверка процессов на порту ${port}..."
    
    # Получить PID всех процессов слушающих на порту
    local pids=$(lsof -ti :${port} 2>/dev/null || true)
    
    if [ -z "$pids" ]; then
        echo "   ✅ Портом ${port} никто не занят"
        return 0
    fi
    
    echo "   🛑 Найдены процессы на порту ${port}: ${pids}"
    
    # Попытка graceful shutdown
    for pid in $pids; do
        echo "   • Отправка SIGTERM процессу $pid..."
        kill -TERM "$pid" 2>/dev/null || true
    done
    
    # Ждем 3 секунды
    sleep 3
    
    # Проверка, остались ли процессы
    pids=$(lsof -ti :${port} 2>/dev/null || true)
    
    if [ -z "$pids" ]; then
        echo "   ✅ Процессы на порту ${port} успешно остановлены"
        return 0
    fi
    
    # Force kill если остались
    echo "   ⚠️  Некоторые процессы еще активны, force kill..."
    for pid in $pids; do
        echo "   • Force kill процесса $pid..."
        kill -9 "$pid" 2>/dev/null || true
    done
    
    sleep 1
    echo "   ✅ Порт ${port} освобожден"
}

cleanup_old_processes() {
    echo ""
    echo "========================================="
    echo "🧹 Очистка старых процессов и портов"
    echo "========================================="
    
    # Остановка по портам
    kill_processes_by_port "$WEBAPP_PORT" "WebApp"
    kill_processes_by_port "$JOBS_PORT" "Jobs"
    
    if [ "$ENABLE_CLO_TUNNEL" = true ]; then
        kill_processes_by_port "$CLO_WEBAPP_PORT" "CLO WebApp Tunnel"
    fi
    
    echo "✅ Очистка портов завершена"
}

stop_and_remove_service() {
    local service_name=$1
    local service_file="/etc/systemd/system/${service_name}.service"
    
    if systemctl list-unit-files | grep -q "^${service_name}.service"; then
        echo "🛑 Остановка службы ${service_name}..."
        systemctl stop "${service_name}.service" || true
        systemctl disable "${service_name}.service" || true
        
        if [ -f "$service_file" ]; then
            rm -f "$service_file"
        fi
        
        systemctl daemon-reload
        echo "✅ Старая служба ${service_name} удалена"
    fi
}

install_service() {
    local service_name=$1
    local install_dir=$2
    local publish_dir=$3
    local exec_name=$4
    local service_user=$5
    local app_url=$6
    local description=$7
    
    echo ""
    echo "========================================="
    echo "⚙️  Установка службы: ${service_name}"
    echo "========================================="
    
    # Удаление старой службы
    stop_and_remove_service "$service_name"
    
    # Удаление старых файлов
    if [ -d "$install_dir" ]; then
        echo "🗑️  Удаление старых файлов из ${install_dir}..."
        rm -rf "$install_dir"
    fi
    
    
    # Создание директории и копирование файлов
    echo "📁 Создание директории ${install_dir}..."
    mkdir -p "$install_dir"
    
    echo "📋 Копирование файлов из ${publish_dir}..."
    cp -r "$publish_dir"/* "$install_dir/"
    
    # Создание пользователя если не существует
    if [ "$service_user" != "root" ] && ! id "$service_user" &>/dev/null; then
        echo "👤 Создание пользователя ${service_user}..."
        useradd -r -s /bin/false "$service_user" || true
    fi
    
    # Установка прав
    echo "🔐 Установка прав доступа..."
    chown -R "${service_user}:${service_user}" "$install_dir"
    chmod +x "${install_dir}/${exec_name}"
    
    # Создание директории логов
    local log_dir="${install_dir}/logs"
    mkdir -p "$log_dir"
    chown -R "${service_user}:${service_user}" "$log_dir"
    
    # Создание файла службы
    local service_file="/etc/systemd/system/${service_name}.service"
    echo "📝 Создание файла службы ${service_file}..."
    
    cat <<EOF > "$service_file"
[Unit]
Description=${description}
After=network.target ${WEBAPP_SERVICE_NAME}.service
Wants=${WEBAPP_SERVICE_NAME}.service

[Service]
Type=simple
WorkingDirectory=${install_dir}
ExecStart=${install_dir}/${exec_name}
Restart=always
RestartSec=10
SyslogIdentifier=${service_name}
User=${service_user}

# Environment variables
Environment=ASPNETCORE_ENVIRONMENT=${ENVIRONMENT}
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
Environment=ASPNETCORE_URLS=${app_url}

# Limits
LimitNOFILE=65536

[Install]
WantedBy=multi-user.target
EOF
    
    echo "✅ Файл службы создан"
}

install_clo_tunnel() {
    local service_name=$1
    local port=$2
    local description=$3
    
    if [ "$ENABLE_CLO_TUNNEL" != true ]; then
        echo "ℹ️  CLO туннель отключен в конфигурации"
        return
    fi
    
    echo ""
    echo "========================================="
    echo "🌐 Установка CLO туннеля: ${service_name}"
    echo "========================================="
    
    # Проверка наличия бинаря CLO
    if [ ! -f "$CLO_BIN" ]; then
        echo "⚠️  CLO бинарь не найден по пути ${CLO_BIN}. Пропускаем..."
        return
    fi
    
    if [ ! -x "$CLO_BIN" ]; then
        chmod +x "$CLO_BIN"
    fi
    
    # Удаление старой службы
    stop_and_remove_service "$service_name"
    
    # Создание файла службы
    local service_file="/etc/systemd/system/${service_name}.service"
    echo "📝 Создание файла службы ${service_file}..."
    
    cat <<EOF > "$service_file"
[Unit]
Description=${description}
After=network-online.target
Wants=network-online.target

[Service]
Type=simple
ExecStart=${CLO_BIN} publish http ${port}
Restart=always
RestartSec=5
User=${CLO_USER}
WorkingDirectory=/root
StandardOutput=journal
StandardError=journal

[Install]
WantedBy=multi-user.target
EOF
    
    echo "✅ CLO туннель ${service_name} создан для порта ${port}"
}

start_services() {
    echo ""
    echo "========================================="
    echo "🚀 Запуск служб"
    echo "========================================="
    
    systemctl daemon-reload
    
    # Запуск WebApp
    echo "▶️  Запуск ${WEBAPP_SERVICE_NAME}..."
    systemctl enable "${WEBAPP_SERVICE_NAME}.service"
    systemctl start "${WEBAPP_SERVICE_NAME}.service"
    
    # Запуск Jobs
    echo "▶️  Запуск ${JOBS_SERVICE_NAME}..."
    systemctl enable "${JOBS_SERVICE_NAME}.service"
    systemctl start "${JOBS_SERVICE_NAME}.service"
    
    # Запуск CLO туннелей
    if [ "$ENABLE_CLO_TUNNEL" = true ] && [ -x "$CLO_BIN" ]; then
        if systemctl list-unit-files | grep -q "^${CLO_WEBAPP_SERVICE_NAME}.service"; then
            echo "▶️  Запуск ${CLO_WEBAPP_SERVICE_NAME}..."
            systemctl enable "${CLO_WEBAPP_SERVICE_NAME}.service"
            systemctl start "${CLO_WEBAPP_SERVICE_NAME}.service"
        fi
    fi
    
    echo "✅ Все службы запущены"
}

show_status() {
    echo ""
    echo "========================================="
    echo "📊 Статус служб"
    echo "========================================="
    
    systemctl status "${WEBAPP_SERVICE_NAME}.service" --no-pager --lines=5 || true
    echo ""
    systemctl status "${JOBS_SERVICE_NAME}.service" --no-pager --lines=5 || true
    
    if [ "$ENABLE_CLO_TUNNEL" = true ] && [ -x "$CLO_BIN" ]; then
        if systemctl list-unit-files | grep -q "^${CLO_WEBAPP_SERVICE_NAME}.service"; then
            echo ""
            systemctl status "${CLO_WEBAPP_SERVICE_NAME}.service" --no-pager --lines=5 || true
        fi
    fi
}

show_summary() {
    echo ""
    echo "========================================="
    echo "✅ РАЗВЕРТЫВАНИЕ ЗАВЕРШЕНО"
    echo "========================================="
    echo ""
    echo "📍 WebApp:"
    echo "   - Служба: ${WEBAPP_SERVICE_NAME}"
    echo "   - URL: ${WEBAPP_URL}"
    echo "   - Директория: ${WEBAPP_INSTALL_DIR}"
    echo "   - Логи: journalctl -u ${WEBAPP_SERVICE_NAME} -f"
    echo ""
    echo "📍 Jobs:"
    echo "   - Служба: ${JOBS_SERVICE_NAME}"
    echo "   - URL: ${JOBS_URL}"
    echo "   - Директория: ${JOBS_INSTALL_DIR}"
    echo "   - Логи: journalctl -u ${JOBS_SERVICE_NAME} -f"
    
    if [ "$ENABLE_CLO_TUNNEL" = true ] && [ -x "$CLO_BIN" ]; then
        echo ""
        echo "📍 CLO Туннели:"
        if systemctl list-unit-files | grep -q "^${CLO_WEBAPP_SERVICE_NAME}.service"; then
            echo "   - WebApp туннель: ${CLO_WEBAPP_SERVICE_NAME} (порт ${CLO_WEBAPP_PORT})"
            echo "     Логи: journalctl -u ${CLO_WEBAPP_SERVICE_NAME} -f"
        fi
    fi
    
    echo ""
    echo "🔧 Полезные команды:"
    echo "   - systemctl restart ${WEBAPP_SERVICE_NAME}  # перезапустить WebApp"
    echo "   - systemctl restart ${JOBS_SERVICE_NAME}     # перезапустить Jobs"
    echo "   - systemctl status ${WEBAPP_SERVICE_NAME}    # статус WebApp"
    echo "   - systemctl status ${JOBS_SERVICE_NAME}      # статус Jobs"
    echo ""
}

# ==============================================================================
# ОСНОВНОЙ ПРОЦЕСС
# ==============================================================================

main() {
    echo "========================================="
    echo "🚀 AdLawyer API - Скрипт развертывания"
    echo "========================================="
    echo "Дата: $(date '+%Y-%m-%d %H:%M:%S')"
    echo ""
    
    # Очистка старых публикаций
    if [ -d "${REPO_ROOT}/publish" ]; then
        echo "🗑️  Очистка старых публикаций..."
        rm -rf "${REPO_ROOT}/publish"
    fi
    
    # Загрузка пользовательской конфигурации, если существует
    local config_file="$(dirname "$0")/deploy.config.sh"
    if [ -f "$config_file" ]; then
        echo "📝 Загрузка конфигурации из deploy.config.sh..."
        source "$config_file"
        echo "✅ Конфигурация загружена"
        echo ""
    fi
    
    # Проверки
    check_root
    install_dotnet_if_needed
    
    # Публикация проектов
    publish_project "WebApp" "$WEBAPP_PROJECT_PATH" "$WEBAPP_PUBLISH_DIR"
    publish_project "Jobs" "$JOBS_PROJECT_PATH" "$JOBS_PUBLISH_DIR"
    
    # Установка служб
    install_service \
        "$WEBAPP_SERVICE_NAME" \
        "$WEBAPP_INSTALL_DIR" \
        "$WEBAPP_PUBLISH_DIR" \
        "$WEBAPP_EXEC_NAME" \
        "$WEBAPP_USER" \
        "$WEBAPP_URL" \
        "AdLawyer WebApp Service"
    
    install_service \
        "$JOBS_SERVICE_NAME" \
        "$JOBS_INSTALL_DIR" \
        "$JOBS_PUBLISH_DIR" \
        "$JOBS_EXEC_NAME" \
        "$JOBS_USER" \
        "$JOBS_URL" \
        "AdLawyer Jobs Service"
    
    # Установка CLO туннелей
    install_clo_tunnel \
        "$CLO_WEBAPP_SERVICE_NAME" \
        "$CLO_WEBAPP_PORT" \
        "CLO CloudPub tunnel for AdLawyer WebApp"
        
    # Очистка старых процессов и портов
    cleanup_old_processes
    
    # Запуск служб
    start_services
    
    # Небольшая пауза перед показом статуса
    sleep 2
    
    # Показ статуса и итоговой информации
    show_status
    show_summary
}

# Запуск основного процесса
main "$@"

