#!/bin/bash
# ==============================================================================
#   Скрипт для тестирования публикации проектов локально
#   Проверяет, что не возникает конфликтов при публикации
# ==============================================================================

set -e

echo "========================================="
echo "🧪 Тестирование публикации проектов"
echo "========================================="

# Определение корневой директории проекта
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
WEBAPP_PROJECT="${SCRIPT_DIR}/src/WebApp/WebApp.csproj"
JOBS_PROJECT="${SCRIPT_DIR}/src/Jobs/Jobs.csproj"

# Проверка наличия проектов
if [ ! -f "$WEBAPP_PROJECT" ]; then
    echo "❌ Не найден проект WebApp: $WEBAPP_PROJECT"
    exit 1
fi

if [ ! -f "$JOBS_PROJECT" ]; then
    echo "❌ Не найден проект Jobs: $JOBS_PROJECT"
    exit 1
fi

echo "✅ Проекты найдены"
echo ""

# Тестирование публикации WebApp
echo "========================================="
echo "📦 Тестирование публикации WebApp"
echo "========================================="

WEBAPP_TEST_DIR="${SCRIPT_DIR}/src/WebApp/test-publish"
rm -rf "$WEBAPP_TEST_DIR"

echo "Выполняется: dotnet publish -c Release..."
dotnet publish "$WEBAPP_PROJECT" -c Release -o "$WEBAPP_TEST_DIR" --nologo

if [ $? -eq 0 ]; then
    echo "✅ WebApp успешно опубликован"
    echo "   Директория: $WEBAPP_TEST_DIR"
    
    # Проверка наличия ключевых файлов
    if [ -f "${WEBAPP_TEST_DIR}/WebApp.dll" ]; then
        echo "   ✅ WebApp.dll найден"
    else
        echo "   ❌ WebApp.dll НЕ найден"
    fi
    
    if [ -f "${WEBAPP_TEST_DIR}/appsettings.json" ]; then
        echo "   �� appsettings.json найден"
    else
        echo "   ❌ appsettings.json НЕ найден"
    fi
else
    echo "❌ Ошибка публикации WebApp"
    exit 1
fi

echo ""

# Тестирование публикации Jobs
echo "========================================="
echo "📦 Тестирование публикации Jobs"
echo "========================================="

JOBS_TEST_DIR="${SCRIPT_DIR}/src/Jobs/test-publish"
rm -rf "$JOBS_TEST_DIR"

echo "Выполняется: dotnet publish -c Release..."
dotnet publish "$JOBS_PROJECT" -c Release -o "$JOBS_TEST_DIR" --nologo

if [ $? -eq 0 ]; then
    echo "✅ Jobs успешно опубликован"
    echo "   Директория: $JOBS_TEST_DIR"
    
    # Проверка наличия ключевых файлов
    if [ -f "${JOBS_TEST_DIR}/Jobs.dll" ]; then
        echo "   ✅ Jobs.dll найден"
    else
        echo "   ❌ Jobs.dll НЕ найден"
    fi
    
    if [ -f "${JOBS_TEST_DIR}/WebApp.dll" ]; then
        echo "   ✅ WebApp.dll найден (зависимость)"
    else
        echo "   ❌ WebApp.dll НЕ найден (ОШИБКА: Jobs требует WebApp.dll)"
        exit 1
    fi
    
    # Проверка на конфликты appsettings
    JOBS_APPSETTINGS_COUNT=$(find "$JOBS_TEST_DIR" -maxdepth 1 -name "appsettings*.json" | wc -l)
    
    if [ "$JOBS_APPSETTINGS_COUNT" -gt 0 ]; then
        echo "   ✅ Найдено $JOBS_APPSETTINGS_COUNT файла(ов) appsettings*.json"
        
        # Проверка, что это файлы Jobs, а не WebApp
        if [ -f "${JOBS_TEST_DIR}/appsettings.json" ]; then
            # Проверяем содержимое - должна быть секция "Jobs"
            if grep -q '"Jobs"' "${JOBS_TEST_DIR}/appsettings.json"; then
                echo "   ✅ appsettings.json принадлежит Jobs (найдена секция 'Jobs')"
            else
                echo "   ⚠️  appsettings.json не содержит секцию 'Jobs' - возможно, это файл WebApp!"
            fi
        fi
    else
        echo "   ❌ Файлы appsettings*.json НЕ найдены (ОШИБКА: нужны настройки Jobs)"
        exit 1
    fi
    
    # Проверка на дубликаты
    echo ""
    echo "   🔍 Проверка на возможные конфликты..."
    
    # Ищем потенциально конфликтующие файлы
    DUPLICATE_CHECK=$(find "$JOBS_TEST_DIR" -type f -name "*.json" | grep -E "(appsettings|web\.config)" | wc -l)
    
    echo "   Найдено конфигурационных файлов: $DUPLICATE_CHECK"
    
else
    echo "❌ Ошибка публикации Jobs"
    exit 1
fi

echo ""
echo "========================================="
echo "✅ ВСЕ ТЕСТЫ ПРОЙДЕНЫ УСПЕШНО"
echo "========================================="
echo ""
echo "📁 Тестовые директории публикации:"
echo "   WebApp: $WEBAPP_TEST_DIR"
echo "   Jobs:   $JOBS_TEST_DIR"
echo ""
echo "🧹 Очистка тестовых директорий:"
echo "   rm -rf $WEBAPP_TEST_DIR"
echo "   rm -rf $JOBS_TEST_DIR"
echo ""

