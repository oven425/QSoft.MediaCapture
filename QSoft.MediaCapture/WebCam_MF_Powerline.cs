using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{
    public partial class WebCam_MF
    {
        public void SetPowerLine()
        {
            var hr = m_pEngine.GetSource(out var pSource);
            hr = pSource.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out var mediasource);
            var ks = mediasource as IKsControl;
            ks = null;

            //KSIDENTIFIER ss;
            //ss.__union_0 = new __struct_ks_2__union_0()
            //{
            //    __field_0 = new()
            //    {
            //        Set = DirectN.KSMedia.PROPSETID_VIDCAP_VIDEOPROCAMP,
            //        Id = (uint)DirectN.KSPROPERTY_VIDEOPROCAMP.KSPROPERTY_VIDEOPROCAMP_POWERLINE_FREQUENCY,
            //        Flags = DirectN.Constants.KSPROPERTY_TYPE_GET

            //    }
            //};

            ////DirectN.KSMedia.ksprop ss;
            ////IKsPropertySet ss;




            //using var mem = new ComMemory(Marshal.SizeOf<uint>());
            //var ssoi = (uint)Marshal.SizeOf(typeof(KSIDENTIFIER));
            //hr = ks.KsProperty(ref ss, (uint)Marshal.SizeOf(typeof(KSIDENTIFIER)), mem.Pointer, 10, out var retr);

            //KSPROPERTY ksProp;
            //ksProp.Set = PROPSETID_VIDCAP_VIDEOPROCAMP; // 屬性集合：攝影機 Video ProcAmp
            //ksProp.Id = KSPROPERTY_VIDEOPROCAMP_POWERLINE_FREQUENCY; // 欲設定的屬性
            //ksProp.Flags = KSPROPERTY_TYPE_SET; // 指示進行設定

            //// 4. 呼叫 IKsControl::KsProperty 將期望的 frequency 值寫入到裝置中
            //hr = pKsControl->KsProperty(
            //        &ksProp,              // 指定要取得／設定的屬性
            //        sizeof(ksProp),       // 結構大小
            //        &frequency,           // 要傳入的值（例如 POWERLINE_50HZ 或 POWERLINE_60HZ）
            //        sizeof(frequency),    // 數值大小
            //        nullptr);             // 可選取得的 byte 數（此處不需要）

        }
    }
}
