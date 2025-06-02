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
            return [.. attr.EnumDeviceSources().Select(x =>
            {
                var mf = new WebCam_MF
                {
                    FriendName = x.GetString(MFConstants.MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME),
                    SymbolLinkName = x.GetString(MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_SYMBOLIC_LINK),
                    
                    CaptureObj = x
                };
                return mf;
            })];
        }

        public static WebCam_MF CreateFromSymbollink(string data)
        {
            
            var attrs = MFFunctions.MFCreateAttributes(2);
            attrs.Set(MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
            attrs.Set(MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_SYMBOLIC_LINK, data);
            IMFActivate act;
            var hhr = DirectN.Functions.MFCreateDeviceSourceActivate(attrs.Object, out act);
            var mfcreate = new ComObject<IMFActivate>(act);
            var aa = mfcreate.GetString(MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_SYMBOLIC_LINK);
            return new WebCam_MF
            {
                FriendName = mfcreate.GetString(MFConstants.MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME),
                SymbolLinkName = mfcreate.GetString(MFConstants.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_SYMBOLIC_LINK),
                CaptureObj = mfcreate
            };
        }
    }

    
}
