using System;

using System.Collections.Generic;
using System.Text;
using Galssoft.VKontakteWM.Components.Common.Localization;
using Galssoft.VKontakteWM.Properties;

namespace Galssoft.VKontakteWM.Common
{
    public static class ExceptionTranslation
    {
        public static string TranslateException(VKException vkex)
        {
            string message = string.Empty;

            switch (vkex.LocalizedMessage)
            {
                    // 0
                case ExceptionMessage.UnknownError:
                    message = Resources.VK_ERRORS_UnknownError;
                    break;

                    // 1
                case ExceptionMessage.ServiceStartError:
                    message = Resources.VK_ERRORS_ServiceStartError;
                    break;

                    // 2
                case ExceptionMessage.NonstandardServerError:
                    message = Resources.VK_ERRORS_NonstandardServerError;
                    break;

                    // 3
                case ExceptionMessage.ServerUnavalible:
                    message = Resources.VK_ERRORS_ServerUnavalible;
                    break;

                    // 4

                    // 5
                case ExceptionMessage.NoSavedToken:
                    message = Resources.VK_ERRORS_NoSavedToken;
                    break;

                    // 6
                case ExceptionMessage.IncorrectLoginOrPassword:
                    message = Resources.VK_ERRORS_IncorrectLoginOrPassword;
                    break;

                    // 7
                case ExceptionMessage.NoConnection:
                    message = Resources.VK_ERRORS_NoConnection;
                    break;

                    // 8
                case ExceptionMessage.FloodControl:
                    message = Resources.VK_ERRORS_FloodControl;
                    break;

                    // 9
                case ExceptionMessage.OperationIsProhibitedByPrivacy:
                    message = Resources.VK_ERRORS_OperationIsProhibitedByPrivacy;
                    break;

                    // 10
                case ExceptionMessage.AccountBloked:
                    message = Resources.VK_ERRORS_AccountBloked;
                    break;     
           
                    //11
                case ExceptionMessage.UnsuccessfullOperation:
                    message = Resources.VK_ERRORS_UnsuccessfullOperation;
                    break;
           
                    //12
                case ExceptionMessage.NoLinkedApplication:
                    message = Resources.VK_ERRORS_NoLinkedApplication;
                    break;
            }

            return message;            
        }
    }
}
