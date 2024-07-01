using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Server;

namespace QSoft.MediaCapture.WPF
{
    internal partial class MFCaptureEngineOnSampleCallback_D3DImage:MFCaptureEngineOnSampleCallback
    {
        D3DImage m_D3dImage;
        readonly System.Windows.Threading.DispatcherPriority m_DispatcherPriority;
        public MFCaptureEngineOnSampleCallback_D3DImage(D3DImage img, System.Windows.Threading.DispatcherPriority dispatcherpriority)
        {
            this.m_D3dImage = img;
            m_DispatcherPriority = dispatcherpriority;
        }

        protected override void OnSample(IntPtr data, uint len)
        {
            
            this.m_D3dImage.Dispatcher.Invoke(() =>
            {
                this.WriteFrame(data, D3DFMT_NV12);
                this.m_D3dImage.Lock();

                this.m_D3dImage.AddDirtyRect(new Int32Rect(0,0, this.m_Width, this.m_Height));
                this.m_D3dImage.Unlock();
            }, m_DispatcherPriority);
        }
    }

    internal partial class MFCaptureEngineOnSampleCallback_D3DImage
    {
        public IDirect3DSurface9 BackBuffer => m_pBackBuffer;
        IDirect3DSurface9 m_pBackBuffer;
        IDirect3DSurface9 m_pd3dSurface;
        IDirect3D9 m_pD3D;
        IDirect3DDevice9 m_pd3dDevice;
        int m_Width;
        int m_Height;
        public void Init(int width, int height)
        {

            this.m_Width = width;
            m_Height = height;
            m_tagRECT = new tagRECT(0, 0, this.m_Width, this.m_Height);

            EnsureHWND();
            m_pD3D = DirectN.Functions.Direct3DCreate9(DirectN.Constants.D3D9b_SDK_VERSION);

            DirectN._D3DDISPLAYMODE d3dDisplayMode = new DirectN._D3DDISPLAYMODE();
            var hr = m_pD3D.GetAdapterDisplayMode(DirectN.Constants.D3DADAPTER_DEFAULT, ref d3dDisplayMode);


            DirectN._D3DCAPS9 d3dcaps = new _D3DCAPS9();
            hr = m_pD3D.GetDeviceCaps(DirectN.Constants.D3DADAPTER_DEFAULT, DirectN._D3DDEVTYPE.D3DDEVTYPE_HAL, ref d3dcaps);
            var parameters = new DirectN._D3DPRESENT_PARAMETERS_64();
            parameters.Windowed = true;
            parameters.BackBufferCount = 1;
            parameters.BackBufferHeight = (uint)height;
            parameters.BackBufferWidth = (uint)width;
            parameters.SwapEffect = _D3DSWAPEFFECT.D3DSWAPEFFECT_DISCARD;
            parameters.BackBufferFormat = d3dDisplayMode.Format;


            hr = m_pD3D.CreateDevice(
            DirectN.Constants.D3DADAPTER_DEFAULT,
            DirectN._D3DDEVTYPE.D3DDEVTYPE_HAL,
            m_hwnd,
            DirectN.Constants.D3DCREATE_HARDWARE_VERTEXPROCESSING | DirectN.Constants.D3DCREATE_MULTITHREADED | DirectN.Constants.D3DCREATE_FPU_PRESERVE,
            parameters.StructureToPtr(),
            out m_pd3dDevice);
            //842094158

            //_D3DFORMAT nv12 = (_D3DFORMAT)842094158;
            //_D3DFORMAT yv12 = (_D3DFORMAT)842094169;
            hr = m_pd3dDevice.CreateOffscreenPlainSurface((uint)width, (uint)height, D3DFMT_NV12, _D3DPOOL.D3DPOOL_DEFAULT, out m_pd3dSurface, IntPtr.Zero);

            hr = m_pd3dDevice.GetBackBuffer(0, 0, _D3DBACKBUFFER_TYPE.D3DBACKBUFFER_TYPE_MONO, out this.m_pBackBuffer);
            hr = 0;

        }
#if NET8_0_OR_GREATER
        [LibraryImport("kernel32.dll", EntryPoint = "RtlCopyMemory", SetLastError = false)]
        internal static partial void CopyMemory(IntPtr dest, IntPtr src, uint count);

#else
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        internal static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
#endif

