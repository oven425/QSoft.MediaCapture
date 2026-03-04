using DirectN;
using System;
using System.Runtime.InteropServices;

namespace WpfApp_D3DImage
{
    /// <summary>
    /// Direct3D9 DLL Import 和 COM Interface 定義
    /// 統一管理所有 Direct3D9 相關函數和介面
    /// </summary>
    public partial class WebCamD3D9
    {
        #region Direct3D9 DLL Imports

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct _D3DSURFACE_DESC
        {
            public _D3DFORMAT Format;

            public _D3DRESOURCETYPE Type;

            public uint Usage;

            public _D3DPOOL Pool;

            public _D3DMULTISAMPLE_TYPE MultiSampleType;

            public uint MultiSampleQuality;

            public uint Width;

            public uint Height;
        }
        public struct tagRECT
        {
            public int left;

            public int top;

            public int right;

            public int bottom;

            public tagRECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
        }
            /// <summary>
            /// 建立 IDirect3D9 物件
            /// </summary>
            [DllImport("d3d9", ExactSpelling = true)]
        public static extern IDirect3D9 Direct3DCreate9(uint SDKVersion);

        /// <summary>
        /// 建立 IDirect3D9Ex 物件 (Enhanced version for Vista+)
        /// </summary>
        [DllImport("d3d9", ExactSpelling = true)]
        public static extern HRESULT Direct3DCreate9Ex(uint SDKVersion, out IDirect3D9Ex pD3D);

        #endregion

        #region DXVA2 DLL Imports

        /// <summary>
        /// 建立 DXVA2 Direct3D 設備管理器
        /// </summary>
        [DllImport("dxva2", ExactSpelling = true)]
        public static extern HRESULT DXVA2CreateDirect3DDeviceManager9(
            out uint pResetToken,
            out IDirect3DDeviceManager9 ppDeviceManager);

        #endregion

        #region COM Interfaces

