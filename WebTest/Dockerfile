# ===== 1. Этап сборки =====
# Используем образ SDK для компиляции приложения
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем ТОЛЬКО файлы проектов (для кэширования зависимостей)
COPY *.csproj .
RUN dotnet restore

# Копируем ВЕСЬ код и собираем приложение
COPY . .
RUN dotnet publish -c Release -o /app

# ===== 2. Этап запуска =====
# Используем образ только с рантаймом (без SDK, чтобы уменьшить размер)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Копируем собранное приложение из предыдущего этапа
COPY --from=build /app .

# Открываем порты (Kestrel по умолчанию слушает 80 и 443)
ENV ASPNETCORE_URLS=http://+:5160
EXPOSE 5160

# Запускаем приложение
ENTRYPOINT ["dotnet", "WebTest.dll"]