using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.MediaCapture
{

    //https://code.stanford.edu/mpsilver/2208_kinect_sdk/-/blob/81e355c5fc26034743205132b3de9cf8b961543d/src/color/mfcamerareader.cpp
    public partial class WebCam_MF
    {
        FaceDetectionControl? m_FaceDetectionControl;
        public FaceDetectionControl? FaceDetectionControl=> this.m_FaceDetectionControl ??= new(this.m_pEngine);
    }

    public class FaceDetectionControl : ExtendedCameraControl
    {
        public FaceDetectionControl(IMFCaptureEngine? engine)
            : base(engine, KSPROPERTY_CAMERACONTROL_EXTENDED.KSPROPERTY_CAMERACONTROL_EXTENDED_FACEDETECTION)
        {
            var hr = this.GetCapabilities(out var cap);
            if(hr == HRESULTS.S_OK)
            {
                var preview = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_PREVIEW;
                if (preview == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_PREVIEW)
                {
                    SupportStates.Add(FaceDetectionState.PREVIEW);
                }
                var video = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_VIDEO;
                if (video == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_VIDEO)
                {
                    SupportStates.Add(FaceDetectionState.VIDEO);
                }
                var photo = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_PHOTO;
                if (photo == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_PHOTO)
                {
                    SupportStates.Add(FaceDetectionState.PHOTO);
                }
                var blink = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_BLINK;
                if (blink == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_BLINK)
                {
                    SupportStates.Add(FaceDetectionState.BLINK);
                }
                var smile = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_SMILE;
                if (smile == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_SMILE)
                {
                    SupportStates.Add(FaceDetectionState.SMILE);
                }
            }
        }
        readonly public List<FaceDetectionState> SupportStates = [];

        public FaceDetectionState GetState()
        {
            System.Diagnostics.Debug.WriteLine($"Torch GetState");
            var hr = this.Get(out var mode);
            System.Diagnostics.Debug.WriteLine($"Torch GetState:{mode}");
            if (hr == HRESULTS.S_OK)
            {
                //var getv = mode switch
                //{
                //    0 => TorchLightState.OFF,
                //    1 => TorchLightState.ON,
                //    2 => TorchLightState.ON_ADJUSTABLEPOWER,
                //    _ => TorchLightState.OFF
                //};
                //return getv;
            }
            return FaceDetectionState.PREVIEW;

            return FaceDetectionState.OFF| FaceDetectionState.BLINK;
        }

        public event EventHandler<FaceDetectionEventArgs>? FaceDetectionEvent;
    }

    [Flags]
    public enum FaceDetectionState
    {
        OFF = 0x0000000000000000,
        PREVIEW = 0x0000000000000001,
        VIDEO = 0x0000000000000002,
        PHOTO = 0x0000000000000004,
        BLINK = 0x0000000000000008,
        SMILE = 0x0000000000000010
    }



    public class FaceDetectionEventArgs : EventArgs
    {
        public List<tagFaceRectInfo> RawData { get; } = [];
        public List<(double left, double top, double right, double bottom)> FaceRects { get; } = [];
    }
}
