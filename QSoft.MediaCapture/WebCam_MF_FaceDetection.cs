using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{

    //https://code.stanford.edu/mpsilver/2208_kinect_sdk/-/blob/81e355c5fc26034743205132b3de9cf8b961543d/src/color/mfcamerareader.cpp
    public partial class WebCam_MF
    {
        public void InitFaceDection()
        {
            var enums = Enum.GetValues(typeof(KSPROPERTY_CAMERACONTROL_EXTENDED));
            var gg = enums.OfType<KSPROPERTY_CAMERACONTROL_EXTENDED>()
                .Select(x =>
                
                    new
                    {
                        x,
                        suuport = new ExtendedCameraControl(this.m_pEngine, x).IsSupported
                    }
                )
                .GroupBy(x=>x.suuport);
            foreach(var group  in gg)
            {
                System.Diagnostics.Trace.WriteLine($"Supprt:{group.Key}");
                foreach(var ooin  in group)
                {
                    System.Diagnostics.Trace.WriteLine(ooin.x);
                }
            }
            //foreach (var oo in enums)
            //{
            //    var ext = new ExtendedCameraControl(this.m_pEngine, (KSPROPERTY_CAMERACONTROL_EXTENDED)oo);
            //    System.Diagnostics.Trace.WriteLine($"{oo}:{ext.IsSupported}");
            //}
            this.FaceDetectionControl = new (this.m_pEngine);
            //this.FaceDetectionControl.GetCapabilities(out var cc);
            //System.Diagnostics.Trace.WriteLine($"FaceDection:{this.FaceDetectionControl.IsSupported} {cc}");
        }
        public FaceDetectionControl? FaceDetectionControl { set; get; }
    }

    public class FaceDetectionControl : ExtendedCameraControl
    {
        public FaceDetectionControl(IMFCaptureEngine? engine)
            : base(engine, KSPROPERTY_CAMERACONTROL_EXTENDED.KSPROPERTY_CAMERACONTROL_EXTENDED_ISO)
        {
        }

    }

}
