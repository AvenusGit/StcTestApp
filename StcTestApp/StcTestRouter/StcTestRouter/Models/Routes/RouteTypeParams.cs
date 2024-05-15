using StcTestRouter.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StcTestRouter.Models.Routes
{
    /// <summary>
    /// Структура для хранения и описания типов параметров маршрута
    /// </summary>
    public struct RouteTypeParams
    {
        /// <summary>
        /// Структура для хранения и описания типов параметров маршрута
        /// <paramref name="types">Типы параметров маршрута</paramref>
        /// </summary>
        public RouteTypeParams(Type[] types)
        {
            Types = types ?? new Type[0];
        }

        /// <summary>
        /// Конструктор структуры описания типа параметров маршрута использующий для создания массив сегментов.
        /// Массив может быть двух видов: шаблоны параметров (например вида {a:int}) или сами значения.
        /// </summary>
        /// <param name="dynamicSegments">Массив строк сегментов</param>
        /// <param name="isTemplateSegments">Флаг, указывающий является ли каждый элемент входного массива шаблоном или значением</param>
        /// <exception cref="RouterParseException">Исключение, если не удалось распарсить входной массив</exception>
        public RouteTypeParams(string[] dynamicSegments, bool isTemplateSegments)
        {
            Types = new Type[dynamicSegments.Length];
            if (isTemplateSegments)
            {
                for (int i = 0; i < dynamicSegments.Length; i++)
                {
                    if (DynamicSegment.IsDynamicSegmentTemplate(dynamicSegments[i]))
                    {
                        DynamicSegment? dynamicSegment;
                        if (DynamicSegment.TryParse(dynamicSegments[i], out dynamicSegment))
                            Types[i] = dynamicSegment!.Type;
                        else throw new RouterParseException();
                    }
                    else throw new RouterParseException();
                }
            }
            else
            {
                for(int i = 0;i < dynamicSegments.Length;i++)
                {
                    Type? segmentType = DynamicSegment.GetTypeFromParamValue(dynamicSegments[i]);
                    if(segmentType is not null)
                        Types[i] = segmentType;
                    else throw new RouterParseException();
                }                
            }                            
        }

        /// <summary>
        /// Типы параметров маршрута
        /// </summary>
        public Type[] Types {  get; set; }

        public override bool Equals(object? obj)
        {
            if(obj is RouteTypeParams)
            {
                return Types.SequenceEqual(Types);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked // редкий случай, когда переполнение это хорошо
            {
                int hash = 283;
                for (int i = 0; i < Types.Length; i++)
                    hash = hash * 293 * Types[i].GetHashCode() * i;                  
                return hash;
            }            
        }
    }
}
