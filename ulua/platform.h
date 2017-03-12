// Copyright (C) Pixel Studio
// All rights reserved.
// 
// Author : HongYiHan
// Date   : 2/25/2017
// Comment: ulua dependence
// 

#ifdef __ANDROID__
#	define ULUA_INLINE inline
#else
#	define ULUA_INLINE
#endif

#if defined(__ANDROID__) || \
    defined(__x86_64__) || \
    defined(__MACOS__)
#define ulua_snprintf snprintf
#define ulua_strncasecmp strncasecmp
#else
#define ulua_snprintf _snprintf
#define ulua_strncasecmp  _strnicmp
#endif
