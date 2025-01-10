using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{
    //public class WhiteBalanceControl
    //{
    //    public long Max { private set; get; }
    //    public long Min { private set; get; }
    //    public long Step { private set; get; }
    //    public bool IsSupported { private set; get; }
    //    public long Value => GetValue();
    //    readonly IMFCaptureEngine? m_pEngine;
    //    public WhiteBalanceControl(IMFCaptureEngine? engine)
    //    {
    //        m_pEngine = engine;
    //        GetIAMVideoProcAmp(amp =>
    //        {
    //            var hr = amp.GetRange((int)DirectN.tagVideoProcAmpProperty.VideoProcAmp_WhiteBalance, out var min, out var max, out var delta, out var defaut, out var flag);
    //            if (hr == HRESULTS.S_OK)
    //            {
    //                this.Min = min;
    //                this.Max = max;
    //                this.Step = delta;
    //                IsSupported = true;
    //                var ff = (DirectN.tagVideoProcAmpFlags)flag;
    //            }
    //            return hr;
    //        });
    //    }

    //    HRESULT GetIAMVideoProcAmp(Func<IAMVideoProcAmp, HRESULT> func)
    //    {
    //        if (m_pEngine is null) return HRESULTS.MF_E_NOT_INITIALIZED;
    //        IMFCaptureSource? capturesource = null;
    //        IMFMediaSource? mediasource = null;
    //        try
    //        {
    //            var hr = m_pEngine.GetSource(out capturesource);
    //            if (hr != HRESULTS.S_OK || capturesource == null) return HRESULTS.S_FALSE;
    //            capturesource.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out mediasource);
    //            var videoprocamp = mediasource as IAMVideoProcAmp;
    //            if (videoprocamp != null)
    //            {
    //                return func.Invoke(videoprocamp);
    //            }
    //        }
    //        finally
    //        {
    //            WebCam_MF.SafeRelease(mediasource);
    //            WebCam_MF.SafeRelease(capturesource);
    //        }
    //        return HRESULTS.S_OK;
    //    }

    //    public ColorTemperaturePreset Preset { private set; get; } = ColorTemperaturePreset.Auto;

    //    long GetValue()
    //    {
    //        //isauto = true;
    //        var auto = true;
    //        long vv1 = 0;
    //        GetIAMVideoProcAmp(amp =>
    //        {
    //            var hr = amp.Get((int)DirectN.tagVideoProcAmpProperty.VideoProcAmp_WhiteBalance, out var vv, out var flag);
    //            if (hr == HRESULTS.S_OK)
    //            {
    //                var ff = (DirectN.tagVideoProcAmpFlags)flag;
    //                auto = ff switch
    //                {
    //                    tagVideoProcAmpFlags.VideoProcAmp_Flags_Auto => true,
    //                    tagVideoProcAmpFlags.VideoProcAmp_Flags_Manual => false,
    //                    _ => true
    //                };
    //                vv1 = vv;
    //            }
    //            return hr;
    //        });
    //        //isauto = auto;
    //        return vv1;
    //    }


    //    public HRESULT SetValue(int value, bool auto)
    //    {
    //        var flag = auto switch
    //        {
    //            true => DirectN.tagVideoProcAmpFlags.VideoProcAmp_Flags_Auto,
    //            false => DirectN.tagVideoProcAmpFlags.VideoProcAmp_Flags_Manual
    //        };
    //        var aa = value / this.Step;
    //        aa = aa * this.Step;
    //        var hr = GetIAMVideoProcAmp(amp =>
    //        {
    //            return amp.Set((int)DirectN.tagVideoProcAmpProperty.VideoProcAmp_WhiteBalance, (int)aa, (int)flag);
    //        });
    //        return hr;
    //    }



    //}
    //public enum ColorTemperaturePreset
    //{
    //    Auto,
    //    Manual
    //}



    public partial class WebCam_MF
    {
        WhiteBalanceControl? m_WhiteBalanceControl;
        public WhiteBalanceControl WhiteBalanceControl=> m_WhiteBalanceControl ??= new(m_pEngine);
    }



    public class WhiteBalanceControl(IMFCaptureEngine? engine): AMVideoProcAmp(engine, tagVideoProcAmpProperty.VideoProcAmp_WhiteBalance)
    {
        public long Max =>Range.Max;
        public long Min => Range.Min;
        public long Step => Range.Step;

    }

}
