﻿using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

//https://learn.microsoft.com/zh-tw/windows/win32/api/mfidl/nn-mfidl-imfcameraconfigurationmanager

namespace QSoft.MediaCapture
{
    public enum CameraRotates
    {
        Rotate0 = 0,
        Rotate90 = 90,
        Rotate180 = 180,
        Rotate270 = 270,
        Rotate90Colockwise = -90,
        Rotate270Colockwise = -270,
    }
    public class WebCam_MF_Setting
    {
        //use shared property may be caused flashlight, torch not work
        public bool Shared { set; get; }
        public bool IsMirror { set; get; }
        public CameraRotates Rotate { set; get; }
        public bool UseD3D { set; get; }
    }

    //public class KK:IMFCameraOcclusionStateReportCallback
    //{
    //    public HRESULT OnOcclusionStateReport(IMFCameraOcclusionStateReport occlusionStateReport)
    //    {
    //        var hr = occlusionStateReport.GetOcclusionState(out var state);
    //        return hr;
    //    }
    //}
    public sealed partial class WebCam_MF : IDisposable
    {
        WebCam_MF_Setting m_Setting = new WebCam_MF_Setting();
        public WebCam_MF_Setting Setting => m_Setting;
        public string FriendName { private set; get; } = "";
        public string SymbolLinkName { private set; get; } = "";
        public IComObject<IMFActivate>? CaptureObj { private set; get; }


        void DestroyCaptureEngine()
        {
            SafeRelease(m_pEngine);

            g_pDXGIMan?.ResetDevice(g_pDX11Device, g_ResetToken);
            SafeRelease(g_pDX11Device);
            SafeRelease(g_pDXGIMan);

            //m_WhiteBalanceControl = null;
        }

        IMFCaptureEngine? m_pEngine;
        //IMFCapturePreviewSink? m_pPreview;
        //Dictionary<PreviewMediaType, List<IMFMediaType>> m_PreviewTypes = new();
        TaskCompletionSource<HRESULT>? m_TaskInitialize;
        async public Task<HRESULT?> InitCaptureEngine(WebCam_MF_Setting setting)
        {
            //DirectN.MFFunctions.MFStartup();
            //var hr1 = DirectN.Functions.MFCreateCameraOcclusionStateMonitor(this.SymbolLinkName, new KK(), out var monitor);
            //var supportstates = monitor.GetSupportedStates();

            //var eeeoo = (MFCameraOcclusionState)supportstates;
            //monitor.Start();

            //var hr2 = DirectN.Functions.MFCreateSensorGroup(this.SymbolLinkName, out var sensorGroup);
            //sensorGroup.GetSensorDeviceCount(out var deviceCount);
            m_Setting.IsMirror = setting.IsMirror;
            m_Setting.Rotate = setting.Rotate;
            m_Setting.Shared = setting.Shared;
            m_Setting.UseD3D = setting.UseD3D;
            m_TaskInitialize = new TaskCompletionSource<HRESULT>();
            HRESULT? hr = HRESULTS.S_OK;
            IMFAttributes? pAttributes = null;
            IMFCaptureEngineClassFactory? pFactory = null;

            DestroyCaptureEngine();
            try
            {

                if (hr != HRESULTS.S_OK) return hr;
                hr = MFFunctions.MFCreateAttributes(out pAttributes, 1);
                if (hr != HRESULTS.S_OK) return hr;
                if (m_Setting.UseD3D)
                {
                    hr = CreateD3DManager();
                    if (hr != HRESULTS.S_OK) return hr;
                    hr = pAttributes.SetUnknown(MFConstants.MF_CAPTURE_ENGINE_D3D_MANAGER, g_pDXGIMan);
                }
                if (m_Setting.Shared)
                {
                    pAttributes.SetUINT32(MFConstants.MF_DEVSOURCE_ATTRIBUTE_FRAMESERVER_SHARE_MODE, 1);
                }
                pAttributes.SetUINT32(MFConstants.MF_LOW_LATENCY, 1);
                if (hr != HRESULTS.S_OK) return hr;
                var tty = Type.GetTypeFromCLSID(DirectN.MFConstants.CLSID_MFCaptureEngineClassFactory, true);
                if (tty == null) return hr;
                pFactory = Activator.CreateInstance(tty) as IMFCaptureEngineClassFactory;
                object? o = null;
                hr = pFactory?.CreateInstance(DirectN.MFConstants.CLSID_MFCaptureEngine, typeof(IMFCaptureEngine).GUID, out o);
                if (hr != HRESULTS.S_OK) return hr;
                m_pEngine = o as IMFCaptureEngine;

                var sw = System.Diagnostics.Stopwatch.StartNew();
                hr = m_pEngine?.Initialize(this, pAttributes, null, this.CaptureObj?.Object);
                if (hr != HRESULTS.S_OK) return hr;
                
                hr = await m_TaskInitialize.Task;
                
                //MF_CAPTURE_ENGINE_MEDIASOURCE_CONFIG
                sw.Stop();
                //this.SupporCategory();
                ExtendedCameraControl.TetsALL(m_pEngine);
                InitFlashLight();
                InitTorch();
                //InitBackgroundSegmentation();
                //var bv = this.BackgroundSegmentation;
                //this.BackgroundSegmentation?.SetState(BackgroundSegmentation.BackgroundSegmentationState.Blur);

                //KsMedia.KsControl ks = new KsMedia.KsControl(m_pEngine);
                KsMedia.ExtensionUnit xu = new KsMedia.ExtensionUnit(m_pEngine);
                //this.SetPowerLine();
            }
            finally
            {
                m_TaskInitialize = null;
                SafeRelease(pAttributes);
                SafeRelease(pFactory);
            }
            return hr;
        }

