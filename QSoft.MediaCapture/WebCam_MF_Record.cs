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
        bool m_bRecording = false;
        TaskCompletionSource<HRESULT>? m_TaskStopRecord;
        public Task<HRESULT> StopRecord()
        {
            m_TaskStopRecord = new TaskCompletionSource<HRESULT>();
            HRESULT hr = HRESULTS.S_OK;

            if (m_bRecording)
            {
                if (m_pEngine != null)
                {
                    hr = m_pEngine.StopRecord(true, false);
                }

                m_bRecording = false;
            }

            return m_TaskStopRecord.Task;
        }

        TaskCompletionSource<HRESULT>? m_TaskStartRecord;
        async public Task<HRESULT> StartRecord(string pszDestinationFile)
        {
            if (m_pEngine == null) return HRESULTS.MF_E_NOT_INITIALIZED;

            if (m_bRecording) return HRESULTS.MF_E_INVALIDREQUEST;
            m_TaskStartRecord = new TaskCompletionSource<HRESULT>();

            var ext = System.IO.Path.GetExtension(pszDestinationFile);
            var guidVideoEncoding = ext switch
            {
                ".mp4"=> MFConstants.MFVideoFormat_H264,
                ".wmv"=> MFConstants.MFVideoFormat_H264,
                _ =>Guid.Empty
            };
            var guidAudioEncoding = ext switch
            {
                ".mp4" => MFConstants.MFAudioFormat_AAC,
                ".wmv" => MFConstants.MFAudioFormat_AAC,
                ".wma"=> MFConstants.MFAudioFormat_WMAudioV9,
                _ => Guid.Empty
            };
            if (guidAudioEncoding == Guid.Empty && guidVideoEncoding == Guid.Empty)
            {
                return HRESULTS.MF_E_INVALIDMEDIATYPE;
            }


            HRESULT hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_RECORD, out var pSink);
            if (hr.IsError) return hr;
            var pRecord = pSink as IMFCaptureRecordSink;


            hr = m_pEngine.GetSource(out var pSource);
            if (hr.IsError) return hr;

            // Clear any existing streams from previous recordings.
            if (pRecord == null) return hr;
            hr = pRecord.RemoveAllStreams();
            if (hr.IsError) return hr;

            hr = pRecord.SetOutputFileName(pszDestinationFile);
            if (hr.IsError) return hr;

            // Configure the video and audio streams.
            if (guidVideoEncoding != Guid.Empty)
            {
                hr = ConfigureVideoEncoding(pSource, pRecord, ref guidVideoEncoding);
                if (hr.IsError) return hr;
            }

            if (guidAudioEncoding != Guid.Empty)
            {
                hr = ConfigureAudioEncoding(pSource, pRecord, guidAudioEncoding);
                if (hr.IsError) return hr;
            }

            hr = m_pEngine.StartRecord();
            if (hr.IsError) return hr;

            m_bRecording = true;
            hr = await m_TaskStartRecord.Task;
        done:
            //SafeRelease(&pSink);
            //SafeRelease(&pSource);
            //SafeRelease(&pRecord);

            return hr;
        }

        HRESULT ConfigureVideoEncoding(IMFCaptureSource pSource, IMFCaptureRecordSink pRecord, ref Guid guidEncodingType)
        {
            IMFMediaType? pMediaType = null;
            IMFMediaType? pMediaType2 = null;
            Guid guidSubType = Guid.Empty;

            // Configure the video format for the recording sink.
            HRESULT hr = pSource.GetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_RECORD, out pMediaType);
            if (hr.IsError) return hr;

            hr = CloneVideoMediaType(pMediaType, guidEncodingType, out pMediaType2);
            if (hr.IsError) return hr;
            if (pMediaType2 == null)
            {
                goto done;
            }

            hr = pMediaType.GetGUID(MFConstants.MF_MT_SUBTYPE, out guidSubType);
            if (hr.IsError) return hr;

            if (guidSubType == MFConstants.MFVideoFormat_H264_ES || guidSubType == MFConstants.MFVideoFormat_H264)
            {
                //When the webcam supports H264_ES or H264, we just bypass the stream. The output from Capture engine shall be the same as the native type supported by the webcam
                hr = pMediaType2.SetGUID(MFConstants.MF_MT_SUBTYPE, MFConstants.MFVideoFormat_H264);
            }
            else
            {
                uint uiEncodingBitrate;
                hr = GetEncodingBitrate(pMediaType2, out uiEncodingBitrate);
                if (hr.IsError)
                {
                    goto done;
                }
                //uiEncodingBitrate = uiEncodingBitrate / 2;
                hr = pMediaType2.SetUINT32(MFConstants.MF_MT_AVG_BITRATE, uiEncodingBitrate);
            }

            if (hr.IsError) return hr;

            // Connect the video stream to the recording sink.
            //DWORD dwSinkStreamIndex;
            if (this.m_Setting.Rotate == 90 || this.m_Setting.Rotate == 270)
            {
                pMediaType2.TryGetSize(MFConstants.MF_MT_FRAME_SIZE, out var w, out var h);
                pMediaType2.SetSize(MFConstants.MF_MT_FRAME_SIZE, h, w);
            }
            using (var cm = new ComMemory(Marshal.SizeOf<uint>()))
            {
                hr = pRecord.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_RECORD, pMediaType2, null, cm.Pointer);
                var streamindex = (uint)Marshal.ReadInt32(cm.Pointer);
                hr = pRecord.SetRotation(streamindex, this.m_Setting.Rotate);
            }
                

        done:
            //SafeRelease(&pMediaType);
            //SafeRelease(&pMediaType2);
            SafeRelease(pMediaType);
            SafeRelease(pMediaType2);
            return hr;
        }

        HRESULT GetEncodingBitrate(IMFMediaType pMediaType, out uint uiEncodingBitrate)
        {
            uiEncodingBitrate = 0;
            uint uiWidth;
            uint uiHeight;
            float uiBitrate;
            uint uiFrameRateNum;
            uint uiFrameRateDenom;

            HRESULT hr = GetFrameSize(pMediaType, out uiWidth, out uiHeight);
            if (hr.IsError) return hr;

            hr = GetFrameRate(pMediaType, out uiFrameRateNum, out uiFrameRateDenom);
            if (hr.IsError) return hr;

            uiBitrate = uiWidth / 3.0f * uiHeight * uiFrameRateNum / uiFrameRateDenom;

            uiEncodingBitrate = (uint)uiBitrate;

        done:

            return hr;
        }

        HRESULT GetFrameSize(IMFMediaType pType, out uint pWidth, out uint pHeight)
        {
            pType.TryGetSize(MFConstants.MF_MT_FRAME_SIZE, out pWidth, out pHeight);
            return HRESULTS.S_OK;
            //return MFFunctions1.MFGetAttributeSize(pType, MFConstants.MF_MT_FRAME_SIZE, out pWidth, out pHeight);
        }

        // Helper function to get the frame rate from a video media type.
        HRESULT GetFrameRate(IMFMediaType pType, out uint pNumerator, out uint pDenominator)
        {
            if (pType.TryGetRatio(MFConstants.MF_MT_FRAME_RATE, out pNumerator, out pDenominator))
            {
                
            }
            return HRESULTS.S_OK;
        }

        HRESULT ConfigureAudioEncoding(IMFCaptureSource pSource, IMFCaptureRecordSink pRecord, Guid guidEncodingType)
        {
            //IMFCollection pAvailableTypes = null;
            IMFMediaType? pMediaType = null;
            IMFAttributes? pAttributes = null;

            // Configure the audio format for the recording sink.

            HRESULT hr = MFFunctions.MFCreateAttributes(out pAttributes, 1);
            if (hr.IsError)
            {
                goto done;
            }

            // Enumerate low latency media types
            hr = pAttributes.SetUINT32(MFConstants.MF_LOW_LATENCY, 1);
            if (hr.IsError)
            {
                goto done;
            }


            // Get a list of encoded output formats that are supported by the encoder.
            //hr = MFTranscodeGetAudioOutputAvailableTypes(guidEncodingType, MFT_ENUM_FLAG_ALL | MFT_ENUM_FLAG_SORTANDFILTER,
            //    pAttributes, out pAvailableTypes);
            if (hr.IsError)
            {
                goto done;
            }
            //DirectN._MFT_ENUM_FLAG

            //MFT_ENUM_FLAG_SORTANDFILTER
            uint flag = (uint)(DirectN._MFT_ENUM_FLAG.MFT_ENUM_FLAG_ALL | DirectN._MFT_ENUM_FLAG.MFT_ENUM_FLAG_SORTANDFILTER);

            IMFCollection colptr;
            
            
            hr = MFTranscodeGetAudioOutputAvailableTypes(guidEncodingType, flag, pAttributes, out colptr);

            hr = colptr.GetElement(0, out var punk);
            if (hr.IsSuccess)
            {
                pMediaType = punk as IMFMediaType;
            }

            // Connect the audio stream to the recording sink.
            using (ComMemory cm = new(Marshal.SizeOf<uint>()))
            {
                hr = pRecord.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_AUDIO, pMediaType, null, cm.Pointer);
                if (hr == HRESULTS.MF_E_INVALIDSTREAMNUMBER)
                {
                    //If an audio device is not present, allow video only recording
                    hr = HRESULTS.S_OK;
                }
            }
        done:
            //SafeRelease(&pAvailableTypes);
            //SafeRelease(&pMediaType);
            //SafeRelease(&pAttributes);
            SafeRelease(pMediaType);
            SafeRelease(pAttributes);
            return hr;
        }
        [DllImport("mf", ExactSpelling = true)]
        public static extern HRESULT MFTranscodeGetAudioOutputAvailableTypes([MarshalAs(UnmanagedType.LPStruct)] Guid guidSubType, uint dwMFTFlags, IMFAttributes pCodecConfig, out IMFCollection ppAvailableTypes);

    }
}
