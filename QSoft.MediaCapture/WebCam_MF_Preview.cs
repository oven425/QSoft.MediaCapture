using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{
    public partial class WebCam_MF
    {
        bool m_IsPreviewing = false;
        TaskCompletionSource<HRESULT>? m_TaskStartPreview;
        async public Task<HRESULT> StartPreview(IntPtr handle)
        {
            if (m_pEngine == null) return HRESULTS.MF_E_NOT_INITIALIZED;

            m_TaskStartPreview = new TaskCompletionSource<HRESULT>();

            if (m_IsPreviewing) return HRESULTS.S_OK;

            IMFCaptureSink? pSink = null;
            IMFMediaType? pMediaType = null;
            IMFMediaType? pMediaType2 = null;
            IMFCaptureSource? pSource = null;
            HRESULT hr = HRESULTS.S_OK;
            try
            {
                // Get a pointer to the preview sink.
                if (m_pPreview == null)
                {
                    hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_PREVIEW, out pSink);
                    if (hr != HRESULTS.S_OK) return hr;
                    m_pPreview = pSink as IMFCapturePreviewSink;
                    if (m_pPreview == null) return HRESULTS.E_NOTIMPL;
                    if (handle != IntPtr.Zero)
                    {
                        hr = m_pPreview.SetRenderHandle(handle);
                        if (hr != HRESULTS.S_OK) return hr;
                    }

                    hr = m_pEngine.GetSource(out pSource);
                    if (hr != HRESULTS.S_OK) return hr;



                    // Configure the video format for the preview sink.
                    hr = pSource.GetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, out pMediaType);
                    if (hr != HRESULTS.S_OK) return hr;

                    hr = CloneVideoMediaType(pMediaType, MFConstants.MFVideoFormat_RGB24, out pMediaType2);
                    if (hr != HRESULTS.S_OK || pMediaType2 == null) return hr;

                    hr = pMediaType2.SetUINT32(MFConstants.MF_MT_ALL_SAMPLES_INDEPENDENT, 1);
                    if (hr != HRESULTS.S_OK) return hr;
                    
                    // Connect the video stream to the preview sink.
                    using var cm = new ComMemory(Marshal.SizeOf<uint>());
                    hr = m_pPreview.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, pMediaType2, null, cm.Pointer);
                    if (hr != HRESULTS.S_OK) return hr;
                    var streamindex = (uint)Marshal.ReadInt32(cm.Pointer);
                    await this.AddVideoProcessorMFT(pSource, streamindex);
                    m_pPreview.SetRotation(streamindex, m_Setting.Rotate);
                }


                hr = m_pEngine.StartPreview();
                if (hr != HRESULTS.S_OK) return hr;
                hr = await m_TaskStartPreview.Task;
                m_TaskStartPreview = null;
                m_IsPreviewing = true;
            }
            finally
            {
                SafeRelease(pSink);
                SafeRelease(pMediaType);
                SafeRelease(pMediaType2);
                SafeRelease(pSource);
            }

            return hr;
        }

        async public Task<HRESULT> StartPreview(IMFCaptureEngineOnSampleCallback samplecallback)
        {
            if (m_pEngine == null) return HRESULTS.MF_E_NOT_INITIALIZED;
            if (m_IsPreviewing) return HRESULTS.S_OK;
            m_TaskStartPreview = new TaskCompletionSource<HRESULT>();
            IMFCaptureSink? pSink = null;
            IMFMediaType? pMediaType = null;
            IMFMediaType? pMediaType2 = null;
            IMFCaptureSource? pSource = null;
            HRESULT hr = HRESULTS.S_OK;
            try
            {
                if (m_pPreview == null)
                {
                    hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_PREVIEW, out pSink);
                    if (hr != HRESULTS.S_OK) return hr;
                    m_pPreview = pSink as IMFCapturePreviewSink;
                    if (m_pPreview == null) return HRESULTS.E_NOTIMPL;

                    hr = m_pEngine.GetSource(out pSource);
                    if (hr != HRESULTS.S_OK) return hr;

                    // Configure the video format for the preview sink.
                    hr = pSource.GetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, out pMediaType);
                    if (hr != HRESULTS.S_OK) return hr;

                    hr = CloneVideoMediaType(pMediaType, MFConstants.MFVideoFormat_RGB24, out pMediaType2);
                    if (hr != HRESULTS.S_OK || pMediaType2 == null) return hr;

                    hr = pMediaType2.SetUINT32(MFConstants.MF_MT_ALL_SAMPLES_INDEPENDENT, 1);
                    if (hr != HRESULTS.S_OK) return hr;

                    // Connect the video stream to the preview sink.
                    using var cm = new ComMemory(Marshal.SizeOf<uint>());
                    hr = m_pPreview.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, pMediaType2, null, cm.Pointer);
                    if (hr != HRESULTS.S_OK) return hr;
                    var streamindex = (uint)Marshal.ReadInt32(cm.Pointer);


                    hr = m_pPreview.SetSampleCallback(streamindex, samplecallback);
                    if (hr != HRESULTS.S_OK) return hr;

                    await this.AddVideoProcessorMFT(pSource, streamindex);

                }


                hr = m_pEngine.StartPreview();
                if (hr != HRESULTS.S_OK) return hr;
                hr = await m_TaskStartPreview.Task;
                m_TaskStartPreview = null;
                m_IsPreviewing = true;
            }
            finally
            {
                SafeRelease(pSink);
                SafeRelease(pMediaType);
                SafeRelease(pMediaType2);
                SafeRelease(pSource);
            }


            return hr;
        }



        TaskCompletionSource<HRESULT>? m_TaskStopPreview;
        async public Task<HRESULT?> StopPreview()
        {
            HRESULT hr = HRESULTS.S_OK;
            try
            {
                if (m_pEngine == null)
                {
                    return HRESULTS.MF_E_NOT_INITIALIZED;
                }
                if (!m_IsPreviewing)
                {
                    return HRESULTS.S_OK;
                }
                if (m_TaskStopPreview == null)
                {
                    this.m_TaskStopPreview = new TaskCompletionSource<HRESULT>(hr);
                }

                hr = m_pEngine.StopPreview();
                if (hr.IsError) return hr;
                hr = await this.m_TaskStopPreview.Task;
                if (hr.IsError) return hr;
                m_IsPreviewing = false;
            }
            finally
            {
                this.m_TaskStopPreview = null;
            }
            return hr;
        }

    }
}
