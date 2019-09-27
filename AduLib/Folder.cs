using System.Collections.Generic;
using System.IO;

namespace AduLib
{
    /// <summary>
    /// Представляет директорию и предоставляет методы экземпляра класса, для статичных методов
    /// класса <see cref="Function"/>. <see cref="Path"/> может отличаться от заданного пути,
    /// т.к путь преобразовывается. Этот класс не наследуется.
    /// </summary>
    public sealed class Folder
    {
        /// <summary>
        /// Получает абсолютный путь к директории.
        /// </summary>
        public string Path { get; }
        /// <summary>
        /// Получает список дисков под которыми смонтирована директория.
        /// </summary>
        public List<char> Drives { get { return Function.GetCouple(Path); } }

        /// <summary>
        /// Получает true, директория монтирована; иначе false.
        /// </summary>
        public bool IsMounted { get { return Function.IsMounted(Path); } }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Folder"/> с путем к директории.
        /// <see cref="Path"/> может отличаться от заданного пути, т.к заданный путь проходит
        /// через <see cref="Function.NormalizesPath(string)"/>.
        /// </summary>
        /// <param name="path">
        /// Абсолютный путь к директории.
        /// </param>
        /// <exception cref="DirectoryNotFoundException">
        /// Путь не указывает на директорию или не существует вовсе.
        /// </exception>
        /// <exception cref="PathInvalidException">
        /// Ошибка при преобразовании пути.
        /// </exception>
        public Folder(string path)
        {
            path = Function.NormalizesPath(path);

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Invalid Directory Path: '{ path }'");

            Path = path;
        }

        /// <summary>
        /// Монтирует директорию под заданным диском.
        /// </summary>
        /// <param name="drive">
        /// Диск. Формата: C, D, E. 
        /// </param>
        /// <exception cref="DriveNotFoundException">
        /// Диск уже используется.
        /// </exception>
        /// <exception cref="UnhandledErrorException">
        /// Необработанная ошибка.
        /// </exception>
        public void Mount(char drive)
        {
            Function.Mount(drive, Path);
        }

        /// <summary>
        /// Удаляет диск, если под ним монтирована директория.
        /// </summary>
        /// <param name="drive">
        /// Диск. Формата: C, D, E.
        /// </param>
        /// <exception cref="DriveNotFoundException">
        /// Директория Adu.Folder.Path не монтирована под заданным диском.
        /// </exception>
        /// <exception cref="UnhandledErrorException">
        /// Необработанная ошибка.
        /// </exception>
        public void Unmount(char drive)
        {
            if (Function.GetCouple(drive) == null)
                throw new DriveNotFoundException($"Drive Not Mounted: '{ drive }'.");

            Function.Unmount(drive);
        }

        /// <summary>
        /// Удаляет все диски, под которыми монтирована директория.
        /// </summary>
        /// <exception cref="UnhandledErrorException">
        /// Необработанная ошибка.
        /// </exception>
        public void UnmountAll()
        {
            Function.Unmount(Path);
        }

        /// <summary>
        /// Проверяет монтирована ли директория под заданным диском.
        /// </summary>
        /// <param name="drive">
        /// Диск. Формата: C, D, E.
        /// </param>
        /// <returns>
        /// true, директория <see cref="Path"/> монтированна под заданным диском; иначе false.
        /// </returns>
        public bool IsMountedUnder(char drive)
        {
            if (Function.GetCouple(Path).Contains(drive))
                return true;

            return false;
        }
    }
}
