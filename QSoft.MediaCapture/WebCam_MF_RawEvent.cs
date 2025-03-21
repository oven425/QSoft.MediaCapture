﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{
    public partial class WebCam_MF: IPassRaw
    {
        public event EventHandler<RawEventArgs>? FrameArrived;

        void IPassRaw.Transert(byte[] data)
        {
            FrameArrived?.Invoke(this, new RawEventArgs(data));
        }
    }

}
