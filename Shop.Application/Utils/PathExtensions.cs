using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Utils
{
    public static class PathExtensions
    {
        #region user avatar

        public static string UserAvatarOrgin = "/img/userAvatar/orgin/";
        public static string UserAvatarOrginServer = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/userAvatar/orgin/");

        public static string UserAvatarThumb = "/img/userAvatar/thumb/";
        public static string UserAvatarThumbServer = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/userAvatar/thumb/");

        #endregion
    }
}
