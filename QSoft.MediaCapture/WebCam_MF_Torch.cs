using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//https://learn.microsoft.com/en-us/windows-hardware/drivers/stream/ksproperty-cameracontrol-extended-torchmode

namespace QSoft.MediaCapture
{
    public partial class WebCam_MF
    {
        public void InitTorch()
        {
            this.TorchLight = new TorchLight(new (this.m_pEngine, KSPROPERTY_CAMERACONTROL_EXTENDED.KSPROPERTY_CAMERACONTROL_EXTENDED_TORCHMODE));
        }
        public TorchLight? TorchLight { private set; get; }
    }
    public class TorchLight(ExtendedCameraControl extctrl) 
    {
        public List<TorchLightState> SupportStates => extctrl.GetCapabilities<TorchLightState>(cap=> 
        {
            List<TorchLightState> supports = [];
            var off = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_OFF;
            if (off == DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_OFF)
            {
                supports.Add(TorchLightState.OFF);
            }
            var on = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_ON;
            if (on == DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_ON)
            {
                supports.Add(TorchLightState.ON);
            }
            var on_adjistablepower = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_ON_ADJUSTABLEPOWER;
            if (on_adjistablepower == DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_ON_ADJUSTABLEPOWER)
            {
                supports.Add(TorchLightState.ON_ADJUSTABLEPOWER);
            }
            return supports;
        });
        public bool IsSupported => extctrl.IsSupported;
        //public TorchLight(IMFCaptureEngine? engine)
        //    : base(engine, KSPROPERTY_CAMERACONTROL_EXTENDED.KSPROPERTY_CAMERACONTROL_EXTENDED_TORCHMODE)
        //{
        //    this.GetCapabilities<TorchLightState>(cap =>
        //    {
        //        List<TorchLightState> supports = [];
        //        var off = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_OFF;
        //        if (off == DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_OFF)
        //        {
        //            supports.Add(TorchLightState.OFF);
        //        }
        //        var on = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_ON;
        //        if (on == DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_ON)
        //        {
        //            supports.Add(TorchLightState.ON);
        //        }
        //        var on_adjistablepower = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_ON_ADJUSTABLEPOWER;
        //        if (on_adjistablepower == DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_ON_ADJUSTABLEPOWER)
        //        {
        //            supports.Add(TorchLightState.ON_ADJUSTABLEPOWER);
        //        }
        //        return supports;
        //    });
        //    //var hr = this.GetCapabilities(out var cap);
        //    //if (hr == HRESULTS.S_OK)
        //    //{
        //    //    var off = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_OFF;
        //    //    if (off == DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_OFF)
        //    //    {
        //    //        m_SupportStates.Add(TorchLightState.OFF);
        //    //    }
        //    //    var on = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_ON;
        //    //    if (on == DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_ON)
        //    //    {
        //    //        m_SupportStates.Add(TorchLightState.ON);
        //    //    }
        //    //    var on_adjistablepower = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_ON_ADJUSTABLEPOWER;
        //    //    if (on_adjistablepower == DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_ON_ADJUSTABLEPOWER)
        //    //    {
        //    //        m_SupportStates.Add(TorchLightState.ON_ADJUSTABLEPOWER);
        //    //    }
        //    //}


        //}
        //public List<TorchLightState> SupportStates => m_SupportStates;


        public void SetState(TorchLightState state)
        {
            if (!extctrl.IsSupported) return;
            ulong setv = state switch
            {
                TorchLightState.OFF => DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_OFF,
                TorchLightState.ON => DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_ON,
                TorchLightState.ON_ADJUSTABLEPOWER => DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_ON_ADJUSTABLEPOWER,
                _ => DirectN.Constants.KSCAMERA_EXTENDEDPROP_VIDEOTORCH_OFF
            };
            extctrl.Set(setv);
        }

        public TorchLightState GetState()
        {
            var hr = extctrl.Get(out var mode);
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
