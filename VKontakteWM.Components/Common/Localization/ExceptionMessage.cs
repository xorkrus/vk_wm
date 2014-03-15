namespace Galssoft.VKontakteWM.Components.Common.Localization
{
    /// <summary>
    /// Локализованные сообщения об ошибках
    /// </summary>
    public enum ExceptionMessage
    {
        // Обязательно для элементов перечисления указывать численное значение и комментарий (по возможности, совпадающий с русским вариантом локализованной строки)
        // Это значение будет являться идентификатором строки в локализованных ресурсах
        // По мере надобности сюда добавлять новые члены

        UnknownError = 0,                   //Неизвестная ошибка
        ServiceStartError = 1,              //Ошибка запуска сервиса
        NonstandardServerError = 2,         //Нестандартная ошибка на сервере, повторите операцию
        ServerUnavalible = 3,               //Сервер временно недоступен
        NoSavedToken = 5,                   //Токен авторизации не сохранён
        IncorrectLoginOrPassword = 6,       //Неверный логин или пароль
        NoConnection = 7,                   //Нет интернета
        FloodControl = 8,                   //Флуд-контроль
        OperationIsProhibitedByPrivacy = 9, //Операция запрещена приватностью
        AccountBloked = 10,                 //Аккаунт временно заблокирован
        UnsuccessfullOperation = 11,        //Операция прошла неуспешно
        NoLinkedApplication = 12            //Приложение не привязано на сайте vkontakte.ru
    }
}
