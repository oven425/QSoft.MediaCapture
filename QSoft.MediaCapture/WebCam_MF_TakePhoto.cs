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
        TaskCompletionSource<HRESULT>? m_TaskTakephoto;
        public async Task<HRESULT> TakePhoto(string pszFileName, uint width = 0, uint height = 0)
        {
            var ext = System.IO.Path.GetExtension(pszFileName);
            var photoformat = ext switch
            {
                ".jpg" => WICConstants.GUID_ContainerFormatJpeg,
                ".bmp"=>WICConstants.GUID_ContainerFormatBmp,
                ".png"=>WICConstants.GUID_ContainerFormatPng,
                _ => Guid.Empty
            };
            if(photoformat == Guid.Empty)
            {
                return HRESULTS.MF_E_UNSUPPORTED_FORMAT;
            }
            this.m_TaskTakephoto = new TaskCompletionSource<HRESULT>();
            IMFCaptureSink? pSink = null;
            IMFCapturePhotoSink? pPhoto = null;
            IMFCaptureSource? pSource = null;
            IMFMediaType? pMediaType = null;
            IMFMediaType? pMediaType2 = null;
            bool bHasPhotoStream = true;
            HRESULT hr = HRESULTS.S_OK;
            if (m_pEngine == null)
            {
                goto done;
            }
            // Get a pointer to the photo sink.
            hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.MF_CAPTURE_ENGINE_SINK_TYPE_PHOTO, out pSink);
            if (hr.IsError)
            {
                goto done;
            }
            pPhoto = pSink as IMFCapturePhotoSink;
            if (pPhoto == null)
            {
                goto done;
            }
            hr = m_pEngine.GetSource(out pSource);
            if (hr.IsError)
            {
                goto done;
            }

            //if (this.VideoFormats.ContainsKey(MF_CAPTURE_ENGINE_STREAM_CATEGORY.MF_CAPTURE_ENGINE_STREAM_CATEGORY_PHOTO_DEPENDENT))
            //{
            //    var type = this.PhotoForamts.FirstOrDefault(x => x.width == width && x.height == height);
            //    if (type.mediatype != null)
            //    {
            //        hr = pSource.SetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_PHOTO, type.mediatype);
            //    }
            //}


            hr = pSource.GetCurrentDeviceMediaType((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_PHOTO, out pMediaType);
            if (hr.IsError)
            {
                goto done;
            }



            //Configure the photo format
            hr = CreatePhotoMediaType(pMediaType, photoformat, out pMediaType2);
            if (hr.IsError)
            {
                goto done;
            }

            hr = pPhoto.RemoveAllStreams();
            if (hr.IsError)
            {
                goto done;
            }

            //DWORD dwSinkStreamIndex;
            //IntPtr pp = Marshal.AllocHGlobal(4);
            // Try to connect the first still image stream to the photo sink
            //uint dwSinkStreamIndex = 0;
            if (bHasPhotoStream)
            {
                using var cm = new ComMemory(Marshal.SizeOf<uint>());
                hr = pPhoto.AddStream((uint)MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM.FOR_PHOTO, pMediaType2, null, cm.Pointer);
                //dwSinkStreamIndex = (uint)Marshal.ReadInt32(cm.Pointer);
            }

            if (hr.IsError)
            {
                goto done;
            }

            hr = pPhoto.SetOutputFileName(pszFileName);
            if (hr.IsError)
            {
                goto done;
            }

            hr = m_pEngine.TakePhoto();
            if (hr.IsError)
            {
                goto done;
            }

            //m_bPhotoPending = true;
            hr = await m_TaskTakephoto.Task;
        done:
            SafeRelease(pSink);
            SafeRelease(pPhoto);
            SafeRelease(pSource);
            SafeRelease(pMediaType);
            SafeRelease(pMediaType2);
            return hr;
        }

        HRESULT CreatePhotoMediaType(IMFMediaType pSrcMediaType, Guid format, out IMFMediaType? ppPhotoMediaType)
        {
            //*ppPhotoMediaType = NULL;
            ppPhotoMediaType = null;
            //const UINT32 uiFrameRateNumerator = 30;
            //const UINT32 uiFrameRateDenominator = 1;

            IMFMediaType? pPhotoMediaType = null;

            HRESULT hr = MFFunctions.MFCreateMediaType(out pPhotoMediaType);
            if (hr.IsError)
            {
                goto done;
            }

            hr = pPhotoMediaType.SetGUID(MFConstants.MF_MT_MAJOR_TYPE, MFConstants.MFMediaType_Image);
            if (hr.IsError)
            {
                goto done;
            }


            hr = pPhotoMediaType.SetGUID(MFConstants.MF_MT_SUBTYPE, format);
            if (hr.IsError)
            {
                goto done;
            }

            hr = CopyAttribute(pSrcMediaType, pPhotoMediaType, MFConstants.MF_MT_FRAME_SIZE);
            if (hr.IsError)
            {
                goto done;
            }

            ppPhotoMediaType = pPhotoMediaType;
        //(*ppPhotoMediaType)->AddRef();

        done:
            //SafeRelease(&pPhotoMediaType);
            return hr;
        }

    }
}
