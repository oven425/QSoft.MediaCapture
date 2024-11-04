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
        public void InitTorch()
        {
            this.TorchLight = new TorchLight(this.m_pEngine);
        }
        public TorchLight? TorchLight { private set; get; }
    }
    //https://learn.microsoft.com/en-us/windows-hardware/drivers/stream/ksproperty-cameracontrol-extended-torchmode
    public class TorchLight : ExtendedCameraControl
    {
        public TorchLight(IMFCaptureEngine? engine)
            : base(engine, KSPROPERTY_CAMERACONTROL_EXTENDED.KSPROPERTY_CAMERACONTROL_EXTENDED_TORCHMODE)
        {
        }
    }

}
