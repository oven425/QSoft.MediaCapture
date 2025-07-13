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
        readonly Dictionary<uint, IMFVideoProcessorControl> m_VideoProcessors = [];
        //no support yuy2 foramt
        async Task<HRESULT> AddVideoProcessorMFT(IMFCaptureSource source, uint streamindex)
        {
            IMFVideoProcessorControl? videoprocessmft;
            if (m_VideoProcessors.TryGetValue(streamindex, out IMFVideoProcessorControl? value))
            {
                videoprocessmft = value;
            }
            else
            {
                var videoprocesstype = Type.GetTypeFromCLSID(DirectN.MFConstants.CLSID_VideoProcessorMFT);
                videoprocessmft = Activator.CreateInstance(videoprocesstype) as IMFVideoProcessorControl;
            }
            IMFMediaType? pMediaType = null;
            try
            {
                source.GetCurrentDeviceMediaType(streamindex, out pMediaType);
                //this.m_VideoProcessor2 = this.m_VideoProcessor as IMFVideoProcessorControl2;
                HRESULT hr;
                if (this.m_Setting.IsMirror && videoprocessmft != null)
                {
                    hr = videoprocessmft.SetMirror(_MF_VIDEO_PROCESSOR_MIRROR.MIRROR_HORIZONTAL);
                    //m_VideoProcessor.SetRotation(_MF_VIDEO_PROCESSOR_ROTATION.ROTATION_NONE);
                    //m_VideoProcessor2.SetRotationOverride(90);
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

        async Task<HRESULT> RemoveVideoProcessorMFT(IMFCaptureSource source, uint streamindex)
        {
            m_TaskRemoveAllEffect = new();
            source.RemoveAllEffects(streamindex);
            var hr = await m_TaskRemoveAllEffect.Task;
            return hr;

        }

        TaskCompletionSource<HRESULT>? m_TaskRemoveAllEffect;
        async Task<HRESULT> RemoveAllVideoProcessorMFT(IMFCaptureSource source)
        {

            foreach (var oo in this.m_VideoProcessors)
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

        }
    }
}
