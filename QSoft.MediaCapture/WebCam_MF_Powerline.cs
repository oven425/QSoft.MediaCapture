using DirectN;
using QSoft.MediaCapture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
namespace QSoft.MediaCapture.KsMedia
{
    public class KsControl
    {
        readonly IMFCaptureEngine? m_pEngine;
        public KsControl(IMFCaptureEngine? engine)
        {
            this.m_pEngine = engine;
            FindXU();
            Set1(1, 6);
            //SetBB();
            KsTopologyInfo();
            //this.GetRange();
            //this.Set();
            //this.Get();
        }

        void FindXU()
        {
            if (m_pEngine == null) return;
            HRESULT hr = HRESULTS.S_OK;
            IKsControl? ks = null;
            IMFCaptureSource? pSource = null;
            hr = m_pEngine.GetSource(out pSource);
            if (hr != HRESULTS.S_OK || pSource == null) return;
            hr = pSource.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out var mediasource);
            if (hr != HRESULTS.S_OK || mediasource == null) return;


            IKsTopologyInfo pKsTopologyInfo = mediasource as IKsTopologyInfo;


            pKsTopologyInfo.get_NumNodes(out var numNodes);
            System.Diagnostics.Trace.WriteLine("========== Device Topology ==========");
            System.Diagnostics.Trace.WriteLine($"Total Nodes:  {numNodes}");

            // 1. 先列出所有節點
            System.Diagnostics.Trace.WriteLine("--- Nodes ---");
            for (uint i = 0; i < numNodes; ++i)
            {
                pKsTopologyInfo.get_NodeType(i, out var nodeType);
                pKsTopologyInfo.get_Category(i, out var category);

                if (nodeType == new Guid("941C7AC0-C559-11D0-8A2B-00A0C9255AC1"))
                {

                }
            }


            System.Diagnostics.Trace.WriteLine("=====================================");

            WebCam_MF.SafeRelease(pKsTopologyInfo);
        }

