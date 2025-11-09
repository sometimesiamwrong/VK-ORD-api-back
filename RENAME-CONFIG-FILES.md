# Изменения в структуре конфигурационных файлов

## Проблема
При публикации проекта Jobs возникал конфликт:
```
error NETSDK1152: Found multiple publish output files with the same relative path:
- appsettings.Development.json (из WebApp)
- appsettings.Development.json (из Jobs)
- appsettings.json (из WebApp)
- appsettings.json (из Jobs)
- appsettings.Production.json (из WebApp)
- appsettings.Production.json (из Jobs)
```

## Решение
Переименовали файлы конфигурации для каждого проекта:

### Jobs
- `appsettings.json` → `jobs.appsettings.json`
- `appsettings.Development.json` → `jobs.appsettings.Development.json`
- `appsettings.Production.json` → `jobs.appsettings.Production.json`

### WebApp
- `appsettings.json` → `webapp.appsettings.json`
- `appsettings.Development.json` → `webapp.appsettings.Development.json`
- `appsettings.Production.json` → `webapp.appsettings.Production.json`

## Внесенные изменения

### 1. Файлы переименованы
✅ `src/Jobs/appsettings*.json` → `src/Jobs/jobs.appsettings*.json`
✅ `src/WebApp/appsettings*.json` → `src/WebApp/webapp.appsettings*.json`

### 2. Jobs.csproj упрощен
Удалены все специальные таргеты для исключения файлов WebApp - теперь они не нужны.

### 3. Program.cs обновлены

**Jobs/Program.cs:**
```csharp
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("jobs.appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"jobs.appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
```

**WebApp/Program.cs:**
```csharp
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("webapp.appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"webapp.appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
```

### 4. GitLab CI обновлен
Добавлены команды для сброса локальных изменений перед git pull:
```yaml
- 'ssh $SSH_USER@$SSH_HOST "cd /root/AdLawyerApi && git reset --hard HEAD"'
- 'ssh $SSH_USER@$SSH_HOST "cd /root/AdLawyerApi && git clean -fd"'
- 'ssh $SSH_USER@$SSH_HOST "cd /root/AdLawyerApi && git pull origin main"'
```

## Проверка

### Локальная сборка
```bash
cd src/Jobs
dotnet build -c Release  # ✅ Успешно
dotnet publish -c Release -o ./publish-test  # ✅ Успешно
```

### Проверка опубликованных файлов
В директории `src/Jobs/publish-test/`:
- ✅ `jobs.appsettings.json`
- ✅ `jobs.appsettings.Development.json`
- ✅ `jobs.appsettings.Production.json`
- ✅ `webapp.appsettings.json`
- ✅ `webapp.appsettings.Development.json`
- ✅ `webapp.appsettings.Production.json`
- ✅ `Jobs.dll`
- ✅ `WebApp.dll`

**Нет конфликтов!** ✅

## Развертывание

После коммита и пуша изменений:

### На сервере
Старые файлы `appsettings.json` на сервере будут автоматически заменены на новые при деплое, так как в GitLab CI добавлены команды `git reset --hard`.

### Скрипт развертывания
Скрипт `deploy.sh` не требует изменений - он просто копирует все файлы из директории публикации.

### Systemd службы
Файлы служб не требуют изменений - переменные окружения `ASPNETCORE_ENVIRONMENT` работают автоматически.

## Преимущества нового подхода

1. **Простота** - не нужны сложные MSBuild таргеты
2. **Независимость** - каждый проект полностью независим при публикации
3. **Ясность** - по имени файла сразу понятно, к какому проекту он относится
4. **Надежность** - нет конфликтов при публикации

## Что дальше?

При необходимости можно обновить документацию:
- README.md
- DEPLOYMENT-GUIDE.md  
- CONFIGURATION-ENVIRONMENTS.md

Но это не критично - основное работает!
