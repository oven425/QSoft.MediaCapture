using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{
    public class CaptureSourceInfo(string id, string name)
    {
        public string ID => id;
        public string Name => name;
    }
    public class CaptureSource
    {
        public static void FindAllVideoCapture()
        {
            var attr = MFFunctions.MFCreateAttributes();
            attr.Set(MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
            //return attr.EnumDeviceSources().Select(x =>
            //{
            //    var mf = new WebCam_MF
            //    {
            //        FriendName = x.GetString(MFConstants.MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME),
            //        SymbolLinkName = x.GetString(MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_SYMBOLIC_LINK),
            //        CaptureObj = x
            //    };
            //    return mf;
            //}).ToList();
        }

        public static void FindAllAudioCapture()
        {

        }
    }
}
