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
    public partial class WebCam_MF
    {
        TaskCompletionSource<HRESULT>? m_TaskStartPreview;
        async public Task<HRESULT> StartPreview(IntPtr handle)
        {
            if (m_pEngine == null) return HRESULTS.MF_E_NOT_INITIALIZED;

            m_TaskStartPreview = new TaskCompletionSource<HRESULT>();


            IMFCaptureSink? pSink = null;
            IMFMediaType? pMediaType = null;
            IMFMediaType? pMediaType2 = null;
            IMFCaptureSource? pSource = null;
            IMFCapturePreviewSink? pPreview = null;
            HRESULT hr = HRESULTS.S_OK;
            try
            {
                hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_PREVIEW, out pSink);
                if (hr != HRESULTS.S_OK) return hr;
                pPreview = pSink as IMFCapturePreviewSink;
                if (pPreview == null) return HRESULTS.E_NOTIMPL;
                if (handle != IntPtr.Zero)
                {
                    hr = pPreview.SetRenderHandle(handle);
                    if (hr != HRESULTS.S_OK) return hr;
                }

                hr = m_pEngine.GetSource(out pSource);
                if (hr != HRESULTS.S_OK) return hr;



                // Configure the video format for the preview sink.
                hr = pSource.GetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, out pMediaType);
                if (hr != HRESULTS.S_OK) return hr;

                hr = WebCam_MF.CloneVideoMediaType(pMediaType, MFConstants.MFVideoFormat_RGB32, out pMediaType2);
                if (hr != HRESULTS.S_OK || pMediaType2 == null) return hr;

                hr = pMediaType2.SetUINT32(MFConstants.MF_MT_ALL_SAMPLES_INDEPENDENT, 1);
                if (hr != HRESULTS.S_OK) return hr;

                // Connect the video stream to the recording sink.
                if (this.m_Setting.Rotate == CameraRotates.Rotate90
                    || this.m_Setting.Rotate == CameraRotates.Rotate270
                    || this.m_Setting.Rotate == CameraRotates.Rotate90Colockwise
                    || this.m_Setting.Rotate == CameraRotates.Rotate270Colockwise)
                {
                    pMediaType2.TryGetSize(MFConstants.MF_MT_FRAME_SIZE, out var w, out var h);
                    pMediaType2.SetSize(MFConstants.MF_MT_FRAME_SIZE, h, w);
                }


                using var cm = new ComMemory(Marshal.SizeOf<uint>());
                hr = pPreview.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, pMediaType2, null, cm.Pointer);
                if (hr != HRESULTS.S_OK) return hr;
                var streamindex = (uint)Marshal.ReadInt32(cm.Pointer);
                System.Diagnostics.Trace.WriteLine($"preview streamindex:{streamindex}");

                System.Diagnostics.Debug.WriteLine($"AddStream preview{streamindex}");
                await this.AddVideoProcessorMFT(pSource, streamindex);


                pPreview.SetRotation(streamindex, (uint)m_Setting.Rotate);


                hr = m_pEngine.StartPreview();
                if (hr != HRESULTS.S_OK) return hr;
                hr = await m_TaskStartPreview.Task;
                m_TaskStartPreview = null;
                //m_IsPreviewing = true;
            }
            finally
            {
                SafeRelease(pPreview);
                SafeRelease(pSink);
                SafeRelease(pMediaType);
                SafeRelease(pMediaType2);
                SafeRelease(pSource);
            }

            return hr;
        }

        async public Task<HRESULT> StartPreview(MFCaptureEngineOnSampleCallback callback)
        {
            if (m_pEngine == null) return HRESULTS.MF_E_NOT_INITIALIZED;
            //if (m_IsPreviewing) return HRESULTS.S_OK;
            m_TaskStartPreview = new TaskCompletionSource<HRESULT>();
            IMFCaptureSink? pSink = null;
            IMFMediaType? pMediaType = null;
            IMFMediaType? pMediaType2 = null;
            IMFCaptureSource? pSource = null;
            IMFCapturePreviewSink? pPreview = null;
            HRESULT hr = HRESULTS.S_OK;
            try
            {
                hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_PREVIEW, out pSink);
                if (hr != HRESULTS.S_OK) return hr;
                pPreview = pSink as IMFCapturePreviewSink;
                if (pPreview == null) return HRESULTS.E_NOTIMPL;

                hr = m_pEngine.GetSource(out pSource);
                if (hr != HRESULTS.S_OK) return hr;


                // Configure the video format for the preview sink.
                hr = pSource.GetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, out pMediaType);
                if (hr != HRESULTS.S_OK) return hr;

                hr = WebCam_MF.CloneVideoMediaType(pMediaType, MFConstants.MFVideoFormat_RGB32, out pMediaType2);
                if (hr != HRESULTS.S_OK || pMediaType2 == null) return hr;

                hr = pMediaType2.SetUINT32(MFConstants.MF_MT_ALL_SAMPLES_INDEPENDENT, 1);
                if (hr != HRESULTS.S_OK) return hr;

                // Connect the video stream to the preview sink.
                using var cm = new ComMemory(Marshal.SizeOf<uint>());
                hr = pPreview.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, pMediaType2, null, cm.Pointer);
                if (hr != HRESULTS.S_OK) return hr;
                var streamindex = (uint)Marshal.ReadInt32(cm.Pointer);
                System.Diagnostics.Debug.WriteLine($"AddStream preview{streamindex}");
                hr = pPreview.SetSampleCallback(streamindex, callback);
                if (hr != HRESULTS.S_OK) return hr;
                if (this.m_Setting.IsMirror)
                {
                    await this.AddVideoProcessorMFT(pSource, streamindex);
                }


                hr = m_pEngine.StartPreview();
                if (hr != HRESULTS.S_OK) return hr;
                hr = await m_TaskStartPreview.Task;
                m_TaskStartPreview = null;

            }
            finally
            {
                SafeRelease(pPreview);
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
            //if (m_IsPreviewing) return HRESULTS.S_OK;
            m_TaskStartPreview = new TaskCompletionSource<HRESULT>();
            IMFCaptureSink? pSink = null;
            IMFMediaType? pMediaType = null;
            IMFMediaType? pMediaType2 = null;
            IMFCaptureSource? pSource = null;
            IMFCapturePreviewSink? pPreview = null;
            HRESULT hr = HRESULTS.S_OK;
            try
            {
                hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_PREVIEW, out pSink);
                if (hr != HRESULTS.S_OK) return hr;
                pPreview = pSink as IMFCapturePreviewSink;
                if (pPreview == null) return HRESULTS.E_NOTIMPL;

                hr = m_pEngine.GetSource(out pSource);
                if (hr != HRESULTS.S_OK) return hr;


                // Configure the video format for the preview sink.
                hr = pSource.GetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, out pMediaType);
                if (hr != HRESULTS.S_OK) return hr;

                hr = WebCam_MF.CloneVideoMediaType(pMediaType, MFConstants.MFVideoFormat_RGB32, out pMediaType2);
                if (hr != HRESULTS.S_OK || pMediaType2 == null) return hr;

                hr = pMediaType2.SetUINT32(MFConstants.MF_MT_ALL_SAMPLES_INDEPENDENT, 1);
                if (hr != HRESULTS.S_OK) return hr;

                // Connect the video stream to the preview sink.
                using var cm = new ComMemory(Marshal.SizeOf<uint>());
                hr = pPreview.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, pMediaType, null, cm.Pointer);
                if (hr != HRESULTS.S_OK) return hr;
                var streamindex = (uint)Marshal.ReadInt32(cm.Pointer);
                System.Diagnostics.Debug.WriteLine($"AddStream preview{streamindex}");
                hr = pPreview.SetSampleCallback(streamindex, samplecallback);
                if (hr != HRESULTS.S_OK) return hr;
                if (this.m_Setting.IsMirror)
                {
                    await this.AddVideoProcessorMFT(pSource, streamindex);
                }


                hr = m_pEngine.StartPreview();
                if (hr != HRESULTS.S_OK) return hr;
                hr = await m_TaskStartPreview.Task;
                m_TaskStartPreview = null;
                //m_IsPreviewing = true;
            }
            finally
            {
                SafeRelease(pPreview);
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
            IMFCaptureSink? pSink = null;
            IMFCaptureSource? pSource = null;
            try
            {
                if (m_pEngine == null)
                {
                    return HRESULTS.MF_E_NOT_INITIALIZED;
                }
                //if (!m_IsPreviewing)
                //{
                //    return HRESULTS.S_OK;
                //}
                this.m_TaskStopPreview = new();
                hr = m_pEngine.StopPreview();
                if (hr.IsError) return hr;
                hr = await this.m_TaskStopPreview.Task;
                if (hr.IsError) return hr;

                hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_PREVIEW, out pSink);
                if (hr.IsError) return hr;
                if (pSink is IMFCapturePreviewSink preview)
                {
                    preview.RemoveAllStreams();
                    SafeRelease(preview);
                }
                hr = m_pEngine.GetSource(out pSource);
                if (hr != HRESULTS.S_OK) return hr;
                //await RemoveAllVideoProcessorMFT(pSource);
                //m_IsPreviewing = false;
            }
            finally
            {
                SafeRelease(pSink);
                SafeRelease(pSource);
                this.m_TaskStopPreview = null;
            }
            return hr;
        }

    }
}
