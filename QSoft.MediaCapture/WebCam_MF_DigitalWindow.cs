using DirectN;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{
    public partial class WebCam_MF
    {
        DigitalWindow? m_DigitalWindow;
        public DigitalWindow DigitalWindow => m_DigitalWindow ??= new DigitalWindow(m_pEngine);
    }

    public class DigitalWindow : ExtendedCameraControl
    {
        public IReadOnlyList<DigitalWindowState> SupportStates { get; } = [];

        public DigitalWindow(IMFCaptureEngine? engine)
            : base(engine, KSPROPERTY_CAMERACONTROL_EXTENDED.KSPROPERTY_CAMERACONTROL_EXTENDED_DIGITALWINDOW)
        {
            var hr = this.GetCapabilities(out var cap);
            if (hr == HRESULTS.S_OK)
            {
                SupportStates = [.. ParseState(cap)];
            }
            var hdr = new ExtendedCameraControl(engine, KSPROPERTY_CAMERACONTROL_EXTENDED.KSPROPERTY_CAMERACONTROL_EXTENDED_VIDEOHDR);
            //var dic = new ExtendedCameraControl(engine, KSPROPERTY_CAMERACONTROL_EXTENDED.KSPROPERTY_CAMERACONTROL_EXTENDED_DIGITALWINDOW_CONFIGCAPS);
            //if(dic.IsSupported)
            //{
            //    dic.GetPayload(out var buf);
            //    var s1 = Marshal.SizeOf<tagKSCAMERA_EXTENDEDPROP_HEADER>();
            //    var s2 = Marshal.SizeOf<tagKSCAMERA_EXTENDEDPROP_DIGITALWINDOW_CONFIGCAPSHEADER>();
            //    var s3 = Marshal.SizeOf<tagKSCAMERA_EXTENDEDPROP_DIGITALWINDOW_CONFIGCAPS>();
            //    var s4 = buf.Length - s2;
            //    BinaryReader br = new BinaryReader(new MemoryStream(buf));
            //    var size = br.ReadInt32();
            //    var count = br.ReadInt32();
            //    for (int i = 0; i < count; i++)
            //    {
            //        var resolutionx = br.ReadInt32();
            //        var resolutiony = br.ReadInt32();
            //        var porchtop = br.ReadInt32();
            //        var porchleft = br.ReadInt32();
            //        var porchbottom = br.ReadInt32();
            //        var porchright = br.ReadInt32();
            //        var NonUpscalingWindowSize = br.ReadInt32();
            //        var MinWindowSize = br.ReadInt32();
            //        var MaxWindowSize = br.ReadInt32();
            //        var Reserved = br.ReadInt32();
            //        System.Diagnostics.Trace.WriteLine($"{resolutionx}x{resolutiony}");
            //    }
            //}
            
            
        }

        public DigitalWindowState GetState()
        {
            var ss = this.Get(out var mode);
            return (DigitalWindowState)mode;
        }

        public void SetState(DigitalWindowState state)
        {
            this.Set((ulong)state);
        }

        IEnumerable<DigitalWindowState> ParseState(ulong cap)
        {
            yield return DigitalWindowState.Manual;
            var blur = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_DIGITALWINDOW_AUTOFACEFRAMING;
            if (blur == DirectN.Constants.KSCAMERA_EXTENDEDPROP_DIGITALWINDOW_AUTOFACEFRAMING)
            {
                yield return DigitalWindowState.AutoFaceFraming;
            }

        }

    }

    public enum DigitalWindowState
    {
        Manual = 0x0000000000000000,
        AutoFaceFraming = 0x0000000000000001,
    }
}
