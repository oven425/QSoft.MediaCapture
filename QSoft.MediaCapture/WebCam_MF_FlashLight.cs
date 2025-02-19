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
        readonly List<FlashState> m_SupportStates = [];
        public FlashLight(IMFCaptureEngine? engine)
            : base(engine, KSPROPERTY_CAMERACONTROL_EXTENDED.KSPROPERTY_CAMERACONTROL_EXTENDED_FLASHMODE)
        {
            var hr = this.GetCapabilities(out var cap);
            if (hr == HRESULTS.S_OK)
            {
                var off = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_OFF;
                if (off == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_OFF)
                {
                    m_SupportStates.Add(FlashState.OFF);
                }
                var on = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_ON;
                if (on == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_ON)
                {
                    m_SupportStates.Add(FlashState.ON);
                }
                var on_adjistablepower = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_ON_ADJUSTABLEPOWER;
                if (on_adjistablepower == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_ON_ADJUSTABLEPOWER)
                {
                    m_SupportStates.Add(FlashState.ON_ADJUSTABLEPOWER);
                }

                var auto = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_AUTO;
                if (auto == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_AUTO)
                {
                    m_SupportStates.Add(FlashState.AUTO);
                }

                var auto_adjistablepower = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_AUTO_ADJUSTABLEPOWER;
                if (auto_adjistablepower == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_AUTO_ADJUSTABLEPOWER)
                {
                    m_SupportStates.Add(FlashState.AUTO_ADJUSTABLEPOWER);
                }
            }
        }

        public List<FlashState> SupportStates => m_SupportStates;

        public void SetState(FlashState state)
        {
            if (!this.IsSupported) return;
            ulong setv = state switch
            {
                FlashState.OFF => DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_OFF,
                FlashState.ON => DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_ON,
                FlashState.ON_ADJUSTABLEPOWER => DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_ON_ADJUSTABLEPOWER,
                FlashState.AUTO => DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_AUTO,
                FlashState.AUTO_ADJUSTABLEPOWER => DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_AUTO_ADJUSTABLEPOWER,
                _ => DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_OFF
            };
            this.Set(setv);
        }

        public FlashState GetState()
        {
            System.Diagnostics.Debug.WriteLine($"Flash GetState");
            var hr = this.Get(out var mode);
            System.Diagnostics.Debug.WriteLine($"Flash GetState:{mode}");
            if (hr == HRESULTS.S_OK)
            {
                var getv = mode switch
                {
                    DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_OFF => FlashState.OFF,
                    DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_ON => FlashState.ON,
                    DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_ON_ADJUSTABLEPOWER => FlashState.ON_ADJUSTABLEPOWER,
                    DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_AUTO => FlashState.AUTO,
                    DirectN.Constants.KSCAMERA_EXTENDEDPROP_FLASH_AUTO_ADJUSTABLEPOWER => FlashState.AUTO_ADJUSTABLEPOWER,
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
        AUTO_ADJUSTABLEPOWER
    }

}
