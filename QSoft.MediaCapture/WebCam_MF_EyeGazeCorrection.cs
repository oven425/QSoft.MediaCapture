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
        readonly EyeGazeCorrection? m_EyeGazeCorrection;
        public EyeGazeCorrection EyeGazeCorrection => m_EyeGazeCorrection ?? new EyeGazeCorrection(m_pEngine);
    }

    public class EyeGazeCorrection : ExtendedCameraControl
    {
        public EyeGazeCorrection(IMFCaptureEngine? engine)
            : base(engine, KSPROPERTY_CAMERACONTROL_EXTENDED.KSPROPERTY_CAMERACONTROL_EXTENDED_EYEGAZECORRECTION)
        {
            var hr = this.GetCapabilities(out var cap);
            if (hr == HRESULTS.S_OK)
            {
                //SupportStates = [.. ParseState(cap)];
            }
        }
    }
}
