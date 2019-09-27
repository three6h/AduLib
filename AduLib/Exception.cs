using System;

namespace AduLib
{
    /// <summary>
    /// Исключение, которое выдается, если, при вызове утилиты, произошла необработанная ошибка.
    /// </summary>
    public class UnhandledErrorException : Exception
    {
        /// <summary>
        /// Получает аргументы, переданные утилите.
        /// </summary>
        public string Arguments { get; }

        /// <summary>
        /// Получает экземпляр класса System.Exception, который вызвал текущее исключение.
        /// </summary>
        public new Exception InnerException { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="UnhandledErrorException"/>
        /// с аргументами, переданные утилите.
        /// </summary>
        /// <param name="args">
        /// Аргументы, которые были переданы утилите.
        /// </param>
        public UnhandledErrorException(string args) : base($"Invalid Arguments: '{ args }'.")
        { Arguments = args; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="UnhandledErrorException"/>
        /// с экземпляром исключения, вызвавшего данное исключение.
        /// </summary>
        /// <param name="innerException">
        /// Исключение, вызвавшее текущее исключение.
        /// </param>
        public UnhandledErrorException(Exception innerException) :
            base($"An exception was raised: { innerException }.")
        { InnerException = innerException; }
    }

    /// <summary>
    /// Исключение, которое выдается, при попытке преобразовать синтаксически некорректный путь.
    /// </summary>
    public class PathInvalidException : Exception
    {
        /// <summary>
        /// Получает путь.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Получает экземпляр класса System.Exception, который вызвал текущее исключение.
        /// </summary>
        public new Exception InnerException { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PathInvalidException"/> c путем.
        /// </summary>
        /// <param name="path">
        /// Путь.
        /// </param>
        public PathInvalidException(string path) : base($"Path Invalid : '{ path }'.")
        { Path = path; }

        /// <summary>
        /// Инициализирует новый экземпляр класса Adu.PathInvalidException c путем
        /// и экземпляром исключения, вызвавшего данное исключение.
        /// </summary>
        /// <param name="path">
        /// Путь.
        /// </param>
        /// <param name="innerException">
        /// Исключение, вызвавшее текущее исключение.
        /// </param>
        public PathInvalidException(string path, Exception innerException)
            : base($"Path Invalid : '{ path }'. An exception was raised: { innerException }.", innerException)
        {
            Path = path;
            InnerException = innerException;
        }
    }
}