# Important hint
if submodule Direct or DirectCore show new version, please don't update

# Quick start
# Create from symbolLink
```c#
var allcameras = QSoft.DevCon.DevConExtension.KSCATEGORY_VIDEO_CAMERA.DevicesFromInterface()
                    .Select(x => new
                    {
                        symbollink = x.DevicePath(),
                        panel = x.As().Panel()
                    });
var firstcamera = allcameras.FirstOrDefault();
if(firstcamera is null) return;
var camera = WebCam_MF.CreateFromSymbollink(firstcamera.symbollink);
await camera.InitCaptureEngine(new WebCam_MF_Setting()
    {
        IsMirror = firstcamera.panel==CameraPanel.Front
    });
await camera.StartPreview(host.Child.Handle);
```

# Init and start preview
```c#
var camera = QSoft.MediaCapture.WebCam_MF.GetAllWebCams()
    .Find(x => x.FriendName == "USB2.0 HD UVC WebCam");
if(camera != null)
{
    await camera.InitCaptureEngine();
    await camera.SetPreviewSize(x => x.FirstOrDefault(y=>y.Width>=640));
    await camera.StartPreview(host.Child.Handle);
}
```
## Preview for WPF
```c#
var camera = QSoft.MediaCapture.WebCam_MF.GetAllWebCams()
    .Find(x => x.FriendName == "USB2.0 HD UVC WebCam");
if(camera != null)
{
    await camera.InitCaptureEngine();
    await camera.SetPreviewSize(x => x.FirstOrDefault(y=>y.Width>=640));
    await camera.StartPreview(bmp=>this.image.Source = bmp);
}
```

# Take photo
## support format
* jpg
* png
* bmp
* tif,tiff
```c#
var camera = QSoft.MediaCapture.WebCam_MF.GetAllWebCams()
    .Find(x => x.FriendName == "USB2.0 HD UVC WebCam");
if(camera != null)
{
    await camera.TakePhtot("123.jpg");
}
```

# Record
## Support format:
- mp4
    - video: h264
    - audio: aac
- wmv
    - video: h264
    - audio: aac

```c#
var camera = QSoft.MediaCapture.WebCam_MF.GetAllWebCams()
    .Find(x => x.FriendName == "USB2.0 HD UVC WebCam");
if(camera != null)
{
    await camera.StartRecord("123.mp4");
    await Task.Delay(5000);
    await camera.StopRecord();
}

```

# WhiteBalance contorl
first check this device support  this function after Init camera success
```c#
if (camera.WhiteBalanceControl.IsSupport)
{
    //can set predefined value
    camera.WhiteBalanceControl.Preset = ColorTemperaturePreset.Auto;
}

```

# Flash
first check this device support  this function after Init camera success
```c#
if(camera.FlashLight?.IsSupported)
{
    //Flash support function
    var supportfunc = camera.FlashLight.SupportStates;
    //below usually use state if camera supports flashlight
    camera.FlashLight.SetState(FlashState.OFF);
    camera.FlashLight.SetState(FlashState.ON);
    camera.FlashLight.SetState(FlashState.AUTO);
}

```
# Torch
first check this device support  this function after Init camera success
```c#
if(camera.TorchLight?.IsSupported)
{
    //Flash support function
    var supportfunc = camera.TorchLight.SupportStates;
    //below usually use state if camera supports torch light
    camera.TorchLight.SetState(FlashState.OFF);
    camera.TorchLight.SetState(FlashState.ON);
}
```

# EyeGazeCorrection
first check this device support  this function after Init camera success
```c#
if(camera.EyeGazeCorrection?.IsSupported)
{
    //Flash support function
    var supportfunc = camera.TorchLight.SupportStates;
    //below usually use state if camera supports torch light
    camera.EyeGazeCorrection.SetState(EyeGazeCorrectionState.OFF);
    camera.EyeGazeCorrection.SetState(EyeGazeCorrectionState.ON);
}
```

# BackgroundSegmentation
first check this device support  this function after Init camera success
```c#
if(camera.BackgroundSegmentation?.IsSupported)
{
    //Flash support function
    var supportfunc = camera.TorchLight.SupportStates;
    //below usually use state if camera supports torch light
    camera.BackgroundSegmentation.SetState(BackgroundSegmentationState.OFF);
    camera.BackgroundSegmentation.SetState(BackgroundSegmentationState.Blur);
}
```

# DigitalWindow
first check this device support  this function after Init camera success
```c#
if(camera.DigitalWindow?.IsSupported)
{
    //Flash support function
    var supportfunc = camera.TorchLight.SupportStates;
    //below usually use state if camera supports torch light
    camera.DigitalWindow.SetState(DigitalWindowState.Manual);
    camera.DigitalWindow.SetState(DigitalWindowState.AutoFaceFraming);
}
```

