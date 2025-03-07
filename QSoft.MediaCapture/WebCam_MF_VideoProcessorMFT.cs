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
        //uint m_VideoProcessMFT_Index = 0;
        TaskCompletionSource<HRESULT>? m_TaskAddEffect;
        //IMFVideoProcessorControl? m_VideoProcessor;
        //IMFVideoProcessorControl2? m_VideoProcessor2;
        Dictionary<uint, IMFVideoProcessorControl> m_VideoProcessors = [];
        async Task<HRESULT> AddVideoProcessorMFT(IMFCaptureSource source, uint streamindex)
        {
            if (m_VideoProcessors.ContainsKey(streamindex)) return HRESULTS.S_OK;
            //m_VideoProcessMFT_Index = streamindex;
            IMFMediaType? pMediaType=null;
            try
            {
                source.GetCurrentDeviceMediaType(streamindex, out pMediaType);
                var videoprocesstype = Type.GetTypeFromCLSID(DirectN.MFConstants.CLSID_VideoProcessorMFT);

                var videoprocessmft = Activator.CreateInstance(videoprocesstype) as IMFVideoProcessorControl;
                //this.m_VideoProcessor2 = this.m_VideoProcessor as IMFVideoProcessorControl2;
                HRESULT hr;
                if (this.m_Setting.IsMirror && videoprocessmft != null)
                {
                    hr = videoprocessmft.SetMirror(_MF_VIDEO_PROCESSOR_MIRROR.MIRROR_HORIZONTAL);
                    //m_VideoProcessor.SetRotation(_MF_VIDEO_PROCESSOR_ROTATION.ROTATION_NONE);
                    //m_VideoProcessor2.SetRotationOverride(90);
                    //if (hr != HRESULTS.S_OK) return Task.FromResult(hr);
                }
                

                if (videoprocessmft is IMFTransform mft)
                {
                    hr = mft.SetInputType(0, pMediaType, 0);
                    if (hr != HRESULTS.S_OK) return hr;
                    hr = mft.SetOutputType(0, pMediaType, 0);
                    if (hr != HRESULTS.S_OK) return hr;
                }

                m_TaskAddEffect = new();

                hr = source.AddEffect(streamindex, videoprocessmft);
                if (hr != HRESULTS.S_OK) return hr;
                if(videoprocessmft is not null)
                {
                    m_VideoProcessors[streamindex] = videoprocessmft;
                }
                
                return  await m_TaskAddEffect.Task;
            }
            finally
            {
                SafeRelease(pMediaType);
            }
        }

        TaskCompletionSource<HRESULT>? m_TaskRemoveAllEffect;
        async Task<HRESULT> RemoveAllVideoProcessorMFT(IMFCaptureSource source)
        {
            foreach(var oo in this.m_VideoProcessors)
            {
                m_TaskRemoveAllEffect = new();
                source.RemoveAllEffects(oo.Key);
                var hr = await m_TaskRemoveAllEffect.Task;
                if (hr != HRESULTS.S_OK)
                {
                    return hr;
                }
                SafeRelease(oo.Value);
            }
            m_VideoProcessors.Clear();
            return HRESULTS.S_OK;
            
            ////source.RemoveAllEffects(m_VideoProcessMFT_Index);
            //var hr = await m_TaskRemoveAllEffect.Task;
            //if(hr == HRESULTS.S_OK)
            //{
            //    ClearVideoProcessorMFT();
            //    //SafeRelease(m_VideoProcessor);
            //    //m_VideoProcessor = null;
            //    //m_VideoProcessor2 = null;
            //}
            //return hr;
        }
    }
}
