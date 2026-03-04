using DirectN;
using System;
using System.Runtime.InteropServices;

namespace WpfApp_D3DImage
{
    /// <summary>
    /// Media Foundation DLL Import 和 COM Interface 定義
    /// 統一管理所有 DirectN 相關函數和 Media Foundation 介面
    /// </summary>
    public partial class WebCamD3D9
    {
        #region Media Foundation DLL Imports

        [ComImport]
        [Guid("7fee9e9a-4a89-47a6-899c-b6a53a70fb67")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMFActivate : IMFAttributes
        {
            [PreserveSig]
            new HRESULT GetItem([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][Out] PROPVARIANT pValue);

            [PreserveSig]
            new HRESULT GetItemType([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out _MF_ATTRIBUTE_TYPE pType);

            [PreserveSig]
            new HRESULT CompareItem([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][Out] PROPVARIANT Value, out bool pbResult);

            [PreserveSig]
            new HRESULT Compare(IMFAttributes pTheirs, _MF_ATTRIBUTES_MATCH_TYPE MatchType, out bool pbResult);

            [PreserveSig]
            new HRESULT GetUINT32([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out uint punValue);

            [PreserveSig]
            new HRESULT GetUINT64([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out ulong punValue);

            [PreserveSig]
            new HRESULT GetDouble([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out double pfValue);

            [PreserveSig]
            new HRESULT GetGUID([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out Guid pguidValue);

            [PreserveSig]
            new HRESULT GetStringLength([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out uint pcchLength);

            [PreserveSig]
            new HRESULT GetString([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPWStr)] string pwszValue, uint cchBufSize, IntPtr pcchLength);

            [PreserveSig]
            new HRESULT GetAllocatedString([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, IntPtr ppwszValue, out uint pcchLength);

            [PreserveSig]
            new HRESULT GetBlobSize([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out uint pcbBlobSize);

            [PreserveSig]
            new HRESULT GetBlob([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf, int cbBufSize, IntPtr pcbBlobSize);

            [PreserveSig]
            new HRESULT GetAllocatedBlob([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out IntPtr ppBuf, out uint pcbSize);

            [PreserveSig]
            new HRESULT GetUnknown([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppv);

            [PreserveSig]
            new HRESULT SetItem([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][Out] PROPVARIANT Value);

            [PreserveSig]
            new HRESULT DeleteItem([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey);

            [PreserveSig]
            new HRESULT DeleteAllItems();

            [PreserveSig]
            new HRESULT SetUINT32([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, uint unValue);

            [PreserveSig]
            new HRESULT SetUINT64([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, ulong unValue);

            [PreserveSig]
            new HRESULT SetDouble([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, double fValue);

            [PreserveSig]
            new HRESULT SetGUID([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPStruct)] Guid guidValue);

            [PreserveSig]
            new HRESULT SetString([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPWStr)] string wszValue);

            [PreserveSig]
            new HRESULT SetBlob([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf, int cbBufSize);

            [PreserveSig]
            new HRESULT SetUnknown([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.IUnknown)] object pUnknown);

            [PreserveSig]
            new HRESULT LockStore();

            [PreserveSig]
            new HRESULT UnlockStore();

            [PreserveSig]
            new HRESULT GetCount(out uint pcItems);

            [PreserveSig]
            new HRESULT GetItemByIndex(uint unIndex, out Guid pguidKey, [In][Out] PROPVARIANT pValue);

            [PreserveSig]
            new HRESULT CopyAllItems(IMFAttributes pDest);

            [PreserveSig]
            HRESULT ActivateObject([MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppv);

            [PreserveSig]
            HRESULT ShutdownObject();

            [PreserveSig]
            HRESULT DetachObject();
        }


        [ComImport]
        [Guid("c40a00f2-b93a-4d80-ae8c-5a1c634f58e4")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMFSample : IMFAttributes
        {
            [PreserveSig]
            new HRESULT GetItem([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][Out] PROPVARIANT pValue);

            [PreserveSig]
            new HRESULT GetItemType([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out _MF_ATTRIBUTE_TYPE pType);

            [PreserveSig]
            new HRESULT CompareItem([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][Out] PROPVARIANT Value, out bool pbResult);

            [PreserveSig]
            new HRESULT Compare(IMFAttributes pTheirs, _MF_ATTRIBUTES_MATCH_TYPE MatchType, out bool pbResult);

            [PreserveSig]
            new HRESULT GetUINT32([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out uint punValue);

            [PreserveSig]
            new HRESULT GetUINT64([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out ulong punValue);

            [PreserveSig]
            new HRESULT GetDouble([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out double pfValue);

            [PreserveSig]
            new HRESULT GetGUID([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out Guid pguidValue);

            [PreserveSig]
            new HRESULT GetStringLength([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out uint pcchLength);

            [PreserveSig]
            new HRESULT GetString([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPWStr)] string pwszValue, uint cchBufSize, IntPtr pcchLength);

            [PreserveSig]
            new HRESULT GetAllocatedString([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, IntPtr ppwszValue, out uint pcchLength);

            [PreserveSig]
            new HRESULT GetBlobSize([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out uint pcbBlobSize);

            [PreserveSig]
            new HRESULT GetBlob([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf, int cbBufSize, IntPtr pcbBlobSize);

            [PreserveSig]
            new HRESULT GetAllocatedBlob([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out IntPtr ppBuf, out uint pcbSize);

            [PreserveSig]
            new HRESULT GetUnknown([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppv);

            [PreserveSig]
            new HRESULT SetItem([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][Out] PROPVARIANT Value);

            [PreserveSig]
            new HRESULT DeleteItem([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey);

            [PreserveSig]
            new HRESULT DeleteAllItems();

            [PreserveSig]
            new HRESULT SetUINT32([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, uint unValue);

            [PreserveSig]
            new HRESULT SetUINT64([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, ulong unValue);

            [PreserveSig]
            new HRESULT SetDouble([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, double fValue);

            [PreserveSig]
            new HRESULT SetGUID([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPStruct)] Guid guidValue);

            [PreserveSig]
            new HRESULT SetString([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPWStr)] string wszValue);

            [PreserveSig]
            new HRESULT SetBlob([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf, int cbBufSize);

            [PreserveSig]
            new HRESULT SetUnknown([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.IUnknown)] object pUnknown);

            [PreserveSig]
            new HRESULT LockStore();

            [PreserveSig]
            new HRESULT UnlockStore();

            [PreserveSig]
            new HRESULT GetCount(out uint pcItems);

            [PreserveSig]
            new HRESULT GetItemByIndex(uint unIndex, out Guid pguidKey, [In][Out] PROPVARIANT pValue);

            [PreserveSig]
            new HRESULT CopyAllItems(IMFAttributes pDest);

            [PreserveSig]
            HRESULT GetSampleFlags(out uint pdwSampleFlags);

            [PreserveSig]
            HRESULT SetSampleFlags(uint dwSampleFlags);

            [PreserveSig]
            HRESULT GetSampleTime(out long phnsSampleTime);

            [PreserveSig]
            HRESULT SetSampleTime(long hnsSampleTime);

            [PreserveSig]
            HRESULT GetSampleDuration(out long phnsSampleDuration);

            [PreserveSig]
            HRESULT SetSampleDuration(long hnsSampleDuration);

            [PreserveSig]
            HRESULT GetBufferCount(out uint pdwBufferCount);

            [PreserveSig]
            HRESULT GetBufferByIndex(uint dwIndex, out IMFMediaBuffer ppBuffer);

            [PreserveSig]
            HRESULT ConvertToContiguousBuffer(out IMFMediaBuffer ppBuffer);

            [PreserveSig]
            HRESULT AddBuffer(IMFMediaBuffer pBuffer);

            [PreserveSig]
            HRESULT RemoveBufferByIndex(uint dwIndex);

            [PreserveSig]
            HRESULT RemoveAllBuffers();

            [PreserveSig]
            HRESULT GetTotalLength(out uint pcbTotalLength);

            [PreserveSig]
            HRESULT CopyToBuffer(IMFMediaBuffer pBuffer);
        }


        [DllImport("mf", ExactSpelling = true)]
        public static extern HRESULT MFEnumDeviceSources(IMFAttributes pAttributes, out IntPtr pppSourceActivate, out uint pcSourceActivate);


        [ComImport]
        [Guid("279a808d-aec7-40c8-9c6b-a6b492c78a66")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMFMediaSource : IMFMediaEventGenerator
        {
            [PreserveSig]
            new HRESULT GetEvent(uint dwFlags, out IMFMediaEvent ppEvent);

            [PreserveSig]
            new HRESULT BeginGetEvent(IMFAsyncCallback pCallback, [MarshalAs(UnmanagedType.IUnknown)] object punkState);

            [PreserveSig]
            new HRESULT EndGetEvent(IMFAsyncResult pResult, out IMFMediaEvent ppEvent);

            [PreserveSig]
            new HRESULT QueueEvent(uint met, [MarshalAs(UnmanagedType.LPStruct)] Guid guidExtendedType, HRESULT hrStatus, [In][Out] PROPVARIANT pvValue);

            [PreserveSig]
            HRESULT GetCharacteristics(out uint pdwCharacteristics);

            [PreserveSig]
            HRESULT CreatePresentationDescriptor(out IMFPresentationDescriptor ppPresentationDescriptor);

            [PreserveSig]
            HRESULT Start(IMFPresentationDescriptor pPresentationDescriptor, IntPtr pguidTimeFormat, [In][Out] PROPVARIANT pvarStartPosition);

            [PreserveSig]
            HRESULT Stop();

            [PreserveSig]
            HRESULT Pause();

            [PreserveSig]
            HRESULT Shutdown();
        }


        [DllImport("mfplat", ExactSpelling = true)]
        public static extern HRESULT MFCreateAttributes(out IMFAttributes ppMFAttributes, uint cInitialSize);

        [ComImport]
        [Guid("2cd2d921-c447-44a7-a13c-4adabfc247e3")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMFAttributes
        {
            [PreserveSig]
            HRESULT GetItem([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][Out] PROPVARIANT pValue);

            [PreserveSig]
            HRESULT GetItemType([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out _MF_ATTRIBUTE_TYPE pType);

            [PreserveSig]
            HRESULT CompareItem([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][Out] PROPVARIANT Value, out bool pbResult);

            [PreserveSig]
            HRESULT Compare(IMFAttributes pTheirs, _MF_ATTRIBUTES_MATCH_TYPE MatchType, out bool pbResult);

            [PreserveSig]
            HRESULT GetUINT32([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out uint punValue);

            [PreserveSig]
            HRESULT GetUINT64([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out ulong punValue);

            [PreserveSig]
            HRESULT GetDouble([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out double pfValue);

            [PreserveSig]
            HRESULT GetGUID([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out Guid pguidValue);

            [PreserveSig]
            HRESULT GetStringLength([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out uint pcchLength);

            [PreserveSig]
            HRESULT GetString([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPWStr)] string pwszValue, uint cchBufSize, IntPtr pcchLength);

            [PreserveSig]
            HRESULT GetAllocatedString([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, IntPtr ppwszValue, out uint pcchLength);

            [PreserveSig]
            HRESULT GetBlobSize([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out uint pcbBlobSize);

            [PreserveSig]
            HRESULT GetBlob([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf, int cbBufSize, IntPtr pcbBlobSize);

            [PreserveSig]
            HRESULT GetAllocatedBlob([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, out IntPtr ppBuf, out uint pcbSize);

            [PreserveSig]
            HRESULT GetUnknown([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppv);

            [PreserveSig]
            HRESULT SetItem([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [In][Out] PROPVARIANT Value);

            [PreserveSig]
            HRESULT DeleteItem([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey);

            [PreserveSig]
            HRESULT DeleteAllItems();

            [PreserveSig]
            HRESULT SetUINT32([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, uint unValue);

            [PreserveSig]
            HRESULT SetUINT64([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, ulong unValue);

            [PreserveSig]
            HRESULT SetDouble([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, double fValue);

            [PreserveSig]
            HRESULT SetGUID([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPStruct)] Guid guidValue);

            [PreserveSig]
            HRESULT SetString([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPWStr)] string wszValue);

            [PreserveSig]
            HRESULT SetBlob([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf, int cbBufSize);

            [PreserveSig]
            HRESULT SetUnknown([MarshalAs(UnmanagedType.LPStruct)] Guid guidKey, [MarshalAs(UnmanagedType.IUnknown)] object pUnknown);

            [PreserveSig]
            HRESULT LockStore();

            [PreserveSig]
            HRESULT UnlockStore();

            [PreserveSig]
            HRESULT GetCount(out uint pcItems);

            [PreserveSig]
            HRESULT GetItemByIndex(uint unIndex, out Guid pguidKey, [In][Out] PROPVARIANT pValue);

            [PreserveSig]
            HRESULT CopyAllItems(IMFAttributes pDest);
        }

        /// <summary>
        /// 從媒體來源建立 IMFSourceReader
        /// </summary>
        [DllImport("mfreadwrite", ExactSpelling = true)]
        public static extern HRESULT MFCreateSourceReaderFromMediaSource(
            IMFMediaSource pMediaSource,
            IMFAttributes pAttributes,
            out IMFSourceReader ppSourceReader);

        #endregion

        #region COM Interfaces

        /// <summary>
        /// IMFSourceReader 介面 - 用於讀取媒體樣本
        /// GUID: {70ae66f2-c809-4e4f-8915-bdcb406b7993}
        /// </summary>
        [ComImport]
        [Guid("70ae66f2-c809-4e4f-8915-bdcb406b7993")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMFSourceReader
        {
            /// <summary>
            /// 取得串流選擇狀態
            /// </summary>
            [PreserveSig]
            HRESULT GetStreamSelection(uint dwStreamIndex, out bool pfSelected);

            /// <summary>
            /// 設定串流選擇狀態
            /// </summary>
            [PreserveSig]
            HRESULT SetStreamSelection(uint dwStreamIndex, bool fSelected);

            /// <summary>
            /// 取得原生媒體類型
            /// </summary>
            [PreserveSig]
            HRESULT GetNativeMediaType(uint dwStreamIndex, uint dwMediaTypeIndex, out IMFMediaType ppMediaType);

            /// <summary>
            /// 取得目前媒體類型
            /// </summary>
            [PreserveSig]
            HRESULT GetCurrentMediaType(uint dwStreamIndex, out IMFMediaType ppMediaType);

            /// <summary>
            /// 設定目前媒體類型
            /// </summary>
            [PreserveSig]
            HRESULT SetCurrentMediaType(uint dwStreamIndex, IntPtr pdwReserved, IMFMediaType pMediaType);

            /// <summary>
            /// 設定目前位置
            /// </summary>
            [PreserveSig]
            HRESULT SetCurrentPosition([MarshalAs(UnmanagedType.LPStruct)] Guid guidTimeFormat, [In][Out] PROPVARIANT varPosition);

            /// <summary>
            /// 讀取媒體樣本
            /// </summary>
            [PreserveSig]
            HRESULT ReadSample(
                uint dwStreamIndex,
                uint dwControlFlags,
                IntPtr pdwActualStreamIndex,
                IntPtr pdwStreamFlags,
                IntPtr pllTimestamp,
                IntPtr ppSample);

            /// <summary>
            /// 清除讀取緩衝區
            /// </summary>
            [PreserveSig]
            HRESULT Flush(uint dwStreamIndex);

            /// <summary>
            /// 取得串流服務
            /// </summary>
            [PreserveSig]
            HRESULT GetServiceForStream(
                uint dwStreamIndex,
                [MarshalAs(UnmanagedType.LPStruct)] Guid guidService,
                [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                out IntPtr ppvObject);

            /// <summary>
            /// 取得呈現屬性
            /// </summary>
            [PreserveSig]
            HRESULT GetPresentationAttribute(
                uint dwStreamIndex,
                [MarshalAs(UnmanagedType.LPStruct)] Guid guidAttribute,
                [In][Out] PROPVARIANT pvarAttribute);
        }

        /// <summary>
        /// IMFSourceReaderCallback 介面 - 非同步讀取回調
        /// GUID: {deec8d99-fa1d-4d82-84c2-2c8969944867}
        /// </summary>
        [ComImport]
        [Guid("deec8d99-fa1d-4d82-84c2-2c8969944867")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMFSourceReaderCallback
        {
            /// <summary>
            /// 當媒體樣本讀取完成時調用
            /// </summary>
            [PreserveSig]
            HRESULT OnReadSample(
                HRESULT hrStatus,
                uint dwStreamIndex,
                uint dwStreamFlags,
                long llTimestamp,
                IntPtr pSampleptr);

            /// <summary>
            /// 當清除操作完成時調用
            /// </summary>
            [PreserveSig]
            HRESULT OnFlush(uint dwStreamIndex);

            /// <summary>
            /// 當媒體事件發生時調用
            /// </summary>
            [PreserveSig]
            HRESULT OnEvent(uint dwStreamIndex, IMFMediaEvent pEvent);
        }

        #endregion

        #region Media Foundation Constants (常用的 GUID)

        /// <summary>
        /// 表示視訊格式 RGB32 的 GUID
        /// </summary>
        public static readonly Guid MFVideoFormat_RGB32 = DirectN.MFConstants.MFVideoFormat_RGB32;

        /// <summary>
        /// 表示視訊格式 ARGB32 的 GUID
        /// </summary>
        public static readonly Guid MFVideoFormat_ARGB32 = DirectN.MFConstants.MFVideoFormat_ARGB32;

        /// <summary>
        /// 媒體類型主要類型屬性
        /// </summary>
        public static readonly Guid MF_MT_MAJOR_TYPE = DirectN.MFConstants.MF_MT_MAJOR_TYPE;

        /// <summary>
        /// 媒體類型子類型屬性
        /// </summary>
        public static readonly Guid MF_MT_SUBTYPE = DirectN.MFConstants.MF_MT_SUBTYPE;

        /// <summary>
        /// 來源讀取器非同步回調屬性
        /// </summary>
        public static readonly Guid MF_SOURCE_READER_ASYNC_CALLBACK = DirectN.MFConstants.MF_SOURCE_READER_ASYNC_CALLBACK;

        /// <summary>
        /// 來源讀取器 D3D 設備管理器屬性
        /// </summary>
        public static readonly Guid MF_SOURCE_READER_D3D_MANAGER = DirectN.MFConstants.MF_SOURCE_READER_D3D_MANAGER;

        /// <summary>
        /// 來源讀取器禁用 DXVA 屬性
        /// </summary>
        public static readonly Guid MF_SOURCE_READER_DISABLE_DXVA = DirectN.MFConstants.MF_SOURCE_READER_DISABLE_DXVA;

        /// <summary>
        /// 來源讀取器啟用視訊處理屬性
        /// </summary>
        public static readonly Guid MF_SOURCE_READER_ENABLE_VIDEO_PROCESSING = DirectN.MFConstants.MF_SOURCE_READER_ENABLE_VIDEO_PROCESSING;

        /// <summary>
        /// 來源讀取器啟用高級視訊處理屬性
        /// </summary>
        public static readonly Guid MF_SOURCE_READER_ENABLE_ADVANCED_VIDEO_PROCESSING = DirectN.MFConstants.MF_SOURCE_READER_ENABLE_ADVANCED_VIDEO_PROCESSING;

        /// <summary>
        /// 緩衝區服務 GUID - 用於從 IMFMediaBuffer 獲取 Direct3D Surface
        /// </summary>
        public static readonly Guid MR_BUFFER_SERVICE = DirectN.MFConstants.MR_BUFFER_SERVICE;

        /// <summary>
        /// Direct3D Surface GUID - 用於 MFGetService 查詢
        /// </summary>
        public static readonly Guid IID_IDirect3DSurface9 = new Guid("0cfbaf3a-9ff6-429a-99b3-a2796af8b89b");

        /// <summary>
        /// 讀寫啟用硬體轉換屬性
        /// </summary>
        public static readonly Guid MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS = DirectN.MFConstants.MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS;

        #endregion

        #region 來源讀取器常數

        /// <summary>
        /// 來源讀取器第一個視訊串流索引
        /// </summary>
        public const uint MF_SOURCE_READER_FIRST_VIDEO_STREAM = 0xFFFFFFFC;

        /// <summary>
        /// 來源讀取器所有流索引
        /// </summary>
        public const uint MF_SOURCE_READER_ALL_STREAMS = 0xFFFFFFFE;

        /// <summary>
        /// 來源讀取器媒體類型索引
        /// </summary>
        public const uint MF_SOURCE_READER_CURRENT_TYPE_INDEX = 0xFFFFFFFF;

        #endregion
    }
}
