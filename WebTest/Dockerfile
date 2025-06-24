FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder 
# укажем директорию для нашего приложения внутри контейнера
WORKDIR /WebTest

# Скопируем все файлы из проекта в файловую систему контейнера
COPY . ./
# Запустим restore для загрузки зависимостей
RUN dotnet restore
# Опубликуем собранный dll в папку "output"
RUN dotnet publish -c Release -o output

# Теперь соберём образ, в котором наше приложение 
# будет запущено. Для запуска приложения достаточно
# среды выполнения aspnet, без sdk
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /Application
# Скопируем файлы приложения из предыдущего образа 
COPY --from=builder /WebTest/output .
# укажем команду, которая будет запускать приложение
ENTRYPOINT ["dotnet", "dotnet-app.dll"]