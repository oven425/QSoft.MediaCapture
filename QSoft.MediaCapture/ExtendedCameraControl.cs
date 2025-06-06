﻿using DirectN;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{
    public class ExtendedCameraControl
    {
        [Conditional("DEBUG")]
        public static void TetsALL(IMFCaptureEngine? engine)
        {
            if (engine == null) return;
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
            if (this.Get(out var mode) == HRESULTS.S_OK)
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
                
                //hr = extendedcameracontrol.LockPayload(out var ptr, out var len);

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

        public HRESULT GetPayload(out byte[] buffer)
        {
            buffer = null;
            ulong data = 0;
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

                hr = extendedcameracontrol.LockPayload(out var ptr, out var len);
                buffer = new byte[len];
                Marshal.Copy(ptr, buffer, 0, buffer.Length);
                extendedcameracontrol.UnlockPayload();

                //8+40*count
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
                //var capabilities = extendedcameracontrol.GetCapabilities();
                //System.Diagnostics.Trace.WriteLine($"GetCapabilities {capabilities}");
                mode = extendedcameracontrol.GetFlags();
                System.Diagnostics.Trace.WriteLine($"GetFlags {mode}");
                //hr = extendedcameracontrol.CommitSettings();
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

                mfservice = mediasource as IMFGetService;
                hr = mfservice.GetService(Guid.Empty, new Guid("b91ebfee-ca03-4af4-8a82-a31752f4a0fc"), out var con);
                extendedcameracontroller = con as IMFExtendedCameraController;
                //https://github.com/smourier/DirectN/blob/af1d27a173291bf648d3262952e36629e9420cbc/DirectN/DirectN/Generated/KSPROPERTY_CAMERACONTROL_EXTENDED.cs#L15

                hr = extendedcameracontroller.GetExtendedCameraControl((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.MF_CAPTURE_ENGINE_MEDIASOURCE, (uint)m_KsProperty, out extendedcameracontrol);
                System.Diagnostics.Trace.WriteLine($"GetExtendedCameraControl {hr}");
                if (hr != HRESULTS.S_OK || extendedcameracontrol == null) return hr;
                
                //var capabilities = extendedcameracontrol.GetCapabilities();
                //System.Diagnostics.Trace.WriteLine($"GetCapabilities {capabilities}");
                hr = extendedcameracontrol.SetFlags(mode);
                System.Diagnostics.Trace.WriteLine($"SetFlags {hr}");
                hr = extendedcameracontrol.CommitSettings();
                System.Diagnostics.Trace.WriteLine($"CommitSettings {hr}");
                
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

    }

}
