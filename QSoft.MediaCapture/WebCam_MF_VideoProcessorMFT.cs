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
        uint m_VideoProcessMFT_Index = 0;
        TaskCompletionSource<HRESULT>? m_TaskAddEffect;
        IMFVideoProcessorControl? m_VideoProcessor;
        Task<HRESULT> AddVideoProcessorMFT(IMFCaptureSource source, uint streamindex)
        {
            m_VideoProcessMFT_Index = streamindex;
            IMFMediaType? pMediaType=null;
            try
            {
                source.GetCurrentDeviceMediaType(streamindex, out pMediaType);
                var videoprocesstype = Type.GetTypeFromCLSID(DirectN.MFConstants.CLSID_VideoProcessorMFT);
                this.m_VideoProcessor = Activator.CreateInstance(videoprocesstype) as IMFVideoProcessorControl;

                HRESULT hr;
                if (this.m_Setting.IsMirror && m_VideoProcessor != null)
                {
                    hr = m_VideoProcessor.SetMirror(_MF_VIDEO_PROCESSOR_MIRROR.MIRROR_HORIZONTAL);
                    if (hr != HRESULTS.S_OK) return Task.FromResult(hr);
                }


                if (m_VideoProcessor is IMFTransform mft)
                {
                    hr = mft.SetInputType(0, pMediaType, 0);
                    if (hr != HRESULTS.S_OK) return Task.FromResult(hr);
                    hr = mft.SetOutputType(0, pMediaType, 0);
                    if (hr != HRESULTS.S_OK) return Task.FromResult(hr);
                }

                m_TaskAddEffect = new();

                hr = source.AddEffect(streamindex, m_VideoProcessor);
                if (hr != HRESULTS.S_OK) return Task.FromResult(hr);
                return m_TaskAddEffect.Task;
            }
            finally
            {
                SafeRelease(pMediaType);
            }
        }

        TaskCompletionSource<HRESULT>? m_TaskRemoveAllEffect;
        async Task<HRESULT> RemoveAllVideoProcessorMFT(IMFCaptureSource source)
        {
            m_TaskRemoveAllEffect = new();
            source.RemoveAllEffects(m_VideoProcessMFT_Index);
            var hr = await m_TaskRemoveAllEffect.Task;
            return hr;
        }

    }
}
