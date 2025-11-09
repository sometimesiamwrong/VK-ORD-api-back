# БЫСТРОЕ РЕШЕНИЕ: Hangfire Lock Timeout

## Проблема
```
PostgreSqlDistributedLockException: Could not place a lock on the resource 'hangfire:lock:recurring-job:sync-erir-statuses': Lock timeout.
```

## Решение за 30 секунд

### Windows
```cmd
cd src\Jobs\Scripts
clear-locks.bat
```

Выберите опцию **1** (очистить блокировки старше 5 минут)

### Linux / macOS
```bash
cd src/Jobs/Scripts
chmod +x clear-locks.sh
./clear-locks.sh
```

Выберите опцию **1** (очистить блокировки старше 5 минут)

### Через SQL (если нет скрипта)
```bash
# Подключитесь к PostgreSQL
psql -U vkord_user -d vkord

# Выполните команду
DELETE FROM hangfire.lock WHERE acquired < NOW() - INTERVAL '5 minutes';
```

## Что дальше?

После очистки блокировок:

1. **Перезапустите Jobs приложение**:
   ```bash
   # Docker
   docker-compose restart jobs
   
   # Локально
   dotnet run --project src/Jobs
   ```

2. **Проверьте логи**:
   ```bash
   # Docker
   docker-compose logs -f jobs
   
   # Локально
   tail -f src/Jobs/logs/log-*.txt
   ```

3. **Откройте Hangfire Dashboard**:
   - URL: http://localhost:5002/hangfire
   - Проверьте что recurring job зарегистрирован

## Предотвращение в будущем

✅ **Уже исправлено в коде**:
- Увеличен таймаут блокировки до 1 минуты
- Добавлены автоматические повторные попытки (3 раза)
- Graceful shutdown для корректного освобождения блокировок

✅ **Убедитесь**:
- Запущен только **один экземпляр** Jobs приложения
- При остановке приложения дождитесь завершения текущих задач (до 30 секунд)

## Нужна помощь?

См. подробную документацию:
- [TROUBLESHOOTING.md](../TROUBLESHOOTING.md) - полное руководство по устранению проблем
- [README.md](../README.md) - документация по Jobs приложению

