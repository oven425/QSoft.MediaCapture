// See https://aka.ms/new-console-template for more information

using QSoft.MediaCapture;

var allwebcams = WebCam_MF.GetAllWebCams();
foreach(var webcam in allwebcams)
{
    Console.WriteLine($"{webcam.FriendName}");
}    
Console.WriteLine("Hello, World!");


