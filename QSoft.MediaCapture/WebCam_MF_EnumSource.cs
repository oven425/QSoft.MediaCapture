using DirectN;
using System.Collections.Generic;
using System.Linq;

namespace QSoft.MediaCapture
{
    public partial class WebCam_MF
    {
        public static IReadOnlyCollection<WebCam_MF> GetAllWebCams()
        {
            var attr = MFFunctions.MFCreateAttributes();
            attr.Set(MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
            return attr.EnumDeviceSources().Select(x =>
            {
                var mf = new WebCam_MF
                {
                    FriendName = x.GetString(MFConstants.MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME),
                    SymbolLinkName = x.GetString(MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_SYMBOLIC_LINK),
                    CaptureObj = x
                };
                return mf;
            }).ToList();
        }

    }

    
}
