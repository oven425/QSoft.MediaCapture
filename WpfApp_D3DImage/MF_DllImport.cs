using DirectN;
using System;
using System.Runtime.InteropServices;

namespace WpfApp_D3DImage
{
    /// <summary>
    /// Media Foundation DLL Import 和 COM Interface 定義
    /// 統一管理所有 DirectN 相關函數和 Media Foundation 介面
    /// </summary>
    public static class MF_DllImport
    {
        #region Media Foundation DLL Imports

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
