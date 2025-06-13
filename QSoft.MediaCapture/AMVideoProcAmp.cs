using DirectN;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
namespace QSoft.MediaCapture.Legacy
{
    public class AMVideoProcAmp(IMFCaptureEngine? engine, DirectN.tagVideoProcAmpProperty property) : INotifyPropertyChanged
    {
        internal void Init()
        {
            this.GetRange();
            this.GetValue();
        }

        public string Name => property switch
        {
            DirectN.tagVideoProcAmpProperty.VideoProcAmp_Brightness => "Brightness",
            DirectN.tagVideoProcAmpProperty.VideoProcAmp_Contrast => "Contrast",
            DirectN.tagVideoProcAmpProperty.VideoProcAmp_Hue => "Hue",
            DirectN.tagVideoProcAmpProperty.VideoProcAmp_Saturation => "Saturation",
            DirectN.tagVideoProcAmpProperty.VideoProcAmp_Sharpness => "Sharpness",
            DirectN.tagVideoProcAmpProperty.VideoProcAmp_Gamma => "Gamma",
            DirectN.tagVideoProcAmpProperty.VideoProcAmp_ColorEnable => "ColorEnable",
            DirectN.tagVideoProcAmpProperty.VideoProcAmp_WhiteBalance => "WhiteBalance",
            DirectN.tagVideoProcAmpProperty.VideoProcAmp_BacklightCompensation => "BacklightCompensation",
            DirectN.tagVideoProcAmpProperty.VideoProcAmp_Gain => "Gain",
            _ => ""

        };

        public long Max { get; private set; }
        public long Min { get; private set; }
        public long Step { get; private set; }

        public bool IsAuto
        {
            set => this.SetValue((int)this.m_Value, value);
            get
            {
                var hr = GetValue();
                return this.m_IsAuto;
            }
        }


        public bool IsSupport { get; private set; }
        public long Value
        {
            set
            {
                if (this.m_Value != value)
                {
                    this.m_Value = value;
                    this.SetValue((int)value, this.m_IsAuto);
                    this.Updae();
                }
            }
            get
            {
                var hr = GetValue();
                return m_Value;
            }
        }

        internal void GetRange()
        {
            IMFCaptureSource? capturesource = null;
            IMFMediaSource? mediasource = null;
            try
            {

                if (engine is null) return;
                var hr = engine?.GetSource(out capturesource);
                if (hr != HRESULTS.S_OK || capturesource is null) return;
                hr = capturesource.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out mediasource);
                if (hr != HRESULTS.S_OK) return;

                if (mediasource is IAMVideoProcAmp videoprocamp)
                {
                    //IVideoProcAmp
                    hr = videoprocamp.GetRange((int)property, out var min, out var max, out var step, out var dd, out var caps);
                    if (hr == HRESULTS.S_OK)
                    {
                        this.IsSupport = true;
                        this.Max = max;
                        this.Min = min;
                        this.Step = step;
                    }
                    WebCam_MF.SafeRelease(videoprocamp);
                }

            }
            finally
            {
                WebCam_MF.SafeRelease(mediasource);
                WebCam_MF.SafeRelease(capturesource);
            }
        }

        long m_Value;
        bool m_IsAuto;
        protected void Updae([CallerMemberName] string? name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        internal HRESULT GetValue()
        {
            if (engine is null) return HRESULTS.MF_E_NOT_INITIALIZED;
            IMFCaptureSource? capturesource = null;
            IMFMediaSource? mediasource = null;
            try
            {
                var hr = engine.GetSource(out capturesource);
                if (hr != HRESULTS.S_OK || capturesource == null) return HRESULTS.S_FALSE;
                capturesource.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out mediasource);
                if (mediasource is IAMVideoProcAmp videoprocamp)
                {
                    videoprocamp.Get((int)property, out m_Value, out var flags);
                    var ff = (DirectN.tagVideoProcAmpFlags)flags;
                    this.m_IsAuto = ff == tagVideoProcAmpFlags.VideoProcAmp_Flags_Auto;
                    WebCam_MF.SafeRelease(videoprocamp);
                }
            }
            finally
            {
                WebCam_MF.SafeRelease(mediasource);
                WebCam_MF.SafeRelease(capturesource);
            }
            return HRESULTS.S_OK;
        }

        internal HRESULT SetValue(int value, bool auto)
        {
            var flag = auto switch
            {
                true => DirectN.tagVideoProcAmpFlags.VideoProcAmp_Flags_Auto,
                false => DirectN.tagVideoProcAmpFlags.VideoProcAmp_Flags_Manual
            };
            var aa = value / this.Step;
            aa = aa * this.Step;

            if (engine is null) return HRESULTS.MF_E_NOT_INITIALIZED;
            IMFCaptureSource? capturesource = null;
            IMFMediaSource? mediasource = null;
            try
            {
                var hr = engine.GetSource(out capturesource);
                if (hr != HRESULTS.S_OK || capturesource == null) return HRESULTS.S_FALSE;
                capturesource.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out mediasource);
                if (mediasource is IAMVideoProcAmp videoprocamp)
                {
                    hr = videoprocamp.Set((int)property, (int)aa, (int)flag);
                    if (hr == HRESULTS.S_OK)
                    {
                        this.m_Value = aa;
                        this.m_IsAuto = auto;
                    }
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

}
namespace QSoft.MediaCapture
{
    public partial class WebCam_MF
    {
        Dictionary<DirectN.tagVideoProcAmpProperty, Legacy.AMVideoProcAmp>? m_VideoProcAmps;
        Dictionary<DirectN.tagVideoProcAmpProperty, Legacy.AMVideoProcAmp> InitVideoProcAmps()
        {
            if (m_VideoProcAmps is not null) return m_VideoProcAmps;
            m_VideoProcAmps = [];
            foreach (var property in Enum.GetValues(typeof(DirectN.tagVideoProcAmpProperty)).Cast<DirectN.tagVideoProcAmpProperty>())
            {
                var amp = new Legacy.AMVideoProcAmp(m_pEngine, property);
                amp.Init();
                m_VideoProcAmps.Add(property, amp);
            }
            return VideoProcAmps;
        }
        public Dictionary<DirectN.tagVideoProcAmpProperty, Legacy.AMVideoProcAmp> VideoProcAmps => InitVideoProcAmps();
    }

}
