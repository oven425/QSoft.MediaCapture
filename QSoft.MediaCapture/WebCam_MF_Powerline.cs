using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace QSoft.MediaCapture.KsMedia
{
    public class KsControl
    {
        readonly IMFCaptureEngine? m_pEngine;
        public KsControl(IMFCaptureEngine? engine)
        {
            this.m_pEngine = engine;
            //this.GetRange();
            this.Set();
            this.Get();
        }

        public void Set()
        {
            if (m_pEngine == null) return;
            HRESULT hr = HRESULTS.S_OK;
            IKsControl? ks = null;
            IMFCaptureSource? pSource = null;
            try
            {
                hr = m_pEngine.GetSource(out pSource);
                if (hr != HRESULTS.S_OK || pSource == null) return;
                hr = pSource.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out var mediasource);
                if (hr != HRESULTS.S_OK || mediasource == null) return;
                var set = mediasource as IKsPropertySet;
                ks = mediasource as IKsControl;
                if (ks == null) return;
                KSIDENTIFIER ss;
                ss.__union_0 = new __struct_ks_2__union_0()
                {
                    __field_0 = new()
                    {
                        Set = DirectN.KSMedia.PROPSETID_VIDCAP_VIDEOPROCAMP,
                        Id = (uint)DirectN.KSPROPERTY_VIDEOPROCAMP.KSPROPERTY_VIDEOPROCAMP_POWERLINE_FREQUENCY,
                        Flags = DirectN.Constants.KSPROPERTY_TYPE_SET

                    }
                };
                KSPROPERTY_VIDEOPROCAMP_S pp = new KSPROPERTY_VIDEOPROCAMP_S
                {
                    Flags = 2,
                    Property = ss,
                    Value = 1, // Set to 1 for 50Hz, 2 for 60Hz, etc.
                };


                using var mem = new ComMemory(40);
                Marshal.StructureToPtr(pp, mem.Pointer, false);
                hr = ks.KsProperty(ref pp.Property, 40, mem.Pointer, 40, out var retr);
                var sssm = Marshal.PtrToStructure<KSPROPERTY_VIDEOPROCAMP_S>(mem.Pointer);



            }
            finally
            {
                WebCam_MF.SafeRelease(pSource);
                WebCam_MF.SafeRelease(ks);
            }

        }


        public void GetRange()
        {
            if (m_pEngine == null) return;
            HRESULT hr = HRESULTS.S_OK;
            IKsControl? ks = null;
            IMFCaptureSource? pSource = null;
            try
            {
                hr = m_pEngine.GetSource(out pSource);
                if (hr != HRESULTS.S_OK || pSource == null) return;
                hr = pSource.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out var mediasource);
                if (hr != HRESULTS.S_OK || mediasource == null) return;
                ks = mediasource as IKsControl;
                if (ks == null) return;
                KSIDENTIFIER ss;
                ss.__union_0 = new __struct_ks_2__union_0()
                {
                    __field_0 = new()
                    {
                        Set = DirectN.KSMedia.PROPSETID_VIDCAP_VIDEOPROCAMP,
                        Id = (uint)DirectN.KSPROPERTY_VIDEOPROCAMP.KSPROPERTY_VIDEOPROCAMP_POWERLINE_FREQUENCY,
                        Flags = DirectN.Constants.KSEVENT_TYPE_BASICSUPPORT

                    }
                };
                KSPROPERTY_VIDEOPROCAMP_S pp = new KSPROPERTY_VIDEOPROCAMP_S
                {
                    Property = ss,
                };


                using var mem = new ComMemory(40);
                hr = ks.KsProperty(ref pp.Property, 40, mem.Pointer, (uint)mem.Size, out var retr);
                var sssm = Marshal.PtrToStructure<KSPROPERTY_VIDEOPROCAMP_S>(mem.Pointer);



            }
            finally
            {
                WebCam_MF.SafeRelease(pSource);
                WebCam_MF.SafeRelease(ks);
            }

        }

        public void Get()
        {
            if (m_pEngine == null) return;
            HRESULT hr = HRESULTS.S_OK;
            IKsControl? ks = null;
            IMFCaptureSource? pSource = null;
            try
            {
                hr = m_pEngine.GetSource(out pSource);
                if (hr != HRESULTS.S_OK || pSource == null) return;
                hr = pSource.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out var mediasource);
                if (hr != HRESULTS.S_OK || mediasource == null) return;
                ks = mediasource as IKsControl;
                if (ks == null) return;
                KSIDENTIFIER ss;
                ss.__union_0 = new __struct_ks_2__union_0()
                {
                    __field_0 = new()
                    {
                        Set = DirectN.KSMedia.PROPSETID_VIDCAP_VIDEOPROCAMP,
                        Id = (uint)DirectN.KSPROPERTY_VIDEOPROCAMP.KSPROPERTY_VIDEOPROCAMP_POWERLINE_FREQUENCY,
                        Flags = DirectN.Constants.KSPROPERTY_TYPE_GET

                    }
                };

                using var mem = new ComMemory(40);
                hr = ks.KsProperty(ref ss, 40, mem.Pointer, (uint)mem.Size, out var retr);
                var sssm = Marshal.PtrToStructure<KSPROPERTY_VIDEOPROCAMP_S>(mem.Pointer);

            }
            finally
            {
                WebCam_MF.SafeRelease(pSource);
                WebCam_MF.SafeRelease(ks);
            }

        }

        HRESULT Init()
        {
            if (m_pEngine == null) return HRESULTS.MF_E_NOT_INITIALIZED;
            HRESULT hr = HRESULTS.S_OK;
            IKsControl? ks = null;
            IMFCaptureSource? pSource = null;
            try
            {
                hr = m_pEngine.GetSource(out pSource);
                if (hr != HRESULTS.S_OK || pSource == null) return hr;
                hr = pSource.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out var mediasource);
                if (hr != HRESULTS.S_OK || mediasource == null) return hr;
                ks = mediasource as IKsControl;
                if (ks == null) return hr;
                KSIDENTIFIER ss;
                ss.__union_0 = new __struct_ks_2__union_0()
                {
                    __field_0 = new()
                    {
                        Set = DirectN.KSMedia.PROPSETID_VIDCAP_VIDEOPROCAMP,
                        Id = (uint)DirectN.KSPROPERTY_VIDEOPROCAMP.KSPROPERTY_VIDEOPROCAMP_BRIGHTNESS,
                        Flags = DirectN.Constants.KSPROPERTY_TYPE_BASICSUPPORT

                    }
                };

                //KSPROPERTY_VALUES
                //using var mem = new ComMemory(100);
                //hr = ks.KsProperty(ref ss, 40, mem.Pointer, (uint)mem.Size, out var retr);
                //var sssm = Marshal.PtrToStructure<KSPROPERTY_DESCRIPTION>(mem.Pointer);
                //hr = HRESULTS.S_OK;


            }
            finally
            {
                WebCam_MF.SafeRelease(pSource);
                WebCam_MF.SafeRelease(ks);
            }


            return hr;
        }
    }
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public struct KSPROPERTY_VIDEOPROCAMP_S
    {
        public KSIDENTIFIER Property;

        public int Value;

        public uint Flags;

        public uint Capabilities;
    }
}
//namespace QSoft.MediaCapture
//{
//    public partial class WebCam_MF
//    {   