        public static void SafeRelease<T>(T? obj) where T : class
        {
            if (obj != null && Marshal.IsComObject(obj))
            {
                Marshal.ReleaseComObject(obj);
            }
        }






        internal static HRESULT CloneVideoMediaType(IMFMediaType pSrcMediaType, Guid guidSubType, out IMFMediaType? ppNewMediaType)
        {
            ppNewMediaType = null;
            var hr = MFFunctions.MFCreateMediaType(out var pNewMediaType);
            if (hr.IsError) return hr;
            hr = pNewMediaType.SetGUID(MFConstants.MF_MT_MAJOR_TYPE, MFConstants.MFMediaType_Video);
            if (hr.IsError) return hr;
            hr = pSrcMediaType.GetGUID(MFConstants.MF_MT_SUBTYPE, out var subtype);
            if(subtype == MFConstants.MFVideoFormat_H264_ES)
            {
                hr = pNewMediaType.SetGUID(MFConstants.MF_MT_SUBTYPE, subtype);
                if (hr.IsError) return hr;
            }
            hr = pNewMediaType.SetGUID(MFConstants.MF_MT_SUBTYPE, guidSubType);
            if (hr.IsError) return hr;

            hr = WebCam_MF.CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_FRAME_SIZE);
            if (hr.IsError) return hr;

            hr = WebCam_MF.CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_FRAME_RATE);
            if (hr.IsError) return hr;

            hr = WebCam_MF.CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_PIXEL_ASPECT_RATIO);
            if (hr.IsError) return hr;

            hr = WebCam_MF.CopyAttribute(pSrcMediaType, pNewMediaType, MFConstants.MF_MT_INTERLACE_MODE);
            if (hr.IsError) return hr;

            ppNewMediaType = pNewMediaType;
            return hr;
        }

        static HRESULT CopyAttribute(IMFAttributes pSrc, IMFAttributes pDest, Guid key)
        {
            PROPVARIANT var = new();
            HRESULT hr = pSrc.GetItem(key, var);
            if (hr == HRESULTS.S_OK)
            {
                hr = pDest.SetItem(key, var);
            }
            return hr;
        }



        public void UpdateVideo(int width, int height)
        {
            if (m_pEngine == null) return;
            var hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_PREVIEW, out var pSink);
            if (hr != HRESULTS.S_OK) return;
            var previewsink = pSink as IMFCapturePreviewSink;
            IntPtr prc = Marshal.AllocHGlobal(Marshal.SizeOf<RECT>());
            RECT rc;
            rc.left = 0;
            rc.top = 0;
            rc.right = width;
            rc.bottom = height;

            Marshal.StructureToPtr(rc, prc, true);
            if (previewsink != null)
            {
                previewsink.UpdateVideo(IntPtr.Zero, prc, IntPtr.Zero);
            }
            SafeRelease(previewsink);
            Marshal.FreeHGlobal(prc);

        }

        public async void Dispose()
        {
            await this.StopPreview();
            await this.StopRecord();
            this.DestroyCaptureEngine();
            foreach(var oo in this.m_VideoList.SelectMany(x=>x.Value))
            {
                oo?.Dispose();
            }
            this.m_VideoList.Clear();
        }
    }

    struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    };

    public enum MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM : uint
    {
        FOR_VIDEO_PREVIEW = 0xfffffffa,
        FOR_VIDEO_RECORD = 0xfffffff9,
        FOR_PHOTO = 0xfffffff8,
        FOR_AUDIO = 0xfffffff7,
        FOR_METADATA = 0xfffffff6,
        MF_CAPTURE_ENGINE_MEDIASOURCE = 0xffffffff
    }
}

