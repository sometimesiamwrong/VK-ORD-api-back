using System.Security.Cryptography;
using System.Text;
using Domain.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using WebApp.Services.Interfaces;

namespace WebApp.Services.Implementations
{
    /// <summary>
    /// Сервис для управления выполнением SQL-скриптов при запуске приложения
    /// </summary>
    public class DatabaseScriptService : IDatabaseScriptService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DatabaseScriptService> _logger;

        public DatabaseScriptService(AppDbContext context, ILogger<DatabaseScriptService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> ExecutePendingScriptsAsync(string scriptsPath = "Scripts/")
        {
            try
            {
                // Убеждаемся, что таблица DatabaseScripts существует
                await EnsureDatabaseScriptsTableExistsAsync();

                if (!Directory.Exists(scriptsPath))
                {
                    _logger.LogWarning("Scripts directory '{ScriptsPath}' does not exist", scriptsPath);
                    return 0;
                }

                var scriptFiles = Directory.GetFiles(scriptsPath, "*.sql")
                    .OrderBy(f => Path.GetFileName(f))
                    .ToList();

                _logger.LogInformation("Found {Count} SQL script files in {Path}", scriptFiles.Count, scriptsPath);

                int executedCount = 0;

                foreach (var scriptFile in scriptFiles)
                {
                    var scriptName = Path.GetFileName(scriptFile);
                    
                    if (await IsScriptExecutedAsync(scriptName))
                    {
                        _logger.LogDebug("Script '{ScriptName}' already executed, skipping", scriptName);
                        continue;
                    }

                    try
                    {
                        var scriptContent = await File.ReadAllTextAsync(scriptFile);
                        var success = await ExecuteScriptAsync(scriptName, scriptContent, $"Auto-executed from {scriptFile}");
                        
                        if (success)
                        {
                            executedCount++;
                            _logger.LogInformation("Successfully executed script: {ScriptName}", scriptName);
                        }
                        else
                        {
                            _logger.LogError("Failed to execute script: {ScriptName}", scriptName);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error executing script {ScriptName}", scriptName);
                        throw;
                    }
                }

                _logger.LogInformation("Executed {ExecutedCount} new SQL scripts", executedCount);
                return executedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ExecutePendingScriptsAsync");
                return 0;
            }
        }

        public async Task<bool> IsScriptExecutedAsync(string scriptName)
        {
            try
            {
                // Проверяем только успешно выполненные скрипты
                var script = await _context.DatabaseScripts
                    .FirstOrDefaultAsync(s => s.ScriptName == scriptName);
                
                // Если скрипт не найден или выполнен неуспешно - считаем что не выполнен
                return script != null && script.IsSuccessful;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if script {ScriptName} was executed", scriptName);
                // В случае ошибки доступа к БД считаем скрипт невыполненным
                return false;
            }
        }

        public async Task<bool> ExecuteScriptAsync(string scriptName, string scriptContent, string? description = null)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Вычисляем хэш содержимого скрипта
                var scriptHash = ComputeHash(scriptContent);

                // Проверяем, не выполнен ли уже этот скрипт
                var existingScript = await _context.DatabaseScripts
                    .FirstOrDefaultAsync(s => s.ScriptName == scriptName);

                if (existingScript != null)
                {
                    if (existingScript.IsSuccessful && existingScript.ScriptHash == scriptHash)
                    {
                        _logger.LogDebug("Script '{ScriptName}' already executed successfully with same hash", scriptName);
                        await transaction.RollbackAsync();
                        return true;
                    }
                    
                    if (existingScript.IsSuccessful && existingScript.ScriptHash != scriptHash)
                    {
                        _logger.LogWarning("Script '{ScriptName}' content has changed! Original hash: {OriginalHash}, New hash: {NewHash}", 
                            scriptName, existingScript.ScriptHash, scriptHash);
                        _logger.LogInformation("Re-executing script '{ScriptName}' due to content change", scriptName);
                    }
                    
                    if (!existingScript.IsSuccessful)
                    {
                        _logger.LogInformation("Retrying previously failed script '{ScriptName}'", scriptName);
                    }
                }

                // Выполняем SQL скрипт
                await _context.Database.ExecuteSqlRawAsync(scriptContent);

                // Записываем информацию о выполнении
                var scriptRecord = new DatabaseScript
                {
                    ScriptName = scriptName,
                    ScriptHash = scriptHash,
                    ExecutedAt = DateTime.UtcNow,
                    Description = description,
                    IsSuccessful = true
                };

                if (existingScript != null)
                {
                    _context.DatabaseScripts.Remove(existingScript);
                }

                _context.DatabaseScripts.Add(scriptRecord);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("Successfully executed SQL script: {ScriptName}", scriptName);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, "Error executing SQL script: {ScriptName}", scriptName);

                // Записываем информацию об ошибке
                try
                {
                    var errorRecord = new DatabaseScript
                    {
                        ScriptName = scriptName,
                        ScriptHash = ComputeHash(scriptContent),
                        ExecutedAt = DateTime.UtcNow,
                        Description = description,
                        IsSuccessful = false,
                        ErrorMessage = ex.Message.Length > 2000 ? ex.Message[..2000] : ex.Message
                    };

                    _context.DatabaseScripts.Add(errorRecord);
                    await _context.SaveChangesAsync();
                }
                catch (Exception recordEx)
                {
                    _logger.LogError(recordEx, "Failed to record script execution error");
                }

                return false;
            }
        }

        private async Task EnsureDatabaseScriptsTableExistsAsync()
        {
            try
            {
                // Проверяем существование таблицы и создаем её если нужно
                var tableExists = await _context.Database.ExecuteSqlRawAsync(@"
                    SELECT 1 FROM information_schema.tables 
                    WHERE table_schema = 'public' 
                    AND table_name = 'DatabaseScripts'");
            }
            catch
            {
                // Таблица не существует, создаем её
                await _context.Database.ExecuteSqlRawAsync(@"
                    CREATE TABLE IF NOT EXISTS ""DatabaseScripts"" (
                        ""Id"" UUID PRIMARY KEY,
                        ""ScriptName"" VARCHAR(255) NOT NULL UNIQUE,
                        ""ScriptHash"" VARCHAR(64) NOT NULL,
                        ""ExecutedAt"" TIMESTAMP WITH TIME ZONE NOT NULL,
                        ""Description"" VARCHAR(500),
                        ""IsSuccessful"" BOOLEAN NOT NULL DEFAULT TRUE,
                        ""ErrorMessage"" VARCHAR(2000)
                    )");

                _logger.LogInformation("Created DatabaseScripts table");
            }
        }

        private static string ComputeHash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }
    }
}
