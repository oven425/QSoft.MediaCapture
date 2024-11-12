using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace QSoft.MediaCapture
{
    public partial class MFCaptureEngineOnSampleCallback : IMFCaptureEngineOnSampleCallback
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
        System.Diagnostics.Stopwatch? m_StopWatch;
#endif
        readonly object m_Lock = new object();

        public HRESULT OnSample(IMFSample pSample)
        {
            if (System.Threading.Monitor.TryEnter(this.m_Lock))
            {
#if DEBUG
                
                var attrs = pSample.GetUnknown<IMFAttributes>(WebCam_MF.MFSampleExtension_CaptureMetadata);
                System.Diagnostics.Trace.WriteLine($"attrs: {attrs.Count()}");
                try
                {
                    var buf11 = attrs.Object.GetBlobSize(WebCam_MF.MF_CAPTURE_METADATA_FACEROIS, out var len);
                    System.Diagnostics.Trace.WriteLine($"GetBlobSize: {buf11} {len}");
                    var face_ptr = Marshal.AllocHGlobal((int)len);
                    var header = Marshal.PtrToStructure<tagFaceRectInfoBlobHeader>(face_ptr);
                    System.Diagnostics.Trace.WriteLine($"header: Size:{header.Size} Count:{header.Count}");
                    Marshal.FreeHGlobal( face_ptr );
                }
                catch (Exception ee)
                {
                    System.Diagnostics.Trace.WriteLine(ee.Message);
                }
                attrs?.Dispose();
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

    }

}


