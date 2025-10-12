#!/bin/bash
# ==============================================================================
#   Скрипт для создания или ПОЛНОЙ ПЕРЕУСТАНОВКИ службы systemd
#   для .NET приложения AdLawyerApi + службы туннеля clo
# ==============================================================================

# --- Конфигурация .NET приложения ---
SERVICE_NAME="adlawyer-api"
APP_SOURCE_PATH="/root/vkoridapiwrapper/publish"
EXEC_NAME="WebApp"
APP_ARGS=""
APP_URL="http://*:5000"
USER_FOR_SERVICE="www-data"

# --- Конфигурация CLO туннеля ---
CLO_BIN="/root/clo"
CLO_ARGS="publish http 5000"
CLO_SERVICE_NAME="clo-tunnel"
CLO_USER="root"   # пока бинарь в /root; можно переместить в /usr/local/bin и сменить пользователя

# Проверка root
if [ "$EUID" -ne 0 ]; then
  echo "Ошибка: запустите с sudo ./add-service.sh"
  exit 1
fi

# Проверка исходной директории приложения
if [ ! -d "$APP_SOURCE_PATH" ]; then
  echo "Ошибка: Исходная директория '$APP_SOURCE_PATH' не найдена."
  echo "Соберите проект:"
  echo "  cd /root/adlawyerapi/src/WebApp"
  echo "  dotnet publish -c Release -o ../../publish"
  exit 1
fi

SERVICE_FILE_PATH="/etc/systemd/system/$SERVICE_NAME.service"
DEST_PATH="/var/www/$SERVICE_NAME"

# 1. Удаление старой службы приложения (если есть)
if [ -f "$SERVICE_FILE_PATH" ]; then
  echo "--- Обнаружена существующая служба '$SERVICE_NAME'. Выполняется удаление... ---"
  systemctl stop "$SERVICE_NAME.service" || true
  systemctl disable "$SERVICE_NAME.service" || true
  rm -f "$SERVICE_FILE_PATH"
  systemctl daemon-reload
  rm -rf "$DEST_PATH"
  echo "--- Старая служба и файлы успешно удалены. ---"
  echo
fi

echo "--- Установка новой версии службы '$SERVICE_NAME' ---"
mkdir -p "$DEST_PATH"
cp -r "$APP_SOURCE_PATH"/* "$DEST_PATH/"
chown -R "$USER_FOR_SERVICE:$USER_FOR_SERVICE" "$DEST_PATH"
chmod +x "$DEST_PATH/$EXEC_NAME"

echo "Создание файла службы: $SERVICE_FILE_PATH..."
cat <<EOF > "$SERVICE_FILE_PATH"
[Unit]
Description=AdLawyer API Service - VK ORD API Wrapper
After=network.target

[Service]
Type=simple
WorkingDirectory=$DEST_PATH
ExecStart=$DEST_PATH/$EXEC_NAME $APP_ARGS
Restart=always
RestartSec=10
SyslogIdentifier=$SERVICE_NAME
User=$USER_FOR_SERVICE
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
Environment="ASPNETCORE_URLS=$APP_URL"

# Логирование
StandardOutput=journal
StandardError=journal

[Install]
WantedBy=multi-user.target
EOF

echo "Файл службы приложения создан."

# 2. Настройка службы CLO туннеля
CLO_SERVICE_FILE="/etc/systemd/system/${CLO_SERVICE_NAME}.service"

# Остановить и удалить старую службу CLO (если была)
if systemctl list-unit-files | grep -q "^${CLO_SERVICE_NAME}.service"; then
  echo "--- Обнаружена старая служба '$CLO_SERVICE_NAME'. Удаление... ---"
  systemctl stop "$CLO_SERVICE_NAME.service" || true
  systemctl disable "$CLO_SERVICE_NAME.service" || true
  rm -f "$CLO_SERVICE_FILE"
  systemctl daemon-reload
fi

# Проверка наличия бинаря CLO
if [ ! -x "$CLO_BIN" ]; then
  if [ -f "$CLO_BIN" ]; then
    chmod +x "$CLO_BIN"
  else
    echo "Предупреждение: Бинарь CLO не найден по пути $CLO_BIN. Пропускаю создание службы CLO."
  fi
fi

if [ -x "$CLO_BIN" ]; then
  echo "Создание службы CLO: $CLO_SERVICE_FILE..."
  cat <<EOF > "$CLO_SERVICE_FILE"
[Unit]
Description=CLO CloudPub tunnel for AdLawyer API (http 5000)
After=network-online.target
Wants=network-online.target
Requires=$SERVICE_NAME.service
After=$SERVICE_NAME.service

[Service]
Type=simple
ExecStart=$CLO_BIN $CLO_ARGS
Restart=always
RestartSec=5
User=$CLO_USER
WorkingDirectory=/root
# Перенаправить stdout/stderr в журнал:
StandardOutput=journal
StandardError=journal

[Install]
WantedBy=multi-user.target
EOF
  echo "Файл службы CLO создан."
fi

# 3. Применение и запуск служб
echo "Перезагрузка конфигурации systemd..."
systemctl daemon-reload

echo "Включение автозапуска и запуск .NET службы..."
systemctl enable "$SERVICE_NAME.service"
systemctl start "$SERVICE_NAME.service"

# Ждем запуска основного сервиса
sleep 3

if [ -f "$CLO_SERVICE_FILE" ]; then
  echo "Включение автозапуска и запуск CLO туннеля..."
  systemctl enable "$CLO_SERVICE_NAME.service"
  systemctl start "$CLO_SERVICE_NAME.service"
fi

echo
echo "--- Готово! ---"
echo "Служба '$SERVICE_NAME' установлена и запущена. API слушает $APP_URL."
if [ -f "$CLO_SERVICE_FILE" ]; then
  echo "Туннель '$CLO_SERVICE_NAME' запущен: публикует http 5000 (ссылку смотрите в journalctl -u $CLO_SERVICE_NAME -f)."
fi

# Показываем статусы
echo
echo "=== Статус службы $SERVICE_NAME ==="
systemctl status "$SERVICE_NAME.service" --no-pager

if [ -f "$CLO_SERVICE_FILE" ]; then
  echo
  echo "=== Статус службы $CLO_SERVICE_NAME ==="
  systemctl status "$CLO_SERVICE_NAME.service" --no-pager
fi

echo
echo "=== Полезные команды ==="
echo "Просмотр логов API: journalctl -u $SERVICE_NAME -f"
if [ -f "$CLO_SERVICE_FILE" ]; then
  echo "Просмотр логов туннеля: journalctl -u $CLO_SERVICE_NAME -f"
fi
echo "Перезапуск API: systemctl restart $SERVICE_NAME"
echo "Остановка API: systemctl stop $SERVICE_NAME"
echo "Проверка статуса: systemctl status $SERVICE_NAME"