        /// <summary>
        /// IDirect3D9 介面 - Direct3D9 物件建立者
        /// GUID: {81bdcbca-64d4-426d-ae15-e2c3807e7675}
        /// </summary>
        [ComImport]
        [Guid("81bdcbca-64d4-426d-ae15-e2c3807e7675")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDirect3D9
        {
            /// <summary>
            /// 註冊軟體裝置
            /// </summary>
            [PreserveSig]
            HRESULT RegisterSoftwareDevice(IntPtr pInitializeFunction);

            /// <summary>
            /// 取得介面卡計數
            /// </summary>
            [PreserveSig]
            uint GetAdapterCount();

            /// <summary>
            /// 取得介面卡識別碼
            /// </summary>
            [PreserveSig]
            HRESULT GetAdapterIdentifier(uint Adapter, uint Flags, out IntPtr pIdentifier);

            /// <summary>
            /// 取得介面卡顯示模式計數
            /// </summary>
            [PreserveSig]
            uint GetAdapterModeCount(uint Adapter, uint Format);

            /// <summary>
            /// 列舉介面卡顯示模式
            /// </summary>
            [PreserveSig]
            HRESULT EnumAdapterModes(uint Adapter, uint Format, uint Mode, out IntPtr pMode);

            /// <summary>
            /// 取得介面卡顯示模式
            /// </summary>
            [PreserveSig]
            HRESULT GetAdapterDisplayMode(uint Adapter, out IntPtr pMode);

            /// <summary>
            /// 檢查設備相容性
            /// </summary>
            [PreserveSig]
            HRESULT CheckDeviceType(uint Adapter, int DeviceType, uint AdapterFormat, uint BackBufferFormat, [MarshalAs(UnmanagedType.Bool)] bool Windowed);

            /// <summary>
            /// 檢查表面格式相容性
            /// </summary>
            [PreserveSig]
            HRESULT CheckSurfaceFormat(uint Adapter, int DeviceType, uint SurfaceFormat, [MarshalAs(UnmanagedType.Bool)] bool Windowed);

            /// <summary>
            /// 檢查深度/樣板格式相容性
            /// </summary>
            [PreserveSig]
            HRESULT CheckDepthStencilMatch(uint Adapter, int DeviceType, uint AdapterFormat, uint RenderTargetFormat, uint DepthStencilFormat);

            /// <summary>
            /// 檢查格式轉換
            /// </summary>
            [PreserveSig]
            HRESULT CheckFormatConversion(uint Adapter, int DeviceType, uint SourceFormat, uint TargetFormat);

            /// <summary>
            /// 取得設備上限
            /// </summary>
            [PreserveSig]
            HRESULT GetDeviceCaps(uint Adapter, int DeviceType, out IntPtr pCaps);

            /// <summary>
            /// 取得監視器
            /// </summary>
            [PreserveSig]
            IntPtr GetAdapterMonitor(uint Adapter);

            /// <summary>
            /// 建立設備
            /// </summary>
            [PreserveSig]
            HRESULT CreateDevice(
                uint Adapter,
                int DeviceType,
                IntPtr hFocusWindow,
                uint BehaviorFlags,
                IntPtr pPresentationParameters,
                out IDirect3DDevice9 ppReturnedDeviceInterface);
        }

        /// <summary>
        /// IDirect3D9Ex 介面 - Enhanced Direct3D9 (Vista+)
        /// GUID: {02177241-69fc-400c-8ff1-93a44df6861d}
        /// </summary>
        /// 
        public enum _D3DDEVTYPE
        {
            D3DDEVTYPE_HAL = 1,
            D3DDEVTYPE_REF = 2,
            D3DDEVTYPE_SW = 3,
            D3DDEVTYPE_NULLREF = 4,
            D3DDEVTYPE_FORCE_DWORD = int.MaxValue
        }
        [ComImport]
        [Guid("02177241-69fc-400c-8ff1-93a44df6861d")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDirect3D9Ex : IDirect3D9
        {
            /// <summary>
            /// 列舉介面卡模式(Ex版本)
            /// </summary>
            [PreserveSig]
            HRESULT EnumAdapterModesEx(uint Adapter, ref IntPtr pFilter, uint Mode, out IntPtr pMode);

            /// <summary>
            /// 取得介面卡顯示模式(Ex版本)
            /// </summary>
            [PreserveSig]
            HRESULT GetAdapterDisplayModeEx(uint Adapter, out IntPtr pMode, out IntPtr pRotation);
            
            /// <summary>
            /// 建立設備(Ex版本) - 支援立即上下文和其他增強功能
            /// </summary>
            [PreserveSig]
            HRESULT CreateDeviceEx(
                uint Adapter,
                _D3DDEVTYPE DeviceType,
                IntPtr hFocusWindow,
                uint BehaviorFlags,
                IntPtr pPresentationParameters,
                IntPtr pFullscreenDisplayMode,
                out IDirect3DDevice9Ex ppReturnedDeviceInterface);

            /// <summary>
            /// 建立設備狀態區塊
            /// </summary>
            [PreserveSig]
            HRESULT CreateStateBlock(int Type, out IntPtr ppSB);

            /// <summary>
            /// 列舉設備和裝置
            /// </summary>
            [PreserveSig]
            HRESULT EnumDevices(uint Adapter, out IntPtr pDevicesOut);

            /// <summary>
            /// 取得副本間隔資訊
            /// </summary>
            [PreserveSig]
            HRESULT GetAdapterLUID(uint Adapter, out IntPtr pLUID);
        }

        /// <summary>
        /// IDirect3DDevice9 介面 - Direct3D9 設備
        /// GUID: {d0223b63-bf15-4d47-a8a6-fbaf2d53fe33}
        /// </summary>
        [ComImport]
        [Guid("d0223b63-bf15-4d47-a8a6-fbaf2d53fe33")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDirect3DDevice9
        {
            /// <summary>
            /// 測試合作級別
            /// </summary>
            [PreserveSig]
            HRESULT TestCooperativeLevel();

            /// <summary>
            /// 取得可用紋理記憶體
            /// </summary>
            [PreserveSig]
            uint GetAvailableTextureMem();

            /// <summary>
            /// 逐項驅逐管理資源
            /// </summary>
            [PreserveSig]
            HRESULT EvictManagedResources();

            /// <summary>
            /// 取得直接上下文
            /// </summary>
            [PreserveSig]
            HRESULT GetDirect3D(out IDirect3D9 ppD3D9);

            /// <summary>
            /// 取得設備上限
            /// </summary>
            [PreserveSig]
            HRESULT GetDeviceCaps(out IntPtr pCaps);

            /// <summary>
            /// 取得顯示格式
            /// </summary>
            [PreserveSig]
            HRESULT GetDisplayMode(uint iSwapChain, out IntPtr pMode);

            /// <summary>
            /// 取得建立參數
            /// </summary>
            [PreserveSig]
            HRESULT GetCreationParameters(out IntPtr pParameters);

            /// <summary>
            /// 設定游標屬性
            /// </summary>
            [PreserveSig]
            HRESULT SetCursorProperties(uint XHotSpot, uint YHotSpot, IntPtr pCursorBitmap);

            /// <summary>
            /// 設定游標位置
            /// </summary>
            [PreserveSig]
            void SetCursorPosition(int X, int Y, uint Flags);

            /// <summary>
            /// 顯示游標
            /// </summary>
            [PreserveSig]
            [return: MarshalAs(UnmanagedType.Bool)]
            bool ShowCursor([MarshalAs(UnmanagedType.Bool)] bool bShow);

            /// <summary>
            /// 建立額外的交換鏈結
            /// </summary>
            [PreserveSig]
            HRESULT CreateAdditionalSwapChain(IntPtr pPresentationParameters, out IntPtr pSwapChain);

            /// <summary>
            /// 取得交換鏈結
            /// </summary>
            [PreserveSig]
            HRESULT GetSwapChain(uint iSwapChain, out IntPtr pSwapChain);

            /// <summary>
            /// 取得交換鏈結計數
            /// </summary>
            [PreserveSig]
            uint GetNumberOfSwapChains();

            /// <summary>
            /// 重設設備
            /// </summary>
            [PreserveSig]
            HRESULT Reset(IntPtr pPresentationParameters);

            /// <summary>
            /// 呈現場景
            /// </summary>
            [PreserveSig]
            HRESULT Present(IntPtr pSourceRect, IntPtr pDestRect, IntPtr hDestWindowOverride, IntPtr pDirtyRegion);

            /// <summary>
            /// 取得後台緩衝
            /// </summary>
            [PreserveSig]
            HRESULT GetBackBuffer(uint iSwapChain, uint iBackBuffer, int Type, out IDirect3DSurface9 ppBackBuffer);

            /// <summary>
            /// 取得光柵狀態
            /// </summary>
            [PreserveSig]
            HRESULT GetRasterStatus(uint iSwapChain, out IntPtr pRasterStatus);

            /// <summary>
            /// 設定對話框
            /// </summary>
            [PreserveSig]
            HRESULT SetDialogBoxMode([MarshalAs(UnmanagedType.Bool)] bool bEnableDialogs);

            /// <summary>
            /// 設定遊戲選項
            /// </summary>
            [PreserveSig]
            void SetGammaRamp(uint iSwapChain, uint Flags, IntPtr pRamp);

            /// <summary>
            /// 取得遊戲選項
            /// </summary>
            [PreserveSig]
            void GetGammaRamp(uint iSwapChain, IntPtr pRamp);

            /// <summary>
            /// 建立材質
            /// </summary>
            [PreserveSig]
            HRESULT CreateTexture(
                uint Width,
                uint Height,
                uint Levels,
                uint Usage,
                uint Format,
                uint Pool,
                out IDirect3DTexture9 ppTexture,
                IntPtr pSharedHandle);

            /// <summary>
            /// 建立音量材質
            /// </summary>
            [PreserveSig]
            HRESULT CreateVolumeTexture(
                uint Width,
                uint Height,
                uint Depth,
                uint Levels,
                uint Usage,
                uint Format,
                uint Pool,
                out IntPtr ppVolumeTexture,
                IntPtr pSharedHandle);

            /// <summary>
            /// 建立立方體材質
            /// </summary>
            [PreserveSig]
            HRESULT CreateCubeTexture(
                uint EdgeLength,
                uint Levels,
                uint Usage,
                uint Format,
                uint Pool,
                out IntPtr ppCubeTexture,
                IntPtr pSharedHandle);

            /// <summary>
            /// 建立表面
            /// </summary>
            [PreserveSig]
            HRESULT CreateOffscreenPlainSurface(
                uint Width,
                uint Height,
                uint Format,
                uint Pool,
                out IDirect3DSurface9 ppSurface,
                IntPtr pSharedHandle);

            /// <summary>
            /// 建立渲染目標表面
            /// </summary>
            [PreserveSig]
            HRESULT CreateRenderTarget(
                uint Width,
                uint Height,
                uint Format,
                uint MultiSample,
                uint MultisampleQuality,
                [MarshalAs(UnmanagedType.Bool)] bool Lockable,
                out IDirect3DSurface9 ppSurface,
                IntPtr pSharedHandle);

            /// <summary>
            /// 建立深度/樣板表面
            /// </summary>
            [PreserveSig]
            HRESULT CreateDepthStencilSurface(
                uint Width,
                uint Height,
                uint Format,
                uint MultiSample,
                uint MultisampleQuality,
                [MarshalAs(UnmanagedType.Bool)] bool Discard,
                out IDirect3DSurface9 ppSurface,
                IntPtr pSharedHandle);

            /// <summary>
            /// 更新表面
            /// </summary>
            [PreserveSig]
            HRESULT UpdateSurface(
                IDirect3DSurface9 pSourceSurface,
                IntPtr pSourceRect,
                IDirect3DSurface9 pDestinationSurface,
                IntPtr pDestPoint);

            /// <summary>
            /// 複製矩形
            /// </summary>
            [PreserveSig]
            HRESULT CopyRects(
                IDirect3DSurface9 pSourceSurface,
                IntPtr pSourceRectsArray,
                uint cRects,
                IDirect3DSurface9 pDestinationSurface,
                IntPtr pDestPointsArray);

            /// <summary>
            /// 伸縮矩形 - 用於複製和縮放表面
            /// </summary>
            [PreserveSig]
            HRESULT StretchRect(
                IDirect3DSurface9 pSourceSurface,
                tagRECT pSourceRect,
                IDirect3DSurface9 pDestSurface,
                tagRECT pDestRect,
                uint Filter);

            /// <summary>
            /// 顏色填充
            /// </summary>
            [PreserveSig]
            HRESULT ColorFill(IDirect3DSurface9 pSurface, IntPtr pRect, uint color);

            /// <summary>
            /// 建立頂點緩衝
            /// </summary>
            [PreserveSig]
            HRESULT CreateVertexBuffer(
                uint Length,
                uint Usage,
                uint FVF,
                uint Pool,
                out IntPtr ppVertexBuffer,
                IntPtr pSharedHandle);

            /// <summary>
            /// 建立索引緩衝
            /// </summary>
            [PreserveSig]
            HRESULT CreateIndexBuffer(
                uint Length,
                uint Usage,
                uint Format,
                uint Pool,
                out IntPtr ppIndexBuffer,
                IntPtr pSharedHandle);

            /// <summary>
            /// 建立頂點宣告
            /// </summary>
            [PreserveSig]
            HRESULT CreateVertexDeclaration(
                IntPtr pVertexElements,
                out IntPtr ppDecl);

            /// <summary>
            /// 取得頂點宣告
            /// </summary>
            [PreserveSig]
            HRESULT GetVertexDeclaration(out IntPtr ppDecl);

            /// <summary>
            /// 設定頂點宣告
            /// </summary>
            [PreserveSig]
            HRESULT SetVertexDeclaration(IntPtr pDecl);

            /// <summary>
            /// 取得頂點著色器常數寄存器大小
            /// </summary>
            [PreserveSig]
            HRESULT GetFVFVertexSize(uint FVF, out uint pSize);

            /// <summary>
            /// 建立頂點著色器
            /// </summary>
            [PreserveSig]
            HRESULT CreateVertexShader(IntPtr pFunction, out IntPtr ppShader);

            /// <summary>
            /// 設定頂點著色器
            /// </summary>
            [PreserveSig]
            HRESULT SetVertexShader(IntPtr pShader);

            /// <summary>
            /// 取得頂點著色器
            /// </summary>
            [PreserveSig]
            HRESULT GetVertexShader(out IntPtr ppShader);

            /// <summary>
            /// 設定頂點著色器常數
            /// </summary>
            [PreserveSig]
            HRESULT SetVertexShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

            /// <summary>
            /// 取得頂點著色器常數
            /// </summary>
            [PreserveSig]
            HRESULT GetVertexShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

            /// <summary>
            /// 設定頂點著色器常數(整數)
            /// </summary>
            [PreserveSig]
            HRESULT SetVertexShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

            /// <summary>
            /// 取得頂 vertex著色器常數(整數)
            /// </summary>
            [PreserveSig]
            HRESULT GetVertexShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

            /// <summary>
            /// 設定頂 vertex著色器常數(布林)
            /// </summary>
            [PreserveSig]
            HRESULT SetVertexShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

            /// <summary>
            /// 取得頂 vertex著色器常數(布林)
            /// </summary>
            [PreserveSig]
            HRESULT GetVertexShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

            /// <summary>
            /// 設定頂點資料流來源
            /// </summary>
            [PreserveSig]
            HRESULT SetStreamSource(uint StreamNumber, IntPtr pStreamData, uint OffsetInBytes, uint Stride);

            /// <summary>
            /// 取得頂點資料流來源
            /// </summary>
            [PreserveSig]
            HRESULT GetStreamSource(uint StreamNumber, out IntPtr ppStreamData, out uint pOffsetInBytes, out uint pStride);

            /// <summary>
            /// 設定頂點資料流頻率
            /// </summary>
            [PreserveSig]
            HRESULT SetStreamSourceFreq(uint StreamNumber, uint Divider);

            /// <summary>
            /// 取得頂點資料流頻率
            /// </summary>
            [PreserveSig]
            HRESULT GetStreamSourceFreq(uint StreamNumber, out uint pDivider);

            /// <summary>
            /// 設定索引緩衝
            /// </summary>
            [PreserveSig]
            HRESULT SetIndices(IntPtr pIndexData);

            /// <summary>
            /// 取得索引緩衝
            /// </summary>
            [PreserveSig]
            HRESULT GetIndices(out IntPtr ppIndexData);

            /// <summary>
            /// 建立像素著色器
            /// </summary>
            [PreserveSig]
            HRESULT CreatePixelShader(IntPtr pFunction, out IntPtr ppShader);

            /// <summary>
            /// 設定像素著色器
            /// </summary>
            [PreserveSig]
            HRESULT SetPixelShader(IntPtr pShader);

            /// <summary>
            /// 取得像素著色器
            /// </summary>
            [PreserveSig]
            HRESULT GetPixelShader(out IntPtr ppShader);

            /// <summary>
            /// 設定像素著色器常數
            /// </summary>
            [PreserveSig]
            HRESULT SetPixelShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

            /// <summary>
            /// 取得像素著色器常數
            /// </summary>
            [PreserveSig]
            HRESULT GetPixelShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

            /// <summary>
            /// 設定像素著色器常數(整數)
            /// </summary>
            [PreserveSig]
            HRESULT SetPixelShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

            /// <summary>
            /// 取得像素著色器常數(整數)
            /// </summary>
            [PreserveSig]
            HRESULT GetPixelShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

            /// <summary>
            /// 設定像素著色器常數(布林)
            /// </summary>
            [PreserveSig]
            HRESULT SetPixelShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

            /// <summary>
            /// 取得像素著色器常數(布林)
            /// </summary>
            [PreserveSig]
            HRESULT GetPixelShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

            /// <summary>
            /// 設定剪裁平面
            /// </summary>
            [PreserveSig]
            HRESULT SetClipPlane(uint Index, IntPtr pPlane);

            /// <summary>
            /// 取得剪裁平面
            /// </summary>
            [PreserveSig]
            HRESULT GetClipPlane(uint Index, IntPtr pPlane);

            /// <summary>
            /// 設定呈現狀態
            /// </summary>
            [PreserveSig]
            HRESULT SetRenderState(uint State, uint Value);

            /// <summary>
            /// 取得呈現狀態
            /// </summary>
            [PreserveSig]
            HRESULT GetRenderState(uint State, out uint pValue);

            /// <summary>
            /// 建立狀態區塊
            /// </summary>
            [PreserveSig]
            HRESULT CreateStateBlock(int Type, out IntPtr ppSB);

            /// <summary>
            /// 開始場景
            /// </summary>
            [PreserveSig]
            HRESULT BeginScene();

            /// <summary>
            /// 結束場景
            /// </summary>
            [PreserveSig]
            HRESULT EndScene();

            /// <summary>
            /// 清空
            /// </summary>
            [PreserveSig]
            HRESULT Clear(uint Count, IntPtr pRects, uint Flags, uint Color, float Z, uint Stencil);

            /// <summary>
            /// 設定轉換矩陣
            /// </summary>
            [PreserveSig]
            HRESULT SetTransform(uint State, IntPtr pMatrix);

            /// <summary>
            /// 取得轉換矩陣
            /// </summary>
            [PreserveSig]
            HRESULT GetTransform(uint State, IntPtr pMatrix);

            /// <summary>
            /// 乘法轉換矩陣
            /// </summary>
            [PreserveSig]
            HRESULT MultiplyTransform(uint State, IntPtr pMatrix);

            /// <summary>
            /// 設定視埠
            /// </summary>
            [PreserveSig]
            HRESULT SetViewport(IntPtr pViewport);

            /// <summary>
            /// 取得視埠
            /// </summary>
            [PreserveSig]
            HRESULT GetViewport(IntPtr pViewport);

            /// <summary>
            /// 設定材質
            /// </summary>
            [PreserveSig]
            HRESULT SetMaterial(IntPtr pMaterial);

            /// <summary>
            /// 取得材質
            /// </summary>
            [PreserveSig]
            HRESULT GetMaterial(IntPtr pMaterial);

            /// <summary>
            /// 設定光線
            /// </summary>
            [PreserveSig]
            HRESULT SetLight(uint Index, IntPtr pLight);

            /// <summary>
            /// 取得光線
            /// </summary>
            [PreserveSig]
            HRESULT GetLight(uint Index, IntPtr pLight);

            /// <summary>
            /// 啟用光線
            /// </summary>
            [PreserveSig]
            HRESULT LightEnable(uint Index, [MarshalAs(UnmanagedType.Bool)] bool Enable);

            /// <summary>
            /// 取得光線啟用狀態
            /// </summary>
            [PreserveSig]
            HRESULT GetLightEnable(uint Index, out bool pEnable);

            /// <summary>
            /// 設定紋理貼圖狀態
            /// </summary>
            [PreserveSig]
            HRESULT SetTextureStageState(uint Stage, uint Type, uint Value);

            /// <summary>
            /// 取得紋理貼圖狀態
            /// </summary>
            [PreserveSig]
            HRESULT GetTextureStageState(uint Stage, uint Type, out uint pValue);

            /// <summary>
            /// 取得樣本狀態
            /// </summary>
            [PreserveSig]
            HRESULT GetSamplerState(uint Sampler, uint Type, out uint pValue);

            /// <summary>
            /// 設定樣本狀態
            /// </summary>
            [PreserveSig]
            HRESULT SetSamplerState(uint Sampler, uint Type, uint Value);

            /// <summary>
            /// 驗證設備狀態
            /// </summary>
            [PreserveSig]
            HRESULT ValidateDevice(out uint pNumPasses);

            /// <summary>
            /// 設定調色板項
            /// </summary>
            [PreserveSig]
            HRESULT SetPaletteEntries(uint PaletteNumber, IntPtr pEntries);

            /// <summary>
            /// 取得調色板項
            /// </summary>
            [PreserveSig]
            HRESULT GetPaletteEntries(uint PaletteNumber, IntPtr pEntries);

            /// <summary>
            /// 設定目前調色板
            /// </summary>
            [PreserveSig]
            HRESULT SetCurrentTexturePalette(uint PaletteNumber);

            /// <summary>
            /// 取得目前調色板
            /// </summary>
            [PreserveSig]
            HRESULT GetCurrentTexturePalette(out uint PaletteNumber);

            /// <summary>
            /// 設定紋理材質
            /// </summary>
            [PreserveSig]
            HRESULT SetTexture(uint Stage, IntPtr pTexture);

            /// <summary>
            /// 取得紋理材質
            /// </summary>
            [PreserveSig]
            HRESULT GetTexture(uint Stage, out IntPtr ppTexture);

            /// <summary>
            /// 繪製基本元素
            /// </summary>
            [PreserveSig]
            HRESULT DrawPrimitive(int PrimitiveType, uint StartVertex, uint PrimitiveCount);

            /// <summary>
            /// 繪製基本元素(索引版本)
            /// </summary>
            [PreserveSig]
            HRESULT DrawIndexedPrimitive(int Type, int BaseVertexIndex, uint MinVertexIndex, uint NumVertices, uint startIndex, uint primCount);

            /// <summary>
            /// 繪製基本元素(使用者記憶體版本)
            /// </summary>
            [PreserveSig]
            HRESULT DrawPrimitiveUP(int PrimitiveType, uint PrimitiveCount, IntPtr pVertexStreamZeroData, uint VertexStreamZeroStride);

            /// <summary>
            /// 繪製基本元素(使用者記憶體，索引版本)
            /// </summary>
            [PreserveSig]
            HRESULT DrawIndexedPrimitiveUP(int PrimitiveType, uint MinVertexIndex, uint NumVertices, uint PrimitiveCount, IntPtr pIndexData, uint IndexDataFormat, IntPtr pVertexStreamZeroData, uint VertexStreamZeroStride);

            /// <summary>
            /// 處理頂點
            /// </summary>
            [PreserveSig]
            HRESULT ProcessVertices(uint SrcStartIndex, uint DestIndex, uint VertexCount, IntPtr pDestBuffer, IntPtr pVertexDecl, uint Flags);

            /// <summary>
            /// 建立查詢
            /// </summary>
            [PreserveSig]
            HRESULT CreateQuery(int Type, out IntPtr ppQuery);
        }

        /// <summary>
        /// IDirect3DDevice9Ex 介面 - Enhanced Direct3D9 Device (Vista+)
        /// GUID: {b18b10ce-2649-405a-8f3b-1708f6545290}
        /// </summary>
        [ComImport]
        [Guid("b18b10ce-2649-405a-8f3b-1708f6545290")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDirect3DDevice9Ex : IDirect3DDevice9
        {
            /// <summary>
            /// 設定呈現狀態(Ex版本)
            /// </summary>
            [PreserveSig]
            HRESULT SetConvolutionMonoKernel(uint width, uint height, IntPtr rows, IntPtr columns);

            /// <summary>
            /// 複製紋理至記憶體
            /// </summary>
            [PreserveSig]
            HRESULT ComposeRects(IDirect3DSurface9 pSrc, IDirect3DSurface9 pDst, IDirect3DVertexBuffer9 pSrcRectDescs, uint NumRects, IDirect3DVertexBuffer9 pDstRectDescs, int Operation, int Xoffset, int Yoffset);

            /// <summary>
            /// 取得 GPU 執行緒優先度
            /// </summary>
            [PreserveSig]
            HRESULT GetGPUThreadPriority(out int pPriority);

            /// <summary>
            /// 設定 GPU 執行緒優先度
            /// </summary>
            [PreserveSig]
            HRESULT SetGPUThreadPriority(int Priority);

            /// <summary>
            /// 設定視訊處理渲染狀態
            /// </summary>
            [PreserveSig]
            HRESULT SetMaximumFrameLatency(uint MaxLatency);

            /// <summary>
            /// 取得視訊處理渲染狀態
            /// </summary>
            [PreserveSig]
            HRESULT GetMaximumFrameLatency(out uint pMaxLatency);

            /// <summary>
            /// 檢查資源格式支援
            /// </summary>
            [PreserveSig]
            HRESULT CheckResourceResidency(IntPtr pResourceArray, uint NumResources);

            /// <summary>
            /// 取得顯示模式顏色值 (更新版)
            /// </summary>
            [PreserveSig]
            HRESULT GetDisplayModeEx(uint iSwapChain, out IntPtr pMode, out IntPtr pRotation);

            /// <summary>
            /// 建立紋理材質(增強版)
            /// </summary>
            [PreserveSig]
            HRESULT CreateOffscreenPlainSurfaceEx(uint Width, uint Height, uint Format, uint Pool, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle, uint Usage);

            /// <summary>
            /// 建立渲染目標(增強版)
            /// </summary>
            [PreserveSig]
            HRESULT CreateRenderTargetEx(uint Width, uint Height, uint Format, uint MultiSample, uint MultisampleQuality, [MarshalAs(UnmanagedType.Bool)] bool Lockable, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle, uint Usage);

            /// <summary>
            /// 建立深度/樣板表面(增強版)
            /// </summary>
            [PreserveSig]
            HRESULT CreateDepthStencilSurfaceEx(uint Width, uint Height, uint Format, uint MultiSample, uint MultisampleQuality, [MarshalAs(UnmanagedType.Bool)] bool Discard, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle, uint Usage);

            /// <summary>
            /// 重設設備(增強版)
            /// </summary>
            [PreserveSig]
            HRESULT ResetEx(IntPtr pPresentationParameters, IntPtr pFullscreenDisplayMode);

            /// <summary>
            /// 取得顯示表面鎖定矩形(增強版)
            /// </summary>
            [PreserveSig]
            HRESULT GetDisplaySurfaceData(IDirect3DSurface9 pDestSurface);

            /// <summary>
            /// 等待 GPU 完成指定查詢
            /// </summary>
            [PreserveSig]
            HRESULT WaitForVBlank(uint iSwapChain);

            /// <summary>
            /// 檢查程式設計相關支援
            /// </summary>
            [PreserveSig]
            HRESULT CheckDeviceState(IntPtr hDestinationWindow);
        }

        /// <summary>
        /// IDirect3DSurface9 介面 - Direct3D9 表面
        /// GUID: {0cfbaf3a-9ff6-429a-99b3-a2796af8b89b}
        /// </summary>
        [ComImport]
        [Guid("0cfbaf3a-9ff6-429a-99b3-a2796af8b89b")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDirect3DSurface9
        {
            /// <summary>
            /// 取得設備
            /// </summary>
            [PreserveSig]
            HRESULT GetDevice(out IDirect3DDevice9 ppDevice);

            /// <summary>
            /// 取得表面描述
            /// </summary>
            [PreserveSig]
            HRESULT GetDesc(ref _D3DSURFACE_DESC pDesc);

            /// <summary>
            /// 鎖定矩形
            /// </summary>
            [PreserveSig]
            HRESULT LockRect(out IntPtr pLockedRect, IntPtr pRect, uint Flags);

            /// <summary>
            /// 解鎖矩形
            /// </summary>
            [PreserveSig]
            HRESULT UnlockRect();

            /// <summary>
            /// 取得直接上下文
            /// </summary>
            [PreserveSig]
            HRESULT GetDC(out IntPtr phdc);

            /// <summary>
            /// 釋放直接上下文
            /// </summary>
            [PreserveSig]
            HRESULT ReleaseDC(IntPtr hdc);
        }

        /// <summary>
        /// IDirect3DTexture9 介面 - Direct3D9 紋理
        /// GUID: {85c31227-3de5-4f00-9b3a-f11ac38c18b5}
        /// </summary>
        [ComImport]
        [Guid("85c31227-3de5-4f00-9b3a-f11ac38c18b5")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDirect3DTexture9
        {
            /// <summary>
            /// 取得設備
            /// </summary>
            [PreserveSig]
            HRESULT GetDevice(out IDirect3DDevice9 ppDevice);

            /// <summary>
            /// 設定私有資料
            /// </summary>
            [PreserveSig]
            HRESULT SetPrivateData(ref Guid refguid, IntPtr pData, uint SizeOfData, uint Flags);

            /// <summary>
            /// 取得私有資料
            /// </summary>
            [PreserveSig]
            HRESULT GetPrivateData(ref Guid refguid, IntPtr pData, out uint pSizeOfData);

            /// <summary>
            /// 釋放私有資料
            /// </summary>
            [PreserveSig]
            HRESULT FreePrivateData(ref Guid refguid);

            /// <summary>
            /// 設定優先度
            /// </summary>
            [PreserveSig]
            uint SetPriority(uint PriorityNew);

            /// <summary>
            /// 取得優先度
            /// </summary>
            [PreserveSig]
            uint GetPriority();

            /// <summary>
            /// 強制加載
            /// </summary>
            [PreserveSig]
            void PreLoad();

            /// <summary>
            /// 取得類型
            /// </summary>
            [PreserveSig]
            int GetType();

            /// <summary>
            /// 取得表面級別數
            /// </summary>
            [PreserveSig]
            uint GetLevelCount();

            /// <summary>
            /// 設定自動世代過濾
            /// </summary>
            [PreserveSig]
            HRESULT SetAutoGenFilterType(int FilterType);

            /// <summary>
            /// 取得自動世代過濾
            /// </summary>
            [PreserveSig]
            int GetAutoGenFilterType();

            /// <summary>
            /// 產生 MipMap
            /// </summary>
            [PreserveSig]
            void GenerateMipSubLevels();

            /// <summary>
            /// 取得表面級別
            /// </summary>
            [PreserveSig]
            HRESULT GetSurfaceLevel(uint Level, out IDirect3DSurface9 ppSurfaceLevel);

            /// <summary>
            /// 鎖定矩形
            /// </summary>
            [PreserveSig]
            HRESULT LockRect(uint Level, out IntPtr pLockedRect, IntPtr pRect, uint Flags);

            /// <summary>
            /// 解鎖矩形
            /// </summary>
            [PreserveSig]
            HRESULT UnlockRect(uint Level);

            /// <summary>
            /// 新增表面參考
            /// </summary>
            [PreserveSig]
            HRESULT AddDirtyRect(IntPtr pDirtyRect);
        }

        /// <summary>
        /// IDirect3DVertexBuffer9 介面 - Direct3D9 頂點緩衝
        /// GUID: {b64bb1b5-fd70-4df7-a4a7-1559e4d07760}
        /// </summary>
        [ComImport]
        [Guid("b64bb1b5-fd70-4df7-a4a7-1559e4d07760")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDirect3DVertexBuffer9
        {
            /// <summary>
            /// 取得設備
            /// </summary>
            [PreserveSig]
            HRESULT GetDevice(out IDirect3DDevice9 ppDevice);

            /// <summary>
            /// 設定私有資料
            /// </summary>
            [PreserveSig]
            HRESULT SetPrivateData(ref Guid refguid, IntPtr pData, uint SizeOfData, uint Flags);

            /// <summary>
            /// 取得私有資料
            /// </summary>
            [PreserveSig]
            HRESULT GetPrivateData(ref Guid refguid, IntPtr pData, out uint pSizeOfData);

            /// <summary>
            /// 釋放私有資料
            /// </summary>
            [PreserveSig]
            HRESULT FreePrivateData(ref Guid refguid);

            /// <summary>
            /// 設定優先度
            /// </summary>
            [PreserveSig]
            uint SetPriority(uint PriorityNew);

            /// <summary>
            /// 取得優先度
            /// </summary>
            [PreserveSig]
            uint GetPriority();

            /// <summary>
            /// 強制加載
            /// </summary>
            [PreserveSig]
            void PreLoad();

            /// <summary>
            /// 取得類型
            /// </summary>
            [PreserveSig]
            int GetType();

            /// <summary>
            /// 取得描述
            /// </summary>
            [PreserveSig]
            HRESULT GetDesc(out IntPtr pDesc);

            /// <summary>
            /// 鎖定
            /// </summary>
            [PreserveSig]
            HRESULT Lock(uint OffsetToLock, uint SizeToLock, out IntPtr ppbData, uint Flags);

            /// <summary>
            /// 解鎖
            /// </summary>
            [PreserveSig]
            HRESULT Unlock();
        }

        /// <summary>
        /// IDirect3DDeviceManager9 介面 - DXVA2 設備管理器
        /// GUID: {a0cade0f-06d5-4cf4-a1c7-f3cdd725aa75}
        /// </summary>
        [ComImport]
        [Guid("a0cade0f-06d5-4cf4-a1c7-f3cdd725aa75")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDirect3DDeviceManager9
        {
            /// <summary>
            /// 重設設備
            /// </summary>
            [PreserveSig]
            HRESULT ResetDevice(IDirect3DDevice9 pDevice, uint resetToken);

            /// <summary>
            /// 開啟設備處理
            /// </summary>
            [PreserveSig]
            HRESULT OpenDeviceHandle(out IntPtr phDevice);

            /// <summary>
            /// 關閉設備處理
            /// </summary>
            [PreserveSig]
            HRESULT CloseDeviceHandle(IntPtr hDevice);

            /// <summary>
            /// 測試設備
            /// </summary>
            [PreserveSig]
            HRESULT TestDevice(IntPtr hDevice);

            /// <summary>
            /// 鎖定設備
            /// </summary>
            [PreserveSig]
            HRESULT LockDevice(IntPtr hDevice, out IntPtr ppDevice, [MarshalAs(UnmanagedType.Bool)] bool fBlock);

            /// <summary>
            /// 解鎖設備
            /// </summary>
            [PreserveSig]
            HRESULT UnlockDevice(IntPtr hDevice, [MarshalAs(UnmanagedType.Bool)] bool fSaveState);

            /// <summary>
            /// 取得視訊服務
            /// </summary>
            [PreserveSig]
            HRESULT GetVideoService(IntPtr hDevice, ref Guid riid, out IntPtr ppService);
        }

        #endregion

        #region Direct3D9 Constants

        /// <summary>
        /// Direct3D9 SDK 版本
        /// </summary>
        public const uint D3D_SDK_VERSION = 32;

        /// <summary>
        /// Direct3D 介面卡預設值
        /// </summary>
        public const uint D3DADAPTER_DEFAULT = 0;

        #endregion

        #region Direct3D9 Format Constants

        /// <summary>
        /// 32位 RGB 格式 (XRGB8888)
        /// </summary>
        public const uint D3DFMT_X8R8G8B8 = 22;

        /// <summary>
        /// 32位 ARGB 格式 (ARGB8888)
        /// </summary>
        public const uint D3DFMT_A8R8G8B8 = 21;

        /// <summary>
        /// 24位 RGB 格式
        /// </summary>
        public const uint D3DFMT_R8G8B8 = 20;

        /// <summary>
        /// 16位 RGB 格式
        /// </summary>
        public const uint D3DFMT_R5G6B5 = 23;

        #endregion

        #region Direct3D9 Usage Flags

        /// <summary>
        /// 渲染目標用途
        /// </summary>
        public const uint D3DUSAGE_RENDERTARGET = 0x00000001;

        /// <summary>
        /// 深度樣板用途
        /// </summary>
        public const uint D3DUSAGE_DEPTHSTENCIL = 0x00000002;

        /// <summary>
        /// 動態用途
        /// </summary>
        public const uint D3DUSAGE_DYNAMIC = 0x00000200;

        /// <summary>
        /// 軟體處理
        /// </summary>
        public const uint D3DUSAGE_SOFTWAREPROCESSING = 0x00000010;

        #endregion

        #region Direct3D9 Pool Flags

        /// <summary>
        /// 預設記憶體池
        /// </summary>
        public const uint D3DPOOL_DEFAULT = 0;

        /// <summary>
        /// 管理記憶體池
        /// </summary>
        public const uint D3DPOOL_MANAGED = 1;

        /// <summary>
        /// 系統記憶體池
        /// </summary>
        public const uint D3DPOOL_SYSTEMMEM = 2;

        /// <summary>
        /// 暫存記憶體池
        /// </summary>
        public const uint D3DPOOL_SCRATCH = 3;

        #endregion

        #region Direct3D9 Texture Filter

        /// <summary>
        /// 不進行過濾
        /// </summary>
        public const uint D3DTEXF_NONE = 0;

        /// <summary>
        /// 點過濾
        /// </summary>
        public const uint D3DTEXF_POINT = 1;

        /// <summary>
        /// 線性過濾
        /// </summary>
        public const uint D3DTEXF_LINEAR = 2;

        /// <summary>
        /// 立方體過濾
        /// </summary>
        public const uint D3DTEXF_CUBIC = 3;

        /// <summary>
        /// 異向過濾
        /// </summary>
        public const uint D3DTEXF_ANISOTROPIC = 4;

        #endregion

        #region Direct3D9 Device Creation Flags

        /// <summary>
        /// 硬體頂點處理
        /// </summary>
        public const uint D3DCREATE_HARDWARE_VERTEXPROCESSING = 0x00000001;

        /// <summary>
        /// 軟體頂點處理
        /// </summary>
        public const uint D3DCREATE_SOFTWARE_VERTEXPROCESSING = 0x00000002;

        /// <summary>
        /// 混合頂點處理
        /// </summary>
        public const uint D3DCREATE_MIXED_VERTEXPROCESSING = 0x00000004;

        /// <summary>
        /// 多執行緒
        /// </summary>
        public const uint D3DCREATE_MULTITHREADED = 0x00000004;

        /// <summary>
        /// FPU 保留
        /// </summary>
        public const uint D3DCREATE_FPU_PRESERVE = 0x00000002;

        #endregion
    }
}
