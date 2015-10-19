using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProxy.Core
{
    public partial class ProxyApi
    {
         #region Singleton
        private ProxyApi() { }

        public static readonly ProxyApi Instance;

        static ProxyApi()
        {
            Instance = new ProxyApi();
        }
        #endregion
    }
}
