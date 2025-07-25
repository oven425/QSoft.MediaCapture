using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace QSoft.MediaCapture.KsMedia
{
    //refrence https://github.com/xlloss/uvc_xu_test
    public class ExtensionUnit
    {
        IMFCaptureEngine? m_pEngine;
        IMFCaptureSource m_CaptureSource;
        IMFMediaSource m_MediaSource;
        public ExtensionUnit(IMFCaptureEngine? engine)
        {
            m_pEngine = engine;
            var hr = m_pEngine?.GetSource(out m_CaptureSource);
            hr = m_CaptureSource?.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.MF_CAPTURE_ENGINE_DEVICE_TYPE_VIDEO, out m_MediaSource);
            this.GetNodeId(out var nodeid);
            GetExtensionUnit(new Guid("{23E49ED0-1178-4F31-AE52-D2FB8A8D3B48}"), nodeid, 2);
        }

        IKsTopologyInfo pKsTopologyInfo;
        //Function to set/get parameters of UVC extension unit
        HRESULT GetExtensionUnit(Guid xuGuid, uint dwExtensionNode, uint xuPropertyId)
        {
            Guid pNodeType;
            //IUnknown unKnown;
            IKsControl ks_control;
            //IKsTopologyInfo pKsTopologyInfo = m_MediaSource as IKsTopologyInfo;
            

            //{28F54685-06FD-11D2-B27A-00A0C9223196}
            var uudi = typeof(IKsControl).GUID;
            var hr = pKsTopologyInfo.CreateNodeInstance(dwExtensionNode, typeof(IKsControl).GUID, out var obj);
            //ks_control =  obj as IKsControl;

            //KSIDENTIFIER ss;
            //ss.__union_0 = new __struct_ks_2__union_0()
            //{
            //    __field_0 = new()
            //    {
            //        Set = xuGuid,
            //        Id = xuPropertyId,
            //        Flags = DirectN.Constants.KSPROPERTY_TYPE_GET | DirectN.Constants.KSPROPERTY_TYPE_TOPOLOGY
            //    }
            //};
            //DirectN.KSP_NODE node = new DirectN.KSP_NODE
            //{
            //    Property = ss,
            //    NodeId = dwExtensionNode
            //};
            ////0x4f31117823e49ed0
            ////kspNode.Property.Set = xuGuid;                  // XU GUID
            ////kspNode.NodeId = (ULONG)dwExtensionNode;        // XU Node ID
            ////kspNode.Property.Id = xuPropertyId;             // XU control ID
            ////kspNode.Property.Flags = flags;                 // Set/Get request
            //using var mem = new ComMemory(40);
            //hr = ks_control.KsProperty(ref node.Property, 32, IntPtr.Zero, 0, out var readCount);


            var kks = obj as Define.IKsControl;
            Define.KSP_NODE kspNode = new();
            kspNode.Property.Set = xuGuid;                  // XU GUID
            kspNode.NodeId = (uint)dwExtensionNode;        // XU Node ID
            kspNode.Property.Id = xuPropertyId;             // XU control ID
            kspNode.Property.Flags = DirectN.Constants.KSPROPERTY_TYPE_GET | DirectN.Constants.KSPROPERTY_TYPE_TOPOLOGY;
            hr = kks.KsProperty(ref kspNode.Property, (uint)Marshal.SizeOf<Define.KSP_NODE>(), IntPtr.Zero, 0, out var readCount);
            
            return hr;

        }

        HRESULT FindExtensionNode(IKsTopologyInfo pIksTopologyInfo, out uint pNodeId)
        {
            pNodeId = 0;
            uint numberOfNodes;
            HRESULT hResult = HRESULTS.S_FALSE;

            hResult = pIksTopologyInfo.get_NumNodes(out numberOfNodes);
            if (hResult == HRESULTS.S_OK)
            {
                for (uint i = 0; i < numberOfNodes; i++)
                {
                    hResult = pIksTopologyInfo.get_NodeType(i, out var nodeGuid);
                    if (hResult == HRESULTS.S_OK)
                    {
                        if(nodeGuid == DirectN.KSMedia.KSNODETYPE_DEV_SPECIFIC)
                        {
                            pNodeId = i;
                            hResult = HRESULTS.S_OK;
                            break;
                        }
                    }
                }
            }
            return hResult;
        }

        bool GetNodeId(out uint pNodeId)
        {
            pNodeId = 0;
            //IKsTopologyInfo pKsToplogyInfo = m_MediaSource as IKsTopologyInfo;
            HRESULT hResult;
            uint dwNode;
            pKsTopologyInfo = m_MediaSource as IKsTopologyInfo;
            //hResult = pVideoSource->QueryInterface(__uuidof(IKsTopologyInfo), (void**)&pKsToplogyInfo);
            //if (S_OK == hResult)
            {
                hResult = FindExtensionNode(pKsTopologyInfo, out dwNode);
                //WebCam_MF.SafeRelease(pKsToplogyInfo);
                if (HRESULTS.S_OK == hResult)
                {
                    pNodeId = dwNode;
                    return true;
                }
            }
            return false;
        }
    }

    
}

namespace QSoft.MediaCapture.KsMedia.Define
{
    [StructLayout(LayoutKind.Explicit)]
    public struct KSPROPERTY
    {
        /// <summary>
        /// The 'Set', 'Id', and 'Flags' fields correspond to the inner anonymous struct.
        /// Their offsets are laid out sequentially.
        /// </summary>
        [FieldOffset(0)]
        public Guid Set;

        [FieldOffset(16)]
        public uint Id;

        [FieldOffset(20)]
        public uint Flags;

        /// <summary>
        /// This field overlaps with the structure above, starting at the same memory location (offset 0),
        /// mimicking the behavior of the C union.
        /// </summary>
        [FieldOffset(0)]
        public long Alignment;
    }
    public struct KSP_NODE
    {
        public KSPROPERTY Property;
        public uint NodeId;
        public uint Reserved;
    }


    [ComImport]
    [Guid("28f54685-06fd-11d2-b27a-00a0c9223196")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IKsControl
    {
        [PreserveSig]
        HRESULT KsProperty(ref KSPROPERTY Property, uint PropertyLength, IntPtr PropertyData, uint DataLength, out uint BytesReturned);

        [PreserveSig]
        HRESULT KsMethod(ref KSIDENTIFIER Method, uint MethodLength, IntPtr MethodData, uint DataLength, out uint BytesReturned);

        [PreserveSig]
        HRESULT KsEvent(ref KSIDENTIFIER Event, uint EventLength, IntPtr EventData, uint DataLength, out uint BytesReturned);
    }
}
