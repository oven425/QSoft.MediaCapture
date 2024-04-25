using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace QSoft.MediaCapture.WPF
{
    public static class WebCam_MFExtension_WPF
    {
        public static async Task<HRESULT> StartPreview(this QSoft.MediaCapture.WebCam_MF src, Action<WriteableBitmap?> action)
        {
            src.GetPreviewSize(out var width, out var height);
            WriteableBitmap? bmp = null;
            var dispatcher = Dispatcher.FromThread(System.Threading.Thread.CurrentThread);
            if(dispatcher != null)
            {
                bmp = new WriteableBitmap((int)width, (int)height, 96, 96, PixelFormats.Bgr24, null);
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    bmp = new WriteableBitmap((int)width, (int)height, 96, 96, PixelFormats.Bgr24, null);

                });
            }
            
            var hr = await src.StartPreview(new MFCaptureEngineOnSampleCallback(bmp));
            action?.Invoke(bmp);
            return hr;
        }
    }

    public partial class MFCaptureEngineOnSampleCallback : IMFCaptureEngineOnSampleCallback
    {
#if NET8_0_OR_GREATER
        [LibraryImport("kernel32.dll", EntryPoint = "RtlCopyMemory", SetLastError = false)]
        internal static partial void CopyMemory(IntPtr dest, IntPtr src, uint count);

#else
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        internal static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
#endif

        readonly WriteableBitmap? m_Bmp;
        public MFCaptureEngineOnSampleCallback(WriteableBitmap? data)
        {
            this.m_Bmp = data;
        }
#if DEBUG
        int samplecount = 0;
        System.Diagnostics.Stopwatch? m_StopWatch;
#endif
        readonly object m_Lock = new object();
        
        public HRESULT OnSample(IMFSample pSample)
        {
            if (this.m_Bmp != null && System.Threading.Monitor.TryEnter(this.m_Lock))
            {
#if DEBUG
                if (samplecount == 0)
                {
                    m_StopWatch = System.Diagnostics.Stopwatch.StartNew();
                }
                samplecount++;
                if (samplecount > 100 && m_StopWatch != null)
                {
                    m_StopWatch.Stop();
                    var fps = samplecount / m_StopWatch.Elapsed.TotalSeconds;
                    System.Diagnostics.Trace.WriteLine($"fps:{fps}");
                    samplecount = 0;
                }
#endif
                pSample.GetBufferByIndex(0, out var buf);
                var ptr = buf.Lock(out var max, out var cur);
                
                m_Bmp.Dispatcher.Invoke(() =>
                {
                    m_Bmp.Lock();
                    CopyMemory(m_Bmp.BackBuffer, ptr, cur);
                    m_Bmp.AddDirtyRect(new System.Windows.Int32Rect(0, 0, m_Bmp.PixelWidth, m_Bmp.PixelHeight));
                    m_Bmp.Unlock();
                }, System.Windows.Threading.DispatcherPriority.Background);

                buf.Unlock();
                Marshal.ReleaseComObject(buf);
                Marshal.ReleaseComObject(pSample);
                System.Threading.Monitor.Exit(this.m_Lock);
            }
            else
            {
                Marshal.ReleaseComObject(pSample);
            }

            return HRESULTS.S_OK;
        }
    }
}
