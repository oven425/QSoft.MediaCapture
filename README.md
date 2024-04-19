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
var camera = QSoft.MediaCapture.WebCam_MF.GetAllWebCams().Find(x => x.FriendName == "USB2.0 HD UVC WebCam");
if(camera != null)
{
    await camera.InitCaptureEngine();
    await camera.SetPreviewSize(x => x.FirstOrDefault(y=>y.Width>=640));
    await camera.StartPreview(host.Child.Handle);
}
```
try below code when use WPF
```c#
var camera = QSoft.MediaCapture.WebCam_MF.GetAllWebCams().Find(x => x.FriendName == "USB2.0 HD UVC WebCam");
if(camera != null)
{
    await camera.InitCaptureEngine();
    await camera.SetPreviewSize(x => x.FirstOrDefault(y=>y.Width>=640));
    await camera.StartPreview(x => this.image.Source = x);
}

```

open multi camera same time in WPF
```c#
var aa = QSoft.MediaCapture.WebCam_MF.GetAllWebCams()
    .Select(async (camera, i) =>
    {
        await Task.Run(async () =>
        {
            await camera.InitCaptureEngine();
            //await camera.SetPreviewSize(x => x.ElementAt(1));
            await camera.StartPreview(x =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.image.Source = x;
                });

            });
        });

    }).ToArray();

await Task.WhenAll(aa);

```
