using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace QSoft.MediaCapture
{
    public partial class MFCaptureEngineOnSampleCallback2: IMFCaptureEngineOnSampleCallback2
    {
#if NET8_0_OR_GREATER
        [LibraryImport("kernel32.dll", EntryPoint = "RtlCopyMemory", SetLastError = false)]
        internal static partial void CopyMemory(IntPtr dest, IntPtr src, uint count);

#else
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        internal static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
#endif


#if DEBUG
        int samplecount = 0;
        System.Diagnostics.Stopwatch m_StopWatch = new();
#endif
        readonly object m_Lock = new();

        public HRESULT OnSample(IMFSample pSample)
        {
            if (System.Threading.Monitor.TryEnter(this.m_Lock))
            {
#if DEBUG

                //var attrs = pSample.GetUnknown<IMFAttributes>(DirectN.MFConstants.MFSampleExtension_CaptureMetadata);
                //System.Diagnostics.Trace.WriteLine($"attrs: {attrs.Count()}");
                //try
                //{
                //    var buf11 = attrs.Object.GetBlobSize(DirectN.MFConstants.MF_CAPTURE_METADATA_FACEROIS, out var len);
                //    //Infrared
                //    System.Diagnostics.Trace.WriteLine($"GetBlobSize: {buf11} {len}");
                //    var face_ptr = Marshal.AllocHGlobal((int)len);
                //    var header = Marshal.PtrToStructure<tagFaceRectInfoBlobHeader>(face_ptr);
                //    System.Diagnostics.Trace.WriteLine($"header: Size:{header.Size} Count:{header.Count}");
                //    if(len == 28)
                //    {
                //        var faceinfo = Marshal.PtrToStructure<tagFaceRectInfo>(IntPtr.Add(face_ptr, 8));
                //        System.Diagnostics.Trace.WriteLine($"faceinfo: Size:{faceinfo.Region} confidenceLevel:{faceinfo.confidenceLevel}");
                //    }


                //    Marshal.FreeHGlobal( face_ptr );

                //}
                //catch (Exception ee)
                //{
                //    System.Diagnostics.Trace.WriteLine(ee.Message);
                //}
                //attrs?.Dispose();
                if (samplecount == 0)
                {
                    m_StopWatch.Restart();
                    //m_StopWatch = System.Diagnostics.Stopwatch.StartNew();
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
                pSample.ConvertToContiguousBuffer(out var buf);
                var ptr = buf.Lock(out var max, out var cur);

                OnSample(ptr, cur);

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

        virtual protected void OnSample(IntPtr data, uint len) { }


        public HRESULT OnSynchronizedEvent(IMFMediaEvent pEvent)
        {
            return HRESULTS.S_OK;
        }

    }
}

namespace QSoft.MediaCapture.WPF
{
    internal partial class MFCaptureEngineOnSampleCallback2_WriteableBitmap : MFCaptureEngineOnSampleCallback2
    {
        readonly WriteableBitmap? m_Bmp;
        readonly System.Windows.Threading.DispatcherPriority m_DispatcherPriority;
        public MFCaptureEngineOnSampleCallback2_WriteableBitmap(WriteableBitmap? data, System.Windows.Threading.DispatcherPriority dispatcherpriority)
        {
            m_DispatcherPriority = dispatcherpriority;
            this.m_Bmp = data;
        }
#if NET8_0_OR_GREATER
        [LibraryImport("kernel32.dll", EntryPoint = "RtlCopyMemory", SetLastError = false)]
        internal static partial void CopyMemory(IntPtr dest, IntPtr src, uint count);

#else
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        internal static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
#endif

        protected override void OnSample(nint data, uint len)
        {
            m_Bmp?.Dispatcher.Invoke(() =>
            {
                m_Bmp.Lock();
                CopyMemory(m_Bmp.BackBuffer, data, len);
                m_Bmp.AddDirtyRect(new System.Windows.Int32Rect(0, 0, m_Bmp.PixelWidth, m_Bmp.PixelHeight));
                m_Bmp.Unlock();
            }, m_DispatcherPriority);
        }
    }

}
