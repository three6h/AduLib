# AduLib v0.7.3.4

### .NET Framework 4.5.1 - C# 7.3
AduLib - это библиотека для (ди)монтирования директорий под корневой каталог при помощи утилиты Subst, постовляемой вместе с OS Windows. Обращения к утилите происходит посредством локального процесса.

#### Примечание:

* Для того, чтобы процессы запущеные от имени администратора могли видеть и работать с каталогом созданым при помощи AduLib, нужно чтобы и директория была смонтированна с соответствующими права доступа.

* Утилита Subst, требует чтобы исходные данные передаваемые ей, были определенного формата. Например "C:" должен быть отформатирован в "C:\", a "С:\Windows\" в "С:\Windows", регистр при этом роли не играет. По этой причине путь передаваемый конструктуру класса AduLib.Folder будет отличать от его свойства AduLib.Folder.Path. Получить преобразованный путь можно при помощи метода AduLib.Function.NormalizesPath(string).