        void Set1(uint id, uint nodeid)
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
                        Set = new Guid("{23E49ED0-1178-4F31-AE52-D2FB8A8D3B48}"),
                        Id = id,
                        Flags = DirectN.Constants.KSPROPERTY_TYPE_SET
                    }
                };
                DirectN.KSP_NODE node = new()
                {
                    Property = ss,
                    NodeId = nodeid

                };


                using var mem = new ComMemory(8);
                hr = ks.KsProperty(ref node.Property, 32, IntPtr.Zero, 0, out var retr);
                //var sssm = Marshal.PtrToStructure<KSPROPERTY_VIDEOPROCAMP_S>(mem.Pointer);
                System.Diagnostics.Trace.WriteLine($"KsControl id:{id} modeid:{nodeid} hr={hr}");
                if (hr == HRESULTS.S_OK) { }

            }
            finally
            {
                WebCam_MF.SafeRelease(pSource);
                WebCam_MF.SafeRelease(ks);
            }
        }


        void Get1(uint id, uint modeid)
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
                        Set = new Guid("{26B8105A-0713-4870-979D-DA79444BB68E}"),
                        Id = id,
                        Flags = DirectN.Constants.KSPROPERTY_TYPE_GET

                    }
                };
                DirectN.KSP_NODE node = new DirectN.KSP_NODE
                {
                    Property = ss,
                    NodeId = modeid
                };


                using var mem = new ComMemory(40);
                Marshal.StructureToPtr(node, mem.Pointer, false);
                hr = ks.KsProperty(ref node.Property, 40, mem.Pointer, 40, out var retr);
                //var sssm = Marshal.PtrToStructure<KSPROPERTY_VIDEOPROCAMP_S>(mem.Pointer);
                System.Diagnostics.Trace.WriteLine($"KsControl id:{id} modeid:{modeid} hr={hr}");
                if (hr == HRESULTS.S_OK) { }

            }
            finally
            {
                WebCam_MF.SafeRelease(pSource);
                WebCam_MF.SafeRelease(ks);
            }
        }

        void SetBB()
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
                        Set = DirectN.KSMedia.KSPROPERTYSETID_ExtendedCameraControl,
                        Id = 6,
                        Flags = DirectN.Constants.KSPROPERTY_TYPE_SET

                    }
                };
                KSPROPERTY_Ext_S pp = new()
                {
                    Property = ss,
                    Value = 0xff16010100000000,
                };


                using var mem = new ComMemory(40);
                Marshal.StructureToPtr(pp, mem.Pointer, false);
                hr = ks.KsProperty(ref pp.Property, 40, mem.Pointer, 40, out var retr);
                //var sssm = Marshal.PtrToStructure<KSPROPERTY_VIDEOPROCAMP_S>(mem.Pointer);



            }
            finally
            {
                WebCam_MF.SafeRelease(pSource);
                WebCam_MF.SafeRelease(ks);
            }
        }

        //https://github.com/3dtof/DemoApplications/blob/master/Calculus/tv_auto_on_off/voxel-sdk/libvoxel/Voxel/UVCXU.cpp
        public void KsTopologyInfo()
        {
            if (m_pEngine == null) return;
            HRESULT hr = HRESULTS.S_OK;
            IKsControl? ks = null;
            IMFCaptureSource? pSource = null;
            hr = m_pEngine.GetSource(out pSource);
            if (hr != HRESULTS.S_OK || pSource == null) return;
            hr = pSource.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out var mediasource);
            if (hr != HRESULTS.S_OK || mediasource == null) return;


            IKsTopologyInfo pKsTopologyInfo = mediasource as IKsTopologyInfo;


            pKsTopologyInfo.get_NumNodes(out var numNodes);
            System.Diagnostics.Trace.WriteLine("========== Device Topology ==========");
            System.Diagnostics.Trace.WriteLine($"Total Nodes:  {numNodes}");

            // 1. 先列出所有節點
            System.Diagnostics.Trace.WriteLine("--- Nodes ---");
            for (uint i = 0; i < numNodes; ++i)
            {
                pKsTopologyInfo.get_NodeType(i, out var nodeType);
                pKsTopologyInfo.get_Category(i, out var category);

                if (nodeType == new Guid("941C7AC0-C559-11D0-8A2B-00A0C9255AC1"))
                {

                }
                //WCHAR nodeName[128] = { 0 };
                //DWORD nameLen = 0;
                //pKsTopologyInfo->get_NodeName(i, nodeName, 128, &nameLen);

                //std::wcout << L"Node " << i << L": "
                //           << NodeTypeToString(nodeType).c_str()
                //           << L" (" << (nameLen > 0 ? nodeName : L"No Name") << L")"
                //           << std::endl;

                //System.Diagnostics.Trace.WriteLine($"Node {i}: {NodeTypeToString(nodeType)} ({(nameLen > 0 ? nodeName : "No Name")})");
            }

            // 2. 再列出所有連接
            System.Diagnostics.Trace.WriteLine("--- Connections (Data Flow) ---");
            for (uint i = 0; i < numNodes; ++i)
            {
                hr = pKsTopologyInfo.get_ConnectionInfo(i, out var connection);
                if (hr.IsSuccess)
                {
                    //// FromNodePin 和 ToNodePin 指的是節點上的針腳編號
                    //std::wcout << L"Node " << connection.FromNode
                    //           << L" (Pin " << connection.FromNodePin << L")"
                    //           << L"  --->  "
                    //           << L"Node " << connection.ToNode
                    //           << L" (Pin " << connection.ToNodePin << L")"
                    //           << std::endl;
                    System.Diagnostics.Trace.WriteLine($"Node {connection.FromNode} (Pin {connection.FromNodePin}) ---> Node {connection.ToNode} (Pin {connection.ToNodePin})");
                }
            }

            System.Diagnostics.Trace.WriteLine("=====================================");

            WebCam_MF.SafeRelease(pKsTopologyInfo);

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

    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public struct KSPROPERTY_Ext_S
    {
        public KSIDENTIFIER Property;

        public ulong Value;

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