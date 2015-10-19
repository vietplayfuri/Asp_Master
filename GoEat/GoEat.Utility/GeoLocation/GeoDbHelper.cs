using System;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace GoEat.Utility
{
    public class GeoDbHelper
    {

        #region Singleton
        private GeoDbHelper() { }

        public static readonly GeoDbHelper Instance;

        #endregion

    }
}
