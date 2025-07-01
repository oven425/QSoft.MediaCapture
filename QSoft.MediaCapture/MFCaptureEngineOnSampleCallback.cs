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
    public class RawEventArgs(byte[] raw) : EventArgs
    {
        public byte[] RawData { get; } = raw;
    }

    interface IPassRaw
    {
        internal void Transert(byte[] data);
    }

    public partial class MFCaptureEngineOnSampleCallback
    {
        internal QSoft.MediaCapture.WebCam_MF? Parent { set; get; }
        internal IPassRaw? TranseRaw { set; get; }
        byte[]? m_RawBuffer;
        internal void RawEvent(IntPtr data, uint len)
        {
            if (TranseRaw is not null)
            {
                if (m_RawBuffer?.Length != len)
                {
                    m_RawBuffer = new byte[len];
                }
                Marshal.Copy(data, this.m_RawBuffer, 0, (int)len);
                TranseRaw.Transert(m_RawBuffer);
            }

        }
    }
    public partial class MFCaptureEngineOnSampleCallback : IMFCaptureEngineOnSampleCallback
    {
//#if NET8_0_OR_GREATER
//        [LibraryImport("kernel32.dll", EntryPoint = "RtlCopyMemory", SetLastError = false)]
//        internal static partial void CopyMemory(IntPtr dest, IntPtr src, uint count);

//#else
//        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
//        internal static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
//#endif


#if DEBUG
        int samplecount = 0;
        readonly System.Diagnostics.Stopwatch m_StopWatch = new();
#endif
        readonly object m_Lock = new();
        public HRESULT OnSample(IMFSample pSample)
        {
            if (System.Threading.Monitor.TryEnter(this.m_Lock))
            {
#if DEBUG
                using var attrs = pSample.GetUnknown<IMFAttributes>(DirectN.MFConstants.MFSampleExtension_CaptureMetadata);
                if (attrs is not null)
                {
                    //var cc = attrs.Object.Count();
                    //for(int i=0; i<cc; i++)
                    //{
                    //    PROPVARIANT pv = new PROPVARIANT();
                    //    attrs.Object.GetItemByIndex((uint)i, out var kk, pv);
                    //}
                    var face = attrs.Object.GetBlob(DirectN.MFConstants.MF_CAPTURE_METADATA_FACEROIS);
                    this.Parent?.FaceDetectionControl?.ParseFaceDetectionData(face);
                }
                
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
                RawEvent(ptr, cur);
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


