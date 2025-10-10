# 🔍 Yandex Search App

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Docker](https://img.shields.io/badge/Docker-Enabled-blue.svg)](https://www.docker.com/)
[![Selenium](https://img.shields.io/badge/Selenium-4.24.0-green.svg)](https://selenium.dev/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## 📋 Описание проекта

**Yandex Search App** — это веб-приложение для автоматизированного поиска и анализа результатов в поисковой системе Яндекс. Приложение использует Selenium WebDriver для автоматизации браузера и извлечения данных с веб-страниц.

### 🎯 Основные возможности

- **Автоматизированный поиск** в Яндексе с настраиваемыми параметрами
- **Анализ содержимого страниц** с подсчетом вхождений заданного паттерна
- **Экспорт результатов** в формате JSON
- **Измерение времени загрузки** каждой страницы
- **Поддержка пагинации** для получения больших объемов данных
- **Docker-контейнеризация** для простого развертывания

## 🏗️ Архитектура

Проект построен с использованием **Clean Architecture** принципов:

```
📁 src/
├── 🎮 Controllers/          # Слой представления (MVC)
│   └── SearchController.cs
├── 🏢 Models/              # Модели данных
│   ├── SearchRequest.cs
│   └── SearchResult.cs
├── ⚙️ Services/            # Бизнес-логика
│   └── SearchService.cs
├── 🎨 Views/               # Razor представления
│   └── Search/
└── 🌐 wwwroot/             # Статические ресурсы
```

### 🧩 Компоненты системы

- **SearchController** — обработка HTTP-запросов и управление потоком данных
- **SearchService** — основная бизнес-логика поиска и анализа
- **Selenium WebDriver** — автоматизация браузера Chrome
- **Docker Compose** — оркестрация контейнеров

## 🚀 Технологический стек

### Backend
- **.NET 8.0** — основная платформа
- **ASP.NET Core MVC** — веб-фреймворк
- **Selenium WebDriver 4.24.0** — автоматизация браузера
- **ChromeDriver** — драйвер для Chrome

### Frontend
- **Razor Pages** — серверный рендеринг
- **Bootstrap** — CSS-фреймворк
- **jQuery** — JavaScript библиотека

### DevOps
- **Docker** — контейнеризация
- **Docker Compose** — оркестрация сервисов
- **Selenium Grid** — распределенное выполнение тестов

## 🛠️ Установка и запуск

### Предварительные требования

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

### Локальная разработка

1. **Клонирование репозитория**
   ```bash
   git clone <repository-url>
   cd yandex-search
   ```

2. **Установка зависимостей**
   ```bash
   cd src
   dotnet restore
   ```

3. **Настройка конфигурации**
   
   Создайте файл `appsettings.Development.json`:
   ```json
   {
     "YandexSearchUrl": "https://yandex.ru/search/",
     "SELENIUM_HUB_URL": "http://localhost:4444/wd/hub"
   }
   ```

4. **Запуск Selenium Hub**
   ```bash
   docker run -d -p 4444:4444 selenium/standalone-chrome:latest
   ```

5. **Запуск приложения**
   ```bash
   dotnet run
   ```

### Docker-развертывание

1. **Сборка и запуск всех сервисов**
   ```bash
   docker-compose up --build
   ```

2. **Доступ к приложению**
   
   Откройте браузер и перейдите по адресу: `http://localhost:5400`

## 📖 Использование

### Основные параметры поиска

| Параметр | Описание | Пример |
|----------|----------|---------|
| **Search Keyword** | Ключевое слово для поиска | "ASP.NET Core" |
| **Start Index** | Начальный индекс результатов | 0 |
| **Page Size** | Количество результатов на странице | 10 |
| **Search Pattern** | Паттерн для поиска на страницах | "Microsoft" |

### Пример использования

1. Введите ключевое слово для поиска
2. Укажите количество результатов (Page Size)
3. При необходимости задайте паттерн для анализа
4. Нажмите "Search"
5. Просмотрите результаты и экспортируйте в JSON при необходимости

## 🔧 Конфигурация

### Переменные окружения

```bash
# URL поисковой системы Яндекс
YandexSearchUrl=https://yandex.ru/search/

# URL Selenium Hub
SELENIUM_HUB_URL=http://selenium:4444/wd/hub

# Окружение ASP.NET Core
ASPNETCORE_ENVIRONMENT=Production
```

### Настройки Chrome WebDriver

Приложение использует следующие настройки Chrome для обхода защиты от автоматизации:

- `--headless` — безголовый режим
- `--disable-blink-features=AutomationControlled` — отключение детекции автоматизации
- `--no-sandbox` — отключение sandbox для Docker
- `--disable-dev-shm-usage` — оптимизация для контейнеров

## 📊 Структура данных

### SearchRequest
```csharp
public class SearchRequest
{
    public string? SearchKeyword { get; set; }    // Ключевое слово
    public int StartIndex { get; set; }           // Начальный индекс
    public int PageSize { get; set; }             // Размер страницы
    public string? SearchPattern { get; set; }    // Паттерн поиска
}
```

### SearchResult
```csharp
public class SearchResult
{
    public string? RequestedUrl { get; set; }     // URL страницы
    public string? PageTitle { get; set; }        // Заголовок страницы
    public double TimeTaken { get; set; }         // Время загрузки (сек)
    public int SearchPatternCount { get; set; }   // Количество вхождений паттерна
}
```

## 🧪 Тестирование

### Запуск тестов
```bash
# Локальное тестирование
dotnet test

# Тестирование в Docker
docker-compose -f docker-compose.test.yml up --build
```

### Примеры тестовых сценариев

1. **Поиск по ключевому слову** — проверка корректности извлечения результатов
2. **Анализ паттернов** — проверка подсчета вхождений
3. **Экспорт данных** — проверка генерации JSON
4. **Обработка ошибок** — проверка обработки недоступных страниц

## 🚀 Производительность

### Оптимизации

- **Асинхронная загрузка** — использование WebDriverWait для ожидания элементов
- **Пакетная обработка** — группировка запросов для снижения нагрузки
- **Кэширование результатов** — возможность сохранения результатов поиска
- **Headless режим** — снижение потребления ресурсов

### Мониторинг

- Измерение времени выполнения каждого запроса
- Логирование ошибок и исключений
- Отслеживание использования ресурсов

## 🔒 Безопасность

### Рекомендации по безопасности

- **Ротация User-Agent** — использование различных заголовков браузера
- **Задержки между запросами** — имитация человеческого поведения
- **Обработка CAPTCHA** — детекция и обработка защитных механизмов
- **Валидация входных данных** — проверка параметров поиска

## 📈 Масштабирование

### Горизонтальное масштабирование

- **Selenium Grid** — распределение нагрузки между несколькими узлами
- **Kubernetes** — оркестрация контейнеров в кластере
- **Load Balancer** — балансировка нагрузки между экземплярами

### Вертикальное масштабирование

- **Увеличение ресурсов** — CPU и память для контейнеров
- **Оптимизация запросов** — кэширование и пулинг соединений

## 🤝 Вклад в проект

### Процесс разработки

1. **Fork** репозитория
2. Создайте **feature branch** (`git checkout -b feature/amazing-feature`)
3. **Commit** изменения (`git commit -m 'Add amazing feature'`)
4. **Push** в branch (`git push origin feature/amazing-feature`)
5. Создайте **Pull Request**

### Стандарты кода

- Следование принципам **SOLID**
- Использование **async/await** для асинхронных операций
- **Unit тесты** для бизнес-логики
- **XML документация** для публичных API

## 📝 Лицензия

Этот проект распространяется под лицензией MIT.
## 👥 Авторы

- **Fullstack Developer** — *Архитектура и разработка* — [GitHub](https://github.com/JmihPojiloy)

## 📞 Контакты

Если у вас есть вопросы или предложения:

- **GitHub Issues**: [Создать issue](https://github.com/yourusername/pojiloy-robot/issues)
- **Email**: dmitry.podschipkov@mail.ru
- **LinkedIn**: [Профиль](www.linkedin.com/in/dpjmpj)

---

<div align="center">

**Сделано с ❤️ для автоматизации поиска**

[🔝 Наверх](#-yandex-search-app)

</div>