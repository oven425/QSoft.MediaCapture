using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{
    //video_capture_dxgi_device_manager.cc
    //https://chromium.googlesource.com/chromium/src/+/4a5565fb749ca181fcf0b28b7ac6092562b1cf94/media/capture/video/win/video_capture_dxgi_device_manager.cc
    public partial class WebCam_MF
    {
        uint g_ResetToken = 0;
        IMFDXGIDeviceManager? g_pDXGIMan;
        HRESULT CreateD3DManager()
        {
            var hr = CreateDX11Device(out g_pDX11Device, out var pDX11DeviceContext, out var FeatureLevel);

            if (hr == HRESULTS.S_OK)
            {
                hr = DirectN.MFFunctions.MFCreateDXGIDeviceManager(out g_ResetToken, out g_pDXGIMan);
            }

            if (hr == HRESULTS.S_OK && g_pDXGIMan != null)
            {
                hr = g_pDXGIMan.ResetDevice(g_pDX11Device, g_ResetToken);
            }
            SafeRelease(pDX11DeviceContext);

            return hr;
        }

        ID3D11Device? g_pDX11Device;
        HRESULT CreateDX11Device(out ID3D11Device ppDevice, out ID3D11DeviceContext ppDeviceContext, out D3D_FEATURE_LEVEL pFeatureLevel)
        {
            D3D_FEATURE_LEVEL[] levels = new[]{
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_1,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_0,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_3,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_2,
                D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_1
            };

            //var hr = DirectN.D3D11Functions.D3D11CreateDevice(
            //    null,
            //    D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE,
            //    IntPtr.Zero,
            //    (uint)DirectN.D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_VIDEO_SUPPORT,
            //    levels,
            //    (uint)levels.Length,
            //    7,
            //    out ppDevice,
            //    out pFeatureLevel,
            //    out ppDeviceContext
            //    );


            var device_flags =
      (DirectN.D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_VIDEO_SUPPORT | DirectN.D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_BGRA_SUPPORT);
            var hr = DirectN.D3D11Functions.D3D11CreateDevice(
                null,
                D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE,
                IntPtr.Zero,
                (uint)device_flags,
                null,
                0,
                7,
                out ppDevice,
                out pFeatureLevel,
                out ppDeviceContext
                );

            if (hr == HRESULTS.S_OK)
            {
                ID3D10Multithread? pMultithread;
                pMultithread = ppDevice as ID3D10Multithread;
                if (hr == HRESULTS.S_OK)
                {
                    pMultithread?.SetMultithreadProtected(true);
                }
            }

            return hr;
        }

    }
}
