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
        readonly List<TorchLightState> m_SupportStates = [];
        public TorchLight(IMFCaptureEngine? engine)
            : base(engine, KSPROPERTY_CAMERACONTROL_EXTENDED.KSPROPERTY_CAMERACONTROL_EXTENDED_TORCHMODE)
        {
            var hr = this.GetCapabilities(out var cap);
            if (hr == HRESULTS.S_OK)
            {
                var off = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_OFF;
                if (off == DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_OFF)
                {
                    m_SupportStates.Add(TorchLightState.OFF);
                }
                var on = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_ON;
                if (on == DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_ON)
                {
                    m_SupportStates.Add(TorchLightState.ON);
                }
                var on_adjistablepower = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_ON_ADJUSTABLEPOWER;
                if (on_adjistablepower == DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_ON_ADJUSTABLEPOWER)
                {
                    m_SupportStates.Add(TorchLightState.ON_ADJUSTABLEPOWER);
                }
            }


        }
        public List<TorchLightState> SupportStates => m_SupportStates;


        public void SetState(TorchLightState state)
        {
            if (!this.IsSupported) return;
            ulong setv = state switch
            {
                TorchLightState.OFF => DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_OFF,
                TorchLightState.ON => 1,
                TorchLightState.ON_ADJUSTABLEPOWER => 2,
                _ => 0
            };
            this.Set(setv);
        }

        public TorchLightState GetState()
        {
            var hr = this.Get(out var mode);
            if (hr == HRESULTS.S_OK)
            {
                var getv = mode switch
                {
                    0 => TorchLightState.OFF,
                    1 => TorchLightState.ON,
                    2 => TorchLightState.ON_ADJUSTABLEPOWER,
                    _ => TorchLightState.OFF
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
