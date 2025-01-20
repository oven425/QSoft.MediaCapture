using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{
    public partial class WebCam_MF
    {
        WhiteBalanceControl? m_WhiteBalanceControl;
        public WhiteBalanceControl WhiteBalanceControl
        {
            get
            {
                if(m_WhiteBalanceControl is null)
                {
                    m_WhiteBalanceControl = new(m_pEngine);
                    m_WhiteBalanceControl.Init();
                    var bb = m_WhiteBalanceControl.IsAuto;
                }
                return m_WhiteBalanceControl;
            }
        }
    }

    public enum ColorTemperaturePreset
    {
        Auto,
        Manual,
        Cloudy,
        Daylight,
        Flash,
        Fluorescent,
        Tungsten,
        Candlelight
    }

    public class WhiteBalanceControl(IMFCaptureEngine? engine): AMVideoProcAmp(engine, tagVideoProcAmpProperty.VideoProcAmp_WhiteBalance)
    {
        public ColorTemperaturePreset Preset { get; }

    }

}
