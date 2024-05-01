using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StcTestRouter.Interfaces
{
    /// <summary>
    /// Интерфейс маршрутизатора
    /// </summary>
    public interface IRouter
    {
        /// <summary>
        /// Зарегистрировать маршрут без параметров
        /// </summary>
        /// <param name="template">Строка шаблона</param>
        /// <param name="action">Действие, которое необходимо выполнить при сопоставлении маршрута с шаблоном</param>
        void RegisterRoute(string template, Action action);
        /// <summary>
        /// Зарегистрировать маршрут с одним параметром
        /// </summary>
        /// <typeparam name="T">Тип параметра маршрута</typeparam>
        /// <param name="template">Строка шаблона</param>
        /// <param name="action">Действие, которое необходимо выполнить при сопоставлении маршрута с шаблоном</param>
        void RegisterRoute<T>(string template, Action<T> action);
        /// <summary>
        /// Зарегистрировать маршрут с двумя параметрами
        /// </summary>
        /// <typeparam name="T1">Тип первого параметра маршрута</typeparam>
        /// <typeparam name="T2">Тип второго параметра маршрута</typeparam>
        /// <param name="template">Строка шаблона</param>
        /// <param name="action">Действие, которое необходимо выполнить при сопоставлении маршрута с шаблоном</param>
        void RegisterRoute<T1,T2>(string template, Action<T1,T2> action);
        /// <summary>
        /// Вызвать метод соответствующий маршруту
        /// </summary>
        /// <param name="route">Строка маршрута</param>
        void Route(string route);
    }
}
