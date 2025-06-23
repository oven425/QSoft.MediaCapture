using DirectN;
using System;

namespace QSoft.MediaCapture
{
    public partial class WebCam_MF
    {
        [Obsolete("please use VideoProcAmps[DirectN.tagVideoProcAmpProperty.VideoProcAmp_WhiteBalance]")]
        WhiteBalanceControl? m_WhiteBalanceControl;
        [Obsolete]
        public WhiteBalanceControl WhiteBalanceControl
        {
            get
            {
                if (m_WhiteBalanceControl is null)
                {
                    m_WhiteBalanceControl = new(m_pEngine);
                    m_WhiteBalanceControl.Init();
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
    [Obsolete]
    public class WhiteBalanceControl(IMFCaptureEngine? engine)
        : Legacy.AMVideoProcAmp(engine, tagVideoProcAmpProperty.VideoProcAmp_WhiteBalance)
    {
        public ColorTemperaturePreset Preset
        {
            set
            {

                switch (value)
                {
                    case ColorTemperaturePreset.Auto:
                        //this.IsAuto = true;
                        this.SetValue(0, true);
                        break;
                    case ColorTemperaturePreset.Manual:
                        this.SetValue(0, false);
                        // Set manual value here
                        break;
                    case ColorTemperaturePreset.Candlelight:
                        this.SetValue(1900, false); // 大約 1,800 - 2,000 K
                        break;
                    case ColorTemperaturePreset.Cloudy:
                        this.SetValue(6500, false); // 大約 6,000 - 7,000 K
                        break;
                    case ColorTemperaturePreset.Daylight:
                        this.SetValue(5250, false); // 大約 5,000 - 5,500 K
                        break;
                    case ColorTemperaturePreset.Flash:
                        this.SetValue(5750, false); // 大約 5,500 - 6,000 K
                        break;
                    case ColorTemperaturePreset.Fluorescent:
                        this.SetValue(4500, false); // 大約 4,000 - 5,000 K
                        break;
                    case ColorTemperaturePreset.Tungsten:
                        this.SetValue(2950, false);// 大約 2,700 - 3,200 K
                        break;
                }
            }
            get
            {
                if (!this.IsAuto)
                {
                    var vv = this.Value;
                    switch (vv)
                    {
                        case 1900:
                            return ColorTemperaturePreset.Candlelight;
                        case 6500:
                            return ColorTemperaturePreset.Cloudy;
                        case 5250:
                            return ColorTemperaturePreset.Daylight;
                        case 5750:
                            return ColorTemperaturePreset.Flash;
                        case 4500:
                            return ColorTemperaturePreset.Fluorescent;
                        case 2950:
                            return ColorTemperaturePreset.Tungsten;
                        default:
                            return ColorTemperaturePreset.Manual;
                    }
                }

                return ColorTemperaturePreset.Auto;
            }
        }
    }
}