        const _D3DFORMAT D3DFMT_NV12 = (_D3DFORMAT)842094158;
        const _D3DFORMAT D3DFMT_YV12 = (_D3DFORMAT)842094169;
        public void WriteFrame(IntPtr data, _D3DFORMAT format)
        {
            DirectN._D3DLOCKED_RECT_64 d3d_rect = new _D3DLOCKED_RECT_64();
            var hr = this.m_pd3dSurface.LockRect(ref d3d_rect, IntPtr.Zero, DirectN.Constants.D3DLOCK_DISCARD);

            var w = m_Width;
            var h = m_Height;
            var stride = d3d_rect.Pitch;


            if (format == _D3DFORMAT.D3DFMT_YUY2)
            {
                for (int i = 0; i < h; i++)
                {
                    CopyMemory(d3d_rect.pBits + i * stride, data + i * w, (uint)w);
                }
            }
            else if (format == D3DFMT_NV12)
            {
                for (int i = 0; i < h; i++)
                {
                    CopyMemory(d3d_rect.pBits + i * stride, data + i * w, (uint)w);
                }
                var offset = h * stride;
                var offset1 = w * h;
                for (int i = 0; i < h / 2; i++)
                {
                    CopyMemory(d3d_rect.pBits + offset + i * stride, data + offset1 + i * w, (uint)w);
                }
            }
            else if (format == D3DFMT_YV12)
            {
                for (int i = 0; i < h; i++)
                {
                    CopyMemory(d3d_rect.pBits + i * stride, data + i * w, (uint)w);
                }


                for (int i = 0; i < h / 2; i++)
                {
                    CopyMemory(d3d_rect.pBits + stride * h + i * stride / 2, data + w * h + w * h / 4 + i * w / 2, (uint)w / 2);
                }

                for (int i = 0; i < h / 2; i++)
                {
                    CopyMemory(d3d_rect.pBits + stride * h + stride * h / 4 + i * stride / 2, data + w * h + i * w / 2, (uint)w / 2);
                }
            }

            this.m_pd3dSurface.UnlockRect();
            this.Render();
        }

        tagRECT m_tagRECT;
        void Render()
        {
            if (m_pd3dDevice != null)
            {
                //m_pd3dDevice.Clear(0, null, DirectN.Constants.D3DCLEAR_TARGET, D3DCOLOR_XRGB(0, 0, 0), 1.0f, 0);
                if (m_pd3dDevice.BeginScene() == HRESULTS.S_OK)
                {

                    var hr1 = m_pd3dDevice.StretchRect(m_pd3dSurface/*NULL*/, m_tagRECT, m_pBackBuffer, m_tagRECT, _D3DTEXTUREFILTERTYPE.D3DTEXF_LINEAR);
                    hr1 = m_pd3dDevice.EndScene();
                }
                m_pd3dDevice.Present(new tagRECT(0, 0, 0, 0), new tagRECT(0, 0, 0, 0), IntPtr.Zero, new _RGNDATA());
            }
        }

