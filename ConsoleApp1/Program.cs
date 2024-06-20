// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System;
//https://github.com/dotnet/samples/tree/main/core/interop/comwrappers/Tutorial
Console.WriteLine("Hello, World!");
//var webcam = QSoft.MediaCapture.WebCam_MF.GetAllWebCams();
//var hr = await webcam.InitCaptureEngine();
var hr = MFFuns.MFCreateAttributes(out var attrs, 1);
hr = 100;


[GeneratedComInterface]
[Guid("2cd2d921-c447-44a7-a13c-4adabfc247e3")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public partial interface IMFAttributes
{
    [PreserveSig]
    int GetItem(Guid guidKey, [In][Out] PROPVARIANT pValue);

    [PreserveSig]
    int GetItemType(Guid guidKey, out _MF_ATTRIBUTE_TYPE pType);

    [PreserveSig]
    int CompareItem(    Guid guidKey, [In][Out] PROPVARIANT Value, out bool pbResult);

    [PreserveSig]
    int Compare(IMFAttributes pTheirs, _MF_ATTRIBUTES_MATCH_TYPE MatchType, out bool pbResult);

    [PreserveSig]
    int GetUINT32(Guid guidKey, out uint punValue);

    [PreserveSig]
    int GetUINT64(Guid guidKey, out ulong punValue);

    [PreserveSig]
    int GetDouble(Guid guidKey, out double pfValue);

    [PreserveSig]
    int GetGUID(Guid guidKey, out Guid pguidValue);

    [PreserveSig]
    int GetStringLength(Guid guidKey, out uint pcchLength);

    [PreserveSig]
    int GetString(Guid guidKey, [MarshalAs(UnmanagedType.LPWStr)] string pwszValue, uint cchBufSize, IntPtr pcchLength);

    [PreserveSig]
    int GetAllocatedString(Guid guidKey, IntPtr ppwszValue, out uint pcchLength);

    [PreserveSig]
    int GetBlobSize(Guid guidKey, out uint pcbBlobSize);

    [PreserveSig]
    int GetBlob(Guid guidKey, [Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf, int cbBufSize, IntPtr pcbBlobSize);

    [PreserveSig]
    int GetAllocatedBlob(Guid guidKey, out IntPtr ppBuf, out uint pcbSize);

    [PreserveSig]
    int GetUnknown(Guid guidKey, Guid riid,  out IntPtr ppv);

    [PreserveSig]
    int SetItem(Guid guidKey, [In][Out] PROPVARIANT Value);

    [PreserveSig]
    int DeleteItem(Guid guidKey);

    [PreserveSig]
    int DeleteAllItems();

    [PreserveSig]
    int SetUINT32(Guid guidKey, uint unValue);

    [PreserveSig]
    int SetUINT64(Guid guidKey, ulong unValue);

    [PreserveSig]
    int SetDouble(Guid guidKey, double fValue);

    [PreserveSig]
    int SetGUID(Guid guidKey, Guid guidValue);

    [PreserveSig]
    int SetString(Guid guidKey, [MarshalAs(UnmanagedType.LPWStr)] string wszValue);

    [PreserveSig]
    int SetBlob(Guid guidKey, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf, int cbBufSize);

    [PreserveSig]
    int SetUnknown(Guid guidKey, IntPtr pUnknown);

    [PreserveSig]
    int LockStore();

    [PreserveSig]
    int UnlockStore();

    [PreserveSig]
    int GetCount(out uint pcItems);

    [PreserveSig]
    int GetItemByIndex(uint unIndex, out Guid pguidKey, [In][Out] PROPVARIANT pValue);

    [PreserveSig]
    int CopyAllItems(IMFAttributes pDest);
}

public partial class MFFuns
{
    [LibraryImport("Mfplat", EntryPoint = "MFCreateAttributes")]
    [return: MarshalAs(UnmanagedType.I4)]
    public static partial int MFCreateAttributes(out IMFAttributes ppMFAttributes, uint cInitialSize);
}

