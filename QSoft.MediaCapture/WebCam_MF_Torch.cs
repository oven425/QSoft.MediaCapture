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

        public void SetState(TorchLightState state)
        {
            if (!this.IsSupported) return;
            ulong setv = state switch
            {
                TorchLightState.OFF=>0,
                TorchLightState.ON=>1,
                TorchLightState.ON_ADJUSTABLEPOWER=>2,
                _=>0
            };
            this.Set(setv);
        }

        public TorchLightState GetState()
        {
            var hr = this.Get(out var mode);
            if(hr == HRESULTS.S_OK)
            {
                var getv = mode switch
                {
                    0=> TorchLightState.OFF,
                    1=> TorchLightState.ON,
                    2=> TorchLightState.ON_ADJUSTABLEPOWER,
                    _=> TorchLightState.OFF
                };
                return getv;
            }
            return TorchLightState.OFF;
        }

     }

    public enum TorchLightState
    {
        OFF,
        ON,
        ON_ADJUSTABLEPOWER
    }

}
