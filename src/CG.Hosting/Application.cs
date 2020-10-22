using System.Diagnostics;

namespace CG.Hosting
{
    /// <summary>
    /// This class represents a singleton implementation of the <see cref="IApplication"/>
    /// interface.
    /// </summary>
    public sealed class Application : SingletonBase<Application>, IApplication
    {
        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="Application"/>
        /// class.
        /// </summary>
        [DebuggerStepThrough]
        private Application() { }

        #endregion
    }
}
