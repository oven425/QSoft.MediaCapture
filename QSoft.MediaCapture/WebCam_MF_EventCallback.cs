using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QSoft.MediaCapture
{
    
    public class MediaCaptureFailedEventArgs:EventArgs
    {
        public uint Code { set; get; }
        public string Message { set; get; } = "";
    }
    public partial class WebCam_MF : IMFCaptureEngineOnEventCallback
    {
        public event EventHandler<MediaCaptureFailedEventArgs>? MediaCaptureFailedEventHandler;
        public HRESULT OnEvent(IMFMediaEvent pEvent)
        {
            HRESULT hr = pEvent.GetStatus(out HRESULT hrStatus);
            if (hr != HRESULTS.S_OK)
            {
                hrStatus = hr;
            }

            hr = pEvent.GetExtendedType(out Guid guidType);
            if (hr == HRESULTS.S_OK)
            {
                if (guidType == MFConstants.MF_CAPTURE_ENGINE_INITIALIZED)
                {
                    m_TaskInitialize?.SetResult(hrStatus);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_PREVIEW_STARTED)
                {
                    m_TaskStartPreview?.SetResult(hrStatus);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_PREVIEW_STOPPED)
                {
                    m_TaskStopPreview?.SetResult(hrStatus);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_RECORD_STARTED)
                {
                    m_TaskStartRecord?.SetResult(hrStatus);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_RECORD_STOPPED)
                {
                    m_TaskStopRecord?.SetResult(hrStatus);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_PHOTO_TAKEN)
                {
                    this.m_TaskTakephoto?.SetResult(hrStatus);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_EFFECT_ADDED)
                {
                    m_TaskAddEffect?.SetResult(hrStatus);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_ALL_EFFECTS_REMOVED)
                {
                    m_TaskRemoveAllEffect?.SetResult(hrStatus);
                }
                else if (guidType == MFConstants.MF_CAPTURE_SOURCE_CURRENT_DEVICE_MEDIA_TYPE_SET)
                {
                    m_TaskSetCurrentType?.SetResult(hrStatus);
                    //m_TaskSetMediaType?.SetResult(hrStatus);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_ERROR)
                {
                    MediaCaptureFailedEventHandler?.Invoke(this, new MediaCaptureFailedEventArgs()
                    {
                        Code = (uint)hrStatus,
                        Message = Marshal.GetExceptionForHR(hrStatus.Value)?.Message ?? string.Empty,
                    });
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine(guidType);
                    // This is an event we don't know about, we don't really care and there's
                    // no clean way to report the error so just set the event and fall through.
                    //SetEvent(m_pManager->m_hEvent);
                }
            }
            
            return HRESULTS.S_OK;
        }
    }

    
}
