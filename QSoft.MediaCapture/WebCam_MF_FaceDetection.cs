using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{
    public partial class WebCam_MF
    {
        public void InitFaceDection()
        {
            this.FaceDetectionControl = new (this.m_pEngine);
            this.FaceDetectionControl.GetCapabilities(out var cc);
            System.Diagnostics.Trace.WriteLine($"FaceDection:{this.FaceDetectionControl.IsSupported} {cc}");
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
