using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoPlay.Web.Helpers
{
    public enum FlashLevel
    {
        Primary = 1,
        Success = 2,
        Info = 3,
        Warning = 4,
        Danger = 5,
        Error = 6,
        Alert = 7
    }

    public static class FlashHelper
    {
        public static void Flash(this Controller controller, string message, FlashLevel level)
        {
            IList<string> messages = null;
            string key = String.Format("flash-{0}", level.ToString().ToLower());

            messages = (controller.TempData.ContainsKey(key))
                ? (IList<string>)controller.TempData[key]
                : new List<string>();

            messages.Add(message);

            controller.TempData[key] = messages;
        }
    }

    
}