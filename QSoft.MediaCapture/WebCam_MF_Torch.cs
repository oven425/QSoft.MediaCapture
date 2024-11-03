using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{
    public partial class WebCam_MF
    {
        public void InitTorch()
        {
            this.TorchLight = new TorchLight(this.m_pEngine);
        }
        public TorchLight? TorchLight { private set; get; }
    }
    //https://learn.microsoft.com/en-us/windows-hardware/drivers/stream/ksproperty-cameracontrol-extended-torchmode
    public class TorchLight : ExtendedCameraControl
    {
        //public bool IsSupported { private set; get; } = false;
        //readonly IMFCaptureEngine? m_pEngine;
        public TorchLight(IMFCaptureEngine? engine)
            : base(engine, KSPROPERTY_CAMERACONTROL_EXTENDED.KSPROPERTY_CAMERACONTROL_EXTENDED_TORCHMODE)
        {
            {
                //m_pEngine = engine;
            }

            //public HRESULT Get()
            //{
            //    return HRESULTS.S_OK;
            //}

            //public HRESULT Set(ulong mode)
            //{
            //    IMFCaptureSource? pSource = null;
            //    IMFGetService? mfservice = null;
            //    IMFExtendedCameraController? extendedcameracontroller = null;
            //    IMFExtendedCameraControl? extendedcameracontrol = null;
            //    if (m_pEngine is null) return HRESULTS.MF_E_NOT_INITIALIZED;
            //    try
            //    {
            //        var hr = m_pEngine.GetSource(out pSource);
            //        var sser = pSource.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out var ds);

            //        mfservice = ds as IMFGetService;
            //        hr = mfservice.GetService(Guid.Empty, new Guid("b91ebfee-ca03-4af4-8a82-a31752f4a0fc"), out var con);
            //        extendedcameracontroller = con as IMFExtendedCameraController;
            //        //https://github.com/smourier/DirectN/blob/af1d27a173291bf648d3262952e36629e9420cbc/DirectN/DirectN/Generated/KSPROPERTY_CAMERACONTROL_EXTENDED.cs#L15
            //        hr = extendedcameracontroller.GetExtendedCameraControl(0xffffffff, (int)KSPROPERTY_CAMERACONTROL_EXTENDED.KSPROPERTY_CAMERACONTROL_EXTENDED_TORCHMODE, out extendedcameracontrol);
            //        if (hr != HRESULTS.S_OK) return hr;
            //        System.Diagnostics.Trace.WriteLine($"GetExtendedCameraControl {hr}");
            //        var capabilities = extendedcameracontrol.GetCapabilities();
            //        System.Diagnostics.Trace.WriteLine($"GetCapabilities {capabilities}");
            //        hr = extendedcameracontrol.SetFlags(mode);
            //        System.Diagnostics.Trace.WriteLine($"SetFlags {hr}");
            //        hr = extendedcameracontrol.CommitSettings();
            //        System.Diagnostics.Trace.WriteLine($"CommitSettings {hr}");

            //    }
            //    finally
            //    {
            //        WebCam_MF.SafeRelease(extendedcameracontrol);
            //        WebCam_MF.SafeRelease(extendedcameracontroller);
            //        WebCam_MF.SafeRelease(mfservice);
            //        WebCam_MF.SafeRelease(pSource);
            //    }
            //    return HRESULTS.S_OK;
            //}
        }
    }

}
