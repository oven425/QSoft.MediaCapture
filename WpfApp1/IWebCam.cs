using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public interface IWebCam
    {
        void Start();
        void Stop();
    }

    public class MediaFundation_WebCam : IWebCam
    {
        public void Start()
        {
        }

        public void Stop()
        {
        }
    }

    public class WebCamOption
    {
        public IWebCam Build()
        {
            return new MediaFundation_WebCam();
        }

        public WebCamOption SetPreview(IntPtr hwnd)
        {
            return this;
        }

        public WebCamOption SetPreview(Action<int ,int, byte[]> avtion)
        {
            return this;
        }


    }
}
