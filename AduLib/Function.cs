using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AduLib
{
    /// <summary>
    /// Предоставляет статические методы для создания и удаления виртуального диска. Получаемый
    /// виртуальный диск предоставляет информацию из заданной директории. Все действия производятся,
    /// при помощи вызовов к встроенной утилите "Subst". Этот класс не наследуется.
    /// </summary>
    public static class Function
    {
        /// <summary>
        /// Предоставляет доступ для создания и запуска процесса, вызывающего утилиту.
        /// </summary>
        static readonly Process proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "subst.exe",
                RedirectStandardOutput = true,

                /* Для того, чтобы не мешать основному процессу.
                 * Например, без этих настроек, вызов API мешает основному процессу в Windows Forms.
                 */
                CreateNoWindow = true,
                UseShellExecute = false
            }
        };

        /// <summary>
        /// Вызывает утилиту с заданными аргументами, возвращает ее выходные данные.
        /// </summary>
        /// <param name="args">
        /// Аргументы, передающиеся утилите.
        /// </param>
        /// <exception cref="UnhandledErrorException">
        /// Eсли утилита вернула значение отличное 0, <see cref="UnhandledErrorException.Arguments"/>
        /// получает переданные аргументы. Иначе <see cref="UnhandledErrorException.InnerException"/>
        /// содержит экземпляр исключения вызвавший ошибку.
        /// </exception>
        /// <returns>
        /// Список строк текстовых выходных данных. Каждый элемент отдельная строка.
        /// </returns>
        static List<string> Call(string args = "")
        {
            proc.StartInfo.Arguments = args;

            try
            {
                proc.Start();
                proc.WaitForExit();

                if (proc.ExitCode != 0)
                    throw new UnhandledErrorException(args);

                var rows = new List<string>();

                var row = proc.StandardOutput.ReadLine();
                while (row != null)
                {
                    rows.Add(row);
                    row = proc.StandardOutput.ReadLine();
                }
                return rows;
            }
            catch (Exception e) { throw new UnhandledErrorException(e); }
        }

        /// <summary>
        /// Преобразует путь в форму, понятную утилите. Например: «C:» в «C:\» и «C:\Users \» в
        /// «C:\Users» и переводит букву диска в верхний регистр.
        /// </summary>
        /// <param name="path">
        /// Путь.
        /// </param>
        /// <exception cref="PathInvalidException">
        /// При преобразовании была выдана одна из исключений:
        /// System.ArgumentException: path представляет собой строку нулевой длины, содержит только
        /// пробелы или содержит недопустимые символы.
        /// System.Security.SecurityException: У вызывающего объекта отсутствуют необходимые
        /// разрешения.
        /// System.ArgumentNullException: path имеет значение null.
        /// System.NotSupportedException: path содержит двоеточие «:», которое не является частью
        /// идентификатора тома (например, «c: \»).
        /// System.IO.PathTooLongException: path превышает максимальную длину пути.
        /// </exception>
        /// <returns>
        /// Строка преобразованного пути.
        /// </returns>
        public static string NormalizesPath(string path)
        {
            try { path = Path.GetFullPath(path); }

            catch (Exception e) { throw new PathInvalidException(path, e); }

            if (path.Length > 3)
                path = path.TrimEnd(@"\ /".ToCharArray());

            return path;
        }

        /// <summary>
        /// Возвращает пары ключ/значение где, ключ - диск; значение - директория.
        /// </summary>
        /// <exception cref="UnhandledErrorException">
        /// Необработанная ошибка.
        /// </exception>
        /// <returns>
        /// Пары ключ/значение. Диск, формата: C, D, E. Директория - абсолютный путь к директории.
        /// </returns>
        public static Dictionary<char, string> GetMounted()
        {
            var mounteds = new Dictionary<char, string>();

            foreach (var row in Call())
                if (row.Length != 0)
                    mounteds.Add(row[0], row.Substring(8));

            return mounteds;
        }

        /// <summary>
        /// Возвращает список дисков под которыми монтирована директория.
        /// </summary>
        /// <param name="path">
        /// Абсолютный путь к директории.
        /// </param>
        /// <exception cref="PathInvalidException">
        /// Ошибка при преобразовании пути.
        /// </exception>
        /// <exception cref="UnhandledErrorException">
        /// Необработанная ошибка.
        /// </exception>
        /// <returns>
        /// Список дисков. Формата: C, D, E. 
        /// </returns>
        public static List<char> GetCouple(string path)
        {
            path = NormalizesPath(path).ToLower();
            var drives = new List<char>();

            foreach (var row in Call())
                if (row.Substring(8).ToLower() == path)
                    drives.Add(row[0]);

            return drives;
        }

        /// <summary>
        /// Возвращает директорию, которая монтирована под заданным диском.
        /// </summary>
        /// <param name="drive">
        /// Диск. Формата: C, D, E. 
        /// </param>
        /// <exception cref="UnhandledErrorException">
        /// Необработанная ошибка.
        /// </exception>
        /// <returns>
        /// Строка абсолютного пути к директории; null, если такого нет.
        /// </returns>
        public static string GetCouple(char drive)
        {
            foreach (var row in Call())
                if (row[0] == drive)
                    return row.Substring(8);

            return null;
        }

        /// <summary>
        /// Возвращает список созданных дисков.
        /// </summary>
        /// <exception cref="UnhandledErrorException">
        /// Необработанная ошибка.
        /// </exception>
        /// <returns>
        /// Список дисков. Формата: C, D, E. 
        /// </returns>
        public static List<char> GetMountedDrives()
        {
            var drives = new List<char>();

            foreach (var row in Call())
                drives.Add(row[0]);

            return drives;
        }

        /// <summary>
        /// Возвращает список монтированных директорий.
        /// </summary>
        /// <exception cref="UnhandledErrorException">
        /// Необработанная ошибка.
        /// </exception>
        /// <returns>
        /// Список строк абсолютных путей к директории.
        /// </returns>
        public static List<string> GetMountedPaths()
        {
            var drives = new List<string>();

            foreach (var row in Call())
                drives.Add(row.Substring(8));

            return drives;
        }

        /// <summary>
        /// Проверяет корректно ли работает утилита.
        /// </summary>
        /// <returns>
        /// true, утилита работает корректно; иначе false.
        /// </returns>
        public static bool IsWorks()
        {
            try { Call(); }

            catch (UnhandledErrorException) { return false; }

            return true;
        }

        /// <summary>
        /// Проверяет монтирована ли директория.
        /// </summary>
        /// <param name="path">
        /// Абсолютный путь к директории.
        /// </param>
        /// <exception cref="PathInvalidException">
        /// Ошибка при преобразовании пути.
        /// </exception>
        /// <exception cref="UnhandledErrorException">
        /// Необработанная ошибка.
        /// </exception>
        /// <returns>
        /// true, директория монтирована; иначе false.
        /// </returns>
        public static bool IsMounted(string path)
        {
            path = NormalizesPath(path).ToLower();

            foreach (var row in Call())
                if (row.Substring(8).ToLower() == path)
                    return true;

            return false;
        }

        /// <summary>
        /// Проверяет создан ли диск.
        /// </summary>
        /// <param name="drive">
        /// Диск. Формата: C, D, E. 
        /// </param>
        /// <exception cref="UnhandledErrorException">
        /// Необработанная ошибка.
        /// </exception>
        /// <returns>
        /// true, если диск был создан; иначе false.
        /// </returns>
        public static bool IsMounted(char drive)
        {
            foreach (var row in Call())
                if (row[0] == drive)
                    return true;

            return false;
        }

        /// <summary>
        /// Монтирует заданную директорию под заданным диском.
        /// </summary>
        /// <param name="drive">
        /// Диск. Формата: C, D, E.
        /// </param>
        /// <param name="path">
        /// Абсолютный путь к директории.
        /// </param>
        /// <exception cref="DriveNotFoundException">
        /// Диск уже используется.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// Путь не указывает на директорию.
        /// </exception>
        /// <exception cref="PathInvalidException">
        /// Ошибка при преобразовании пути.
        /// </exception>
        /// <exception cref="UnhandledErrorException">
        /// Необработанная ошибка.
        /// </exception>
        public static void Mount(char drive, string path)
        {
            if (Array.Exists(System.IO.DriveInfo.GetDrives(), i => i.Name[0] == drive))
                throw new DriveNotFoundException($"Drive Used: '{ drive }'.");

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Invalid Directory Path: '{ path }'.");

            path = NormalizesPath(path);

            Call($"{ drive }: \"{ path }\"");
        }

        /// <summary>
        /// Удаляет все диски, под которыми монтирована заданная директория.
        /// </summary>
        /// <param name="path">
        /// Абсолютный путь к директории.
        /// </param>
        /// <exception cref="PathInvalidException">
        /// Ошибка при преобразовании пути.
        /// </exception>
        /// <exception cref="UnhandledErrorException">
        /// Необработанная ошибка.
        /// </exception>
        public static void Unmount(string path)
        {
            path = NormalizesPath(path).ToLower();

            foreach (var row in Call())
                if (row.Substring(8).ToLower() == path)
                    Call($"{ row[0] }: /d");
        }

        /// <summary>
        /// Удаляет заданный диск.
        /// </summary>
        /// <param name="drive">
        /// Диск. Формата: C, D, E.
        /// </param>
        /// <exception cref="DriveNotFoundException">
        /// Диск не был создан.
        /// </exception>
        /// <exception cref="UnhandledErrorException">
        /// Необработанная ошибка.
        /// </exception>
        public static void Unmount(char drive)
        {
            if (!GetMountedDrives().Contains(drive))
                throw new DriveNotFoundException($"Drive not mounted: '{ drive }'.");

            Call($"{ drive }: /d");
        }
    }
}
