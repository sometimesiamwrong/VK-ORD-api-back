@echo off
REM Скрипт для очистки зависших блокировок Hangfire (Windows)
REM Использование: clear-locks.bat

setlocal EnableDelayedExpansion

echo === Hangfire Lock Cleanup Utility ===
echo.

REM Параметры подключения к PostgreSQL
if "%DB_HOST%"=="" set DB_HOST=localhost
if "%DB_PORT%"=="" set DB_PORT=5432
if "%DB_NAME%"=="" set DB_NAME=vkord
if "%DB_USER%"=="" set DB_USER=vkord_user

echo Параметры подключения:
echo   Host: %DB_HOST%
echo   Port: %DB_PORT%
echo   Database: %DB_NAME%
echo   User: %DB_USER%
echo.

REM Проверка наличия psql
where psql >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ОШИБКА: psql не найден в PATH
    echo Установите PostgreSQL client или добавьте его в PATH
    echo Пример: C:\Program Files\PostgreSQL\17\bin
    pause
    exit /b 1
)

REM Запрос пароля если не установлен
if "%DB_PASSWORD%"=="" (
    set /p DB_PASSWORD="Введите пароль для PostgreSQL: "
)

echo.
echo 1. Текущие блокировки:
echo.
psql -h %DB_HOST% -p %DB_PORT% -U %DB_USER% -d %DB_NAME% -c "SELECT resource, acquired, NOW() - acquired as age FROM hangfire.lock ORDER BY acquired DESC;"
echo.

REM Подсчет блокировок
for /f %%i in ('psql -h %DB_HOST% -p %DB_PORT% -U %DB_USER% -d %DB_NAME% -t -c "SELECT COUNT(*) FROM hangfire.lock;"') do set LOCK_COUNT=%%i
set LOCK_COUNT=%LOCK_COUNT: =%

echo Всего блокировок: %LOCK_COUNT%
echo.

if "%LOCK_COUNT%"=="0" (
    echo Нет активных блокировок. Все в порядке!
    pause
    exit /b 0
)

REM Проверка старых блокировок
for /f %%i in ('psql -h %DB_HOST% -p %DB_PORT% -U %DB_USER% -d %DB_NAME% -t -c "SELECT COUNT(*) FROM hangfire.lock WHERE acquired < NOW() - INTERVAL '5 minutes';"') do set OLD_LOCKS=%%i
set OLD_LOCKS=%OLD_LOCKS: =%

echo Блокировок старше 5 минут: %OLD_LOCKS%
echo.

if not "%OLD_LOCKS%"=="0" (
    echo ВНИМАНИЕ: Обнаружены зависшие блокировки!
    echo.
    echo Что вы хотите сделать?
    echo   1^) Очистить блокировки старше 5 минут ^(рекомендуется^)
    echo   2^) Очистить ВСЕ блокировки ^(только если Jobs не запущен!^)
    echo   3^) Выход
    echo.
    
    set /p choice="Выберите опцию (1-3): "
    
    if "!choice!"=="1" (
        echo Очистка блокировок старше 5 минут...
        psql -h %DB_HOST% -p %DB_PORT% -U %DB_USER% -d %DB_NAME% -c "DELETE FROM hangfire.lock WHERE acquired < NOW() - INTERVAL '5 minutes';"
        echo Готово!
    ) else if "!choice!"=="2" (
        echo.
        echo ВНИМАНИЕ: Это удалит ВСЕ блокировки!
        echo ВНИМАНИЕ: Убедитесь, что Jobs приложение не запущено!
        echo.
        set /p confirm="Продолжить? (yes/no): "
        if "!confirm!"=="yes" (
            echo Очистка всех блокировок...
            psql -h %DB_HOST% -p %DB_PORT% -U %DB_USER% -d %DB_NAME% -c "DELETE FROM hangfire.lock;"
            echo Готово!
        ) else (
            echo Отменено.
        )
    ) else if "!choice!"=="3" (
        echo Выход.
        exit /b 0
    ) else (
        echo Неверный выбор.
        pause
        exit /b 1
    )
    
    echo.
    echo Оставшиеся блокировки:
    psql -h %DB_HOST% -p %DB_PORT% -U %DB_USER% -d %DB_NAME% -c "SELECT resource, acquired, NOW() - acquired as age FROM hangfire.lock ORDER BY acquired DESC;"
) else (
    echo Все блокировки актуальные ^(моложе 5 минут^).
)

echo.
echo === Готово ===
pause