        string szAppName = "D3DImageSample";
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr DefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U2)]
        static extern short RegisterClass([In] ref WNDCLASS lpwcx);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr CreateWindowEx(
  uint dwExStyle,
  [MarshalAs(UnmanagedType.LPWStr)]
  string lpClassName,
  [MarshalAs(UnmanagedType.LPWStr)]
  string lpWindowName,
  uint dwStyle,
  int X,
  int Y,
  int nWidth,
  int nHeight,
  IntPtr hWndParent,
  IntPtr hMenu,
  IntPtr hInstance,
  IntPtr lpParam
);
        IntPtr m_hwnd = IntPtr.Zero;
        void EnsureHWND()
        {
            //HRESULT hr = S_OK;

            if (m_hwnd == IntPtr.Zero)
            {
                WNDCLASS wndclass;

                wndclass.style = 3;
                wndclass.lpfnWndProc = DefWindowProc;
                wndclass.cbClsExtra = 0;
                wndclass.cbWndExtra = 0;
                wndclass.hInstance = IntPtr.Zero;
                wndclass.hIcon = IntPtr.Zero;
                wndclass.hCursor = IntPtr.Zero;
                wndclass.hbrBackground = IntPtr.Zero;
                wndclass.lpszMenuName = null;
                wndclass.lpszClassName = szAppName;

                var gg = RegisterClass(ref wndclass);
                var err = Marshal.GetLastWin32Error();

                m_hwnd = CreateWindowEx(0, wndclass.lpszClassName,
                    "D3DImageSample",
                    0x00cf0000,
                    0,                   // Initial X
                    0,                   // Initial Y
                    0,                   // Width
                    0,                   // Height
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero);
                err = Marshal.GetLastWin32Error();

                err = 0;
            }

            //Cleanup:
            //    return hr;
        }

    }
    public partial class YUVRender
    {
        public IDirect3DSurface9 BackBuffer => m_pBackBuffer;
        IDirect3DSurface9 m_pBackBuffer;
        IDirect3DSurface9 m_pd3dSurface;
        IDirect3D9 m_pD3D;
        IDirect3DDevice9 m_pd3dDevice;
        int m_Width;
        int m_Height;
        public void Init(int width, int height)
        {

            this.m_Width = width;
            m_Height = height;
            EnsureHWND();
            m_pD3D = DirectN.Functions.Direct3DCreate9(DirectN.Constants.D3D9b_SDK_VERSION);

            DirectN._D3DDISPLAYMODE d3dDisplayMode = new DirectN._D3DDISPLAYMODE();
            var hr = m_pD3D.GetAdapterDisplayMode(DirectN.Constants.D3DADAPTER_DEFAULT, ref d3dDisplayMode);


            DirectN._D3DCAPS9 d3dcaps = new _D3DCAPS9();
            hr = m_pD3D.GetDeviceCaps(DirectN.Constants.D3DADAPTER_DEFAULT, DirectN._D3DDEVTYPE.D3DDEVTYPE_HAL, ref d3dcaps);
            var parameters = new DirectN._D3DPRESENT_PARAMETERS_64();
            parameters.Windowed = true;
            parameters.BackBufferCount = 1;
            parameters.BackBufferHeight = (uint)height;
            parameters.BackBufferWidth = (uint)width;
            parameters.SwapEffect = _D3DSWAPEFFECT.D3DSWAPEFFECT_DISCARD;

            hr = m_pD3D.CreateDevice(
            DirectN.Constants.D3DADAPTER_DEFAULT,
            DirectN._D3DDEVTYPE.D3DDEVTYPE_HAL,
            m_hwnd,
            DirectN.Constants.D3DCREATE_HARDWARE_VERTEXPROCESSING | DirectN.Constants.D3DCREATE_MULTITHREADED | DirectN.Constants.D3DCREATE_FPU_PRESERVE,
            parameters.StructureToPtr(),
            out m_pd3dDevice);
            //842094158

            _D3DFORMAT nv12 = (_D3DFORMAT)842094158;
            _D3DFORMAT yv12 = (_D3DFORMAT)842094169;
            hr = m_pd3dDevice.CreateOffscreenPlainSurface((uint)width, (uint)height, D3DFMT_NV12, _D3DPOOL.D3DPOOL_DEFAULT, out m_pd3dSurface, IntPtr.Zero);

            hr = m_pd3dDevice.GetBackBuffer(0, 0, _D3DBACKBUFFER_TYPE.D3DBACKBUFFER_TYPE_MONO, out this.m_pBackBuffer);
            hr = 0;

        }
#if NET8_0_OR_GREATER
        [LibraryImport("kernel32.dll", EntryPoint = "RtlCopyMemory", SetLastError = false)]
        internal static partial void CopyMemory(IntPtr dest, IntPtr src, uint count);

#else
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        internal static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
#endif

        public const _D3DFORMAT D3DFMT_NV12 = (_D3DFORMAT)842094158;
        public const _D3DFORMAT D3DFMT_YV12 = (_D3DFORMAT)842094169;
        public void WriteFrame(IntPtr data, _D3DFORMAT format)
        {
            DirectN._D3DLOCKED_RECT_64 d3d_rect = new _D3DLOCKED_RECT_64();
            var hr = this.m_pd3dSurface.LockRect(ref d3d_rect, IntPtr.Zero, DirectN.Constants.D3DLOCK_DISCARD);

            var w = m_Width;
            var h = m_Height;
            var stride = d3d_rect.Pitch;


            if (format == _D3DFORMAT.D3DFMT_YUY2)
            {
                for (int i = 0; i < h; i++)
                {
                    CopyMemory(d3d_rect.pBits + i * stride, data + i * w, (uint)w);
                }
            }
            else if (format == D3DFMT_NV12)
            {
                for (int i = 0; i < h; i++)
                {
                    CopyMemory(d3d_rect.pBits + i * stride, data + i * w, (uint)w);
                }
                var offset = h * stride;
                var offset1 = w * h;
                for (int i = 0; i < h / 2; i++)
                {
                    CopyMemory(d3d_rect.pBits + offset + i * stride, data + offset1 + i * w, (uint)w);
                }
            }
            else if (format == D3DFMT_YV12)
            {
                for (int i = 0; i < h; i++)
                {
                    CopyMemory(d3d_rect.pBits + i * stride, data + i * w, (uint)w);
                }


                for (int i = 0; i < h / 2; i++)
                {
                    CopyMemory(d3d_rect.pBits + stride * h + i * stride / 2, data + w * h + w * h / 4 + i * w / 2, (uint)w / 2);
                }

                for (int i = 0; i < h / 2; i++)
                {
                    CopyMemory(d3d_rect.pBits + stride * h + stride * h / 4 + i * stride / 2, data + w * h + i * w / 2, (uint)w / 2);
                }
            }

            this.m_pd3dSurface.UnlockRect();
            this.Render();
        }

        public void YV12(IntPtr data)
        {
            DirectN._D3DLOCKED_RECT_64 d3d_rect = new _D3DLOCKED_RECT_64();
            var hr = this.m_pd3dSurface.LockRect(ref d3d_rect, IntPtr.Zero, DirectN.Constants.D3DLOCK_DISCARD);

            var w = m_Width;
            var h = m_Height;
            var stride = d3d_rect.Pitch;

            int i = 0;
            for (i = 0; i < h; i++)
            {
                CopyMemory(d3d_rect.pBits + i * stride, data + i * w, (uint)w);
            }


            for (i = 0; i < h / 2; i++)
            {
                CopyMemory(d3d_rect.pBits + stride * h + i * stride / 2, data + w * h + w * h / 4 + i * w / 2, (uint)w / 2);
            }

            for (i = 0; i < h / 2; i++)
            {
                CopyMemory(d3d_rect.pBits + stride * h + stride * h / 4 + i * stride / 2, data + w * h + i * w / 2, (uint)w / 2);
            }

            this.m_pd3dSurface.UnlockRect();
            this.Render();
        }

        public void YUY2(IntPtr data)
        {
            DirectN._D3DLOCKED_RECT_64 d3d_rect = new _D3DLOCKED_RECT_64();
            var hr = this.m_pd3dSurface.LockRect(ref d3d_rect, IntPtr.Zero, DirectN.Constants.D3DLOCK_DISCARD);

            var w = m_Width;
            var h = m_Height;
            var stride = d3d_rect.Pitch;

            for (var i = 0; i < h; i++)
            {
                CopyMemory(d3d_rect.pBits + i * stride, data + i * w * 2, (uint)(w * 2));
            }

            this.m_pd3dSurface.UnlockRect();
            this.Render();
        }

        public void NV12(IntPtr data)
        {
            DirectN._D3DLOCKED_RECT_64 d3d_rect = new _D3DLOCKED_RECT_64();
            var hr = this.m_pd3dSurface.LockRect(ref d3d_rect, IntPtr.Zero, DirectN.Constants.D3DLOCK_DISCARD);

            var w = m_Width;
            var h = m_Height;
            var stride = d3d_rect.Pitch;

            int i = 0;
            for (i = 0; i < h; i++)
            {
                CopyMemory(d3d_rect.pBits + i * stride, data + i * w, (uint)w);
            }
            var offset = h * stride;
            var offset1 = w * h;
            for (i = 0; i < h / 2; i++)
            {
                CopyMemory(d3d_rect.pBits + offset + i * stride, data + offset1 + i * w, (uint)w);
            }

            this.m_pd3dSurface.UnlockRect();
            this.Render();
        }

        void Render()
        {
            if (m_pd3dDevice != null)
            {
                //m_pd3dDevice.Clear(0, null, DirectN.Constants.D3DCLEAR_TARGET, D3DCOLOR_XRGB(0, 0, 0), 1.0f, 0);
                if (m_pd3dDevice.BeginScene() == HRESULTS.S_OK)
                {

                    var hr1 = m_pd3dDevice.StretchRect(m_pd3dSurface/*NULL*/, new tagRECT(0, 0, 720, 404), m_pBackBuffer, new tagRECT(0, 0, 720, 404), _D3DTEXTUREFILTERTYPE.D3DTEXF_LINEAR);
                    hr1 = m_pd3dDevice.EndScene();
                }
                var hr = m_pd3dDevice.Present(new tagRECT(0, 0, 0, 0), new tagRECT(0, 0, 0, 0), IntPtr.Zero, new _RGNDATA());
                hr = 0;
            }
        }

        string szAppName = "D3DImageSample";
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr DefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U2)]
        static extern short RegisterClass([In] ref WNDCLASS lpwcx);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr CreateWindowEx(
  uint dwExStyle,
  [MarshalAs(UnmanagedType.LPWStr)]
  string lpClassName,
  [MarshalAs(UnmanagedType.LPWStr)]
  string lpWindowName,
  uint dwStyle,
  int X,
  int Y,
  int nWidth,
  int nHeight,
  IntPtr hWndParent,
  IntPtr hMenu,
  IntPtr hInstance,
  IntPtr lpParam
);
        IntPtr m_hwnd = IntPtr.Zero;
        void EnsureHWND()
        {
            //HRESULT hr = S_OK;

            if (m_hwnd == IntPtr.Zero)
            {
                WNDCLASS wndclass;

                wndclass.style = 3;
                wndclass.lpfnWndProc = DefWindowProc;
                wndclass.cbClsExtra = 0;
                wndclass.cbWndExtra = 0;
                wndclass.hInstance = IntPtr.Zero;
                wndclass.hIcon = IntPtr.Zero;
                wndclass.hCursor = IntPtr.Zero;
                wndclass.hbrBackground = IntPtr.Zero;
                wndclass.lpszMenuName = null;
                wndclass.lpszClassName = szAppName;

                var gg = RegisterClass(ref wndclass);
                var err = Marshal.GetLastWin32Error();

                m_hwnd = CreateWindowEx(0, wndclass.lpszClassName,
                    "D3DImageSample",
                    0x00cf0000,
                    0,                   // Initial X
                    0,                   // Initial Y
                    0,                   // Width
                    0,                   // Height
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero);
                err = Marshal.GetLastWin32Error();

                err = 0;
            }

            //Cleanup:
            //    return hr;
        }


    }
}
