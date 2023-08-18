using DirectN;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//https://github.com/MicrosoftDocs/win32/blob/docs/desktop-src/medfound/directx-surface-buffer.md
namespace QSoft.MediaCapture
{
    public enum MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM : uint
    {
        FOR_VIDEO_PREVIEW = 0xfffffffa,
        FOR_VIDEO_RECORD = 0xfffffff9,
        FOR_PHOTO = 0xfffffff8,
        FOR_AUDIO = 0xfffffff7,
        FOR_METADATA = 0xfffffff6,
        MF_CAPTURE_ENGINE_MEDIASOURCE = 0xffffffff
    }
    public class WebCam_MF : IMFCaptureEngineOnEventCallback, IDisposable
    {
        public static WebCam_MF PhotoMode()
        {
            return new WebCam_MF();
        }
        public static WebCam_MF RecordMode()
        {
            return new WebCam_MF();
        }
        uint g_ResetToken = 0;
        IMFDXGIDeviceManager g_pDXGIMan;
        HRESULT CreateD3DManager()
        {
            HRESULT hr = DirectN.HRESULTS.S_OK;
            D3D_FEATURE_LEVEL FeatureLevel;
            ID3D11DeviceContext pDX11DeviceContext;
            hr = CreateDX11Device(out g_pDX11Device, out pDX11DeviceContext, out FeatureLevel);

            if (hr == HRESULTS.S_OK)
            {
                //g_pDXGIMan = DirectN.MFFunctions.MFCreateDXGIDeviceManager(out g_ResetToken);
                hr = DirectN.MFFunctions.MFCreateDXGIDeviceManager(out g_ResetToken, out g_pDXGIMan);
                //hr = MFCreateDXGIDeviceManager(&g_ResetToken, &g_pDXGIMan);
            }

            if (hr == HRESULTS.S_OK)
            {
                hr = g_pDXGIMan.ResetDevice(g_pDX11Device, g_ResetToken);
            }
            Marshal.ReleaseComObject(pDX11DeviceContext);

            return hr;
        }

        ID3D11Device g_pDX11Device;
        ID3D11DeviceContext m_pDeviceContext;
        HRESULT CreateDX11Device(out ID3D11Device ppDevice, out ID3D11DeviceContext ppDeviceContext, out D3D_FEATURE_LEVEL pFeatureLevel)
        {
            HRESULT hr = HRESULTS.S_OK;


            D3D_FEATURE_LEVEL[] levels = new[]{
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_1,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_0,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_3,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_2,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_1
            };

            hr = DirectN.D3D11Functions.D3D11CreateDevice(
                null,
                D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE,
                IntPtr.Zero,
                (uint)DirectN.D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_VIDEO_SUPPORT,
                levels,
                (uint)levels.Length,
                7,
                out ppDevice,
                out pFeatureLevel,
                out ppDeviceContext
                );

            if (hr == HRESULTS.S_OK)
            {
                ID3D10Multithread pMultithread;
                pMultithread = ppDevice as ID3D10Multithread;
                //hr = ppDevice.QueryInterface(IID_PPV_ARGS(&pMultithread)));
                if (hr == HRESULTS.S_OK)
                {
                    var bb = pMultithread.SetMultithreadProtected(true);
                }
            }

            return hr;
        }
        public Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, List<(Guid format, uint width, uint height, double fps, IMFMediaType mediatype)>> VideoFormats = new Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, List<(Guid format, uint width, uint height, double fps, IMFMediaType mediatype)>>();

        TaskCompletionSource<HRESULT> m_TaskCompelete;
        bool m_IsMirror = false;
        async public Task<HRESULT> InitializeCaptureManager(object videosource, bool ismirror = false)
        {
            m_IsMirror = ismirror;
            m_TaskCompelete = new TaskCompletionSource<HRESULT>();
            HRESULT hr = HRESULTS.S_OK;
            IMFAttributes pAttributes = null;
            IMFCaptureEngineClassFactory pFactory = null;

            DestroyCaptureEngine();

            //Create a D3D Manager
            hr = CreateD3DManager();
            if (hr != HRESULTS.S_OK)
            {
                goto Exit;
            }
            hr = MFFunctions.MFCreateAttributes(out pAttributes, 1);
            if (hr != HRESULTS.S_OK)
            {
                goto Exit;
            }
            hr = pAttributes.SetUnknown(MFConstants.MF_CAPTURE_ENGINE_D3D_MANAGER, g_pDXGIMan);
            if (hr != HRESULTS.S_OK)
            {
                goto Exit;
            }

            pFactory = Activator.CreateInstance(Type.GetTypeFromCLSID(DirectN.MFConstants.CLSID_MFCaptureEngineClassFactory)) as IMFCaptureEngineClassFactory;
            object o;
            pFactory.CreateInstance(DirectN.MFConstants.CLSID_MFCaptureEngine, typeof(IMFCaptureEngine).GUID, out o);
            m_pEngine = o as IMFCaptureEngine;
            hr = m_pEngine.Initialize(this, pAttributes, null, videosource);
            if (hr != HRESULTS.S_OK)
            {
                goto Exit;
            }
            hr = m_pEngine.GetSource(out var pSource);
            if (hr != HRESULTS.S_OK)
            {
                goto Exit;
            }
            hr = await m_TaskCompelete.Task;
            var medias = pSource.GetAllMediaType();
            //var mm = medias[MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_VIDEO_CAPTURE].GetVideoData().GroupBy(x=>x.format);
            //Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, List<(Guid format, uint width, uint height, double fps)>> videos = new Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, List<(Guid format, uint width, uint height, double fps)>>();
            foreach (var media in medias)
            {
                VideoFormats[media.Key] = media.Value.GetVideoData().ToList();
            }
            SafeRelease(pSource);
        Exit:
            if (null != pAttributes)
            {
                SafeRelease(pAttributes);
                //pAttributes->Release();
                //pAttributes = NULL;
            }
            if (null != pFactory)
            {
                SafeRelease(pFactory);
                //pFactory->Release();
                //pFactory = NULL;
            }
            return hr;
        }

