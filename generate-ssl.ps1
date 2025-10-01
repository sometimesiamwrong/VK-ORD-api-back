# PowerShell скрипт для генерации самоподписанного SSL сертификата для VkOrdApiWrapper
# Использование: .\generate-ssl.ps1

param(
    [string]$OutputDir = "ssl",
    [int]$ValidDays = 3650,
    [string]$CommonName = "localhost"
)

Write-Host "🔐 Генерация самоподписанного SSL сертификата для VkOrdApiWrapper..." -ForegroundColor Green

# Создаем директорию для SSL сертификатов
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
    Write-Host "📁 Создана директория: $OutputDir" -ForegroundColor Yellow
}

# Параметры сертификата
$CertParams = @{
    Subject = "CN=$CommonName, O=VkOrdApiWrapper, OU=IT Department, L=Moscow, S=Moscow, C=RU"
    DnsName = @("localhost", "*.localhost", "vkord-api", "*.vkord-api", "127.0.0.1")
    CertStoreLocation = "Cert:\CurrentUser\My"
    KeyAlgorithm = "RSA"
    KeyLength = 2048
    NotAfter = (Get-Date).AddDays($ValidDays)
    KeyUsage = @("DigitalSignature", "KeyEncipherment")
    Type = "SSLServerAuthentication"
}

try {
    Write-Host "🔑 Генерируем самоподписанный сертификат..." -ForegroundColor Yellow
    
    # Создаем сертификат
    $cert = New-SelfSignedCertificate @CertParams
    
    Write-Host "✅ Сертификат создан с отпечатком: $($cert.Thumbprint)" -ForegroundColor Green
    
    # Экспортируем сертификат в файлы
    $certPath = Join-Path $OutputDir "vkord-api.crt"
    $keyPath = Join-Path $OutputDir "vkord-api.key" 
    $pfxPath = Join-Path $OutputDir "vkord-api.pfx"
    
    # Экспортируем в PFX (с пустым паролем)
    Write-Host "📦 Экспортируем PFX файл..." -ForegroundColor Yellow
    $pfxPassword = ConvertTo-SecureString -String "" -Force -AsPlainText
    Export-PfxCertificate -Cert $cert -FilePath $pfxPath -Password $pfxPassword | Out-Null
    
    # Экспортируем публичный сертификат
    Write-Host "📜 Экспортируем CRT файл..." -ForegroundColor Yellow
    Export-Certificate -Cert $cert -FilePath $certPath -Type CERT | Out-Null
    
    # Для приватного ключа используем OpenSSL если доступен, иначе инструкции
    if (Get-Command openssl -ErrorAction SilentlyContinue) {
        Write-Host "🔓 Экспортируем приватный ключ..." -ForegroundColor Yellow
        & openssl pkcs12 -in $pfxPath -nocerts -out $keyPath -nodes -passin pass:
        
        # Очищаем ключ от лишней информации
        $keyContent = Get-Content $keyPath | Where-Object { $_ -match "-----BEGIN PRIVATE KEY-----" -or $_ -match "-----END PRIVATE KEY-----" -or ($_ -notmatch "^[a-zA-Z]" -and $_ -ne "") }
        $keyContent | Set-Content $keyPath
    } else {
        Write-Host "⚠️  OpenSSL не найден. Для извлечения приватного ключа выполните:" -ForegroundColor Yellow
        Write-Host "   openssl pkcs12 -in $pfxPath -nocerts -out $keyPath -nodes -passin pass:" -ForegroundColor Cyan
    }
    
    # Удаляем сертификат из хранилища (оставляем только файлы)
    Remove-Item -Path "Cert:\CurrentUser\My\$($cert.Thumbprint)" -Force
    
    Write-Host ""
    Write-Host "✅ SSL сертификат успешно создан!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📁 Файлы сертификата:" -ForegroundColor Cyan
    Write-Host "   Сертификат:     $certPath" -ForegroundColor White
    Write-Host "   Приватный ключ: $keyPath" -ForegroundColor White  
    Write-Host "   PFX файл:       $pfxPath" -ForegroundColor White
    Write-Host ""
    Write-Host "📋 Информация о сертификате:" -ForegroundColor Cyan
    Write-Host "   Субъект:        $($cert.Subject)" -ForegroundColor White
    Write-Host "   Отпечаток:      $($cert.Thumbprint)" -ForegroundColor White
    Write-Host "   Действителен с: $($cert.NotBefore)" -ForegroundColor White
    Write-Host "   Действителен до: $($cert.NotAfter)" -ForegroundColor White
    Write-Host "   DNS имена:      $($cert.DnsNameList -join ', ')" -ForegroundColor White
    Write-Host ""
    Write-Host "🔧 Для использования в Docker добавьте volume mapping:" -ForegroundColor Cyan
    Write-Host "   volumes:" -ForegroundColor White
    Write-Host "     - ./ssl:/app/ssl:ro" -ForegroundColor White
    Write-Host ""
    Write-Host "⚠️  Это самоподписанный сертификат. Браузеры будут показывать предупреждение о безопасности." -ForegroundColor Yellow
    Write-Host "   Для обхода предупреждений используйте флаг -k в curl или игнорируйте SSL ошибки в коде." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "🚀 Готово! Теперь можно запускать приложение с HTTPS." -ForegroundColor Green
    
} catch {
    Write-Host "❌ Ошибка при создании сертификата: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
