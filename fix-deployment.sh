#!/bin/bash
# ==============================================================================
#   Скрипт для быстрого исправления проблем после деплоя
#   Останавливает старые службы, чистит порты и перезапускает
# ==============================================================================

set -e

echo "========================================="
echo "🔧 Исправление проблем после деплоя"
echo "========================================="

# Проверка прав root
if [ "$EUID" -ne 0 ]; then
    echo "❌ Запустите скрипт с правами root: sudo ./fix-deployment.sh"
    exit 1
fi

WEBAPP_SERVICE="adlawyer-webapp"
JOBS_SERVICE="adlawyer-jobs"

echo ""
echo "📋 Шаг 1: Остановка служб..."
echo "========================================="

systemctl stop ${WEBAPP_SERVICE} 2>/dev/null || echo "⚠️  Служба ${WEBAPP_SERVICE} не запущена"
systemctl stop ${JOBS_SERVICE} 2>/dev/null || echo "⚠️  Служба ${JOBS_SERVICE} не запущена"

echo "✅ Службы остановлены"

echo ""
echo "📋 Шаг 2: Проверка занятых портов..."
echo "========================================="

# Функция для освобождения порта
free_port() {
    local port=$1
    local pids=$(lsof -ti :$port 2>/dev/null || true)
    
    if [ -n "$pids" ]; then
        echo "⚠️  Порт $port занят процессами: $pids"
        echo "   Завершение процессов..."
        echo "$pids" | xargs -r kill -9
        sleep 2
        echo "✅ Порт $port освобожден"
    else
        echo "✅ Порт $port свободен"
    fi
}

# Проверка и освобождение портов
free_port 5000
free_port 5001
free_port 5100
free_port 5101

echo ""
echo "📋 Шаг 3: Очистка старых файлов служб..."
echo "========================================="

# Удаление старых файлов служб
rm -f /etc/systemd/system/${WEBAPP_SERVICE}.service
rm -f /etc/systemd/system/${JOBS_SERVICE}.service

# Перезагрузка конфигурации systemd
systemctl daemon-reload

echo "✅ Старые файлы служб удалены"

echo ""
echo "📋 Шаг 4: Очистка директорий приложений..."
echo "========================================="

WEBAPP_DIR="/var/www/adlawyer-webapp"
JOBS_DIR="/var/www/adlawyer-jobs"

if [ -d "$WEBAPP_DIR" ]; then
    echo "🗑️  Удаление $WEBAPP_DIR..."
    rm -rf "$WEBAPP_DIR"
fi

if [ -d "$JOBS_DIR" ]; then
    echo "🗑️  Удаление $JOBS_DIR..."
    rm -rf "$JOBS_DIR"
fi

echo "✅ Директории очищены"

echo ""
echo "========================================="
echo "✅ Подготовка завершена!"
echo "========================================="
echo ""
echo "Теперь можно запустить деплой:"
echo "  cd /root/AdLawyerApi"
echo "  git pull"
echo "  ./deploy.sh"
echo ""

