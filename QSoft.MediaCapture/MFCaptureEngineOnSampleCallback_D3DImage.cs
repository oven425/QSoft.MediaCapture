using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace QSoft.MediaCapture
{
    internal class MFCaptureEngineOnSampleCallback_D3DImage:MFCaptureEngineOnSampleCallback
    {
        public MFCaptureEngineOnSampleCallback_D3DImage(D3DImage img)
        {
            var d3d9 = DirectN.Functions.Direct3DCreate9(DirectN.Constants.D3D9b_SDK_VERSION);
            _D3DDISPLAYMODE mode = new _D3DDISPLAYMODE();
            d3d9.GetAdapterDisplayMode(DirectN.Constants.D3DADAPTER_DEFAULT, ref mode);

            d3d9.CreateDevice(DirectN.Constants.D3DADAPTER_DEFAULT, _D3DDEVTYPE.D3DDEVTYPE_HAL, IntPtr.Zero, 0, IntPtr.Zero, out var d3de);
        }

        protected override void OnSample(IntPtr data, uint len)
        {
        }
    }
}
