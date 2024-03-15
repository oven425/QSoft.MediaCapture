using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QSoft.MediaCapture.WPF
{
    public static partial class WebCam_MF_Extension
    {
        public static void StartPreview(this WebCam_MF src, Action<WriteableBitmap> action)
        {
            var sz = src.PreviewSize;
            var bmp = new WriteableBitmap(sz.width,sz.height, 96,96,PixelFormats.Bgr24, null);
            //src.StartPreviewToCustomSinkAsync(new MFCaptureEngineOnSampleCallback(bmp));
            action(bmp);
        }
    }
    public class MFCaptureEngineOnSampleCallback : IMFCaptureEngineOnSampleCallback
    {
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
        WriteableBitmap m_Bmp;
        public MFCaptureEngineOnSampleCallback(WriteableBitmap data)
        {
            this.m_Bmp = data;
        }
        int samplecount = 0;
        DateTime time = DateTime.Now;
        byte[] m_Buffer;
        object m_Lock = new object();
        public HRESULT OnSample(IMFSample pSample)
        {
            if (System.Threading.Monitor.TryEnter(this.m_Lock))
            {
                samplecount++;
                if (samplecount > 100)
                {
                    var ts = DateTime.Now - time;
                    var fps = samplecount / ts.TotalSeconds;
                    samplecount = 0;
                    time = DateTime.Now;
                    System.Diagnostics.Trace.WriteLine($"fps:{fps}");
                }
                pSample.GetBufferByIndex(0, out var buf);
                var ptr = buf.Lock(out var max, out var cur);
                //m_Buffer = new byte[cur];
                //Marshal.Copy(ptr, m_Buffer, 0, m_Buffer.Length);
                m_Bmp.Dispatcher.Invoke(() =>
                {
                    m_Bmp.Lock();
                    CopyMemory(m_Bmp.BackBuffer, ptr, cur);
                    //Marshal.Copy(this.m_Buffer, 0, m_Bmp.BackBuffer, this.m_Buffer.Length);
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
