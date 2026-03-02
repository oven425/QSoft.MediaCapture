#pragma once
#include "WebCamD3D9.h"
class WebCamD3D9Async:public WebCamD3D9, IMFSourceReaderCallback
{
public:
    WebCamD3D9Async(HANDLE hEvent) :
        m_nRefCount(1), m_hEvent(hEvent), m_bEOS(FALSE), m_hrStatus(S_OK)
    {
        InitializeCriticalSection(&m_critsec);
    }
	void Start() override
	{
        IMFMediaSource* pSource = NULL;
        HRESULT hr = CreateVideoDeviceSource(&pSource);
        if (SUCCEEDED(hr))
        {
            IMFAttributes* pAttributes = NULL;
            hr = MFCreateAttributes(&pAttributes, 6);
            pAttributes->SetUINT32(MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS, TRUE);
            pAttributes->SetUINT32(MF_SOURCE_READER_DISABLE_DXVA, FALSE);
            pAttributes->SetUINT32(MF_SOURCE_READER_ENABLE_VIDEO_PROCESSING, FALSE);
            pAttributes->SetUINT32(MF_SOURCE_READER_ENABLE_ADVANCED_VIDEO_PROCESSING, TRUE);
            hr = pAttributes->SetUnknown(MF_SOURCE_READER_ASYNC_CALLBACK, this);
            pAttributes->SetUnknown(MF_SOURCE_READER_D3D_MANAGER, pDeviceManager);

            hr = ::MFCreateSourceReaderFromMediaSource(pSource, pAttributes, &m_pSourceReader);
            CComPtr<IMFMediaType> pMediaType;
            hr = m_pSourceReader->GetCurrentMediaType((DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM, &pMediaType);
            GUID subtype;
            hr = pMediaType->GetGUID(MF_MT_SUBTYPE, &subtype);
            hr = pMediaType->SetGUID(MF_MT_SUBTYPE, MFVideoFormat_RGB32);

            //hr = m_pSourceReader->SetCurrentMediaType((DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM, NULL, pMediaType);
            
            hr = m_pSourceReader->ReadSample(MF_SOURCE_READER_FIRST_VIDEO_STREAM, 0, NULL, NULL, NULL, NULL);

            //while (true)
            //{

            //    DWORD index = 0;
            //    DWORD flag = 0;
            //    CComPtr<IMFSample> sample;
            //    LONGLONG timestamp;

            //    hr = m_pSourceReader->ReadSample((DWORD)MF_SOURCE_READER_FIRST_VIDEO_STREAM, 0, &index, &flag, &timestamp, &sample);
            //    if (sample == nullptr)
            //    {
            //        continue;
            //    }
            //    CComPtr<IMFMediaBuffer> pBuffer;

            //    HRESULT hr = sample->GetBufferByIndex(0, &pBuffer);
            //    if (SUCCEEDED(hr))
            //    {
            //        IDirect3DSurface9* ppSurface = nullptr;
            //        hr = MFGetService(pBuffer, MR_BUFFER_SERVICE, IID_PPV_ARGS(&ppSurface));
            //        ppSurface->Release();
            //    }

            //}


            // Use the media source for capture or preview.
        }
	}
    // IUnknown methods
    STDMETHODIMP QueryInterface(REFIID iid, void** ppv)
    {
        static const QITAB qit[] =
        {
            QITABENT(WebCamD3D9Async, IMFSourceReaderCallback),
            { 0 },
        };
        return QISearch(this, qit, iid, ppv);
    }
    STDMETHODIMP_(ULONG) AddRef()
    {
        return InterlockedIncrement(&m_nRefCount);
    }
    STDMETHODIMP_(ULONG) Release()
    {
        ULONG uCount = InterlockedDecrement(&m_nRefCount);
        if (uCount == 0)
        {
            delete this;
        }
        return uCount;
    }

    // IMFSourceReaderCallback methods
    STDMETHODIMP OnReadSample(HRESULT hrStatus, DWORD dwStreamIndex, DWORD dwStreamFlags, LONGLONG llTimestamp, IMFSample* pSample) 
    {
        if(pSample != nullptr)
        {
            CComPtr<IMFMediaBuffer> pBuffer;

            HRESULT hr = pSample->GetBufferByIndex(0, &pBuffer);
            if (SUCCEEDED(hr))
            {
                IDirect3DSurface9* ppSurface = nullptr;
                hr = MFGetService(pBuffer, MR_BUFFER_SERVICE, IID_PPV_ARGS(&ppSurface));
                ppSurface->Release();
            }
		}
        
        auto hr = m_pSourceReader->ReadSample(MF_SOURCE_READER_FIRST_VIDEO_STREAM, 0, NULL, NULL, NULL, NULL);

        return S_OK;
    }

    STDMETHODIMP OnEvent(DWORD, IMFMediaEvent*)
    {
        return S_OK;
    }

    STDMETHODIMP OnFlush(DWORD)
    {
        return S_OK;
    }
private:
    long                m_nRefCount;        // Reference count.
    CRITICAL_SECTION    m_critsec;
    HANDLE              m_hEvent;
    BOOL                m_bEOS;
    HRESULT             m_hrStatus;
};

