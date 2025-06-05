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
        readonly EyeGazeCorrection? m_EyeGazeCorrection;
        public EyeGazeCorrection EyeGazeCorrection => m_EyeGazeCorrection ?? new EyeGazeCorrection(m_pEngine);
    }

    public class EyeGazeCorrection : ExtendedCameraControl
    {
        public IReadOnlyList<EyeGazeCorrectionState> SupportStates { get; } = [];

        public EyeGazeCorrection(IMFCaptureEngine? engine)
            : base(engine, KSPROPERTY_CAMERACONTROL_EXTENDED.KSPROPERTY_CAMERACONTROL_EXTENDED_EYEGAZECORRECTION)
        {
            var hr = this.GetCapabilities(out var cap);
            if (hr == HRESULTS.S_OK)
            {
                SupportStates = [.. ParseState(cap)];
            }
        }

        public EyeGazeCorrectionState[] GetState()
        {
            var ss = this.Get(out var mode);
            return [.. ParseState(mode)];
        }

        public void SetState(params EyeGazeCorrectionState[] states)
        {
            if (states.Any(x => x == EyeGazeCorrectionState.OFF))
            {
                this.Set(0);
                return;
            }
            ulong combinedState = states.Aggregate(0UL, (current, next) => current | (ulong)next);
            this.Set(combinedState);
        }

        IEnumerable<EyeGazeCorrectionState> ParseState(ulong cap)
        {
            yield return EyeGazeCorrectionState.OFF;
            var blur = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_EYEGAZECORRECTION_ON;
            if (blur == DirectN.Constants.KSCAMERA_EXTENDEDPROP_EYEGAZECORRECTION_ON)
            {
                yield return EyeGazeCorrectionState.ON;
            }
            var mask = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_EYEGAZECORRECTION_STARE;
            if (mask == DirectN.Constants.KSCAMERA_EXTENDEDPROP_EYEGAZECORRECTION_STARE)
            {
                yield return EyeGazeCorrectionState.STARE;
            }
        }

    }

    [Flags]
    public enum EyeGazeCorrectionState
    {
        OFF = 0x0000000000000000,
        ON = 0x0000000000000001,
        STARE = 0x0000000000000002
    }
}
