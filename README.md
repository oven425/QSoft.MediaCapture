# Important hint
if submodule Direct or Direct Core show new version, please don't update

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
var camera = QSoft.MediaCapture.WebCam_MF.GetAllWebCams().Find(x => x.FriendName == "USB2.0 HD UVC WebCam");
if(camera != null)
{
    await camera.InitCaptureEngine();
    await camera.StartPreview(host.Child.Handle);
}
```
try below code when use WPF
```c#
var camera = QSoft.MediaCapture.WebCam_MF.GetAllWebCams().Find(x => x.FriendName == "USB2.0 HD UVC WebCam");
if(camera != null)
{
    await camera.InitCaptureEngine();
    await camera.StartPreview(x => this.image.Source = x);
}

```