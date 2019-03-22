using System;
using System.Configuration;
using System.Runtime.CompilerServices;
using AVS.CoreLib.Infrastructure.Config;
using AVS.CoreLib._System;

namespace AVS.CoreLib.Infrastructure
{
    /// <summary>
    /// Provides access to the singleton instance of the Nop engine.
    /// </summary>
    public class EngineContext
    {
        #region Methods

        /// <summary>
        /// Initializes a static instance of the DefaultEngine
        /// Note: call it during application start
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Initialize(bool forceRecreate, IAppConfig config)
        {
            if (Singleton<IEngine>.Instance == null || forceRecreate)
            {
                Singleton<IEngine>.Instance = new DefaultEngine();
                Singleton<IEngine>.Instance.Initialize(config);
            }
            return Singleton<IEngine>.Instance;
        }

        /// <summary>
        /// Sets the static engine instance to the supplied engine. Use this method to supply your own engine implementation.
        /// </summary>
        /// <param name="engine">The engine to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the singleton Nop engine used to access Nop services.
        /// </summary>
        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                {
                    throw new NotInitializedEngineContextException();
                }
                return Singleton<IEngine>.Instance;
            }
        }

        #endregion
    }

    public class NotInitializedEngineContextException : ApplicationException
    {
        public NotInitializedEngineContextException() : base("EngineContext has not been initialized")
        {
        }
    }
}