        void DestroyCaptureEngine()
        {
            //if (NULL != m_hEvent)
            //{
            //    CloseHandle(m_hEvent);
            //    m_hEvent = NULL;
            //}
            SafeRelease(m_pPreview);
            SafeRelease(m_pEngine);
            //SafeRelease(m_pCallback);

            if (g_pDXGIMan != null)
            {
                g_pDXGIMan.ResetDevice(g_pDX11Device, g_ResetToken);
            }
            SafeRelease(g_pDX11Device);
            SafeRelease(g_pDXGIMan);

            //m_bPreviewing = false;
            //m_bRecording = false;
            //m_bPhotoPending = false;
            //m_errorID = 0;
        }

        IMFCaptureEngine m_pEngine;
        IMFCapturePreviewSink m_pPreview;

        //CaptureEngineCB* m_pCallback;

        bool m_bPreviewing;
        bool m_bRecording;
        bool m_bPhotoPending;

        uint m_errorID;
        //HANDLE m_hEvent;
        //HANDLE m_hpwrRequest;
        bool m_fPowerRequestSet;

        TaskCompletionSource<HRESULTS> m_StartPreviewTask = new TaskCompletionSource<HRESULTS>();
        async public Task<HRESULTS> StartPreview(IntPtr hwnd)
        {
            if (m_pEngine == null)
            {
                return HRESULTS.MF_E_NOT_INITIALIZED;
            }

            if (m_bPreviewing == true)
            {
                return HRESULTS.S_OK;
            }

            IMFCaptureSink pSink = null;
            IMFMediaType pMediaType = null;
            IMFMediaType pMediaType2 = null;
            IMFCaptureSource pSource = null;
            //IntPtr dwSinkStreamIndex = IntPtr.Zero;
            HRESULT hr = HRESULTS.S_OK;
            // Get a pointer to the preview sink.
            if (m_pPreview == null)
            {
                hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_PREVIEW, out pSink);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }
                m_pPreview = pSink as IMFCapturePreviewSink;

                hr = m_pPreview.SetRenderHandle(hwnd);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }

