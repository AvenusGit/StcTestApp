using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StcTestRouter.Exceptions
{
    /// <summary>
    /// Базовое исключение маршрутизатора
    /// </summary>
    public class RouterException : Exception
    {
        public RouterException(string message)
        : base(message ?? "Ошибка маршрутизатора") { }
    }
    /// <summary>
    /// Исключение маршрутизатора при невозможности спарсить маршрут
    /// </summary>
    public class RouterParseException : RouterException
    {
        /// <summary>
        /// Исключение маршрутизатора при невозможности спарсить маршрут
        /// </summary>
        public RouterParseException()
        : base("Ошибка маршрутизатора: не удалось распознать строку шаблона маршрута") { }
    }
    /// <summary>
    /// Исключение маршрутизатора при отсутствии искомого маршрута
    /// </summary>
    public class RouterNotFoundExceptions : RouterException
    {
        /// <summary>
        /// Исключение маршрутизатора при отсутствии искомого маршрута
        /// </summary>
        public RouterNotFoundExceptions()
        : base("Ошибка маршрутизатора: не найден маршрут, соответствующий запросу") { }
    }
    /// <summary>
    /// Исключение маршрутизатора при попытке добавить маршрут при наличии в списке такого же маршрута
    /// </summary>
    public class RouteExistException : RouterException
    {
        /// <summary>
        /// Исключение маршрутизатора при попытке добавить маршрут при наличии в списке такого же маршрута
        /// </summary>
        public RouteExistException()
        : base("Ошибка маршрутизатора: не удалось добавить маршрут - маршрут с таким шаблоном и набором параметров уже существует") { }
    }
    /// <summary>
    /// Исключение маршрутизатора при несоответствии количества параметров у запроса и сопоставляемого маршрута
    /// </summary>
    public class RouterWrongParamsCountExceptions : RouterException
    {
        /// <summary>
        /// Исключение маршрутизатора при несоответствии количества параметров у запроса и сопоставляемого маршрута
        /// </summary>
        public RouterWrongParamsCountExceptions()
        : base("Ошибка маршрутизатора: несовпадение по количеству параметров маршрута и запроса") {}
    }
    /// <summary>
    /// Исключение маршрутизатора при несоответствии типов параметров у запроса и сопоставляемого маршрута
    /// </summary>
    public class RouterWrongParamsTypesExceptions : RouterException
    {
        /// <summary>
        /// Исключение маршрутизатора при несоответствии типов параметров у запроса и сопоставляемого маршрута
        /// </summary>
        public RouterWrongParamsTypesExceptions()
        : base("Ошибка маршрутизатора: несовпадение по типу параметров маршрута и запроса") {}
    }
}
