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
    public partial class MFCaptureEngineOnSampleCallback2 : IMFCaptureEngineOnSampleCallback2
    {
#if NET8_0_OR_GREATER
        //[LibraryImport("kernel32.dll", EntryPoint = "RtlCopyMemory", SetLastError = false)]
        //internal static partial void CopyMemory(IntPtr dest, IntPtr src, uint count);

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
                System.Diagnostics.Trace.WriteLine($"OnSample len: {cur}");
                //OnSample(ptr, cur);

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

        virtual protected void OnSample(IntPtr data, uint len) 
        {
            
        }


        public HRESULT OnSynchronizedEvent(IMFMediaEvent pEvent)
        {
            System.Diagnostics.Trace.WriteLine($"Synchronized event: {pEvent}");
            PROPVARIANT p = new PROPVARIANT();
            var hr = pEvent.GetValue(p);
            var t = p.VarType;
            if(p.Value is IMFMediaType mediatype)
            {
                var fps = mediatype.Fps();
                if (mediatype.TryGetSize(MFConstants.MF_MT_FRAME_SIZE, out var w, out var h))
                {
                    this.OnMediaTypeChanged(w, h);
                }
                Marshal.ReleaseComObject(mediatype);
            }
            
            //Task.Delay(2000).Wait(); // Simulate some processing delay
            //System.Diagnostics.Trace.WriteLine($"Synchronized event1: {pEvent}");
            Marshal.ReleaseComObject(pEvent);
            return HRESULTS.S_OK;
        }

        protected virtual void OnMediaTypeChanged(uint width, uint height) { }



    }
}

////namespace QSoft.MediaCapture.WPF
////{
////    internal partial class MFCaptureEngineOnSampleCallback2_WriteableBitmap : MFCaptureEngineOnSampleCallback2
////    {
////        readonly WriteableBitmap? m_Bmp;
////        readonly System.Windows.Threading.DispatcherPriority m_DispatcherPriority;
////        public MFCaptureEngineOnSampleCallback2_WriteableBitmap(WriteableBitmap? data, System.Windows.Threading.DispatcherPriority dispatcherpriority)
////        {
////            m_DispatcherPriority = dispatcherpriority;
////            this.m_Bmp = data;
////        }
////#if NET8_0_OR_GREATER
////        [LibraryImport("kernel32.dll", EntryPoint = "RtlCopyMemory", SetLastError = false)]
////        internal static partial void CopyMemory(IntPtr dest, IntPtr src, uint count);

////#else
////        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
////        internal static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
////#endif

////        protected override void OnSample(nint data, uint len)
////        {
////            m_Bmp?.Dispatcher.Invoke(() =>
////            {
////                m_Bmp.Lock();
////                CopyMemory(m_Bmp.BackBuffer, data, len);
////                m_Bmp.AddDirtyRect(new System.Windows.Int32Rect(0, 0, m_Bmp.PixelWidth, m_Bmp.PixelHeight));
////                m_Bmp.Unlock();
////            }, m_DispatcherPriority);
////        }
////    }

////}
