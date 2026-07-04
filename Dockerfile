# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# نسخ ملفات المشروع
COPY . .

# Restore + Build + Publish
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

# مهم: غيّر اسم الـ dll حسب مشروعك
ENTRYPOINT ["dotnet", "MedicalSpace.PdfApi.dll"]