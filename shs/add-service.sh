SERVICE_NAME="vkoridapiwrapper"
APP_SOURCE_PATH="/root/vkoridapiwrapper/publish"
EXEC_NAME="VkOrdApiWrapper"
# Используем переменную окружения для URL, т.к. она имеет высокий приоритет
APP_ARGS="" # Аргументы командной строки можно оставить пустыми
APP_URL="http://*:5000"
USER_FOR_SERVICE="www-data"
# --- Конец конфигурации ---


# Проверка, что скрипт запущен от имени суперпользователя (root)
if [ "$EUID" -ne 0 ]; then
  echo "Ошибка: Пожалуйста, запустите этот скрипт с правами суперпользователя (sudo ./add-service.sh)"
  exit 1
fi

# Проверка существования исходной директории
if [ ! -d "$APP_SOURCE_PATH" ]; then
    echo "Ошибка: Исходная директория '$APP_SOURCE_PATH' не найдена."
    echo "Убедитесь, что вы выполнили команду 'dotnet publish -c Release -o ./publish' в папке проекта."
    exit 1
fi

# --- Начало операций ---

SERVICE_FILE_PATH="/etc/systemd/system/$SERVICE_NAME.service"
DEST_PATH="/var/www/$SERVICE_NAME"

# 1. ПРОВЕРКА И УДАЛЕНИЕ СТАРОЙ СЛУЖБЫ (если существует)
if [ -f "$SERVICE_FILE_PATH" ]; then
    echo "--- Обнаружена существующая служба '$SERVICE_NAME'. Выполняется удаление... ---"

    # Останавливаем службу
    echo "1.1. Остановка службы..."
    systemctl stop "$SERVICE_NAME.service"

    # Отключаем автозапуск
    echo "1.2. Отключение автозапуска..."
    systemctl disable "$SERVICE_NAME.service"

    # Удаляем файл службы
    echo "1.3. Удаление файла службы..."
    rm "$SERVICE_FILE_PATH"

    # Перезагружаем конфигурацию systemd
    echo "1.4. Перезагрузка конфигурации systemd..."
    systemctl daemon-reload

    # Удаляем старую директорию с приложением
    echo "1.5. Удаление старых файлов приложения из $DEST_PATH..."
    rm -rf "$DEST_PATH"

    echo "--- Старая служба и файлы успешно удалены. ---"
    echo
fi

echo "--- Установка новой версии службы '$SERVICE_NAME' ---"

# 2. Создание директории назначения
echo "2. Создание директории: $DEST_PATH..."
mkdir -p "$DEST_PATH"

# 3. Копирование файлов приложения
echo "3. Копирование файлов приложения..."
cp -r "$APP_SOURCE_PATH"/* "$DEST_PATH/"

# 4. Установка прав доступа
echo "4. Установка прав доступа..."
chown -R "$USER_FOR_SERVICE:$USER_FOR_SERVICE" "$DEST_PATH"
chmod +x "$DEST_PATH/$EXEC_NAME"

# 5. Создание файла службы systemd
echo "5. Создание файла службы: $SERVICE_FILE_PATH..."
cat <<EOF > "$SERVICE_FILE_PATH"
[Unit]
Description=Служба для .NET приложения $SERVICE_NAME
After=network.target

[Service]
WorkingDirectory=$DEST_PATH
ExecStart=$DEST_PATH/$EXEC_NAME $APP_ARGS
Restart=always
RestartSec=10
SyslogIdentifier=$SERVICE_NAME
User=$USER_FOR_SERVICE
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
Environment="ASPNETCORE_URLS=$APP_URL"

[Install]
WantedBy=multi-user.target
EOF

echo "Файл службы успешно создан."

# 6. Перезагрузка, включение и запуск службы
echo "6. Управление службой через systemd..."
systemctl daemon-reload
systemctl enable "$SERVICE_NAME.service"
systemctl start "$SERVICE_NAME.service"

echo
echo "--- Готово! ---"
echo "Служба '$SERVICE_NAME' успешно установлена и запущена."
echo "Приложение должно быть доступно на порту, указанном в APP_URL ($APP_URL)."
echo

# Показываем финальный статус
systemctl status "$SERVICE_NAME.service"