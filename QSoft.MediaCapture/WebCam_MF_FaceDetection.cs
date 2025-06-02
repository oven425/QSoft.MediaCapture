using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
                SupportStates = [.. ParseStates(cap)];
                //var preview = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_PREVIEW;
                //if (preview == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_PREVIEW)
                //{
                //    SupportStates.Add(FaceDetectionState.PREVIEW);
                //}
                //var video = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_VIDEO;
                //if (video == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_VIDEO)
                //{
                //    SupportStates.Add(FaceDetectionState.VIDEO);
                //}
                //var photo = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_PHOTO;
                //if (photo == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_PHOTO)
                //{
                //    SupportStates.Add(FaceDetectionState.PHOTO);
                //}
                //var blink = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_BLINK;
                //if (blink == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_BLINK)
                //{
                //    SupportStates.Add(FaceDetectionState.BLINK);
                //}
                //var smile = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_SMILE;
                //if (smile == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_SMILE)
                //{
                //    SupportStates.Add(FaceDetectionState.SMILE);
                //}
            }
        }
        readonly public List<FaceDetectionState> SupportStates = [];

        IEnumerable<FaceDetectionState> ParseStates(ulong cap)
        {
            yield return FaceDetectionState.OFF;
            var preview = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_PREVIEW;
            if (preview == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_PREVIEW)
            {
                yield return FaceDetectionState.PREVIEW;
                //SupportStates.Add(FaceDetectionState.PREVIEW);
            }
            var video = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_VIDEO;
            if (video == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_VIDEO)
            {
                yield return FaceDetectionState.VIDEO;
                //SupportStates.Add(FaceDetectionState.VIDEO);
            }
            var photo = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_PHOTO;
            if (photo == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_PHOTO)
            {
                yield return FaceDetectionState.PHOTO;
                //SupportStates.Add(FaceDetectionState.PHOTO);
            }
            var blink = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_BLINK;
            if (blink == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_BLINK)
            {
                yield return FaceDetectionState.BLINK;
                //SupportStates.Add(FaceDetectionState.BLINK);
            }
            var smile = cap & DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_SMILE;
            if (smile == DirectN.Constants.KSCAMERA_EXTENDEDPROP_FACEDETECTION_SMILE)
            {
                yield return FaceDetectionState.SMILE;
                //SupportStates.Add(FaceDetectionState.SMILE);
            }
        }


        public FaceDetectionState[] GetState()
        {
            System.Diagnostics.Debug.WriteLine($"Face GetState");
            var hr = this.Get(out var mode);
            System.Diagnostics.Debug.WriteLine($"Face GetState:{mode}");
            if (hr == HRESULTS.S_OK)
            {
                return [.. ParseStates(mode)];
            }
            return [];

        }

        public void SetState(params FaceDetectionState[] states)
        {
            if (states.Length == 0) return;
            //System.Diagnostics.Debug.WriteLine($"Face SetState:{state}");
            HRESULT hr = HRESULTS.S_OK;
            if (states.Any(x => x == FaceDetectionState.OFF))
            {
                hr = this.Set(0);
            }
            else
            {
                ulong combinedState = states.Aggregate(0UL, (current, next) => current | (ulong)next);
                this.Set(combinedState);

            }
            System.Diagnostics.Debug.WriteLine($"Face SetState:{hr}");
        }

        public event EventHandler<FaceDetectionEventArgs>? FaceDetectionEvent;

        internal void ParseFaceDetectionData(byte[] face)
        {
            if (this.FaceDetectionEvent is null) return;
            if(!this.IsSupported) return;
            if(face is null) return;
            using var mem = new System.IO.MemoryStream(face);
            var br = new System.IO.BinaryReader(mem);
            var size = br.ReadUInt32();
            var count = br.ReadUInt32();
            var facerects = new List<(double left, double top, double right, double bottom)>();
            for (int i=0; i<count; i++)
            {
                var left_q31 = br.ReadInt32();
                var top_q31 = br.ReadInt32();
                var right_q31 = br.ReadInt32();
                var bottom_q31 = br.ReadInt32();
                var level = br.ReadInt32();
                var left_ = left_q31 / 2147483648.0;
                var right_ = right_q31 / 2147483648.0;
                var top_ = top_q31 / 2147483648.0;
                var bottom_ = bottom_q31 / 2147483648.0;
                facerects.Add((left_, top_, right_, bottom_));
                //System.Diagnostics.Trace.WriteLine($"face: {left_} {top_} {right_} {bottom_} {level}");
            }
            this.FaceDetectionEvent?.Invoke(this, new (face, facerects));
        }
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



    public class FaceDetectionEventArgs(byte[] raw, List<(double left, double top, double right, double bottom)> faces) : EventArgs
    {
        public byte[] RawData { get; } = raw;
        public List<(double left, double top, double right, double bottom)> FaceRects { get; } = [.. faces];
    }
}