//        public void SetPowerLine()
//        {
//            KsMedia.KsControl ksss = new KsMedia.KsControl(m_pEngine);
//            var hr = m_pEngine.GetSource(out var pSource);
//            hr = pSource.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out var mediasource);
//            var ks = mediasource as IKsControl;

//            //KSPROPERTY_VIDEOPROCAMP
//            KSIDENTIFIER ss;
//            ss.__union_0 = new __struct_ks_2__union_0()
//            {
//                __field_0 = new()
//                {
//                    Set = DirectN.KSMedia.PROPSETID_VIDCAP_VIDEOPROCAMP,
//                    Id = (uint)DirectN.KSPROPERTY_VIDEOPROCAMP.KSPROPERTY_VIDEOPROCAMP_POWERLINE_FREQUENCY,
//                    Flags = DirectN.Constants.KSPROPERTY_TYPE_GET

//                }
//            };
//            KSPROPERTY_VIDEOPROCAMP_S pp = new KSPROPERTY_VIDEOPROCAMP_S
//            {
//                Property = ss,
//            };


//            using var mem = new ComMemory(40);
//            hr = ks.KsProperty(ref pp.Property, 40, mem.Pointer, (uint)mem.Size, out var retr);
//            var sssm = Marshal.PtrToStructure<KSPROPERTY_VIDEOPROCAMP_S>(mem.Pointer);
//            hr = HRESULTS.S_OK;



//            WebCam_MF.SafeRelease(ks);
//        }
//    }

//    [StructLayout(LayoutKind.Sequential, Size =8)]
//    public struct KSPROPERTY_VIDEOPROCAMP_S
//    {
//        public KSIDENTIFIER Property;

//        public int Value;

//        public uint Flags;

//        public uint Capabilities;
//    }
//}
