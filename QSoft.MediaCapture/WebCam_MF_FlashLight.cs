//using DirectN;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace QSoft.MediaCapture
//{
//    public partial class WebCam_MF
//    {
//        public HRESULT InitFlashLight()
//        {
//            this.FlashLight = new FlashLight(this.m_pEngine);
//            return HRESULTS.S_OK;
//        }
//        public FlashLight? FlashLight { private set; get; }

//    }
//    //https://learn.microsoft.com/en-us/windows-hardware/drivers/stream/ksproperty-cameracontrol-extended-flashmode
//    public class FlashLight: ExtendedCameraControl
//    {
//        //#define KSCAMERA_EXTENDEDPROP_FLASH_OFF  0x0000000000000000
//        //#define KSCAMERA_EXTENDEDPROP_FLASH_ON  0x0000000000000001
//        //#define KSCAMERA_EXTENDEDPROP_FLASH_ON_ADJUSTABLEPOWER  0x0000000000000002
//        //#define KSCAMERA_EXTENDEDPROP_FLASH_AUTO  0x0000000000000004
//        //#define KSCAMERA_EXTENDEDPROP_FLASH_AUTO_ADJUSTABLEPOWER  0x0000000000000008
//        //#define KSCAMERA_EXTENDEDPROP_FLASH_REDEYEREDUCTION  0x0000000000000010
//        //#define KSCAMERA_EXTENDEDPROP_FLASH_SINGLEFLASH  0x0000000000000020
//        readonly List<FlashState> m_SupportState = [];
//        public FlashLight(IMFCaptureEngine? engine)
//            : base(engine, KSPROPERTY_CAMERACONTROL_EXTENDED.KSPROPERTY_CAMERACONTROL_EXTENDED_FLASHMODE)
//        {
//        }

//        public List<FlashState> SupportStates => m_SupportState;

//        public void SetState(FlashState state)
//        {
//            if (!this.IsSupported) return;
//            //ulong setv = state switch
//            //{
//            //    TorchLightState.OFF => 0,
//            //    TorchLightState.ON => 1,
//            //    TorchLightState.ON_ADJUSTABLEPOWER => 2,
//            //    _ => 0
//            //};
//            //this.Set(setv);
//        }

//        public FlashState GetState()
//        {
//            var hr = this.Get(out var mode);
//            //if (hr == HRESULTS.S_OK)
//            //{
//            //    var getv = mode switch
//            //    {
//            //        0 => TorchLightState.OFF,
//            //        1 => TorchLightState.ON,
//            //        2 => TorchLightState.ON_ADJUSTABLEPOWER,
//            //        _ => TorchLightState.OFF
//            //    };
//            //    return getv;
//            //}
//            return FlashState.OFF;
//        }


//    }

//    public enum FlashState
//    {
//        ON,
//        OFF,
//        ON_ADJUSTABLEPOWER,
//        AUTO,
//        FLASH_AUTO_ADJUSTABLEPOWER
//    }

//}
