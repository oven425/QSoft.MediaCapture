using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{
    public class AMVideoProcAmp(IMFCaptureEngine? engine, DirectN.tagVideoProcAmpProperty property)
    {
        public static AMVideoProcAmp Create(IMFCaptureEngine engine, DirectN.tagVideoProcAmpProperty property)
        {
            var videprocamp = new AMVideoProcAmp(engine, property);
            videprocamp.GetRange();
            return videprocamp;
        }

        internal void Init()
        {
            this.GetRange();
        }



        protected AMVideoProcAmpRange? m_Range;
        public bool IsAuto { get; private set; }
        long m_Preset;
        protected long Preset => m_Preset;
        bool m_Support;
        public bool IsSupport { get => m_Support; }
        protected AMVideoProcAmpRange Range => m_Range ??= GetRange();
        protected AMVideoProcAmpRange GetRange()
        {
            IMFCaptureSource? capturesource = null;
            IMFMediaSource? mediasource = null;
            try
            {
                if (engine is null) return new(0, 0, 0);
                engine?.GetSource(out capturesource);
                var hr = capturesource.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out mediasource);

                if (mediasource is IAMVideoProcAmp videoprocamp)
                {
                    hr = videoprocamp.GetRange((int)property, out var min, out var max, out var step, out var dd, out var caps);
                    if (hr == HRESULTS.S_OK)
                    {
                        m_Support = true;
                        return new AMVideoProcAmpRange(max, min, step);
                    }
                }

            }
            finally
            {
                WebCam_MF.SafeRelease(mediasource);
                WebCam_MF.SafeRelease(capturesource);
            }
            return new(0, 0, 0);
        }


        protected HRESULT GetValue()
        {
            IMFCaptureSource? capturesource = null;
            IMFMediaSource? mediasource = null;
            try
            {
                var hr = engine.GetSource(out capturesource);
                if (hr != HRESULTS.S_OK || capturesource == null) return HRESULTS.S_FALSE;
                capturesource.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out mediasource);
                var videoprocamp = mediasource as IAMVideoProcAmp;
                if (videoprocamp != null)
                {
                    videoprocamp.Get((int)property, out var vv, out var flag);
                }
            }
            finally
            {
                WebCam_MF.SafeRelease(mediasource);
                WebCam_MF.SafeRelease(capturesource);
            }
            return HRESULTS.S_OK;
        }



        public HRESULT SetValue(int value, bool auto)
        {
            var flag = auto switch
            {
                true => DirectN.tagVideoProcAmpFlags.VideoProcAmp_Flags_Auto,
                false => DirectN.tagVideoProcAmpFlags.VideoProcAmp_Flags_Manual
            };
            var aa = value / this.Range.Step;
            aa = aa * this.Range.Step;

            if (engine is null) return HRESULTS.MF_E_NOT_INITIALIZED;
            IMFCaptureSource? capturesource = null;
            IMFMediaSource? mediasource = null;
            try
            {
                var hr = engine.GetSource(out capturesource);
                if (hr != HRESULTS.S_OK || capturesource == null) return HRESULTS.S_FALSE;
                capturesource.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out mediasource);
                var videoprocamp = mediasource as IAMVideoProcAmp;
                if (videoprocamp != null)
                {
                    videoprocamp.Set((int)property, (int)aa, (int)flag);
                }
            }
            finally
            {
                WebCam_MF.SafeRelease(mediasource);
                WebCam_MF.SafeRelease(capturesource);
            }
            return HRESULTS.S_OK;
        }
    }

    public class AMVideoProcAmpRange(long max, long min, long step)
    {
        public long Max => max;
        public long Min => min;
        public long Step => step;
    }
}
