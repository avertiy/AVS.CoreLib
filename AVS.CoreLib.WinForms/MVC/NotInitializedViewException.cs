using System;

namespace AVS.CoreLib.WinForms.MVC
{
    public class NotInitializedViewException : ApplicationException
    {
        public NotInitializedViewException(string view) : base($"{view} was not initialized")
        {
        }
    }
}