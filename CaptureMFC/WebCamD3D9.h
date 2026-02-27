#pragma once
#include <mfidl.h>
#include <mfapi.h>
#include <mfreadwrite.h>
#include <d3d9.h>
#include <initguid.h> 
#include <evr.h>
#include <dxva2api.h>


#pragma comment(lib, "Mfuuid.lib")
#pragma comment(lib, "Mfplat.lib")
#pragma comment(lib, "Mf.lib")
#pragma comment(lib, "Mfreadwrite.lib")
#pragma comment(lib, "Evr.lib")
#pragma comment(lib, "D3d9.lib")
#pragma comment(lib, "dxva2.lib")



class WebCamD3D9
{
public:
    WebCamD3D9()
    {
		::MFStartup(MF_VERSION);
        InitD3D9();
    }
    void Start()
    {
        IMFMediaSource* pSource = NULL;
        HRESULT hr = CreateVideoDeviceSource(&pSource);
        if (SUCCEEDED(hr))
        {
            IMFSourceReader* m_pSourceReader;
            IMFAttributes* pAttributes = NULL;
            hr = MFCreateAttributes(&pAttributes, 5);
            pAttributes->SetUINT32(MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS, TRUE);
            pAttributes->SetUINT32(MF_SOURCE_READER_DISABLE_DXVA, FALSE);
            pAttributes->SetUINT32(MF_SOURCE_READER_ENABLE_VIDEO_PROCESSING, FALSE);
            pAttributes->SetUINT32(MF_SOURCE_READER_ENABLE_ADVANCED_VIDEO_PROCESSING, TRUE);
            pAttributes->SetUnknown(MF_SOURCE_READER_D3D_MANAGER, pDeviceManager);
			
            hr = ::MFCreateSourceReaderFromMediaSource(pSource, pAttributes, &m_pSourceReader);
			CComPtr<IMFMediaType> pMediaType;
 			hr = m_pSourceReader->GetCurrentMediaType((DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM, &pMediaType);
			GUID subtype;
            hr = pMediaType->GetGUID(MF_MT_SUBTYPE, &subtype);
			hr = pMediaType->SetGUID(MF_MT_SUBTYPE, MFVideoFormat_RGB32);

            hr = m_pSourceReader->SetCurrentMediaType((DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM, NULL, pMediaType);
            while (true)
            {

                DWORD index = 0;
                DWORD flag = 0;
				CComPtr<IMFSample> sample;
                LONGLONG timestamp;

                hr = m_pSourceReader->ReadSample((DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM, 0, &index, &flag, &timestamp, &sample);
                if(sample == nullptr)
                {
                    continue;
				}
                CComPtr<IMFMediaBuffer> pBuffer;

                HRESULT hr = sample->GetBufferByIndex(0, &pBuffer);
                if (SUCCEEDED(hr))
                {
                    IDirect3DSurface9* ppSurface = nullptr;
                    hr = MFGetService(pBuffer, MR_BUFFER_SERVICE, IID_PPV_ARGS(&ppSurface));
                    ppSurface->Release();
                }

            }


            // Use the media source for capture or preview.
            pSource->Release();
        }
	}
private:
    IDirect3D9* pD3D = nullptr;
    IDirect3D9Ex* pD3D9Ex = nullptr;
    IDirect3DDevice9* pDevice = nullptr;
    IDirect3DDevice9Ex* pDeviceEx = nullptr;
    IDirect3DDeviceManager9* pDeviceManager = nullptr;
    void InitD3D9Ex()
    {
        HRESULT hr = Direct3DCreate9Ex(D3D_SDK_VERSION, &pD3D9Ex);
        D3DPRESENT_PARAMETERS d3dpp;
        ZeroMemory(&d3dpp, sizeof(d3dpp));
        d3dpp.Windowed = TRUE;
        d3dpp.SwapEffect = D3DSWAPEFFECT_DISCARD;
        d3dpp.hDeviceWindow = GetDesktopWindow();
        hr = pD3D9Ex->CreateDeviceEx(
            D3DADAPTER_DEFAULT,
            D3DDEVTYPE_HAL,
            GetDesktopWindow(),
            D3DCREATE_HARDWARE_VERTEXPROCESSING | D3DCREATE_MULTITHREADED | D3DCREATE_FPU_PRESERVE,
            &d3dpp,
            nullptr,
            &pDeviceEx
        );
        UINT resetToken = 0;
        hr = DXVA2CreateDirect3DDeviceManager9(&resetToken, &pDeviceManager);
        if (FAILED(hr))
        {
            pDevice->Release();
            return;
        }
        hr = pDeviceManager->ResetDevice(pDeviceEx, resetToken);
        if (FAILED(hr))
        {
            pDeviceManager->Release();
            pDevice->Release();
            return;
        }
	}
    void InitD3D9()
    {
        pD3D = Direct3DCreate9(D3D_SDK_VERSION);
        D3DPRESENT_PARAMETERS d3dpp;
        ZeroMemory(&d3dpp, sizeof(d3dpp));
        d3dpp.Windowed = TRUE;
        d3dpp.SwapEffect = D3DSWAPEFFECT_DISCARD;
        d3dpp.hDeviceWindow = GetDesktopWindow();
        HRESULT hr = pD3D->CreateDevice(
            D3DADAPTER_DEFAULT,
            D3DDEVTYPE_HAL,
            GetDesktopWindow(),
            D3DCREATE_HARDWARE_VERTEXPROCESSING| D3DCREATE_MULTITHREADED| D3DCREATE_FPU_PRESERVE,
            &d3dpp,
            &pDevice
        );
        UINT resetToken = 0;
        hr = DXVA2CreateDirect3DDeviceManager9(&resetToken, &pDeviceManager);
        if (FAILED(hr))
        {
            pDevice->Release();
            return;
        }

        hr = pDeviceManager->ResetDevice(pDevice, resetToken);
        if (FAILED(hr))
        {
            pDeviceManager->Release();
            pDevice->Release();
            return;
        }
	}
    HRESULT CreateVideoDeviceSource(IMFMediaSource** ppSource)
    {
        *ppSource = nullptr;

        IMFMediaSource* pSource = NULL;
        IMFAttributes* pAttributes = NULL;
        IMFActivate** ppDevices = NULL;

        // Create an attribute store to specify the enumeration parameters.
        HRESULT hr = MFCreateAttributes(&pAttributes, 1);
        if (FAILED(hr))
        {
            goto done;
        }

        // Source type: video capture devices
        hr = pAttributes->SetGUID(
            MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE,
            MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID
        );
        if (FAILED(hr))
        {
            goto done;
        }

        // Enumerate devices.
        UINT32 count;
        hr = MFEnumDeviceSources(pAttributes, &ppDevices, &count);
        if (FAILED(hr))
        {
            goto done;
        }

        if (count == 0)
        {
            hr = E_FAIL;
            goto done;
        }

        // Create the media source object.
        hr = ppDevices[0]->ActivateObject(IID_PPV_ARGS(&pSource));
        if (FAILED(hr))
        {
            goto done;
        }

        *ppSource = pSource;
        (*ppSource)->AddRef();

    done:
        //SafeRelease(&pAttributes);

        for (DWORD i = 0; i < count; i++)
        {
            //SafeRelease(&ppDevices[i]);
        }
        CoTaskMemFree(ppDevices);
        //SafeRelease(&pSource);
        return hr;
    }
};

