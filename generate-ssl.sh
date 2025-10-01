#!/bin/bash

# Скрипт для генерации самоподписанного SSL сертификата для VkOrdApiWrapper
# Использование: ./generate-ssl.sh

set -e

echo "🔐 Генерация самоподписанного SSL сертификата для VkOrdApiWrapper..."

# Создаем директорию для SSL сертификатов
SSL_DIR="ssl"
mkdir -p "$SSL_DIR"

# Параметры сертификата
CERT_DAYS=3650  # 10 лет
KEY_SIZE=2048
COUNTRY="RU"
STATE="Moscow"
CITY="Moscow"
ORG="VkOrdApiWrapper"
OU="IT Department"
CN="localhost"

# Создаем конфигурационный файл для сертификата
cat > "$SSL_DIR/cert.conf" << EOF
[req]
default_bits = $KEY_SIZE
prompt = no
default_md = sha256
distinguished_name = dn
req_extensions = v3_req

[dn]
C=$COUNTRY
ST=$STATE
L=$CITY
O=$ORG
OU=$OU
CN=$CN

[v3_req]
basicConstraints = CA:FALSE
keyUsage = nonRepudiation, digitalSignature, keyEncipherment
subjectAltName = @alt_names

[alt_names]
DNS.1 = localhost
DNS.2 = *.localhost
DNS.3 = vkord-api
DNS.4 = *.vkord-api
IP.1 = 127.0.0.1
IP.2 = ::1
IP.3 = 0.0.0.0
EOF

echo "📝 Создан конфигурационный файл: $SSL_DIR/cert.conf"

# Генерируем приватный ключ
echo "🔑 Генерируем приватный ключ..."
openssl genrsa -out "$SSL_DIR/vkord-api.key" $KEY_SIZE

# Генерируем сертификат
echo "📜 Генерируем самоподписанный сертификат..."
openssl req -new -x509 -key "$SSL_DIR/vkord-api.key" -out "$SSL_DIR/vkord-api.crt" -days $CERT_DAYS -config "$SSL_DIR/cert.conf" -extensions v3_req

# Создаем PFX файл для .NET (если нужен)
echo "📦 Создаем PFX файл для .NET..."
openssl pkcs12 -export -out "$SSL_DIR/vkord-api.pfx" -inkey "$SSL_DIR/vkord-api.key" -in "$SSL_DIR/vkord-api.crt" -passout pass:

# Устанавливаем правильные права доступа
chmod 600 "$SSL_DIR/vkord-api.key"
chmod 644 "$SSL_DIR/vkord-api.crt"
chmod 600 "$SSL_DIR/vkord-api.pfx"

# Выводим информацию о сертификате
echo ""
echo "✅ SSL сертификат успешно создан!"
echo ""
echo "📁 Файлы сертификата:"
echo "   Приватный ключ: $SSL_DIR/vkord-api.key"
echo "   Сертификат:     $SSL_DIR/vkord-api.crt"
echo "   PFX файл:       $SSL_DIR/vkord-api.pfx"
echo "   Конфигурация:   $SSL_DIR/cert.conf"
echo ""
echo "📋 Информация о сертификате:"
openssl x509 -in "$SSL_DIR/vkord-api.crt" -text -noout | grep -E "(Subject:|Issuer:|Not Before:|Not After:|DNS:|IP Address:)"
echo ""
echo "🔧 Для использования в Docker добавьте volume mapping:"
echo "   volumes:"
echo "     - ./ssl:/app/ssl:ro"
echo ""
echo "⚠️  Это самоподписанный сертификат. Браузеры будут показывать предупреждение о безопасности."
echo "   Для обхода предупреждений используйте флаг -k в curl или игнорируйте SSL ошибки в коде."
echo ""
echo "🚀 Готово! Теперь можно запускать приложение с HTTPS."
