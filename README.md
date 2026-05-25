1.Установите .NET Framework 4.8: https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48
2.Проверить установилась ли версия: dotnet --version
3.Установите Git: https://git-scm.com/downloads
4.Проверить установилась ли версия: git --version
5.Установить PostgreSQL Server и pgAdmin4
6.В pgAdmin или через командную строку с помощью строки CREATE DATABASE education_db, создаётся новая БД 
7.Пользователь запускает SQL-скрипт из Приложения А в pgAdmin. Скрипт сам создаст таблицы и заполнит их начальными данными.
8.Пользователь запускает SQL-скрипт из Приложения Б в pgAdmin.
Скрипт сам добавляет тестовые данные в таблицы
9.В файле LoginForm.cs пользователь указывает параметры своей БД (порт, имя БД, логин, пароль).
10.Откройте командную строку (cmd) 
11.git clone https://github.com/alextim06/kursach228
12.Перейдите в каталог с файлом проекта: cd kursach228
13.Команда скомпилирует ваш проект и создаст исполняемый файл: dotnet build
