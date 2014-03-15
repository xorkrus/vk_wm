using System;

using System.Collections.Generic;
using System.Text;

namespace Galssoft.VKontakteWM.Components.Common.ResponseClasses
{
    /// <summary>
    /// Список новых фоторафий друзей (сокращение)
    /// </summary>
    public class ShortUpdatesPhotosResponse
    {
        /// <summary>
        /// ID фотографии
        /// </summary>
        public List<int> suprPhotoID = new List<int>();
    }

    public class UpdatesPhotosResponse
    {
        public int uprPostCount { get; set; }

        public List<PhotoData> uprPhotoDatas = new List<PhotoData>();

        public UpdatesPhotosResponse()
        {
            uprPostCount = 0;
        }
    }

    public class PhotoData
    {
        public int pdPhotoID { get; set; }

        public int pdUserID { get; set; }
        
        public string pdPhotoURL130px { get; set; }

        public string pdPhotoURL604px { get; set; }

        public PhotoData()
        {
            pdPhotoID = 0;

            pdUserID = 0;

            pdPhotoURL130px = string.Empty;

            pdPhotoURL130px = string.Empty;
        }
    }
}
