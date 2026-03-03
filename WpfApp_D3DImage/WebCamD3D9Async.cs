using DirectN;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace WpfApp_D3DImage
{
    //public class WebCamD3D9Async : WebCamD3D9, WpfApp_D3DImage.WebCamD3D9Async.IMFSourceReaderCallback
    //{
    //    IMFSourceReader m_pSourceReader;

    //    public WebCamD3D9Async(D3DImage d3dimage, WindowInteropHelper helper) 
    //        : base(d3dimage, helper)
    //    {
    //    }

    //    public void Start()
    //    {
    //        var hr = CreateVideoDeviceSource(out var pSource);
    //        //if (SUCCEEDED(hr))

    //        IMFAttributes pAttributes = null;
    //        hr = DirectN.MFFunctions.MFCreateAttributes(out pAttributes, 6);
    //        pAttributes.SetUINT32(DirectN.MFConstants.MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS, 1);
    //        pAttributes.SetUINT32(DirectN.MFConstants.MF_SOURCE_READER_DISABLE_DXVA, 0);
    //        pAttributes.SetUINT32(DirectN.MFConstants.MF_SOURCE_READER_ENABLE_VIDEO_PROCESSING, 0);
    //        pAttributes.SetUINT32(DirectN.MFConstants.MF_SOURCE_READER_ENABLE_ADVANCED_VIDEO_PROCESSING, 1);
    //        pAttributes.SetUnknown(DirectN.MFConstants.MF_SOURCE_READER_ASYNC_CALLBACK, this);
    //        pAttributes.SetUnknown(DirectN.MFConstants.MF_SOURCE_READER_D3D_MANAGER, pDeviceManager);


    //        hr = MFCreateSourceReaderFromMediaSource(pSource, pAttributes, out m_pSourceReader);

    //        hr = m_pSourceReader.GetNativeMediaType(0xFFFFFFFC, 0, out var pMediaType);
    //        //GUID subtype;
    //        hr = pMediaType.GetGUID(DirectN.MFConstants.MF_MT_SUBTYPE, out var subtype);
    //        hr = pMediaType.SetGUID(DirectN.MFConstants.MF_MT_SUBTYPE, DirectN.MFConstants.MFVideoFormat_ARGB32);
    //        hr = DirectN.MFFunctions.MFCreateMediaType(out var tt);
    //        tt.SetGUID(DirectN.MFConstants.MF_MT_MAJOR_TYPE, DirectN.MFConstants.MFMediaType_Video);
    //        tt.SetGUID(DirectN.MFConstants.MF_MT_SUBTYPE, DirectN.MFConstants.MFVideoFormat_RGB32);
    //        hr = m_pSourceReader.SetCurrentMediaType(0xFFFFFFFC, IntPtr.Zero, tt);

    //        hr = m_pSourceReader.ReadSample(0xFFFFFFFC, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

    //    }
    //    object m_Lock = new object();
    //    public HRESULT OnReadSample(HRESULT hrStatus, uint dwStreamIndex, uint dwStreamFlags, long llTimestamp, IntPtr pSampleptr)
    //    {
    //        if (System.Threading.Monitor.TryEnter(m_Lock))
    //        {
                
    //            try
    //            {
    //                this.m_D3DImage.Dispatcher.Invoke(() =>
    //                {
    //                    if (pSampleptr != IntPtr.Zero)
    //                    {
    //                        Marshal.AddRef(pSampleptr);
    //                        IDirect3DSurface9 surface = null;
    //                        var pSample = Marshal.GetObjectForIUnknown(pSampleptr) as IMFSample;
    //                        var hr = pSample.GetBufferByIndex(0, out var pBuffer);
    //                        hr = DirectN.MFFunctions.MFGetService(pBuffer, DirectN.MFConstants.MR_BUFFER_SERVICE, new Guid("0cfbaf3a-9ff6-429a-99b3-a2796af8b89b"), out var dd);
    //                        surface = (IDirect3DSurface9)dd;

    //                        if (this.pRenderTexture == null)
    //                        {
    //                            DirectN._D3DSURFACE_DESC desc = new DirectN._D3DSURFACE_DESC();
    //                            surface.GetDesc(ref desc);
    //                            CreateWPFCompatibleSurface(desc);
    //                            var ptr = Marshal.GetIUnknownForObject(pRenderSurface);
    //                            this.m_D3DImage.Lock();
    //                            this.m_D3DImage.SetBackBuffer(System.Windows.Interop.D3DResourceType.IDirect3DSurface9, ptr);
    //                            this.m_D3DImage.Unlock();
    //                        }


    //                        pDeviceEx.StretchRect(
    //            surface,
    //            new tagRECT(0, 0, 1280, 720), pRenderSurface, new tagRECT(0, 0, 1280, 720), DirectN._D3DTEXTUREFILTERTYPE.D3DTEXF_NONE);
    //                        this.m_D3DImage.Lock();
    //                        this.m_D3DImage.AddDirtyRect(new Int32Rect(0, 0, 1280, 720));
    //                        this.m_D3DImage.Unlock();
    //                        Marshal.ReleaseComObject(surface);
    //                        Marshal.ReleaseComObject(pBuffer);
    //                        Marshal.ReleaseComObject(pSample);
    //                        Marshal.Release(pSampleptr);

    //                    }
    //                    var hr1 = m_pSourceReader.ReadSample(0xFFFFFFFC, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
    //                });
    //            }
    //            finally
    //            {
    //                System.Threading.Monitor.Exit(m_Lock);
    //            }
    //        }

    //        return HRESULTS.S_OK;
    //    }

    //    public HRESULT OnFlush(uint dwStreamIndex)
    //    {
    //        return HRESULTS.S_OK;
    //    }

    //    public HRESULT OnEvent(uint dwStreamIndex, IMFMediaEvent pEvent)
    //    {
    //        return HRESULTS.S_OK;
    //    }

    //    [DllImport("mfreadwrite", ExactSpelling = true)]
    //    static extern HRESULT MFCreateSourceReaderFromMediaSource(IMFMediaSource pMediaSource, IMFAttributes pAttributes, out IMFSourceReader ppSourceReader);


    //    [ComImport]
    //    [Guid("70ae66f2-c809-4e4f-8915-bdcb406b7993")]
    //    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //    public interface IMFSourceReader
    //    {
    //        [PreserveSig]
    //        HRESULT GetStreamSelection(uint dwStreamIndex, out bool pfSelected);

    //        [PreserveSig]
    //        HRESULT SetStreamSelection(uint dwStreamIndex, bool fSelected);

    //        [PreserveSig]
    //        HRESULT GetNativeMediaType(uint dwStreamIndex, uint dwMediaTypeIndex, out IMFMediaType ppMediaType);

    //        [PreserveSig]
    //        HRESULT GetCurrentMediaType(uint dwStreamIndex, out IMFMediaType ppMediaType);

    //        [PreserveSig]
    //        HRESULT SetCurrentMediaType(uint dwStreamIndex, IntPtr pdwReserved, IMFMediaType pMediaType);

    //        [PreserveSig]
    //        HRESULT SetCurrentPosition([MarshalAs(UnmanagedType.LPStruct)] Guid guidTimeFormat, [In][Out] PROPVARIANT varPosition);

    //        [PreserveSig]
    //        HRESULT ReadSample(uint dwStreamIndex, uint dwControlFlags, IntPtr pdwActualStreamIndex, IntPtr pdwStreamFlags, IntPtr pllTimestamp, IntPtr ppSample);

    //        [PreserveSig]
    //        HRESULT Flush(uint dwStreamIndex);

    //        [PreserveSig]
    //        HRESULT GetServiceForStream(uint dwStreamIndex, [MarshalAs(UnmanagedType.LPStruct)] Guid guidService, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppvObject);

    //        [PreserveSig]
    //        HRESULT GetPresentationAttribute(uint dwStreamIndex, [MarshalAs(UnmanagedType.LPStruct)] Guid guidAttribute, [In][Out] PROPVARIANT pvarAttribute);
    //    }

    //    [ComImport]
    //    [Guid("deec8d99-fa1d-4d82-84c2-2c8969944867")]
    //    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //    public interface IMFSourceReaderCallback
    //    {
    //        [PreserveSig]
    //        HRESULT OnReadSample(HRESULT hrStatus, uint dwStreamIndex, uint dwStreamFlags, long llTimestamp, IntPtr pSampleptr);

    //        [PreserveSig]
    //        HRESULT OnFlush(uint dwStreamIndex);

    //        [PreserveSig]
    //        HRESULT OnEvent(uint dwStreamIndex, IMFMediaEvent pEvent);
    //    }

    //}
}
