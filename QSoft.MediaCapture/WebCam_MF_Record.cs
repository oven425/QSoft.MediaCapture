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
        public bool IsRecording { private set; get; }

        TaskCompletionSource<HRESULT>? m_TaskStopRecord;
        public Task<HRESULT> StopRecord()
        {
            m_TaskStopRecord = new TaskCompletionSource<HRESULT>();
            HRESULT hr = HRESULTS.S_OK;

            if (IsRecording)
            {
                if (m_pEngine != null)
                {
                    hr = m_pEngine.StopRecord(true, false);
                }

                IsRecording = false;
            }

            return m_TaskStopRecord.Task;
        }

        //async public Task<HRESULT> StartRecord1(string pszDestinationFile)
        //{
        //    if (m_pEngine == null) return HRESULTS.MF_E_NOT_INITIALIZED;

        //    if (m_bRecording) return HRESULTS.MF_E_INVALIDREQUEST;
        //    IMFCaptureSink? pSink = null;
        //    IMFCaptureSource? pSource = null;
        //    HRESULT hr = HRESULTS.S_OK;
        //    try
        //    {

        //        m_TaskStartRecord = new TaskCompletionSource<HRESULT>();

        //        var ext = System.IO.Path.GetExtension(pszDestinationFile);
        //        var guidVideoEncoding = ext switch
        //        {
        //            ".mp4" => MFConstants.MFVideoFormat_H264,
        //            ".wmv" => MFConstants.MFVideoFormat_H264,
        //            _ => Guid.Empty
        //        };
        //        var guidAudioEncoding = ext switch
        //        {
        //            ".mp4" => MFConstants.MFAudioFormat_AAC,
        //            ".wmv" => MFConstants.MFAudioFormat_AAC,
        //            ".wma" => MFConstants.MFAudioFormat_WMAudioV9,
        //            _ => Guid.Empty
        //        };
        //        if (guidAudioEncoding == Guid.Empty && guidVideoEncoding == Guid.Empty)
        //        {
        //            return HRESULTS.MF_E_INVALIDMEDIATYPE;
        //        }


        //        hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_RECORD, out pSink);
        //        if (hr.IsError) return hr;
        //        var pRecord = pSink as IMFCaptureRecordSink;


        //        hr = m_pEngine.GetSource(out pSource);
        //        if (hr.IsError) return hr;

        //        // Clear any existing streams from previous recordings.
        //        if (pRecord == null) return hr;
        //        hr = pRecord.RemoveAllStreams();
        //        if (hr.IsError) return hr;

        //        //hr = pRecord.SetOutputFileName(pszDestinationFile);
        //        var f = pszDestinationFile;
        //        hr = DirectN.Functions.MFCreateFile(__MIDL___MIDL_itf_mfobjects_0000_0018_0001.MF_ACCESSMODE_READWRITE, __MIDL___MIDL_itf_mfobjects_0000_0018_0002.MF_OPENMODE_APPEND_IF_EXIST, __MIDL___MIDL_itf_mfobjects_0000_0018_0003.MF_FILEFLAGS_NONE, f, out var pFile);
        //        var MFTranscodeContainerType_MPEG4 = new Guid(0xdc6cd05d, 0xb9d0, 0x40ef, 0xbd, 0x35, 0xfa, 0x62, 0x2c, 0x1a, 0xb2, 0x8a);
        //        hr = pRecord.SetOutputByteStream(pFile, MFTranscodeContainerType_MPEG4);


        //        if (hr.IsError) return hr;

        //        // Configure the video and audio streams.
        //        if (guidVideoEncoding != Guid.Empty)
        //        {
        //            hr = await ConfigureVideoEncoding(pSource, pRecord, guidVideoEncoding);
        //            if (hr.IsError) return hr;
        //        }

        //        if (guidAudioEncoding != Guid.Empty)
        //        {
        //            hr = ConfigureAudioEncoding(pSource, pRecord, guidAudioEncoding);
        //            if (hr.IsError) return hr;
        //        }

        //        hr = m_pEngine.StartRecord();
        //        if (hr.IsError) return hr;

        //        m_bRecording = true;
        //        hr = await m_TaskStartRecord.Task;
        //    }
        //    finally
        //    {
        //        SafeRelease(pSink);
        //        SafeRelease(pSource);
        //    }

        //    return hr;
        //}



        TaskCompletionSource<HRESULT>? m_TaskStartRecord;
        async public Task<HRESULT> StartRecord(string pszDestinationFile)
        {
            if (m_pEngine == null) return HRESULTS.MF_E_NOT_INITIALIZED;

            if (IsRecording) return HRESULTS.MF_E_INVALIDREQUEST;
            IMFCaptureSink? pSink = null;
            IMFCaptureSource? pSource = null;
            HRESULT hr = HRESULTS.S_OK;
            try
            {

                m_TaskStartRecord = new TaskCompletionSource<HRESULT>();

                var ext = System.IO.Path.GetExtension(pszDestinationFile);
                var guidVideoEncoding = ext switch
                {
                    ".mp4" => MFConstants.MFVideoFormat_H264,
                    ".wmv" => MFConstants.MFVideoFormat_H264,
                    _ => Guid.Empty
                };
                var guidAudioEncoding = ext switch
                {
                    ".mp4" => MFConstants.MFAudioFormat_AAC,
                    ".wmv" => MFConstants.MFAudioFormat_AAC,
                    ".wma" => MFConstants.MFAudioFormat_WMAudioV9,
                    _ => Guid.Empty
                };
                if (guidAudioEncoding == Guid.Empty && guidVideoEncoding == Guid.Empty)
                {
                    return HRESULTS.MF_E_INVALIDMEDIATYPE;
                }


                hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_RECORD, out pSink);
                if (hr.IsError) return hr;
                var pRecord = pSink as IMFCaptureRecordSink;


                hr = m_pEngine.GetSource(out pSource);
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
                    hr = await ConfigureVideoEncoding(pSource, pRecord, guidVideoEncoding);
                    if (hr.IsError) return hr;
                }

                if (guidAudioEncoding != Guid.Empty)
                {
                    hr = ConfigureAudioEncoding(pSource, pRecord, guidAudioEncoding);
                    if (hr.IsError) return hr;
                }

                hr = m_pEngine.StartRecord();
                if (hr.IsError) return hr;

                hr = await m_TaskStartRecord.Task;
                this.IsRecording = true;

            }
            finally
            {
                SafeRelease(pSink);
                SafeRelease(pSource);
            }

            return hr;
        }

        async Task<HRESULT> ConfigureVideoEncoding(IMFCaptureSource pSource, IMFCaptureRecordSink pRecord, Guid guidEncodingType)
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
            if (this.m_Setting.Rotate == CameraRotates.Rotate90
                || this.m_Setting.Rotate == CameraRotates.Rotate270
                || this.m_Setting.Rotate == CameraRotates.Rotate90Colockwise
                || this.m_Setting.Rotate == CameraRotates.Rotate270Colockwise)
            {
                pMediaType2.TryGetSize(MFConstants.MF_MT_FRAME_SIZE, out var w, out var h);
                pMediaType2.SetSize(MFConstants.MF_MT_FRAME_SIZE, h, w);
            }
            //if (this.m_Setting.Rotate == 90 || this.m_Setting.Rotate == 270)
            //{
            //    pMediaType2.TryGetSize(MFConstants.MF_MT_FRAME_SIZE, out var w, out var h);
            //    pMediaType2.SetSize(MFConstants.MF_MT_FRAME_SIZE, h, w);
            //}
            using (var cm = new ComMemory(Marshal.SizeOf<uint>()))
            {
                hr = pRecord.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_VIDEO_RECORD, pMediaType2, null, cm.Pointer);
                var streamindex = (uint)Marshal.ReadInt32(cm.Pointer);
                hr = pRecord.SetRotation(streamindex, (uint)this.m_Setting.Rotate);
                System.Diagnostics.Trace.WriteLine($"AddStream record{streamindex}");
                await this.AddVideoProcessorMFT(pSource, streamindex);
            }
        done:
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
            IMFMediaType? pMediaType = null;
            IMFAttributes? pAttributes = null;
            IMFCollection? pAvailableTypes = null;
            try
            {
                HRESULT hr = MFFunctions.MFCreateAttributes(out pAttributes, 1);
                if (hr.IsError) return hr;

                // Enumerate low latency media types
                hr = pAttributes.SetUINT32(MFConstants.MF_LOW_LATENCY, 1);
                if (hr.IsError) return hr;


                uint flag = (uint)(DirectN._MFT_ENUM_FLAG.MFT_ENUM_FLAG_ALL | DirectN._MFT_ENUM_FLAG.MFT_ENUM_FLAG_SORTANDFILTER);


                hr = MFTranscodeGetAudioOutputAvailableTypes(guidEncodingType, flag, pAttributes, out pAvailableTypes);

                //hr = DirectN.Functions.MFTranscodeGetAudioOutputAvailableTypes(guidEncodingType, flag, pAttributes, out pAvailableTypes);
                if (hr.IsError) return hr;
                hr = pAvailableTypes.GetElement(0, out var punk);
                if (hr.IsError) return hr;
                pMediaType = punk as IMFMediaType;

                using ComMemory cm = new(Marshal.SizeOf<uint>());
                hr = pRecord.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_AUDIO, pMediaType, null, cm.Pointer);
                if (hr == HRESULTS.MF_E_INVALIDSTREAMNUMBER)
                {
                    //If an audio device is not present, allow video only recording
                    hr = HRESULTS.S_OK;
                }
            }
            finally
            {
                SafeRelease(pAvailableTypes);
                SafeRelease(pMediaType);
                SafeRelease(pAttributes);
            }
            return HRESULTS.S_OK;
        }
        [DllImport("mf", ExactSpelling = true)]
        public static extern HRESULT MFTranscodeGetAudioOutputAvailableTypes([MarshalAs(UnmanagedType.LPStruct)] Guid guidSubType, uint dwMFTFlags, IMFAttributes pCodecConfig, out IMFCollection ppAvailableTypes);

        static readonly Guid MFTranscodeContainerType_MPEG4 = new Guid(0xdc6cd05d, 0xb9d0, 0x40ef, 0xbd, 0x35, 0xfa, 0x62, 0x2c, 0x1a, 0xb2, 0x8a);

    }
}
