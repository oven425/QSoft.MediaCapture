using DirectN;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture.Legacy
{
    public class AMCameraControl(IMFCaptureEngine? engine, DirectN.tagCameraControlProperty property):INotifyPropertyChanged
    {
        internal void Init()
        {
            this.GetRange();
            this.GetValue();
        }

        public string Name => property switch
        {
            DirectN.tagCameraControlProperty.CameraControl_Pan=>"Pan",
            DirectN.tagCameraControlProperty.CameraControl_Tilt => "Tilt",
            DirectN.tagCameraControlProperty.CameraControl_Zoom =>"Zoom",
            DirectN.tagCameraControlProperty.CameraControl_Roll =>"Roll",
            DirectN.tagCameraControlProperty.CameraControl_Iris=>"Iris",
            DirectN.tagCameraControlProperty.CameraControl_Focus=>"Focus",
            DirectN.tagCameraControlProperty.CameraControl_Exposure =>"Exposure",
            _ =>""

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
                this.SetValue((int)value, this.m_IsAuto);
            }
            get
            {
                var hr = GetValue();
                return m_Value;
            }
        }
        //protected AMVideoProcAmpRange Range => m_Range ??= GetRange();
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
                if (mediasource is IAMCameraControl videoprocamp)
                {
                    hr = videoprocamp.GetRange((int)property, out var min, out var max, out var step, out var dd, out var caps);
                    if (hr == HRESULTS.S_OK)
                    {
                        this.IsSupport = true;
                        this.Max = (int)max;
                        this.Min = (int)min;
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

        public event PropertyChangedEventHandler PropertyChanged;
        void Update([CallerMemberName] string? name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

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
                var videoprocamp = mediasource as IAMCameraControl;
                if (videoprocamp != null)
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
                var videoprocamp = mediasource as IAMCameraControl;
                if (videoprocamp != null)
                {
                    hr = videoprocamp.Set((int)property, (int)aa, (int)flag);
                    if (hr == HRESULTS.S_OK)
                    {
                        this.m_Value = aa;
                        this.m_IsAuto = auto;
                    }
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

    }

}

namespace QSoft.MediaCapture
{
    public partial class WebCam_MF
    {
        Dictionary<DirectN.tagCameraControlProperty, Legacy.AMCameraControl>? m_AMCameraControls;
        Dictionary<DirectN.tagCameraControlProperty, Legacy.AMCameraControl> InitCameraControls()
        {
            if (m_AMCameraControls is not null) return m_AMCameraControls;
            m_AMCameraControls = [];
            foreach (var property in Enum.GetValues(typeof(DirectN.tagCameraControlProperty)).Cast<DirectN.tagCameraControlProperty>())
            {
                var amp = new Legacy.AMCameraControl(m_pEngine, property);
                amp.Init();
                m_AMCameraControls.Add(property, amp);
            }
            return m_AMCameraControls;
        }
        public Dictionary<DirectN.tagCameraControlProperty, Legacy.AMCameraControl> CameraControls => InitCameraControls();
    }


}
