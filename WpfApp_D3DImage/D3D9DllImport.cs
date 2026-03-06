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

        [ComImport]
        [Guid("02177241-69fc-400c-8ff1-93a44df6861d")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDirect3D9Ex : DirectN.IDirect3D9
        {
            [PreserveSig]
            new HRESULT RegisterSoftwareDevice(IntPtr pInitializeFunction);

            [PreserveSig]
            new uint GetAdapterCount();

            [PreserveSig]
            new HRESULT GetAdapterIdentifier(uint Adapter, uint Flags, IntPtr pIdentifier);

            [PreserveSig]
            new uint GetAdapterModeCount(uint Adapter, _D3DFORMAT Format);

            [PreserveSig]
            new HRESULT EnumAdapterModes(uint Adapter, _D3DFORMAT Format, uint Mode, ref _D3DDISPLAYMODE pMode);

            [PreserveSig]
            new HRESULT GetAdapterDisplayMode(uint Adapter, ref _D3DDISPLAYMODE pMode);

            [PreserveSig]
            new HRESULT CheckDeviceType(uint Adapter, _D3DDEVTYPE DevType, _D3DFORMAT AdapterFormat, _D3DFORMAT BackBufferFormat, bool bWindowed);

            [PreserveSig]
            new HRESULT CheckDeviceFormat(uint Adapter, _D3DDEVTYPE DeviceType, _D3DFORMAT AdapterFormat, uint Usage, _D3DRESOURCETYPE RType, _D3DFORMAT CheckFormat);

            [PreserveSig]
            new HRESULT CheckDeviceMultiSampleType(uint Adapter, _D3DDEVTYPE DeviceType, _D3DFORMAT SurfaceFormat, bool Windowed, _D3DMULTISAMPLE_TYPE MultiSampleType, ref uint pQualityLevels);

            [PreserveSig]
            new HRESULT CheckDepthStencilMatch(uint Adapter, _D3DDEVTYPE DeviceType, _D3DFORMAT AdapterFormat, _D3DFORMAT RenderTargetFormat, _D3DFORMAT DepthStencilFormat);

            [PreserveSig]
            new HRESULT CheckDeviceFormatConversion(uint Adapter, _D3DDEVTYPE DeviceType, _D3DFORMAT SourceFormat, _D3DFORMAT TargetFormat);

            [PreserveSig]
            new HRESULT GetDeviceCaps(uint Adapter, _D3DDEVTYPE DeviceType, ref _D3DCAPS9 pCaps);

            [PreserveSig]
            new IntPtr GetAdapterMonitor(uint Adapter);

            [PreserveSig]
            new HRESULT CreateDevice(uint Adapter, _D3DDEVTYPE DeviceType, IntPtr hFocusWindow, uint BehaviorFlags, IntPtr pPresentationParameters, out IDirect3DDevice9 ppReturnedDeviceInterface);

            [PreserveSig]
            uint GetAdapterModeCountEx(uint Adapter, ref D3DDISPLAYMODEFILTER pFilter);

            [PreserveSig]
            HRESULT EnumAdapterModesEx(uint Adapter, ref D3DDISPLAYMODEFILTER pFilter, uint Mode, ref D3DDISPLAYMODEEX pMode);

            [PreserveSig]
            HRESULT GetAdapterDisplayModeEx(uint Adapter, ref D3DDISPLAYMODEEX pMode, ref D3DDISPLAYROTATION pRotation);

            [PreserveSig]
            HRESULT CreateDeviceEx(uint Adapter, _D3DDEVTYPE DeviceType, IntPtr hFocusWindow, uint BehaviorFlags, IntPtr pPresentationParameters, IntPtr pFullscreenDisplayMode, out IDirect3DDevice9Ex ppReturnedDeviceInterface);

            [PreserveSig]
            HRESULT GetAdapterLUID(uint Adapter, ref _LUID pLUID);
        }


        //[ComImport]
        //[Guid("d0223b96-bf7a-43fd-92bd-a43b0d82b9eb")]
        //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        //public interface IDirect3DDevice9
        //{
        //    [PreserveSig]
        //    HRESULT TestCooperativeLevel();

        //    [PreserveSig]
        //    uint GetAvailableTextureMem();

        //    [PreserveSig]
        //    HRESULT EvictManagedResources();

        //    [PreserveSig]
        //    HRESULT GetDirect3D(out IDirect3D9 ppD3D9);

        //    [PreserveSig]
        //    HRESULT GetDeviceCaps(ref _D3DCAPS9 pCaps);

        //    [PreserveSig]
        //    HRESULT GetDisplayMode(uint iSwapChain, ref _D3DDISPLAYMODE pMode);

        //    [PreserveSig]
        //    HRESULT GetCreationParameters(IntPtr pParameters);

        //    [PreserveSig]
        //    HRESULT SetCursorProperties(uint XHotSpot, uint YHotSpot, IDirect3DSurface9 pCursorBitmap);

        //    [PreserveSig]
        //    void SetCursorPosition(int X, int Y, uint Flags);

        //    [PreserveSig]
        //    bool ShowCursor(bool bShow);

        //    [PreserveSig]
        //    HRESULT CreateAdditionalSwapChain(IntPtr pPresentationParameters, out IDirect3DSwapChain9 pSwapChain);

        //    [PreserveSig]
        //    HRESULT GetSwapChain(uint iSwapChain, out IDirect3DSwapChain9 pSwapChain);

        //    [PreserveSig]
        //    uint GetNumberOfSwapChains();

        //    [PreserveSig]
        //    HRESULT Reset(IntPtr pPresentationParameters);

        //    [PreserveSig]
        //    HRESULT Present(ref tagRECT pSourceRect, ref tagRECT pDestRect, IntPtr hDestWindowOverride, ref _RGNDATA pDirtyRegion);

        //    [PreserveSig]
        //    HRESULT GetBackBuffer(uint iSwapChain, uint iBackBuffer, _D3DBACKBUFFER_TYPE Type, out IDirect3DSurface9 ppBackBuffer);

        //    [PreserveSig]
        //    HRESULT GetRasterStatus(uint iSwapChain, ref _D3DRASTER_STATUS pRasterStatus);

        //    [PreserveSig]
        //    HRESULT SetDialogBoxMode(bool bEnableDialogs);

        //    [PreserveSig]
        //    void SetGammaRamp(uint iSwapChain, uint Flags, ref _D3DGAMMARAMP pRamp);

        //    [PreserveSig]
        //    void GetGammaRamp(uint iSwapChain, ref _D3DGAMMARAMP pRamp);

        //    [PreserveSig]
        //    HRESULT CreateTexture(uint Width, uint Height, uint Levels, uint Usage, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DTexture9 ppTexture, IntPtr pSharedHandle);

        //    [PreserveSig]
        //    HRESULT CreateVolumeTexture(uint Width, uint Height, uint Depth, uint Levels, uint Usage, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DVolumeTexture9 ppVolumeTexture, IntPtr pSharedHandle);

        //    [PreserveSig]
        //    HRESULT CreateCubeTexture(uint EdgeLength, uint Levels, uint Usage, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DCubeTexture9 ppCubeTexture, IntPtr pSharedHandle);

        //    [PreserveSig]
        //    HRESULT CreateVertexBuffer(uint Length, uint Usage, uint FVF, _D3DPOOL Pool, out IDirect3DVertexBuffer9 ppVertexBuffer, IntPtr pSharedHandle);

        //    [PreserveSig]
        //    HRESULT CreateIndexBuffer(uint Length, uint Usage, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DIndexBuffer9 ppIndexBuffer, IntPtr pSharedHandle);

        //    [PreserveSig]
        //    HRESULT CreateRenderTarget(uint Width, uint Height, _D3DFORMAT Format, _D3DMULTISAMPLE_TYPE MultiSample, uint MultisampleQuality, bool Lockable, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle);

        //    [PreserveSig]
        //    HRESULT CreateDepthStencilSurface(uint Width, uint Height, _D3DFORMAT Format, _D3DMULTISAMPLE_TYPE MultiSample, uint MultisampleQuality, bool Discard, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle);

        //    [PreserveSig]
        //    HRESULT UpdateSurface(IDirect3DSurface9 pSourceSurface, ref tagRECT pSourceRect, IDirect3DSurface9 pDestinationSurface, ref tagPOINT pDestPoint);

        //    [PreserveSig]
        //    HRESULT UpdateTexture(IDirect3DBaseTexture9 pSourceTexture, IDirect3DBaseTexture9 pDestinationTexture);

        //    [PreserveSig]
        //    HRESULT GetRenderTargetData(IDirect3DSurface9 pRenderTarget, IDirect3DSurface9 pDestSurface);

        //    [PreserveSig]
        //    HRESULT GetFrontBufferData(uint iSwapChain, IDirect3DSurface9 pDestSurface);

        //    [PreserveSig]
        //    HRESULT StretchRect(IDirect3DSurface9 pSourceSurface, ref tagRECT pSourceRect, IDirect3DSurface9 pDestSurface, ref tagRECT pDestRect, _D3DTEXTUREFILTERTYPE Filter);

        //    [PreserveSig]
        //    HRESULT ColorFill(IDirect3DSurface9 pSurface, ref tagRECT pRect, uint color);

        //    [PreserveSig]
        //    HRESULT CreateOffscreenPlainSurface(uint Width, uint Height, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle);

        //    [PreserveSig]
        //    HRESULT SetRenderTarget(uint RenderTargetIndex, IDirect3DSurface9 pRenderTarget);

        //    [PreserveSig]
        //    HRESULT GetRenderTarget(uint RenderTargetIndex, out IDirect3DSurface9 ppRenderTarget);

        //    [PreserveSig]
        //    HRESULT SetDepthStencilSurface(IDirect3DSurface9 pNewZStencil);

        //    [PreserveSig]
        //    HRESULT GetDepthStencilSurface(out IDirect3DSurface9 ppZStencilSurface);

        //    [PreserveSig]
        //    HRESULT BeginScene();

        //    [PreserveSig]
        //    HRESULT EndScene();

        //    [PreserveSig]
        //    HRESULT Clear(uint Count, ref _D3DRECT pRects, uint Flags, uint Color, float Z, uint Stencil);

        //    [PreserveSig]
        //    HRESULT SetTransform(_D3DTRANSFORMSTATETYPE State, ref _D3DMATRIX pMatrix);

        //    [PreserveSig]
        //    HRESULT GetTransform(_D3DTRANSFORMSTATETYPE State, ref _D3DMATRIX pMatrix);

        //    [PreserveSig]
        //    HRESULT MultiplyTransform(_D3DTRANSFORMSTATETYPE unnamed__0, ref _D3DMATRIX unnamed__1);

        //    [PreserveSig]
        //    HRESULT SetViewport(ref _D3DVIEWPORT9 pViewport);

        //    [PreserveSig]
        //    HRESULT GetViewport(ref _D3DVIEWPORT9 pViewport);

        //    [PreserveSig]
        //    HRESULT SetMaterial(ref _D3DMATERIAL9 pMaterial);

        //    [PreserveSig]
        //    HRESULT GetMaterial(ref _D3DMATERIAL9 pMaterial);

        //    [PreserveSig]
        //    HRESULT SetLight(uint Index, ref _D3DLIGHT9 unnamed__1);

        //    [PreserveSig]
        //    HRESULT GetLight(uint Index, ref _D3DLIGHT9 unnamed__1);

        //    [PreserveSig]
        //    HRESULT LightEnable(uint Index, bool Enable);

        //    [PreserveSig]
        //    HRESULT GetLightEnable(uint Index, ref bool pEnable);

        //    [PreserveSig]
        //    HRESULT SetClipPlane(uint Index, ref float pPlane);

        //    [PreserveSig]
        //    HRESULT GetClipPlane(uint Index, ref float pPlane);

        //    [PreserveSig]
        //    HRESULT SetRenderState(_D3DRENDERSTATETYPE State, uint Value);

        //    [PreserveSig]
        //    HRESULT GetRenderState(_D3DRENDERSTATETYPE State, ref uint pValue);

        //    [PreserveSig]
        //    HRESULT CreateStateBlock(_D3DSTATEBLOCKTYPE Type, out IDirect3DStateBlock9 ppSB);

        //    [PreserveSig]
        //    HRESULT BeginStateBlock();

        //    [PreserveSig]
        //    HRESULT EndStateBlock(out IDirect3DStateBlock9 ppSB);

        //    [PreserveSig]
        //    HRESULT SetClipStatus(ref _D3DCLIPSTATUS9 pClipStatus);

        //    [PreserveSig]
        //    HRESULT GetClipStatus(ref _D3DCLIPSTATUS9 pClipStatus);

        //    [PreserveSig]
        //    HRESULT GetTexture(uint Stage, out IDirect3DBaseTexture9 ppTexture);

        //    [PreserveSig]
        //    HRESULT SetTexture(uint Stage, IDirect3DBaseTexture9 pTexture);

        //    [PreserveSig]
        //    HRESULT GetTextureStageState(uint Stage, _D3DTEXTURESTAGESTATETYPE Type, ref uint pValue);

        //    [PreserveSig]
        //    HRESULT SetTextureStageState(uint Stage, _D3DTEXTURESTAGESTATETYPE Type, uint Value);

        //    [PreserveSig]
        //    HRESULT GetSamplerState(uint Sampler, _D3DSAMPLERSTATETYPE Type, ref uint pValue);

        //    [PreserveSig]
        //    HRESULT SetSamplerState(uint Sampler, _D3DSAMPLERSTATETYPE Type, uint Value);

        //    [PreserveSig]
        //    HRESULT ValidateDevice(ref uint pNumPasses);

        //    [PreserveSig]
        //    HRESULT SetPaletteEntries(uint PaletteNumber, ref tagPALETTEENTRY pEntries);

        //    [PreserveSig]
        //    HRESULT GetPaletteEntries(uint PaletteNumber, ref tagPALETTEENTRY pEntries);

        //    [PreserveSig]
        //    HRESULT SetCurrentTexturePalette(uint PaletteNumber);

        //    [PreserveSig]
        //    HRESULT GetCurrentTexturePalette(ref uint PaletteNumber);

        //    [PreserveSig]
        //    HRESULT SetScissorRect(ref tagRECT pRect);

        //    [PreserveSig]
        //    HRESULT GetScissorRect(ref tagRECT pRect);

        //    [PreserveSig]
        //    HRESULT SetSoftwareVertexProcessing(bool bSoftware);

        //    [PreserveSig]
        //    bool GetSoftwareVertexProcessing();

        //    [PreserveSig]
        //    HRESULT SetNPatchMode(float nSegments);

        //    [PreserveSig]
        //    void GetNPatchMode();

        //    [PreserveSig]
        //    HRESULT DrawPrimitive(_D3DPRIMITIVETYPE PrimitiveType, uint StartVertex, uint PrimitiveCount);

        //    [PreserveSig]
        //    HRESULT DrawIndexedPrimitive(_D3DPRIMITIVETYPE unnamed__0, int BaseVertexIndex, uint MinVertexIndex, uint NumVertices, uint startIndex, uint primCount);

        //    [PreserveSig]
        //    HRESULT DrawPrimitiveUP(_D3DPRIMITIVETYPE PrimitiveType, uint PrimitiveCount, IntPtr pVertexStreamZeroData, uint VertexStreamZeroStride);

        //    [PreserveSig]
        //    HRESULT DrawIndexedPrimitiveUP(_D3DPRIMITIVETYPE PrimitiveType, uint MinVertexIndex, uint NumVertices, uint PrimitiveCount, IntPtr pIndexData, _D3DFORMAT IndexDataFormat, IntPtr pVertexStreamZeroData, uint VertexStreamZeroStride);

        //    [PreserveSig]
        //    HRESULT ProcessVertices(uint SrcStartIndex, uint DestIndex, uint VertexCount, IDirect3DVertexBuffer9 pDestBuffer, IDirect3DVertexDeclaration9 pVertexDecl, uint Flags);

        //    [PreserveSig]
        //    HRESULT CreateVertexDeclaration(ref _D3DVERTEXELEMENT9 pVertexElements, out IDirect3DVertexDeclaration9 ppDecl);

        //    [PreserveSig]
        //    HRESULT SetVertexDeclaration(IDirect3DVertexDeclaration9 pDecl);

        //    [PreserveSig]
        //    HRESULT GetVertexDeclaration(out IDirect3DVertexDeclaration9 ppDecl);

        //    [PreserveSig]
        //    HRESULT SetFVF(uint FVF);

        //    [PreserveSig]
        //    HRESULT GetFVF(ref uint pFVF);

        //    [PreserveSig]
        //    HRESULT CreateVertexShader(IntPtr pFunction, out IDirect3DVertexShader9 ppShader);

        //    [PreserveSig]
        //    HRESULT SetVertexShader(IDirect3DVertexShader9 pShader);

        //    [PreserveSig]
        //    HRESULT GetVertexShader(out IDirect3DVertexShader9 ppShader);

        //    [PreserveSig]
        //    HRESULT SetVertexShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

        //    [PreserveSig]
        //    HRESULT GetVertexShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

        //    [PreserveSig]
        //    HRESULT SetVertexShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

        //    [PreserveSig]
        //    HRESULT GetVertexShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

        //    [PreserveSig]
        //    HRESULT SetVertexShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

        //    [PreserveSig]
        //    HRESULT GetVertexShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

        //    [PreserveSig]
        //    HRESULT SetStreamSource(uint StreamNumber, IDirect3DVertexBuffer9 pStreamData, uint OffsetInBytes, uint Stride);

        //    [PreserveSig]
        //    HRESULT GetStreamSource(uint StreamNumber, out IDirect3DVertexBuffer9 ppStreamData, ref uint pOffsetInBytes, ref uint pStride);

        //    [PreserveSig]
        //    HRESULT SetStreamSourceFreq(uint StreamNumber, uint Setting);

        //    [PreserveSig]
        //    HRESULT GetStreamSourceFreq(uint StreamNumber, ref uint pSetting);

        //    [PreserveSig]
        //    HRESULT SetIndices(IDirect3DIndexBuffer9 pIndexData);

        //    [PreserveSig]
        //    HRESULT GetIndices(out IDirect3DIndexBuffer9 ppIndexData);

        //    [PreserveSig]
        //    HRESULT CreatePixelShader(IntPtr pFunction, out IDirect3DPixelShader9 ppShader);

        //    [PreserveSig]
        //    HRESULT SetPixelShader(IDirect3DPixelShader9 pShader);

        //    [PreserveSig]
        //    HRESULT GetPixelShader(out IDirect3DPixelShader9 ppShader);

        //    [PreserveSig]
        //    HRESULT SetPixelShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

        //    [PreserveSig]
        //    HRESULT GetPixelShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

        //    [PreserveSig]
        //    HRESULT SetPixelShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

        //    [PreserveSig]
        //    HRESULT GetPixelShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

        //    [PreserveSig]
        //    HRESULT SetPixelShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

        //    [PreserveSig]
        //    HRESULT GetPixelShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

        //    [PreserveSig]
        //    HRESULT DrawRectPatch(uint Handle, ref float pNumSegs, ref _D3DRECTPATCH_INFO pRectPatchInfo);

        //    [PreserveSig]
        //    HRESULT DrawTriPatch(uint Handle, ref float pNumSegs, ref _D3DTRIPATCH_INFO pTriPatchInfo);

        //    [PreserveSig]
        //    HRESULT DeletePatch(uint Handle);

        //    [PreserveSig]
        //    HRESULT CreateQuery(_D3DQUERYTYPE Type, out IDirect3DQuery9 ppQuery);
        //}

        [ComImport]
        [Guid("b18b10ce-2649-405a-870f-95f777d4313a")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDirect3DDevice9Ex : IDirect3DDevice9
        {
            [PreserveSig]
            new HRESULT TestCooperativeLevel();

            [PreserveSig]
            new uint GetAvailableTextureMem();

            [PreserveSig]
            new HRESULT EvictManagedResources();

            [PreserveSig]
            new HRESULT GetDirect3D(out IDirect3D9 ppD3D9);

            [PreserveSig]
            new HRESULT GetDeviceCaps(ref _D3DCAPS9 pCaps);

            [PreserveSig]
            new HRESULT GetDisplayMode(uint iSwapChain, ref _D3DDISPLAYMODE pMode);

            [PreserveSig]
            new HRESULT GetCreationParameters(IntPtr pParameters);

            [PreserveSig]
            new HRESULT SetCursorProperties(uint XHotSpot, uint YHotSpot, IDirect3DSurface9 pCursorBitmap);

            [PreserveSig]
            new void SetCursorPosition(int X, int Y, uint Flags);

            [PreserveSig]
            new bool ShowCursor(bool bShow);

            [PreserveSig]
            new HRESULT CreateAdditionalSwapChain(IntPtr pPresentationParameters, out IDirect3DSwapChain9 pSwapChain);

            [PreserveSig]
            new HRESULT GetSwapChain(uint iSwapChain, out IDirect3DSwapChain9 pSwapChain);

            [PreserveSig]
            new uint GetNumberOfSwapChains();

            [PreserveSig]
            new HRESULT Reset(IntPtr pPresentationParameters);

            [PreserveSig]
            new HRESULT Present(ref tagRECT pSourceRect, ref tagRECT pDestRect, IntPtr hDestWindowOverride, ref _RGNDATA pDirtyRegion);

            [PreserveSig]
            new HRESULT GetBackBuffer(uint iSwapChain, uint iBackBuffer, _D3DBACKBUFFER_TYPE Type, out IDirect3DSurface9 ppBackBuffer);

            [PreserveSig]
            new HRESULT GetRasterStatus(uint iSwapChain, ref _D3DRASTER_STATUS pRasterStatus);

            [PreserveSig]
            new HRESULT SetDialogBoxMode(bool bEnableDialogs);

            [PreserveSig]
            new void SetGammaRamp(uint iSwapChain, uint Flags, ref _D3DGAMMARAMP pRamp);

            [PreserveSig]
            new void GetGammaRamp(uint iSwapChain, ref _D3DGAMMARAMP pRamp);

            [PreserveSig]
            new HRESULT CreateTexture(uint Width, uint Height, uint Levels, uint Usage, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DTexture9 ppTexture, out IntPtr pSharedHandle);

            [PreserveSig]
            new HRESULT CreateVolumeTexture(uint Width, uint Height, uint Depth, uint Levels, uint Usage, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DVolumeTexture9 ppVolumeTexture, IntPtr pSharedHandle);

            [PreserveSig]
            new HRESULT CreateCubeTexture(uint EdgeLength, uint Levels, uint Usage, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DCubeTexture9 ppCubeTexture, IntPtr pSharedHandle);

            [PreserveSig]
            new HRESULT CreateVertexBuffer(uint Length, uint Usage, uint FVF, _D3DPOOL Pool, out IDirect3DVertexBuffer9 ppVertexBuffer, IntPtr pSharedHandle);

            [PreserveSig]
            new HRESULT CreateIndexBuffer(uint Length, uint Usage, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DIndexBuffer9 ppIndexBuffer, IntPtr pSharedHandle);

            [PreserveSig]
            new HRESULT CreateRenderTarget(uint Width, uint Height, _D3DFORMAT Format, _D3DMULTISAMPLE_TYPE MultiSample, uint MultisampleQuality, bool Lockable, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle);

            [PreserveSig]
            new HRESULT CreateDepthStencilSurface(uint Width, uint Height, _D3DFORMAT Format, _D3DMULTISAMPLE_TYPE MultiSample, uint MultisampleQuality, bool Discard, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle);

            [PreserveSig]
            new HRESULT UpdateSurface(IDirect3DSurface9 pSourceSurface, ref tagRECT pSourceRect, IDirect3DSurface9 pDestinationSurface, ref tagPOINT pDestPoint);

            [PreserveSig]
            new HRESULT UpdateTexture(IDirect3DBaseTexture9 pSourceTexture, IDirect3DBaseTexture9 pDestinationTexture);

            [PreserveSig]
            new HRESULT GetRenderTargetData(IDirect3DSurface9 pRenderTarget, IDirect3DSurface9 pDestSurface);

            [PreserveSig]
            new HRESULT GetFrontBufferData(uint iSwapChain, IDirect3DSurface9 pDestSurface);

            [PreserveSig]
            new HRESULT StretchRect(IDirect3DSurface9 pSourceSurface, ref tagRECT pSourceRect, IDirect3DSurface9 pDestSurface, ref tagRECT pDestRect, _D3DTEXTUREFILTERTYPE Filter);

            [PreserveSig]
            new HRESULT ColorFill(IDirect3DSurface9 pSurface, ref tagRECT pRect, uint color);

            [PreserveSig]
            new HRESULT CreateOffscreenPlainSurface(uint Width, uint Height, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle);

            [PreserveSig]
            new HRESULT SetRenderTarget(uint RenderTargetIndex, IDirect3DSurface9 pRenderTarget);

            [PreserveSig]
            new HRESULT GetRenderTarget(uint RenderTargetIndex, out IDirect3DSurface9 ppRenderTarget);

            [PreserveSig]
            new HRESULT SetDepthStencilSurface(IDirect3DSurface9 pNewZStencil);

            [PreserveSig]
            new HRESULT GetDepthStencilSurface(out IDirect3DSurface9 ppZStencilSurface);

            [PreserveSig]
            new HRESULT BeginScene();

            [PreserveSig]
            new HRESULT EndScene();

            [PreserveSig]
            new HRESULT Clear(uint Count, ref _D3DRECT pRects, uint Flags, uint Color, float Z, uint Stencil);

            [PreserveSig]
            new HRESULT SetTransform(_D3DTRANSFORMSTATETYPE State, ref _D3DMATRIX pMatrix);

            [PreserveSig]
            new HRESULT GetTransform(_D3DTRANSFORMSTATETYPE State, ref _D3DMATRIX pMatrix);

            [PreserveSig]
            new HRESULT MultiplyTransform(_D3DTRANSFORMSTATETYPE unnamed__0, ref _D3DMATRIX unnamed__1);

            [PreserveSig]
            new HRESULT SetViewport(ref _D3DVIEWPORT9 pViewport);

            [PreserveSig]
            new HRESULT GetViewport(ref _D3DVIEWPORT9 pViewport);

            [PreserveSig]
            new HRESULT SetMaterial(ref _D3DMATERIAL9 pMaterial);

            [PreserveSig]
            new HRESULT GetMaterial(ref _D3DMATERIAL9 pMaterial);

            [PreserveSig]
            new HRESULT SetLight(uint Index, ref _D3DLIGHT9 unnamed__1);

            [PreserveSig]
            new HRESULT GetLight(uint Index, ref _D3DLIGHT9 unnamed__1);

            [PreserveSig]
            new HRESULT LightEnable(uint Index, bool Enable);

            [PreserveSig]
            new HRESULT GetLightEnable(uint Index, ref bool pEnable);

            [PreserveSig]
            new HRESULT SetClipPlane(uint Index, ref float pPlane);

            [PreserveSig]
            new HRESULT GetClipPlane(uint Index, ref float pPlane);

            [PreserveSig]
            new HRESULT SetRenderState(_D3DRENDERSTATETYPE State, uint Value);

            [PreserveSig]
            new HRESULT GetRenderState(_D3DRENDERSTATETYPE State, ref uint pValue);

            [PreserveSig]
            new HRESULT CreateStateBlock(_D3DSTATEBLOCKTYPE Type, out IDirect3DStateBlock9 ppSB);

            [PreserveSig]
            new HRESULT BeginStateBlock();

            [PreserveSig]
            new HRESULT EndStateBlock(out IDirect3DStateBlock9 ppSB);

            [PreserveSig]
            new HRESULT SetClipStatus(ref _D3DCLIPSTATUS9 pClipStatus);

            [PreserveSig]
            new HRESULT GetClipStatus(ref _D3DCLIPSTATUS9 pClipStatus);

            [PreserveSig]
            new HRESULT GetTexture(uint Stage, out IDirect3DBaseTexture9 ppTexture);

            [PreserveSig]
            new HRESULT SetTexture(uint Stage, IDirect3DBaseTexture9 pTexture);

            [PreserveSig]
            new HRESULT GetTextureStageState(uint Stage, _D3DTEXTURESTAGESTATETYPE Type, ref uint pValue);

            [PreserveSig]
            new HRESULT SetTextureStageState(uint Stage, _D3DTEXTURESTAGESTATETYPE Type, uint Value);

            [PreserveSig]
            new HRESULT GetSamplerState(uint Sampler, _D3DSAMPLERSTATETYPE Type, ref uint pValue);

            [PreserveSig]
            new HRESULT SetSamplerState(uint Sampler, _D3DSAMPLERSTATETYPE Type, uint Value);

            [PreserveSig]
            new HRESULT ValidateDevice(ref uint pNumPasses);

            [PreserveSig]
            new HRESULT SetPaletteEntries(uint PaletteNumber, ref tagPALETTEENTRY pEntries);

            [PreserveSig]
            new HRESULT GetPaletteEntries(uint PaletteNumber, ref tagPALETTEENTRY pEntries);

            [PreserveSig]
            new HRESULT SetCurrentTexturePalette(uint PaletteNumber);

            [PreserveSig]
            new HRESULT GetCurrentTexturePalette(ref uint PaletteNumber);

            [PreserveSig]
            new HRESULT SetScissorRect(ref tagRECT pRect);

            [PreserveSig]
            new HRESULT GetScissorRect(ref tagRECT pRect);

            [PreserveSig]
            new HRESULT SetSoftwareVertexProcessing(bool bSoftware);

            [PreserveSig]
            new bool GetSoftwareVertexProcessing();

            [PreserveSig]
            new HRESULT SetNPatchMode(float nSegments);

            [PreserveSig]
            new void GetNPatchMode();

            [PreserveSig]
            new HRESULT DrawPrimitive(_D3DPRIMITIVETYPE PrimitiveType, uint StartVertex, uint PrimitiveCount);

            [PreserveSig]
            new HRESULT DrawIndexedPrimitive(_D3DPRIMITIVETYPE unnamed__0, int BaseVertexIndex, uint MinVertexIndex, uint NumVertices, uint startIndex, uint primCount);

            [PreserveSig]
            new HRESULT DrawPrimitiveUP(_D3DPRIMITIVETYPE PrimitiveType, uint PrimitiveCount, IntPtr pVertexStreamZeroData, uint VertexStreamZeroStride);

            [PreserveSig]
            new HRESULT DrawIndexedPrimitiveUP(_D3DPRIMITIVETYPE PrimitiveType, uint MinVertexIndex, uint NumVertices, uint PrimitiveCount, IntPtr pIndexData, _D3DFORMAT IndexDataFormat, IntPtr pVertexStreamZeroData, uint VertexStreamZeroStride);

            [PreserveSig]
            new HRESULT ProcessVertices(uint SrcStartIndex, uint DestIndex, uint VertexCount, IDirect3DVertexBuffer9 pDestBuffer, IDirect3DVertexDeclaration9 pVertexDecl, uint Flags);

            [PreserveSig]
            new HRESULT CreateVertexDeclaration(ref _D3DVERTEXELEMENT9 pVertexElements, out IDirect3DVertexDeclaration9 ppDecl);

            [PreserveSig]
            new HRESULT SetVertexDeclaration(IDirect3DVertexDeclaration9 pDecl);

            [PreserveSig]
            new HRESULT GetVertexDeclaration(out IDirect3DVertexDeclaration9 ppDecl);

            [PreserveSig]
            new HRESULT SetFVF(uint FVF);

            [PreserveSig]
            new HRESULT GetFVF(ref uint pFVF);

            [PreserveSig]
            new HRESULT CreateVertexShader(IntPtr pFunction, out IDirect3DVertexShader9 ppShader);

            [PreserveSig]
            new HRESULT SetVertexShader(IDirect3DVertexShader9 pShader);

            [PreserveSig]
            new HRESULT GetVertexShader(out IDirect3DVertexShader9 ppShader);

            [PreserveSig]
            new HRESULT SetVertexShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

            [PreserveSig]
            new HRESULT GetVertexShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

            [PreserveSig]
            new HRESULT SetVertexShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

            [PreserveSig]
            new HRESULT GetVertexShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

            [PreserveSig]
            new HRESULT SetVertexShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

            [PreserveSig]
            new HRESULT GetVertexShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

            [PreserveSig]
            new HRESULT SetStreamSource(uint StreamNumber, IDirect3DVertexBuffer9 pStreamData, uint OffsetInBytes, uint Stride);

            [PreserveSig]
            new HRESULT GetStreamSource(uint StreamNumber, out IDirect3DVertexBuffer9 ppStreamData, ref uint pOffsetInBytes, ref uint pStride);

            [PreserveSig]
            new HRESULT SetStreamSourceFreq(uint StreamNumber, uint Setting);

            [PreserveSig]
            new HRESULT GetStreamSourceFreq(uint StreamNumber, ref uint pSetting);

            [PreserveSig]
            new HRESULT SetIndices(IDirect3DIndexBuffer9 pIndexData);

            [PreserveSig]
            new HRESULT GetIndices(out IDirect3DIndexBuffer9 ppIndexData);

            [PreserveSig]
            new HRESULT CreatePixelShader(IntPtr pFunction, out IDirect3DPixelShader9 ppShader);

            [PreserveSig]
            new HRESULT SetPixelShader(IDirect3DPixelShader9 pShader);

            [PreserveSig]
            new HRESULT GetPixelShader(out IDirect3DPixelShader9 ppShader);

            [PreserveSig]
            new HRESULT SetPixelShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

            [PreserveSig]
            new HRESULT GetPixelShaderConstantF(uint StartRegister, IntPtr pConstantData, uint Vector4fCount);

            [PreserveSig]
            new HRESULT SetPixelShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

            [PreserveSig]
            new HRESULT GetPixelShaderConstantI(uint StartRegister, IntPtr pConstantData, uint Vector4iCount);

            [PreserveSig]
            new HRESULT SetPixelShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

            [PreserveSig]
            new HRESULT GetPixelShaderConstantB(uint StartRegister, IntPtr pConstantData, uint BoolCount);

            [PreserveSig]
            new HRESULT DrawRectPatch(uint Handle, ref float pNumSegs, ref _D3DRECTPATCH_INFO pRectPatchInfo);

            [PreserveSig]
            new HRESULT DrawTriPatch(uint Handle, ref float pNumSegs, ref _D3DTRIPATCH_INFO pTriPatchInfo);

            [PreserveSig]
            new HRESULT DeletePatch(uint Handle);

            [PreserveSig]
            new HRESULT CreateQuery(_D3DQUERYTYPE Type, out IDirect3DQuery9 ppQuery);

            [PreserveSig]
            HRESULT SetConvolutionMonoKernel(uint width, uint height, ref float rows, ref float columns);

            [PreserveSig]
            HRESULT ComposeRects(IDirect3DSurface9 pSrc, IDirect3DSurface9 pDst, IDirect3DVertexBuffer9 pSrcRectDescs, uint NumRects, IDirect3DVertexBuffer9 pDstRectDescs, _D3DCOMPOSERECTSOP Operation, int Xoffset, int Yoffset);

            [PreserveSig]
            HRESULT PresentEx(ref tagRECT pSourceRect, ref tagRECT pDestRect, IntPtr hDestWindowOverride, ref _RGNDATA pDirtyRegion, uint dwFlags);

            [PreserveSig]
            HRESULT GetGPUThreadPriority(ref int pPriority);

            [PreserveSig]
            HRESULT SetGPUThreadPriority(int Priority);

            [PreserveSig]
            HRESULT WaitForVBlank(uint iSwapChain);

            [PreserveSig]
            HRESULT CheckResourceResidency(out IDirect3DResource9 pResourceArray, uint NumResources);

            [PreserveSig]
            HRESULT SetMaximumFrameLatency(uint MaxLatency);

            [PreserveSig]
            HRESULT GetMaximumFrameLatency(ref uint pMaxLatency);

            [PreserveSig]
            HRESULT CheckDeviceState(IntPtr hDestinationWindow);

            [PreserveSig]
            HRESULT CreateRenderTargetEx(uint Width, uint Height, _D3DFORMAT Format, _D3DMULTISAMPLE_TYPE MultiSample, uint MultisampleQuality, bool Lockable, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle, uint Usage);

            [PreserveSig]
            HRESULT CreateOffscreenPlainSurfaceEx(uint Width, uint Height, _D3DFORMAT Format, _D3DPOOL Pool, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle, uint Usage);

            [PreserveSig]
            HRESULT CreateDepthStencilSurfaceEx(uint Width, uint Height, _D3DFORMAT Format, _D3DMULTISAMPLE_TYPE MultiSample, uint MultisampleQuality, bool Discard, out IDirect3DSurface9 ppSurface, IntPtr pSharedHandle, uint Usage);

            [PreserveSig]
            HRESULT ResetEx(IntPtr pPresentationParameters, ref D3DDISPLAYMODEEX pFullscreenDisplayMode);

            [PreserveSig]
            HRESULT GetDisplayModeEx(uint iSwapChain, ref D3DDISPLAYMODEEX pMode, ref D3DDISPLAYROTATION pRotation);
        }

        [ComImport]
        [Guid("0cfbaf3a-9ff6-429a-99b3-a2796af8b89b")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDirect3DSurface9 : IDirect3DResource9
        {
            [PreserveSig]
            new HRESULT GetDevice(out IDirect3DDevice9 ppDevice);

            [PreserveSig]
            new HRESULT SetPrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid refguid, IntPtr pData, uint SizeOfData, uint Flags);

            [PreserveSig]
            new HRESULT GetPrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid refguid, IntPtr pData, ref uint pSizeOfData);

            [PreserveSig]
            new HRESULT FreePrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid refguid);

            [PreserveSig]
            new uint SetPriority(uint PriorityNew);

            [PreserveSig]
            new uint GetPriority();

            [PreserveSig]
            new void PreLoad();

            [PreserveSig]
            new _D3DRESOURCETYPE GetType();

            [PreserveSig]
            HRESULT GetContainer([MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppContainer);

            [PreserveSig]
            HRESULT GetDesc(ref _D3DSURFACE_DESC pDesc);

            [PreserveSig]
            HRESULT LockRect(IntPtr pLockedRect, IntPtr pRect, uint Flags);

            [PreserveSig]
            HRESULT UnlockRect();

            [PreserveSig]
            HRESULT GetDC(IntPtr phdc);

            [PreserveSig]
            HRESULT ReleaseDC(IntPtr hdc);
        }

        [ComImport]
        [Guid("85c31227-3de5-4f00-9b3a-f11ac38c18b5")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDirect3DTexture9 : IDirect3DBaseTexture9, IDirect3DResource9
        {
            [PreserveSig]
            new HRESULT GetDevice(out IDirect3DDevice9 ppDevice);

            [PreserveSig]
            new HRESULT SetPrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid refguid, IntPtr pData, uint SizeOfData, uint Flags);

            [PreserveSig]
            new HRESULT GetPrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid refguid, IntPtr pData, ref uint pSizeOfData);

            [PreserveSig]
            new HRESULT FreePrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid refguid);

            [PreserveSig]
            new uint SetPriority(uint PriorityNew);

            [PreserveSig]
            new uint GetPriority();

            [PreserveSig]
            new void PreLoad();

            [PreserveSig]
            new _D3DRESOURCETYPE GetType();

            [PreserveSig]
            new uint SetLOD(uint LODNew);

            [PreserveSig]
            new uint GetLOD();

            [PreserveSig]
            new uint GetLevelCount();

            [PreserveSig]
            new HRESULT SetAutoGenFilterType(_D3DTEXTUREFILTERTYPE FilterType);

            [PreserveSig]
            new _D3DTEXTUREFILTERTYPE GetAutoGenFilterType();

            [PreserveSig]
            new void GenerateMipSubLevels();

            [PreserveSig]
            HRESULT GetLevelDesc(uint Level, ref _D3DSURFACE_DESC pDesc);

            [PreserveSig]
            HRESULT GetSurfaceLevel(uint Level, out IDirect3DSurface9 ppSurfaceLevel);

            [PreserveSig]
            HRESULT LockRect(uint Level, IntPtr pLockedRect, IntPtr pRect, uint Flags);

            [PreserveSig]
            HRESULT UnlockRect(uint Level);

            [PreserveSig]
            HRESULT AddDirtyRect(ref tagRECT pDirtyRect);
        }

        [ComImport]
        [Guid("580ca87e-1d3c-4d54-991d-b7d3e3c298ce")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDirect3DBaseTexture9 : IDirect3DResource9
        {
            [PreserveSig]
            new HRESULT GetDevice(out IDirect3DDevice9 ppDevice);

            [PreserveSig]
            new HRESULT SetPrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid refguid, IntPtr pData, uint SizeOfData, uint Flags);

            [PreserveSig]
            new HRESULT GetPrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid refguid, IntPtr pData, ref uint pSizeOfData);

            [PreserveSig]
            new HRESULT FreePrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid refguid);

            [PreserveSig]
            new uint SetPriority(uint PriorityNew);

            [PreserveSig]
            new uint GetPriority();

            [PreserveSig]
            new void PreLoad();

            [PreserveSig]
            new _D3DRESOURCETYPE GetType();

            [PreserveSig]
            uint SetLOD(uint LODNew);

            [PreserveSig]
            uint GetLOD();

            [PreserveSig]
            uint GetLevelCount();

            [PreserveSig]
            HRESULT SetAutoGenFilterType(_D3DTEXTUREFILTERTYPE FilterType);

            [PreserveSig]
            _D3DTEXTUREFILTERTYPE GetAutoGenFilterType();

            [PreserveSig]
            void GenerateMipSubLevels();
        }


        [ComImport]
        [Guid("05eec05d-8f7d-4362-b999-d1baf357c704")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDirect3DResource9
        {
            [PreserveSig]
            HRESULT GetDevice(out IDirect3DDevice9 ppDevice);

            [PreserveSig]
            HRESULT SetPrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid refguid, IntPtr pData, uint SizeOfData, uint Flags);

            [PreserveSig]
            HRESULT GetPrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid refguid, IntPtr pData, ref uint pSizeOfData);

            [PreserveSig]
            HRESULT FreePrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid refguid);

            [PreserveSig]
            uint SetPriority(uint PriorityNew);

            [PreserveSig]
            uint GetPriority();

            [PreserveSig]
            void PreLoad();

            [PreserveSig]
            new _D3DRESOURCETYPE GetType();
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
