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
        readonly BackgroundSegmentation? m_BackgroundSegmentation;
        public BackgroundSegmentation BackgroundSegmentation => m_BackgroundSegmentation ?? new(this.m_pEngine);
    }

    public class BackgroundSegmentation : ExtendedCameraControl
    {
        public IReadOnlyList<BackgroundSegmentationState> SupportStates { get; } = [];
        public BackgroundSegmentation(IMFCaptureEngine? engine)
            : base(engine, KSPROPERTY_CAMERACONTROL_EXTENDED.KSPROPERTY_CAMERACONTROL_EXTENDED_BACKGROUNDSEGMENTATION)
        {
            var hr = this.GetCapabilities(out var cap);
            if (hr == HRESULTS.S_OK)
            {
                SupportStates = [.. ParseState(cap)];
            }
        }

        IEnumerable<BackgroundSegmentationState> ParseState(ulong cap)
        {
            yield return BackgroundSegmentationState.OFF;
            var blur = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_BACKGROUNDSEGMENTATION_BLUR;
            if (blur == DirectN.Constants.KSCAMERA_EXTENDEDPROP_BACKGROUNDSEGMENTATION_BLUR)
            {
                yield return BackgroundSegmentationState.Blur;
            }
            var mask = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_BACKGROUNDSEGMENTATION_MASK;
            if (mask == DirectN.Constants.KSCAMERA_EXTENDEDPROP_BACKGROUNDSEGMENTATION_MASK)
            {
                yield return BackgroundSegmentationState.Mask;
            }
            var shallowfocus = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_BACKGROUNDSEGMENTATION_SHALLOWFOCUS;
            if (shallowfocus == DirectN.Constants.KSCAMERA_EXTENDEDPROP_BACKGROUNDSEGMENTATION_SHALLOWFOCUS)
            {
                yield return BackgroundSegmentationState.ShallowFocus;
            }
        }

        public BackgroundSegmentationState[] GetState()
        {
            var ss = this.Get(out var mode);
            return [.. ParseState(mode)];
        }

        public void SetState(params BackgroundSegmentationState[] states)
        {
            if(states.Any(x=>x == BackgroundSegmentationState.OFF))
            {
                this.Set(0);
                return;
            }
            ulong combinedState = states.Aggregate(0UL, (current, next) => current | (ulong)next);
            this.Set(combinedState);
        }

        [Flags]
        public enum BackgroundSegmentationState
        {
            OFF = 0x0000000000000000,
            Blur = 0x0000000000000001,
            Mask = 0x0000000000000002,
            ShallowFocus = 0x0000000000000004
        }
    }

}
