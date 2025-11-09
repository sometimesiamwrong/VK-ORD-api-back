#!/bin/bash

# Скрипт для очистки зависших блокировок Hangfire
# Использование: ./clear-locks.sh

set -e

echo "=== Hangfire Lock Cleanup Utility ==="
echo ""

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Параметры подключения к PostgreSQL
DB_HOST="${DB_HOST:-localhost}"
DB_PORT="${DB_PORT:-5432}"
DB_NAME="${DB_NAME:-vkord}"
DB_USER="${DB_USER:-vkord_user}"

echo -e "${YELLOW}Параметры подключения:${NC}"
echo "  Host: $DB_HOST"
echo "  Port: $DB_PORT"
echo "  Database: $DB_NAME"
echo "  User: $DB_USER"
echo ""

# Функция для выполнения SQL
execute_sql() {
    PGPASSWORD="$DB_PASSWORD" psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -c "$1"
}

# 1. Показать текущие блокировки
echo -e "${YELLOW}1. Текущие блокировки:${NC}"
execute_sql "SELECT resource, acquired, NOW() - acquired as age FROM hangfire.lock ORDER BY acquired DESC;"
echo ""

# 2. Подсчет блокировок
LOCK_COUNT=$(PGPASSWORD="$DB_PASSWORD" psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -t -c "SELECT COUNT(*) FROM hangfire.lock;")
echo -e "${YELLOW}Всего блокировок:${NC} $LOCK_COUNT"
echo ""

if [ "$LOCK_COUNT" -eq 0 ]; then
    echo -e "${GREEN}Нет активных блокировок. Все в порядке!${NC}"
    exit 0
fi

# 3. Проверка старых блокировок
OLD_LOCKS=$(PGPASSWORD="$DB_PASSWORD" psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -t -c "SELECT COUNT(*) FROM hangfire.lock WHERE acquired < NOW() - INTERVAL '5 minutes';")
echo -e "${YELLOW}Блокировок старше 5 минут:${NC} $OLD_LOCKS"
echo ""

if [ "$OLD_LOCKS" -gt 0 ]; then
    echo -e "${RED}Обнаружены зависшие блокировки!${NC}"
    echo ""
    echo "Что вы хотите сделать?"
    echo "  1) Очистить блокировки старше 5 минут (рекомендуется)"
    echo "  2) Очистить ВСЕ блокировки (только если Jobs не запущен!)"
    echo "  3) Выход"
    echo ""
    read -p "Выберите опцию (1-3): " choice
    
    case $choice in
        1)
            echo -e "${YELLOW}Очистка блокировок старше 5 минут...${NC}"
            execute_sql "DELETE FROM hangfire.lock WHERE acquired < NOW() - INTERVAL '5 minutes';"
            echo -e "${GREEN}Готово!${NC}"
            ;;
        2)
            echo -e "${RED}ВНИМАНИЕ: Это удалит ВСЕ блокировки!${NC}"
            echo -e "${RED}Убедитесь, что Jobs приложение не запущено!${NC}"
            read -p "Продолжить? (yes/no): " confirm
            if [ "$confirm" == "yes" ]; then
                echo -e "${YELLOW}Очистка всех блокировок...${NC}"
                execute_sql "DELETE FROM hangfire.lock;"
                echo -e "${GREEN}Готово!${NC}"
            else
                echo "Отменено."
            fi
            ;;
        3)
            echo "Выход."
            exit 0
            ;;
        *)
            echo -e "${RED}Неверный выбор.${NC}"
            exit 1
            ;;
    esac
    
    echo ""
    echo -e "${YELLOW}Оставшиеся блокировки:${NC}"
    execute_sql "SELECT resource, acquired, NOW() - acquired as age FROM hangfire.lock ORDER BY acquired DESC;"
else
    echo -e "${GREEN}Все блокировки актуальные (моложе 5 минут).${NC}"
fi

echo ""
echo -e "${GREEN}=== Готово ===${NC}"

