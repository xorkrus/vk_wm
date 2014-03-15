using Galssoft.VKontakteWM.Components;

namespace Galssoft.VKontakteWM.Common
{
    /// <summary>
    /// Класс для инкапсуляции синглтонов.
    /// Сами синглтоны объявлены нестатическими, 
    /// т.к. их нужно тестировать, и может понадобиться иерархия наследования/выделение интерфейсов
    /// </summary>
    internal static class Globals
    {
        // Объект основной бизнес-логики приложения
        public static BaseLogic BaseLogic { get; private set; }
        
        /// <summary>
        /// Инициализация класса. Не использован статический конструктор, т.к. принято решение
        /// инстанцировать синглтоны из одного места (сейчас в Program.cs, в перспективе может добавиться 
        /// инициализация в тестовых методах
        /// </summary>
        /// <param name="baseLogic"></param>
        public static void Init(BaseLogic baseLogic)
        {
            BaseLogic = baseLogic;
        }
    }
}
