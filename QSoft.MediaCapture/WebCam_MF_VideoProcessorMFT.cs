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
        TaskCompletionSource<HRESULT>? m_TaskAddEffect;
        IMFVideoProcessorControl? m_VideoProcessor;
        Task<HRESULT> AddVideoProcessorMFT(IMFCaptureSource source, uint streamindex)
        {
            IMFMediaType pMediaType;
            source.GetCurrentDeviceMediaType(streamindex, out pMediaType);

            //CLSID_CColorControlDmo
            this.m_VideoProcessor = Activator.CreateInstance(Type.GetTypeFromCLSID(DirectN.MFConstants.CLSID_VideoProcessorMFT)!) as IMFVideoProcessorControl;
            HRESULT? hr = HRESULTS.S_OK;
            if(this.m_Setting.IsMirror && m_VideoProcessor != null)
            {
                //m_VideoProcessor.SetRotation(_MF_VIDEO_PROCESSOR_ROTATION.ROTATION_NORMAL);
                //hr = m_VideoProcessor?.SetRotationOverride(90);
                hr = m_VideoProcessor?.SetMirror(_MF_VIDEO_PROCESSOR_MIRROR.MIRROR_HORIZONTAL);
            }
            

            //HRESULT hr;
            IMFTransform? mft = m_VideoProcessor as IMFTransform;
            if (mft != null)
            {
                hr = mft.SetInputType(0, pMediaType, 0);
                //if (COMBase.Failed(hr))
                //{
                //    System.Diagnostics.Trace.WriteLine($"SetInputType  {hr}");
                //    goto done;
                //}


                hr = mft.SetOutputType(0, pMediaType, 0);
                //if (COMBase.Failed(hr))
                //{
                //    System.Diagnostics.Trace.WriteLine($"SetOutputType  {hr}");
                //    goto done;
                //}
            }

            m_TaskAddEffect = new TaskCompletionSource<HRESULT>();

            hr = source.AddEffect(streamindex, m_VideoProcessor);
            return m_TaskAddEffect.Task;
            //object o;
            //pFactory.CreateInstance(DirectN.MFConstants.CLSID_MFCaptureEngine, typeof(IMFCaptureEngine).GUID, out o);
            //m_pEngine = o as IMFVideoProcessorControl;

        }

    }
}
