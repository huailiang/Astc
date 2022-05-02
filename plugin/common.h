//
// Created by huailiang on 2022/5/2.
//

#ifndef ASTCPLUGIN_COMMON_H
#define ASTCPLUGIN_COMMON_H

#if defined(__CYGWIN32__)
#define ASTC_API __declspec(dllexport)
#elif defined(WIN32) || defined(_WIN32) || defined(__WIN32__) || defined(_WIN64) || defined(WINAPI_FAMILY)
#define ASTC_API __declspec(dllexport)
#elif defined(__MACH__) || defined(__ANDROID__) || defined(__linux__) || defined(__QNX__)
#define ASTC_API
#else
#define ASTC_API
#endif

#endif //ASTCPLUGIN_COMMON_H
