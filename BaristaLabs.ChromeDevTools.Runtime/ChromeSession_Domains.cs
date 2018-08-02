namespace BaristaLabs.ChromeDevTools.Runtime
{
    using System;
    
    public partial class ChromeSession
    {
        private Lazy<Runtime.RuntimeAdapter> m_Runtime;

        public ChromeSession()
        {
            m_Runtime = new Lazy<Runtime.RuntimeAdapter>(() => new Runtime.RuntimeAdapter(this));
        }

        /// <summary>
        /// Gets the adapter for the Runtime domain.
        /// </summary>
        public Runtime.RuntimeAdapter Runtime
        {
            get { return m_Runtime.Value; }
        }
    }
}
