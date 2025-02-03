using DirectN;
using System.Collections.Generic;

namespace QSoft.MediaCapture
{
    public partial class WebCam_MF
    {
        public HRESULT InitFlashLight()
        {
            this.FlashLight = new FlashLight(this.m_pEngine);
            return HRESULTS.S_OK;
        }
        public FlashLight? FlashLight { private set; get; }

    }
    //https://learn.microsoft.com/en-us/windows-hardware/drivers/stream/ksproperty-cameracontrol-extended-flashmode
    public class FlashLight : ExtendedCameraControl
    {
        //#define KSCAMERA_EXTENDEDPROP_FLASH_OFF  0x0000000000000000
        //#define KSCAMERA_EXTENDEDPROP_FLASH_ON  0x0000000000000001
        //#define KSCAMERA_EXTENDEDPROP_FLASH_ON_ADJUSTABLEPOWER  0x0000000000000002
        //#define KSCAMERA_EXTENDEDPROP_FLASH_AUTO  0x0000000000000004
        //#define KSCAMERA_EXTENDEDPROP_FLASH_AUTO_ADJUSTABLEPOWER  0x0000000000000008
        //#define KSCAMERA_EXTENDEDPROP_FLASH_REDEYEREDUCTION  0x0000000000000010
        //#define KSCAMERA_EXTENDEDPROP_FLASH_SINGLEFLASH  0x0000000000000020
        readonly List<FlashState> m_SupportState = [];
        public FlashLight(IMFCaptureEngine? engine)
            : base(engine, KSPROPERTY_CAMERACONTROL_EXTENDED.KSPROPERTY_CAMERACONTROL_EXTENDED_FLASHMODE)
        {
        }

        public List<FlashState> SupportStates => m_SupportState;

        public void SetState(FlashState state)
        {
            if (!this.IsSupported) return;
            ulong setv = state switch
            {
                FlashState.OFF => 0,
                FlashState.ON => 1,
                FlashState.ON_ADJUSTABLEPOWER => 2,
                _ => 0
            };
            this.Set(setv);
        }

        public FlashState GetState()
        {
            var hr = this.Get(out var mode);
            if (hr == HRESULTS.S_OK)
            {
                var getv = mode switch
                {
                    0 => FlashState.OFF,
                    1 => FlashState.ON,
                    2 => FlashState.ON_ADJUSTABLEPOWER,
                    _ => FlashState.OFF
                };
                return getv;
            }
            return FlashState.OFF;
        }


    }

    public enum FlashState
    {
        ON,
        OFF,
        ON_ADJUSTABLEPOWER,
        AUTO,
        FLASH_AUTO_ADJUSTABLEPOWER
    }

}
