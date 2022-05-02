cmake_minimum_required(VERSION 2.8)


if ( WIN32 AND NOT CYGWIN AND NOT ( CMAKE_SYSTEM_NAME STREQUAL "WindowsStore" ) AND NOT ANDROID)
	set(CMAKE_C_FLAGS_RELEASE "${CMAKE_C_FLAGS_RELEASE} /MT" CACHE STRING "")
	set(CMAKE_C_FLAGS_DEBUG "${CMAKE_C_FLAGS_DEBUG} /MTd" CACHE STRING "")
	set(CMAKE_CXX_FLAGS_RELEASE "${CMAKE_CXX_FLAGS_RELEASE} /MT" CACHE STRING "")
	set(CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG} /MTd" CACHE STRING "")
endif ()

project(xmedia)


if ( IOS )
	set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -fembed-bitcode")
	set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -fembed-bitcode")
endif ()

set(CMAKE_CXX_STANDARD 14)

# 生成可执行程序
#add_executable(xmedia main.cpp common.h astc.cpp astc.h)

#  生成库文件
if (APPLE)
    if (IOS)
        set(CMAKE_OSX_ARCHITECTURES "$(ARCHS_STANDARD)")
        add_library(xmedia STATIC common.h astc.cpp astc.h)
		set_xcode_property (xmedia IPHONEOS_DEPLOYMENT_TARGET "7.0" "all")
    else ()
#        set(CMAKE_OSX_ARCHITECTURES "$(ARCHS_STANDARD_64_BIT)")
        add_library(xmedia MODULE common.h astc.cpp astc.h)
        set_target_properties ( xmedia PROPERTIES BUNDLE TRUE )
    endif ()
elseif ("${CMAKE_SYSTEM_NAME}" STREQUAL "Switch")
    add_library(xmedia STATIC common.h astc.cpp astc.h)
    target_compile_options(xmedia PRIVATE -m64 -mcpu=cortex-a57+fp+simd+crypto+crc -fno-common -fno-short-enums -ffunction-sections -fdata-sections -fPIC -fms-extensions)
else ( )
    add_library(xmedia SHARED common.h astc.cpp astc.h)
endif ( )
