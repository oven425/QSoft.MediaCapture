
// MFCApplication1.h: PROJECT_NAME 應用程式的主要標頭檔
//

#pragma once

#ifndef __AFXWIN_H__
	#error "對 PCH 在包含此檔案前先包含 'pch.h'"
#endif

#include "resource.h"		// 主要符號


// CMFCApplication1App:
// 查看 MFCApplication1.cpp 以了解此類別的實作
//

class CMFCApplication1App : public CWinApp
{
public:
	CMFCApplication1App();

// 覆寫
public:
	virtual BOOL InitInstance();

// 程式碼實作

	DECLARE_MESSAGE_MAP()
};

extern CMFCApplication1App theApp;
