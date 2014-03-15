using System;

namespace Galssoft.VKontakteWM.Components.Common.Localization
{
    /// <summary>
    /// Этот класс исключения должен использоваться ПОЧТИ ВЕЗДЕ в проекте одноклассники,
    /// т.к. в нём указываются локализованные сообщения об ошибках.
    /// По крайней мере, когда исключение вылетает на уровень GUI, оно должно быть этого типа,
    /// чтобы пользователь увидел локализованное сообщение об ошибке
    /// 
    /// Этот класс ещё будет развиваться (например, для добавления возможности использовать локализованные строки с параметрами)
    /// </summary>  
    public class VKException: Exception
    {
        private ExceptionMessage _localizedMessage;
        
        public ExceptionMessage LocalizedMessage { get { return _localizedMessage; } }

        public VKException(ExceptionMessage localizedMessage)
        {
            _localizedMessage = localizedMessage;
        }

        public VKException(ExceptionMessage localizedMessage, string message): base(message)
        {
            _localizedMessage = localizedMessage;
        }
    }

    public class CacheException : Exception
    {
        public CacheException(string message)
            : base(message)
        {
        }
    }
    public class CorrException : Exception
    {
        public CorrException()
            : base()
        {
        }

    }
}
