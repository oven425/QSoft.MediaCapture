using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace QSoft.MediaCapture.WPF
{
    public static class WebCam_MFExtension_WPF
    {
        //public static async Task<HRESULT> StartPreivew(this QSoft.MediaCapture.WebCam_MF src, Action<ImageSource> action)
        //{
        //    return HRESULTS.S_OK;
        //}

        //static void CreateD3DImage(int width, int height, DispatcherPriority dispatcherpriority, out D3DImage d3dimage, out MFCaptureEngineOnSampleCallback_D3DImage callback)
        //{
        //    d3dimage = new D3DImage();
        //    callback = new MFCaptureEngineOnSampleCallback_D3DImage(d3dimage, dispatcherpriority);
        //    callback.Init(width, height);
        //    d3dimage.Lock();
        //    var ptr = Marshal.GetIUnknownForObject(callback.BackBuffer);
        //    d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, ptr);
        //    d3dimage.Unlock();
        //}

        //public static async Task<HRESULT> StartPreview(this QSoft.MediaCapture.WebCam_MF src, Action<D3DImage?> action, DispatcherPriority dispatcherpriority = DispatcherPriority.Background)
        //{
        //    src.GetPreviewSize(out var width, out var height);
        //    var dispatcher = Dispatcher.FromThread(System.Threading.Thread.CurrentThread);
        //    D3DImage? d3dimage = null;
        //    MFCaptureEngineOnSampleCallback_D3DImage? callback = null;
        //    if (dispatcher != null)
        //    {
        //        CreateD3DImage((int)width, (int)height, dispatcherpriority, out d3dimage, out callback);
        //    }
        //    else
        //    {
        //        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        //        {
        //            CreateD3DImage((int)width, (int)height, dispatcherpriority, out d3dimage, out callback);
        //        });
        //    }

        //    var hr = await src.StartPreview(null, callback);
        //    action?.Invoke(d3dimage);

        //    return hr;
        //}

        //public static async Task<HRESULT> StartPreview2(this QSoft.MediaCapture.WebCam_MF src, Action<WriteableBitmap?> action, System.Windows.Threading.DispatcherPriority dispatcherpriority = DispatcherPriority.Background)
        //{
        //    var enc = src.GetMediaStreamProperties(MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_VIDEO_CAPTURE);
        //    WriteableBitmap? bmp = null;
        //    var dispatcher = Dispatcher.FromThread(System.Threading.Thread.CurrentThread);
        //    if (dispatcher != null)
        //    {
        //        bmp = new WriteableBitmap((int)enc.Width, (int)enc.Height, 96, 96, PixelFormats.Bgr24, null);
        //    }
        //    else
        //    {
        //        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        //        {
        //            bmp = new WriteableBitmap((int)enc.Width, (int)enc.Height, 96, 96, PixelFormats.Bgr24, null);
        //        });
        //    }
        //    var hr = await src.StartPreview(new MFCaptureEngineOnSampleCallback2_WriteableBitmap(bmp, dispatcherpriority));
        //    action?.Invoke(bmp);
        //    return hr;
        //}


        public static async Task<HRESULT> StartPreview(this QSoft.MediaCapture.WebCam_MF src, Action<WriteableBitmap?> action, System.Windows.Threading.DispatcherPriority dispatcherpriority = DispatcherPriority.Background)
        {
            var enc = src.GetMediaStreamProperties(MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_VIDEO_CAPTURE, 0);
            WriteableBitmap? bmp = null;
            var dispatcher = Dispatcher.FromThread(System.Threading.Thread.CurrentThread);
            if(dispatcher != null)
            {
                bmp = new WriteableBitmap((int)enc.Width, (int)enc.Height, 96, 96, PixelFormats.Bgr24, null);
            }
            else
            {
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    bmp = new WriteableBitmap((int)enc.Width, (int)enc.Height, 96, 96, PixelFormats.Bgr24, null);
                });
            }
            var hr = await src.StartPreview(new MFCaptureEngineOnSampleCallback_WriteableBitmap(bmp, dispatcherpriority));
            action?.Invoke(bmp);
            return hr;
        }


        public static async Task<HRESULT> StartPreview(this QSoft.MediaCapture.WebCam_MF src, Func<Image> action, uint streamindex = 0, System.Windows.Threading.DispatcherPriority dispatcherpriority = DispatcherPriority.Background)
        {
            var enc = src.GetMediaStreamProperties(MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_VIDEO_CAPTURE, streamindex);
            WriteableBitmap? bmp = null;
            var dispatcher = Dispatcher.FromThread(System.Threading.Thread.CurrentThread);
            if (dispatcher != null)
            {
                bmp = new WriteableBitmap((int)enc.Width, (int)enc.Height, 96, 96, PixelFormats.Bgra32, null);
            }
            else
            {
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    bmp = new WriteableBitmap((int)enc.Width, (int)enc.Height, 96, 96, PixelFormats.Bgra32, null);
                });
            }
            var mfff = new MFCaptureEngineOnSampleCallback_WriteableBitmap(bmp, dispatcherpriority);
            mfff.TranseRaw = src;
            mfff.Parent = src;
            var hr = await src.StartPreview(mfff);
            if(action?.Invoke() is Image image)
            {
                image.Source = bmp;
                image.LayoutTransform = new RotateTransform((int)src.Setting.Rotate);
            }
            
            return hr;
        }
    }

}