                hr = m_pEngine.GetSource(out pSource);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }
                this.Mirror(pSource, (uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW);
                //// Configure the video format for the preview sink.
                hr = pSource.GetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, out pMediaType);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }

                hr = CloneVideoMediaType(pMediaType, MFConstants.MFVideoFormat_RGB32, out pMediaType2);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }

                hr = pMediaType2.SetUINT32(MFConstants.MF_MT_ALL_SAMPLES_INDEPENDENT, 1);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }

                // Connect the video stream to the preview sink.

                using (var cm = new ComMemory(Marshal.SizeOf<uint>()))
                {
                    hr = m_pPreview.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, pMediaType2, null, cm.Pointer);
                    if (hr != HRESULTS.S_OK)
                    {
                        goto done;
                    }
                    dwSinkStreamIndex = (uint)Marshal.ReadInt32(cm.Pointer);
                }

                //m_pPreview.SetRotation(0, 90);
            }


            hr = m_pEngine.StartPreview();
        //if (!m_fPowerRequestSet && m_hpwrRequest != INVALID_HANDLE_VALUE)
        //{
        //    // NOTE:  By calling this, on SOC systems (AOAC enabled), we're asking the system to not go
        //    // into sleep/connected standby while we're streaming.  However, since we don't want to block
        //    // the device from ever entering connected standby/sleep, we're going to latch ourselves to
        //    // the monitor on/off notification (RegisterPowerSettingNotification(GUID_MONITOR_POWER_ON)).
        //    // On SOC systems, this notification will fire when the user decides to put the device in
        //    // connected standby mode--we can trap this, turn off our media streams and clear this
        //    // power set request to allow the device to go into the lower power state.
        //    m_fPowerRequestSet = (TRUE == PowerSetRequest(m_hpwrRequest, PowerRequestExecutionRequired));
        //}
        done:
            Marshal.ReleaseComObject(pSink);
            Marshal.ReleaseComObject(pMediaType);
            Marshal.ReleaseComObject(pMediaType2);
            Marshal.ReleaseComObject(pSource);


            return await m_StartPreviewTask.Task;
        }
        TaskCompletionSource<HRESULTS> m_SetMediaTypeTask;
        MFCaptureEngineOnSampleCallback m_PreviewCallback;
        uint dwSinkStreamIndex = 0;
        public async Task<HRESULTS> StartPreview(Action<WriteableBitmap> action)
        {
            if (m_pEngine == null)
            {
                return HRESULTS.MF_E_NOT_INITIALIZED;
                //return HRESULTS.MF_E_NOT_INITIALIZED;
            }

            if (m_bPreviewing == true)
            {
                //return await Task.FromResult(HRESULTS.S_OK);
                //return HRESULTS.S_OK;
                return HRESULTS.S_OK;
            }
            if (this.m_StartPreviewTask.Task.IsCompleted == true)
            {
                return HRESULTS.S_OK;
            }

            IMFCaptureSink pSink = null;
            IMFMediaType pMediaType = null;
            IMFMediaType pMediaType2 = null;
            IMFCaptureSource pSource = null;

            HRESULT hr = HRESULTS.S_OK;
            // Get a pointer to the preview sink.
            if (m_pPreview == null)
            {
                hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_PREVIEW, out pSink);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }
                m_pPreview = pSink as IMFCapturePreviewSink;

                //hr = m_pPreview.SetRenderHandle(hwnd);
                //if (hr != HRESULTS.S_OK)
                //{
                //    goto done;
                //}



                hr = m_pEngine.GetSource(out pSource);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }
                if(this.VideoFormats.ContainsKey(MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_VIDEO_CAPTURE))
                {
                    var aa = this.VideoFormats[MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_VIDEO_CAPTURE].OrderByDescending(x => x.width).ThenByDescending(x => x.fps);
                    m_SetMediaTypeTask = new TaskCompletionSource<HRESULTS>();
                    hr = pSource.SetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, aa.First().mediatype);
                    if (hr != HRESULTS.S_OK)
                    {
                        goto done;
                    }
                    hr = await m_SetMediaTypeTask.Task;
                }
                
                //// Configure the video format for the preview sink.
                hr = pSource.GetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, out pMediaType);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }
                
                hr = CloneVideoMediaType(pMediaType, MFConstants.MFVideoFormat_RGB24, out pMediaType2);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }

                hr = pMediaType2.SetUINT32(MFConstants.MF_MT_ALL_SAMPLES_INDEPENDENT, 1);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }

                // Connect the video stream to the preview sink.
                using (var cm = new ComMemory(Marshal.SizeOf<uint>()))
                {
                    hr = m_pPreview.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_PREVIEW, pMediaType2, null, cm.Pointer);
                    if (hr != HRESULTS.S_OK)
                    {
                        goto done;
                    }
                    dwSinkStreamIndex = (uint)Marshal.ReadInt32(cm.Pointer);
                }
                if(this.m_IsMirror == true)
                {
                    this.Mirror(pSource, dwSinkStreamIndex);
                }
                
                uint w = 0;
                uint h = 0;
                MFFunctions1.MFGetAttributeSize(pMediaType2, MFConstants.MF_MT_FRAME_SIZE, out w, out h);
                m_PreviewBmp = new WriteableBitmap((int)w, (int)h, 96, 96, PixelFormats.Bgr24, null);
                m_PreviewCallback = new MFCaptureEngineOnSampleCallback(m_PreviewBmp);
                hr = m_pPreview.SetSampleCallback(dwSinkStreamIndex, m_PreviewCallback);
                if (hr != HRESULTS.S_OK)
                {
                    goto done;
                }


                action(m_PreviewBmp);
            }


            hr = m_pEngine.StartPreview();
        //if (!m_fPowerRequestSet && m_hpwrRequest != INVALID_HANDLE_VALUE)
        //{
        //    // NOTE:  By calling this, on SOC systems (AOAC enabled), we're asking the system to not go
        //    // into sleep/connected standby while we're streaming.  However, since we don't want to block
        //    // the device from ever entering connected standby/sleep, we're going to latch ourselves to
        //    // the monitor on/off notification (RegisterPowerSettingNotification(GUID_MONITOR_POWER_ON)).
        //    // On SOC systems, this notification will fire when the user decides to put the device in
        //    // connected standby mode--we can trap this, turn off our media streams and clear this
        //    // power set request to allow the device to go into the lower power state.
        //    m_fPowerRequestSet = (TRUE == PowerSetRequest(m_hpwrRequest, PowerRequestExecutionRequired));
        //}
        done:
            Marshal.ReleaseComObject(pSink);
            Marshal.ReleaseComObject(pMediaType);
            Marshal.ReleaseComObject(pMediaType2);
            Marshal.ReleaseComObject(pSource);


            return await m_StartPreviewTask.Task;
        }

        


        TaskCompletionSource<HRESULTS> m_StopPreviewTask;
        public Task<HRESULTS> StopPreview()
        {
            HRESULT hr = HRESULTS.S_OK;

            if (m_pEngine == null)
            {
                return Task.FromResult(HRESULTS.MF_E_NOT_INITIALIZED);
            }

            this.m_StopPreviewTask = new TaskCompletionSource<HRESULTS>(hr);
            if (!m_bPreviewing)
            {
                return Task.FromResult(HRESULTS.S_OK);
            }
            hr = m_pEngine.StopPreview();
            if (hr.IsError)
            {
                goto done;
            }
        //WaitForResult();

        //if (m_fPowerRequestSet && m_hpwrRequest != INVALID_HANDLE_VALUE)
        //{
        //    PowerClearRequest(m_hpwrRequest, PowerRequestExecutionRequired);
        //    m_fPowerRequestSet = false;
        //}
        done:
            return this.m_StopPreviewTask.Task;
        }

        void Mirror(IMFCaptureSource source, uint streamindex)
        {
            //IMFVideoProcessorControl
            //source.AddEffect()
            IMFMediaType pMediaType;
            source.GetCurrentDeviceMediaType(streamindex, out pMediaType);
            var vp = Activator.CreateInstance(Type.GetTypeFromCLSID(DirectN.MFConstants.CLSID_VideoProcessorMFT)) as IMFVideoProcessorControl;
            var hr = vp.SetMirror(_MF_VIDEO_PROCESSOR_MIRROR.MIRROR_HORIZONTAL);
            IMFTransform mft = vp as IMFTransform;
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


            hr = source.AddEffect(streamindex, vp);
            //object o;
            //pFactory.CreateInstance(DirectN.MFConstants.CLSID_MFCaptureEngine, typeof(IMFCaptureEngine).GUID, out o);
            //m_pEngine = o as IMFVideoProcessorControl;

        }

        WriteableBitmap m_PreviewBmp;

        TaskCompletionSource<HRESULT> m_Takephoto;
        public Task<HRESULT> TakePhoto(string pszFileName, Func<(int width, int height)> size)
        {
            this.m_Takephoto = new TaskCompletionSource<HRESULT>();
            IMFCaptureSink pSink = null;
            IMFCapturePhotoSink pPhoto = null;
            IMFCaptureSource pSource = null;
            IMFMediaType pMediaType = null;
            IMFMediaType pMediaType2 = null;
            bool bHasPhotoStream = true;

            // Get a pointer to the photo sink.
            HRESULT hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_PHOTO, out pSink);
            if (hr.IsError)
            {
                goto done;
            }
            pPhoto = pSink as IMFCapturePhotoSink;

            hr = m_pEngine.GetSource(out pSource);
            if (hr.IsError)
            {
                goto done;
            }

            if (size != null)
            {
                pSource.SetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_PHOTO, null);
            }

            hr = pSource.GetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_PHOTO, out pMediaType);
            if (hr.IsError)
            {
                goto done;
            }



            //Configure the photo format
            hr = CreatePhotoMediaType(pMediaType, out pMediaType2);
            if (hr.IsError)
            {
                goto done;
            }

            hr = pPhoto.RemoveAllStreams();
            if (hr.IsError)
            {
                goto done;
            }

            //DWORD dwSinkStreamIndex;
            IntPtr pp = Marshal.AllocHGlobal(4);
            // Try to connect the first still image stream to the photo sink
            if (bHasPhotoStream)
            {
                hr = pPhoto.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_PHOTO, pMediaType2, null, pp);
            }

            if (hr.IsError)
            {
                goto done;
            }

            hr = pPhoto.SetOutputFileName(pszFileName);
            if (hr.IsError)
            {
                goto done;
            }

            hr = m_pEngine.TakePhoto();
            if (hr.IsError)
            {
                goto done;
            }

            m_bPhotoPending = true;

        done:
            SafeRelease(pSink);
            SafeRelease(pPhoto);
            SafeRelease(pSource);
            SafeRelease(pMediaType);
            SafeRelease(pMediaType2);
            return this.m_Takephoto.Task;
        }

        HRESULT CreatePhotoMediaType(IMFMediaType pSrcMediaType, out IMFMediaType ppPhotoMediaType)
        {
            //*ppPhotoMediaType = NULL;
            ppPhotoMediaType = null;
            //const UINT32 uiFrameRateNumerator = 30;
            //const UINT32 uiFrameRateDenominator = 1;

            IMFMediaType pPhotoMediaType = null;

            HRESULT hr = MFFunctions.MFCreateMediaType(out pPhotoMediaType);
            if (hr.IsError)
            {
                goto done;
            }

            hr = pPhotoMediaType.SetGUID(MFConstants.MF_MT_MAJOR_TYPE, MFConstants.MFMediaType_Image);
            if (hr.IsError)
            {
                goto done;
            }


            hr = pPhotoMediaType.SetGUID(MFConstants.MF_MT_SUBTYPE, WICConstants.GUID_ContainerFormatJpeg);
            if (hr.IsError)
            {
                goto done;
            }

            hr = CopyAttribute(pSrcMediaType, pPhotoMediaType, MFConstants.MF_MT_FRAME_SIZE);
            if (hr.IsError)
            {
                goto done;
            }

            ppPhotoMediaType = pPhotoMediaType;
        //(*ppPhotoMediaType)->AddRef();

        done:
            //SafeRelease(&pPhotoMediaType);
            return hr;
        }

        HRESULT CloneVideoMediaType(IMFMediaType pSrcMediaType, Guid guidSubType, out IMFMediaType ppNewMediaType)
        {
            IMFMediaType pNewMediaType = null;
            ppNewMediaType = null;
            HRESULT hr = MFFunctions.MFCreateMediaType(out pNewMediaType);
            if (hr.IsError)
            {
                goto done;
            }

            hr = pNewMediaType.SetGUID(MFConstants.MF_MT_MAJOR_TYPE, MFConstants.MFMediaType_Video);
            if (hr.IsError)
            {
                goto done;
            }

            hr = pNewMediaType.SetGUID(MFConstants.MF_MT_SUBTYPE, guidSubType);
            if (hr.IsError)
            {
                goto done;
            }

            hr = CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_FRAME_SIZE);
            if (hr.IsError)
            {
                goto done;
            }

            hr = CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_FRAME_RATE);
            if (hr.IsError)
            {
                goto done;
            }

            hr = CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_PIXEL_ASPECT_RATIO);
            if (hr.IsError)
            {
                goto done;
            }

            hr = CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_INTERLACE_MODE);
            if (hr.IsError)
            {
                goto done;
            }

            ppNewMediaType = pNewMediaType;
        //(ppNewMediaType).AddRef();

        done:
            //SafeRelease(&pNewMediaType);
            return hr;
        }

        HRESULT CopyAttribute(IMFAttributes pSrc, IMFAttributes pDest, Guid key)
        {
            PropVariant var = new PropVariant();
            //PropVariantInit( &var );
            HRESULT hr = pSrc.GetItem(key, var);
            if (hr == HRESULTS.S_OK)
            {
                hr = pDest.SetItem(key, var);
                //PropVariantClear(&var);
            }
            return hr;
        }

        public HRESULT OnEvent(IMFMediaEvent pEvent)
        {
            // We're about to fall asleep, that means we've just asked the CE to stop the preview
            // and record.  We need to handle it here since our message pump may be gone.
            Guid guidType;
            HRESULT hrStatus;
            HRESULT hr = pEvent.GetStatus(out hrStatus);
            if (hr != HRESULTS.S_OK)
            {
                hrStatus = hr;
            }

            hr = pEvent.GetExtendedType(out guidType);
            if (hr == HRESULTS.S_OK)
            {
                if (guidType == MFConstants.MF_CAPTURE_ENGINE_INITIALIZED)
                {
                    m_TaskCompelete.SetResult(HRESULTS.S_OK);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_PREVIEW_STARTED)
                {
                    m_StartPreviewTask.SetResult(HRESULTS.S_OK);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_PREVIEW_STOPPED)
                {
                    m_StopPreviewTask?.SetResult(HRESULTS.S_OK);
                    //m_pManager->OnPreviewStopped(hrStatus);
                    //SetEvent(m_pManager->m_hEvent);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_RECORD_STARTED)
                {
                    m_StartRecordTask.SetResult(HRESULTS.S_OK);
                    //m_pManager->OnRecordStopped(hrStatus);
                    //SetEvent(m_pManager->m_hEvent);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_RECORD_STOPPED)
                {
                    m_WaitStopRecord.SetResult(HRESULTS.S_OK);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_PHOTO_TAKEN)
                {
                    this.m_Takephoto.SetResult(HRESULTS.S_OK);
                }
                else if (guidType == MFConstants.MF_CAPTURE_ENGINE_EFFECT_ADDED)
                {

                }
                else if(guidType == MFConstants.MF_CAPTURE_SOURCE_CURRENT_DEVICE_MEDIA_TYPE_SET)
                {
                    this.m_SetMediaTypeTask.SetResult(HRESULTS.S_OK);
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

        TaskCompletionSource<HRESULT> m_WaitStopRecord;
        public Task<HRESULT> StopRecord()
        {
            m_WaitStopRecord = new TaskCompletionSource<HRESULT>();
            HRESULT hr = HRESULTS.S_OK;
            m_bRecording = false;
            if (m_bRecording)
            {
                hr = m_pEngine.StopRecord(true, false);
            }

            return m_WaitStopRecord.Task;
        }

        TaskCompletionSource<HRESULTS> m_StartRecordTask;
        public Task<HRESULTS> StartRecord(string pszDestinationFile)
        {
            if (m_pEngine == null)
            {
                return Task.FromResult(HRESULTS.MF_E_NOT_INITIALIZED);
            }

            if (m_bRecording == true)
            {
                return Task.FromResult(HRESULTS.MF_E_INVALIDREQUEST);
            }
            m_StartRecordTask = new TaskCompletionSource<HRESULTS>();
            System.IO.FileInfo fileinfo = new System.IO.FileInfo(pszDestinationFile);
            var pszExt = fileinfo.Extension;

            Guid guidVideoEncoding = Guid.Empty;
            Guid guidAudioEncoding = Guid.Empty;
            switch (pszExt)
            {
                case ".mp4":
                    {
                        guidVideoEncoding = MFConstants.MFVideoFormat_H264;
                        guidAudioEncoding = MFConstants.MFAudioFormat_AAC;
                    }
                    break;
                case ".wmv":
                    {
                        guidVideoEncoding = MFConstants.MFVideoFormat_H264;
                        guidAudioEncoding = MFConstants.MFAudioFormat_AAC;
                    }
                    break;
                case ".wma":
                    {
                        guidVideoEncoding = Guid.Empty;
                        guidAudioEncoding = MFConstants.MFAudioFormat_WMAudioV9;
                    }
                    break;
                default:
                    {
                        return Task.FromResult(HRESULTS.MF_E_INVALIDMEDIATYPE);
                    }
            }
            //if (wcscmp(pszExt, L".mp4") == 0)
            //{
            //    guidVideoEncoding = MFVideoFormat_H264;
            //    guidAudioEncoding = MFAudioFormat_AAC;
            //}
            //else if (wcscmp(pszExt, L".wmv") == 0)
            //{
            //    guidVideoEncoding = MFVideoFormat_WMV3;
            //    guidAudioEncoding = MFAudioFormat_WMAudioV9;
            //}
            //else if (wcscmp(pszExt, L".wma") == 0)
            //{
            //    guidVideoEncoding = GUID_NULL;
            //    guidAudioEncoding = MFAudioFormat_WMAudioV9;
            //}
            //else
            //{
            //    return HRESULTS.MF_E_INVALIDMEDIATYPE;
            //}

            IMFCaptureSink pSink = null;
            IMFCaptureRecordSink pRecord = null;
            IMFCaptureSource pSource = null;

            HRESULT hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_RECORD, out pSink);
            if (hr.IsError)
            {
                goto done;
            }
            pRecord = pSink as IMFCaptureRecordSink;
            //hr = pSink->QueryInterface(IID_PPV_ARGS(&pRecord));
            //if (FAILED(hr))
            //{
            //    goto done;
            //}

            hr = m_pEngine.GetSource(out pSource);
            if (hr.IsError)
            {
                goto done;
            }

            // Clear any existing streams from previous recordings.
            hr = pRecord.RemoveAllStreams();
            if (hr.IsError)
            {
                goto done;
            }

            hr = pRecord.SetOutputFileName(pszDestinationFile);
            if (hr.IsError)
            {
                goto done;
            }

            // Configure the video and audio streams.
            if (guidVideoEncoding != Guid.Empty)
            {
                hr = ConfigureVideoEncoding(pSource, pRecord, ref guidVideoEncoding);
                if (hr.IsError)
                {
                    goto done;
                }
            }

            if (guidAudioEncoding != Guid.Empty)
            {
                hr = ConfigureAudioEncoding(pSource, pRecord, guidAudioEncoding);
                if (hr.IsError)
                {
                    goto done;
                }
            }

            hr = m_pEngine.StartRecord();
            if (hr.IsError)
            {
                goto done;
            }

            m_bRecording = true;

        done:
            //SafeRelease(&pSink);
            //SafeRelease(&pSource);
            //SafeRelease(&pRecord);

            return m_StartRecordTask.Task;
        }

        // Helper function to get the frame size from a video media type.
        HRESULT GetFrameSize(IMFMediaType pType, out uint pWidth, out uint pHeight)
        {
            return MFFunctions1.MFGetAttributeSize(pType, MFConstants.MF_MT_FRAME_SIZE, out pWidth, out pHeight);
        }

        // Helper function to get the frame rate from a video media type.
        HRESULT GetFrameRate(IMFMediaType pType, out uint pNumerator, out uint pDenominator)
        {
            return MFFunctions1.MFGetAttributeRatio(pType, MFConstants.MF_MT_FRAME_RATE, out pNumerator, out pDenominator);
        }

        HRESULT GetEncodingBitrate(IMFMediaType pMediaType, out uint uiEncodingBitrate)
        {
            uiEncodingBitrate = 0;
            uint uiWidth;
            uint uiHeight;
            float uiBitrate;
            uint uiFrameRateNum;
            uint uiFrameRateDenom;

            HRESULT hr = GetFrameSize(pMediaType, out uiWidth, out uiHeight);
            if (hr.IsError)
            {
                goto done;
            }

            hr = GetFrameRate(pMediaType, out uiFrameRateNum, out uiFrameRateDenom);
            if (hr.IsError)
            {
                goto done;
            }

            uiBitrate = uiWidth / 3.0f * uiHeight * uiFrameRateNum / uiFrameRateDenom;

            uiEncodingBitrate = (uint)uiBitrate;

        done:

            return hr;
        }


        HRESULT ConfigureVideoEncoding(IMFCaptureSource pSource, IMFCaptureRecordSink pRecord, ref Guid guidEncodingType)
        {
            IMFMediaType pMediaType = null;
            IMFMediaType pMediaType2 = null;
            Guid guidSubType = Guid.Empty;

            // Configure the video format for the recording sink.
            HRESULT hr = pSource.GetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_RECORD, out pMediaType);
            if (hr.IsError)
            {
                goto done;
            }

            hr = CloneVideoMediaType(pMediaType, guidEncodingType, out pMediaType2);
            if (hr.IsError)
            {
                goto done;
            }


            hr = pMediaType.GetGUID(MFConstants.MF_MT_SUBTYPE, out guidSubType);
            if (hr.IsError)
            {
                goto done;
            }

            if (guidSubType == MFConstants.MFVideoFormat_H264_ES || guidSubType == MFConstants.MFVideoFormat_H264)
            {
                //When the webcam supports H264_ES or H264, we just bypass the stream. The output from Capture engine shall be the same as the native type supported by the webcam
                hr = pMediaType2.SetGUID(MFConstants.MF_MT_SUBTYPE, MFConstants.MFVideoFormat_H264);
            }
            else
            {
                uint uiEncodingBitrate;
                hr = GetEncodingBitrate(pMediaType2, out uiEncodingBitrate);
                if (hr.IsError)
                {
                    goto done;
                }
                //uiEncodingBitrate = uiEncodingBitrate / 2;
                hr = pMediaType2.SetUINT32(MFConstants.MF_MT_AVG_BITRATE, uiEncodingBitrate);
            }

            if (hr.IsError)
            {
                goto done;
            }

            // Connect the video stream to the recording sink.
            //DWORD dwSinkStreamIndex;
            IntPtr dwSinkStreamIndex = Marshal.AllocHGlobal(4);
            hr = pRecord.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_RECORD, pMediaType2, null, dwSinkStreamIndex);

        done:
            //SafeRelease(&pMediaType);
            //SafeRelease(&pMediaType2);
            return hr;
        }

        HRESULT ConfigureAudioEncoding(IMFCaptureSource pSource, IMFCaptureRecordSink pRecord, Guid guidEncodingType)
        {
            IMFCollection pAvailableTypes = null;
            IMFMediaType pMediaType = null;
            IMFAttributes pAttributes = null;

            // Configure the audio format for the recording sink.

            HRESULT hr = MFFunctions.MFCreateAttributes(out pAttributes, 1);
            if (hr.IsError)
            {
                goto done;
            }

            // Enumerate low latency media types
            hr = pAttributes.SetUINT32(MFConstants.MF_LOW_LATENCY, 1);
            if (hr.IsError)
            {
                goto done;
            }


            //// Get a list of encoded output formats that are supported by the encoder.
            //hr = MFFunctions.MFTranscodeGetAudioOutputAvailableTypes(guidEncodingType, MFT_ENUM_FLAG_ALL | MFT_ENUM_FLAG_SORTANDFILTER,
            //    pAttributes, out pAvailableTypes);
            //if (hr.IsError)
            //{
            //    goto done;
            //}
            //DirectN._MFT_ENUM_FLAG

            //MFT_ENUM_FLAG_SORTANDFILTER
            uint flag = (uint)(DirectN._MFT_ENUM_FLAG.MFT_ENUM_FLAG_ALL | DirectN._MFT_ENUM_FLAG.MFT_ENUM_FLAG_SORTANDFILTER);

            IMFCollection colptr;
            hr = MFFunctions1.MFTranscodeGetAudioOutputAvailableTypes(guidEncodingType, flag, pAttributes, out colptr);


            hr = colptr.GetElement(0, out var punk);
            if (hr.IsSuccess)
            {
                pMediaType = punk as IMFMediaType;
            }

            // Connect the audio stream to the recording sink.
            using (var cm = new ComMemory(Marshal.SizeOf<uint>()))
            {
                hr = pRecord.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_AUDIO, pMediaType, null, cm.Pointer);
                if (hr == HRESULTS.MF_E_INVALIDSTREAMNUMBER)
                {
                    //If an audio device is not present, allow video only recording
                    hr = HRESULTS.S_OK;
                }
            }
        done:
            //SafeRelease(&pAvailableTypes);
            //SafeRelease(&pMediaType);
            //SafeRelease(&pAttributes);
            return hr;
        }

        void SafeRelease<T>(T obj) where T : class
        {
            if (obj != null)
            {
                Marshal.ReleaseComObject(obj);
            }
        }

        public void Dispose()
        {
            this.DestroyCaptureEngine();
        }
    }

    public static class IMFCaptureSourceEx
    {
        public static List<IMFMediaType> GetAllMediaType(this IMFCaptureSource src, uint streamindex)
        {
            List<IMFMediaType> medias = new List<IMFMediaType>();
            uint index = 0;
            while (true)
            {
                IMFMediaType mediaType;
                var hr = src.GetAvailableDeviceMediaType(streamindex, index, out mediaType);
                if (hr == HRESULTS.MF_E_NO_MORE_TYPES)
                {
                    break;
                }
                medias.Add(mediaType);
                index++;
                //uint w;
                //uint h;
                //mediaType.GetGUID(MFConstants.MF_MT_SUBTYPE, out var subtype);
                //if(subtype == MFConstants.MFImageFormat_JPEG)
                //{

                //}
                //MFFunctions1.MFGetAttributeSize(mediaType, MFConstants.MF_MT_FRAME_SIZE, out w, out h);
            }
            return medias;
        }

        public static IEnumerable<(Guid format, uint width, uint height, double fps, IMFMediaType mediatype)> GetVideoData(this IEnumerable<IMFMediaType> src)
        {
            foreach (var oo in src)
            {
                uint w;
                uint h;
                oo.GetGUID(MFConstants.MF_MT_SUBTYPE, out var subtype);
                if (subtype == MFConstants.MFImageFormat_JPEG)
                {

                }
                MFFunctions1.MFGetAttributeSize(oo, MFConstants.MF_MT_FRAME_SIZE, out w, out h);

                MFFunctions1.MFGetAttributeRatio(oo, MFConstants.MF_MT_FRAME_RATE, out var numerator, out var denominator);
                var fps = (double)numerator / (double)denominator;
                yield return (subtype, w, h, fps, oo);
            }
        }

        public static Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, List<IMFMediaType>> GetAllMediaType(this IMFCaptureSource src)
        {
            Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, List<IMFMediaType>> result = new Dictionary<MF_CAPTURE_ENGINE_STREAM_CATEGORY, List<IMFMediaType>>();
            var hr1 = src.GetDeviceStreamCount(out var count);
            System.Diagnostics.Trace.WriteLine($"GetDeviceStreamCount :hr1:{hr1} count:{count}");
            for (uint i = 0; i < count; i++)
            {
                src.GetDeviceStreamCategory(i, out var caegory);
                var medias = src.GetAllMediaType(i);
                result[caegory] = medias;
            }

            return result;
        }

        public static void Mirror(this IMFCaptureSource src)
        {

        }
    }

    public static class MFFunctions1
    {
        [DllImport("mf", ExactSpelling = true)]
        public static extern HRESULT MFTranscodeGetAudioOutputAvailableTypes([MarshalAs(UnmanagedType.LPStruct)] Guid guidSubType, uint dwMFTFlags, IMFAttributes pCodecConfig, out IMFCollection ppAvailableTypes);

        public static HRESULT MFGetAttributeRatio(
            IMFAttributes pAttributes,
            Guid guidKey,
            out uint punNumerator,
            out uint punDenominator
            )
        {
            return MFGetAttribute2UINT32asUINT64(pAttributes, guidKey, out punNumerator, out punDenominator);
        }

        public static HRESULT MFGetAttributeSize(
            IMFAttributes pAttributes,
            Guid guidKey,
            out uint punWidth,
            out uint punHeight
            )
        {
            return MFGetAttribute2UINT32asUINT64(pAttributes, guidKey, out punWidth, out punHeight);
        }

        public static HRESULT MFGetAttribute2UINT32asUINT64(
            IMFAttributes pAttributes,
            Guid guidKey,
            out uint punHigh32,
            out uint punLow32
            )
        {
            ulong unPacked;
            HRESULT hr;

            hr = pAttributes.GetUINT64(guidKey, out unPacked);
            if (hr.IsError)
            {
                punHigh32 = punLow32 = 0;
                return hr;
            }
            Unpack2UINT32AsUINT64(unPacked, out punHigh32, out punLow32);

            return hr;
        }

        public static void Unpack2UINT32AsUINT64(
            ulong unPacked,
            out uint punHigh,
            out uint punLow
            )
        {
            ulong ul = (ulong)unPacked;
            punHigh = (uint)(ul >> 32);
            punLow = (uint)(ul & 0xffffffff);
        }
    }

    public class MFCaptureEngineOnSampleCallback : IMFCaptureEngineOnSampleCallback
    {
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
        WriteableBitmap m_Bmp;
        public MFCaptureEngineOnSampleCallback(WriteableBitmap data)
        {
            this.m_Bmp = data;
        }
        int samplecount = 0;
        DateTime time = DateTime.Now;
        byte[] m_Buffer;
        object m_Lock = new object();
        public HRESULT OnSample(IMFSample pSample)
        {
            if (System.Threading.Monitor.TryEnter(this.m_Lock) == true)
            {
                samplecount++;
                if(samplecount > 100)
                {
                    var ts = DateTime.Now - time;
                    var fps = samplecount / ts.TotalSeconds;
                    samplecount = 0;
                    time = DateTime.Now;
                    System.Diagnostics.Trace.WriteLine($"fps:{fps}");
                }
                pSample.GetBufferByIndex(0, out var buf);
                var ptr = buf.Lock(out var max, out var cur);
                //m_Buffer = new byte[cur];
                //Marshal.Copy(ptr, m_Buffer, 0, m_Buffer.Length);
                m_Bmp.Dispatcher.Invoke(() =>
                {
                    m_Bmp.Lock();
                    CopyMemory(m_Bmp.BackBuffer, ptr, cur);
                    //Marshal.Copy(this.m_Buffer, 0, m_Bmp.BackBuffer, this.m_Buffer.Length);
                    m_Bmp.AddDirtyRect(new System.Windows.Int32Rect(0, 0, m_Bmp.PixelWidth, m_Bmp.PixelHeight));
                    m_Bmp.Unlock();
                }, System.Windows.Threading.DispatcherPriority.Background);

                buf.Unlock();
                Marshal.ReleaseComObject(buf);
                Marshal.ReleaseComObject(pSample);
                System.Threading.Monitor.Exit(this.m_Lock);
            }
            else
            {
                Marshal.ReleaseComObject(pSample);
            }

            return HRESULTS.S_OK;
        }
    }
}