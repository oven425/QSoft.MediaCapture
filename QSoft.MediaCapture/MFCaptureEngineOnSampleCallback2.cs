using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{
    public class MFCaptureEngineOnSampleCallback2: IMFCaptureEngineOnSampleCallback2
    {
        public HRESULT OnSample(IMFSample pSample)
        {
            Marshal.ReleaseComObject(pSample);
            return HRESULTS.S_OK;
        }

        public HRESULT OnSynchronizedEvent(IMFMediaEvent pEvent)
        {
            return HRESULTS.S_OK;
        }
    }
}
