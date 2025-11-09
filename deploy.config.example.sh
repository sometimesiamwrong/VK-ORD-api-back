#!/bin/bash
# ==============================================================================
#   Пример конфигурации для deploy.sh
#   Скопируйте этот файл в deploy.config.sh и измените под ваше окружение
# ==============================================================================

# ВАЖНО: Не коммитьте deploy.config.sh в git!
# Этот файл может содержать чувствительные данные

# --- Общие настройки ---
# REPO_ROOT="/root/AdLawyerApi"
# DOTNET_VERSION="net8.0"
# ENVIRONMENT="Production"  # Production, Staging, Development

# --- Конфигурация WebApp ---
# WEBAPP_SERVICE_NAME="adlawyer-webapp"
# WEBAPP_PROJECT_PATH="${REPO_ROOT}/src/WebApp"
# WEBAPP_PUBLISH_DIR="${WEBAPP_PROJECT_PATH}/publish"
# WEBAPP_INSTALL_DIR="/var/www/adlawyer-webapp"
# WEBAPP_EXEC_NAME="WebApp"
# WEBAPP_PORT="5000"
# WEBAPP_URL="http://*:${WEBAPP_PORT}"
# WEBAPP_USER="www-data"

# --- Конфигурация Jobs ---
# JOBS_SERVICE_NAME="adlawyer-jobs"
# JOBS_PROJECT_PATH="${REPO_ROOT}/src/Jobs"
# JOBS_PUBLISH_DIR="${JOBS_PROJECT_PATH}/publish"
# JOBS_INSTALL_DIR="/var/www/adlawyer-jobs"
# JOBS_EXEC_NAME="Jobs"
# JOBS_PORT="5001"
# JOBS_URL="http://*:${JOBS_PORT}"
# JOBS_USER="www-data"

# --- Конфигурация CLO туннеля (опционально) ---
# ENABLE_CLO_TUNNEL=true
# CLO_BIN="/root/clo"
# CLO_WEBAPP_SERVICE_NAME="clo-webapp-tunnel"
# CLO_WEBAPP_PORT="${WEBAPP_PORT}"
# CLO_JOBS_SERVICE_NAME="clo-jobs-tunnel"
# CLO_JOBS_PORT="${JOBS_PORT}"
# CLO_USER="root"

# ==============================================================================
# ПРИМЕРЫ КОНФИГУРАЦИЙ ДЛЯ РАЗНЫХ ОКРУЖЕНИЙ
# ==============================================================================

# --- Production сервер ---
# REPO_ROOT="/var/apps/AdLawyerApi"
# ENVIRONMENT="Production"
# WEBAPP_PORT="5000"
# JOBS_PORT="5001"
# ENABLE_CLO_TUNNEL=true
# CLO_BIN="/usr/local/bin/clo"

# --- Staging сервер ---
# REPO_ROOT="/home/staging/AdLawyerApi"
# ENVIRONMENT="Staging"
# WEBAPP_SERVICE_NAME="adlawyer-webapp-staging"
# WEBAPP_PORT="6000"
# WEBAPP_INSTALL_DIR="/var/www/staging/adlawyer-webapp"
# JOBS_SERVICE_NAME="adlawyer-jobs-staging"
# JOBS_PORT="6001"
# JOBS_INSTALL_DIR="/var/www/staging/adlawyer-jobs"
# ENABLE_CLO_TUNNEL=false

# --- Development (локальная разработка) ---
# REPO_ROOT="/home/developer/AdLawyerApi"
# ENVIRONMENT="Development"
# WEBAPP_SERVICE_NAME="adlawyer-webapp-dev"
# WEBAPP_PORT="7000"
# WEBAPP_INSTALL_DIR="/home/developer/deployed/adlawyer-webapp"
# WEBAPP_USER="developer"
# JOBS_SERVICE_NAME="adlawyer-jobs-dev"
# JOBS_PORT="7001"
# JOBS_INSTALL_DIR="/home/developer/deployed/adlawyer-jobs"
# JOBS_USER="developer"
# ENABLE_CLO_TUNNEL=false
