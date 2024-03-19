using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{
    public class WebCam_MF: IMFCaptureEngineOnEventCallback
    {
        public HRESULT OnEvent(IMFMediaEvent pEvent)
        {
            throw new NotImplementedException();
        }

        public static List<WebCam_MF> GetAllWebCam()
        {

            return new List<WebCam_MF>();
        }

        public void InitCaptureEngine()
        {

        }

        public void StartPreview()
        {

        }

        public void StopPreview()
        { 

        }


        public void TakePhoto(string name)
        {

        }

        public void StartRecord(string name)
        {

        }

        public void StopRecord()
        {

        }
    }
}
