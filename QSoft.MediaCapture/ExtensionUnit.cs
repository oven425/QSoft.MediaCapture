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


        //Function to set/get parameters of UVC extension unit
        HRESULT GetExtensionUnit(Guid xuGuid, uint dwExtensionNode, uint xuPropertyId)
        {
            Guid pNodeType;
            //IUnknown unKnown;
            IKsControl ks_control;
            IKsTopologyInfo pKsTopologyInfo = m_MediaSource as IKsTopologyInfo;
            KSP_NODE kspNode;

            
            var hr = pKsTopologyInfo.CreateNodeInstance(dwExtensionNode, typeof(IKsControl).GUID, out var obj);
            ks_control =  obj as IKsControl;

            KSIDENTIFIER ss;
            ss.__union_0 = new __struct_ks_2__union_0()
            {
                __field_0 = new()
                {
                    Set = xuGuid,
                    Id = xuPropertyId,
                    Flags = DirectN.Constants.KSPROPERTY_TYPE_GET | DirectN.Constants.KSPROPERTY_TYPE_TOPOLOGY
                }
            };
            DirectN.KSP_NODE node = new DirectN.KSP_NODE
            {
                Property = ss,
                NodeId = dwExtensionNode
            };

            //kspNode.Property.Set = xuGuid;                  // XU GUID
            //kspNode.NodeId = (ULONG)dwExtensionNode;        // XU Node ID
            //kspNode.Property.Id = xuPropertyId;             // XU control ID
            //kspNode.Property.Flags = flags;                 // Set/Get request
            using var mem = new ComMemory(40);
            hr = ks_control.KsProperty(ref node.Property, 32, mem.Pointer, (uint)mem.Size, out var readCount);
        //CHECK_HR_RESULT(hr, "ks_control->KsProperty(...)");
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
                //DWORD i;
                //GUID nodeGuid;
                //unsigned char* TmpGuid;

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

                //// Did not find the node 
                //if (i == numberOfNodes)
                //{
                //    hResult = S_FALSE;
                //}
            }
            return hResult;
        }

        bool GetNodeId(out uint pNodeId)
        {
            pNodeId = 0;
            IKsTopologyInfo pKsToplogyInfo = m_MediaSource as IKsTopologyInfo;
            HRESULT hResult;
            uint dwNode;

            //hResult = pVideoSource->QueryInterface(__uuidof(IKsTopologyInfo), (void**)&pKsToplogyInfo);
            //if (S_OK == hResult)
            {
                hResult = FindExtensionNode(pKsToplogyInfo, out dwNode);
                //WebCam_MF.SafeRelease(pKsToplogyInfo);
                if (HRESULTS.S_OK == hResult)
                {
                    pNodeId = dwNode;
                    return true;
                }
            }
            return false;
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
    }
}
