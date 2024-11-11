using DirectN;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class FlashLight: ExtendedCameraControl
    {
//#define KSCAMERA_EXTENDEDPROP_FLASH_OFF  0x0000000000000000
//#define KSCAMERA_EXTENDEDPROP_FLASH_ON  0x0000000000000001
//#define KSCAMERA_EXTENDEDPROP_FLASH_ON_ADJUSTABLEPOWER  0x0000000000000002
//#define KSCAMERA_EXTENDEDPROP_FLASH_AUTO  0x0000000000000004
//#define KSCAMERA_EXTENDEDPROP_FLASH_AUTO_ADJUSTABLEPOWER  0x0000000000000008
//#define KSCAMERA_EXTENDEDPROP_FLASH_REDEYEREDUCTION  0x0000000000000010
//#define KSCAMERA_EXTENDEDPROP_FLASH_SINGLEFLASH  0x0000000000000020
        public FlashLight(IMFCaptureEngine? engine)
            : base(engine, KSPROPERTY_CAMERACONTROL_EXTENDED.KSPROPERTY_CAMERACONTROL_EXTENDED_WHITEBALANCEMODE)
        {
        }
    }

    public class ExtendedCameraControl
    {
        [Conditional("DEBUG")]
        static void TetsALL(IMFCaptureEngine engine)
        {
            var enums = Enum.GetValues(typeof(KSPROPERTY_CAMERACONTROL_EXTENDED));
            var gg = enums.OfType<KSPROPERTY_CAMERACONTROL_EXTENDED>()
                .Select(x =>

                    new
                    {
                        x,
                        suuport = new ExtendedCameraControl(engine, x).IsSupported
                    }
                )
                .GroupBy(x => x.suuport);
            foreach (var group in gg)
            {
                System.Diagnostics.Trace.WriteLine($"Supprt:{group.Key}");
                foreach (var ooin in group)
                {
                    System.Diagnostics.Trace.WriteLine(ooin.x);
                }
            }
        }
        public bool IsSupported { protected set; get; } = false;
        readonly IMFCaptureEngine? m_pEngine;
        readonly KSPROPERTY_CAMERACONTROL_EXTENDED m_KsProperty;
        public ExtendedCameraControl(IMFCaptureEngine? pEngine, KSPROPERTY_CAMERACONTROL_EXTENDED ksproperty)
        {
            m_KsProperty = ksproperty;
            m_pEngine = pEngine;
            if(this.Get(out var mode) == HRESULTS.S_OK)
            {
                this.IsSupported = true;
            }
        }

        public HRESULT GetCapabilities(out ulong data)
        {
            data = 0;
            IMFCaptureSource? pSource = null;
            IMFMediaSource? mediasource = null;
            IMFGetService? mfservice = null;
            IMFExtendedCameraController? extendedcameracontroller = null;
            IMFExtendedCameraControl? extendedcameracontrol = null;
            if (m_pEngine is null) return HRESULTS.MF_E_NOT_INITIALIZED;
            try
            {
                var hr = m_pEngine.GetSource(out pSource);
                if (hr != HRESULTS.S_OK) return hr;
                hr = pSource.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out mediasource);
                if (hr != HRESULTS.S_OK) return hr;
                mfservice = mediasource as IMFGetService;
                if (mfservice == null) return HRESULTS.S_FALSE;
                hr = mfservice.GetService(Guid.Empty, new Guid("b91ebfee-ca03-4af4-8a82-a31752f4a0fc"), out var con);
                if (hr != HRESULTS.S_OK) return hr;
                extendedcameracontroller = con as IMFExtendedCameraController;
                if (extendedcameracontroller == null) return HRESULTS.S_FALSE;
                //https://github.com/smourier/DirectN/blob/af1d27a173291bf648d3262952e36629e9420cbc/DirectN/DirectN/Generated/KSPROPERTY_CAMERACONTROL_EXTENDED.cs#L15
                hr = extendedcameracontroller.GetExtendedCameraControl(0xffffffff, (uint)m_KsProperty, out extendedcameracontrol);
                if (hr != HRESULTS.S_OK) return hr;
                //System.Diagnostics.Trace.WriteLine($"GetExtendedCameraControl {hr}");
                data = extendedcameracontrol.GetCapabilities();
                //System.Diagnostics.Trace.WriteLine($"GetCapabilities {data}");

            }
            finally
            {
                WebCam_MF.SafeRelease(extendedcameracontrol);
                WebCam_MF.SafeRelease(extendedcameracontroller);
                WebCam_MF.SafeRelease(mfservice);
                WebCam_MF.SafeRelease(mediasource);
                WebCam_MF.SafeRelease(pSource);
            }
            return HRESULTS.S_OK;
        }

        public HRESULT Get(out ulong mode)
        {
            mode = 0;
            IMFCaptureSource? pSource = null;
            IMFMediaSource? mediasource = null;
            IMFGetService? mfservice = null;
            IMFExtendedCameraController? extendedcameracontroller = null;
            IMFExtendedCameraControl? extendedcameracontrol = null;
            if (m_pEngine is null) return HRESULTS.MF_E_NOT_INITIALIZED;
            try
            {
                var hr = m_pEngine.GetSource(out pSource);
                if(hr != HRESULTS.S_OK) return hr;
                hr = pSource.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out mediasource);
                if (hr != HRESULTS.S_OK) return hr;
                mfservice = mediasource as IMFGetService;
                if (mfservice == null) return HRESULTS.S_FALSE;
                hr = mfservice.GetService(Guid.Empty, new Guid("b91ebfee-ca03-4af4-8a82-a31752f4a0fc"), out var con);
                if (hr != HRESULTS.S_OK) return hr;
                extendedcameracontroller = con as IMFExtendedCameraController;
                if (extendedcameracontroller == null) return HRESULTS.S_FALSE;
                //https://github.com/smourier/DirectN/blob/af1d27a173291bf648d3262952e36629e9420cbc/DirectN/DirectN/Generated/KSPROPERTY_CAMERACONTROL_EXTENDED.cs#L15
                hr = extendedcameracontroller.GetExtendedCameraControl(0xffffffff, (uint)m_KsProperty, out extendedcameracontrol);
                if (hr != HRESULTS.S_OK) return hr;
                //System.Diagnostics.Trace.WriteLine($"GetExtendedCameraControl {hr}");
                var capabilities = extendedcameracontrol.GetCapabilities();
                //System.Diagnostics.Trace.WriteLine($"GetCapabilities {capabilities}");
                mode = extendedcameracontrol.GetFlags();
                //System.Diagnostics.Trace.WriteLine($"GetFlags {mode}");
                hr = extendedcameracontrol.CommitSettings();
                //System.Diagnostics.Trace.WriteLine($"CommitSettings {hr}");

            }
            finally
            {
                WebCam_MF.SafeRelease(extendedcameracontrol);
                WebCam_MF.SafeRelease(extendedcameracontroller);
                WebCam_MF.SafeRelease(mfservice);
                WebCam_MF.SafeRelease(mediasource);
                WebCam_MF.SafeRelease(pSource);
            }
            return HRESULTS.S_OK;

        }

        public HRESULT Set(ulong mode)
        {
            IMFCaptureSource? pSource = null;
            IMFGetService? mfservice = null;
            IMFExtendedCameraController? extendedcameracontroller = null;
            IMFExtendedCameraControl? extendedcameracontrol = null;
            if (m_pEngine is null) return HRESULTS.MF_E_NOT_INITIALIZED;
            try
            {
                var hr = m_pEngine.GetSource(out pSource);
                var sser = pSource.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out var ds);

                mfservice = ds as IMFGetService;
                hr = mfservice.GetService(Guid.Empty, new Guid("b91ebfee-ca03-4af4-8a82-a31752f4a0fc"), out var con);
                extendedcameracontroller = con as IMFExtendedCameraController;
                //https://github.com/smourier/DirectN/blob/af1d27a173291bf648d3262952e36629e9420cbc/DirectN/DirectN/Generated/KSPROPERTY_CAMERACONTROL_EXTENDED.cs#L15
                
                hr = extendedcameracontroller.GetExtendedCameraControl(0xffffffff, (uint)m_KsProperty, out extendedcameracontrol);
                if (hr != HRESULTS.S_OK || extendedcameracontrol == null) return hr;
                //System.Diagnostics.Trace.WriteLine($"GetExtendedCameraControl {hr}");
                var capabilities = extendedcameracontrol.GetCapabilities();
                //System.Diagnostics.Trace.WriteLine($"GetCapabilities {capabilities}");
                hr = extendedcameracontrol.SetFlags(mode);
                //System.Diagnostics.Trace.WriteLine($"SetFlags {hr}");
                hr = extendedcameracontrol.CommitSettings();
                //System.Diagnostics.Trace.WriteLine($"CommitSettings {hr}");

            }
            finally
            {
                WebCam_MF.SafeRelease(extendedcameracontrol);
                WebCam_MF.SafeRelease(extendedcameracontroller);
                WebCam_MF.SafeRelease(mfservice);
                WebCam_MF.SafeRelease(pSource);
            }
            return HRESULTS.S_OK;
        }

    }
}
