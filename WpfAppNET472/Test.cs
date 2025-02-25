using DirectN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using DirectN;

namespace WpfAppNET472
{
    public static class Test
    {
        public static BitmapSource GetVideoSnapshot(this string src)
        {
            DirectN.MFFunctions.MFCreateAttributes(out var attribute, 1);
            attribute.SetUINT32(MFConstants.MF_SOURCE_READER_ENABLE_VIDEO_PROCESSING, 1);

            IMFSourceReader source = null;
            DirectN.Functions.MFCreateSourceReaderFromURL(src, attribute, out source);


            //MediaFoundation.IMFMediaType mediatype = MediaFoundation.MF.CreateMediaType();
            //mediatype.SetGUID(MediaFoundation.MFAttributesClsid.MF_MT_MAJOR_TYPE, MediaFoundation.MFMediaType.Video);
            //mediatype.SetGUID(MediaFoundation.MFAttributesClsid.MF_MT_SUBTYPE, MediaFoundation.MFMediaType.RGB32);
            MFFunctions.MFCreateMediaType(out var mediatype);
            mediatype.SetGUID(MFConstants.MF_MT_MAJOR_TYPE, MFConstants.MFMediaType_Video);
            mediatype.SetGUID(MFConstants.MF_MT_SUBTYPE, MFConstants.MFVideoFormat_RGB32);

            //hr = source.SetCurrentMediaType((int)MediaFoundation.ReadWrite.MF_SOURCE_READER.FirstVideoStream, null, mediatype);
            //hr = source.SetStreamSelection((int)MediaFoundation.ReadWrite.MF_SOURCE_READER.FirstVideoStream, true);

            var hr = source.SetCurrentMediaType(0xFFFFFFFC, IntPtr.Zero, mediatype);
            hr = source.SetStreamSelection(0xFFFFFFFC, true);



            IMFMediaType current_mediatype;
            source.GetCurrentMediaType(0xFFFFFFFC, out current_mediatype);
            //int w = 0;
            //int h = 0;
            current_mediatype.TryGetSize(MFConstants.MF_MT_FRAME_SIZE, out var w, out var h);
            //pMediaType2.SetSize(MFConstants.MF_MT_FRAME_SIZE, h, w);

            //MediaFoundation.MFExtern.MFGetAttributeSize(current_mediatype, MediaFoundation.MFAttributesClsid.MF_MT_FRAME_SIZE, out w, out h);
            
            IMFSample sample;
            long timestamp = 0;
            int streamindex = 0;
            //MediaFoundation.ReadWrite.MF_SOURCE_READER_FLAG flag;
            using(var streamindex_ptr = new ComMemory(Marshal.SizeOf<uint>()))
            using (var flag_ptr = new ComMemory(Marshal.SizeOf<uint>()))
            using (var timestamp_ptr = new ComMemory(Marshal.SizeOf<uint>()))
            {
                hr = source.ReadSample(0xFFFFFFFC
                , 0x01
                , streamindex_ptr.Pointer
                , flag_ptr.Pointer
                , timestamp_ptr.Pointer
                , out sample);
            }
                
            IMFMediaBuffer buffer;
            sample.ConvertToContiguousBuffer(out buffer);

            IntPtr ptr;
            int size1;
            //int size2;
            using (var size1_ptr = new ComMemory(Marshal.SizeOf<uint>()))
            using (var size2 = new ComMemory(Marshal.SizeOf<uint>()))
            {
                buffer.Lock(out ptr, size1_ptr.Pointer, size2.Pointer);
                size1 = Marshal.ReadInt32(size1_ptr.Pointer);
            }

            byte[] bb = new byte[size1];
            Marshal.Copy(ptr, bb, 0, size1);
            return BitmapSource.Create((int)w, (int)h, 96, 96, PixelFormats.Bgr32, null, bb, (int)w * 4);
        }

    }
}
