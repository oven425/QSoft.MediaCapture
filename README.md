# Important hint
if submodule Direct or DirectCore show new version, please don't update

# Quick start
## Search all video capture
```c#
foreach(var oo in QSoft.MediaCapture.WebCam_MF.GetAllWebCams())
{
    System.Diagnostics.Trace.WriteLine(oo.FriendName);
    System.Diagnostics.Trace.WriteLine(oo.SymbolLinkName);
}
```
and filter FriendName, SymbolLinkName get it

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
