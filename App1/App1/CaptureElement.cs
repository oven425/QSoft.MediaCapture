
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Storage;

public class CaptureElement(MediaPlayerElement mediaElement)
{
    MediaFrameSource? frameSource;
    MediaCapture? m_MediaCapture;
    public async Task InitAsync()
    {
        
        var deviceInfoCollection = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture).AsTask();
        var mediaFrameSourceGroup = await MediaFrameSourceGroup.FindAllAsync().AsTask();
        var videoCaptureSourceGroup = mediaFrameSourceGroup.Where(sourceGroup => deviceInfoCollection.Any(deviceInfo => deviceInfo.Id == sourceGroup.Id)).ToList();
        var audios = DeviceInformation.FindAllAsync(DeviceClass.AudioCapture).AsTask();
        this.m_MediaCapture = new MediaCapture();
        foreach (var sourceGroup in videoCaptureSourceGroup)
        {
            await m_MediaCapture.InitializeAsync(new MediaCaptureInitializationSettings()
            {
                VideoDeviceId = sourceGroup.Id,
                PhotoCaptureSource = PhotoCaptureSource.Auto,
            }).AsTask();
            
            frameSource = m_MediaCapture.FrameSources.FirstOrDefault(source => source.Value.Info.MediaStreamType == MediaStreamType.VideoRecord).Value;
            mediaElement.AutoPlay = true;
            
            mediaElement.Source = MediaSource.CreateFromMediaFrameSource(frameSource);
        }


    }

    public void Mirror(bool mirror)
    {
        m_MediaCapture.SetPreviewRotation(VideoRotation.Clockwise270Degrees);
        m_MediaCapture.SetPreviewMirroring(mirror);
    }
    public void StartPreview()
    {
        
    }

    public void StopPreview()
    {

    }

    public async Task StartRecordAsync()
    {
        var myVideos = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Videos);
        StorageFile file = await myVideos.SaveFolder.CreateFileAsync("video.mp4", CreationCollisionOption.GenerateUniqueName);
        //_mediaRecording = await mediaCapture.PrepareLowLagRecordToStorageFileAsync(
        //        MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto), file);
        //await _mediaRecording.StartAsync();
        await m_MediaCapture.StartRecordToStorageFileAsync(MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto), file);
    }

    public async Task StopRecordAsync()
    {
        await m_MediaCapture.StopRecordAsync().AsTask();
    }
}