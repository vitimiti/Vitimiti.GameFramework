// The MIT License (MIT)
//
// Copyright (C) 2026 Victor Matia (vitimiti)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software
// and associated documentation files (the “Software”), to deal in the Software without
// restriction, including without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom
// the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
// BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Vitimiti.GameFramework.NativeInterop;

internal static unsafe partial class Ffi
{
    #region SDL_init.h

    [LibraryImport(LibSdl3)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SDL_Quit();

    #endregion // SDL_init.h

    #region SDL_log.h

    public readonly record struct SDL_LogCategory(int Value)
    {
        public override string ToString()
        {
            if (this == SDL_LOG_CATEGORY_APPLICATION)
            {
                return "Application";
            }
            if (this == SDL_LOG_CATEGORY_ERROR)
            {
                return "Error";
            }
            if (this == SDL_LOG_CATEGORY_ASSERT)
            {
                return "Assert";
            }
            if (this == SDL_LOG_CATEGORY_SYSTEM)
            {
                return "System";
            }
            if (this == SDL_LOG_CATEGORY_AUDIO)
            {
                return "Audio";
            }
            if (this == SDL_LOG_CATEGORY_VIDEO)
            {
                return "Video";
            }
            if (this == SDL_LOG_CATEGORY_RENDER)
            {
                return "Render";
            }
            if (this == SDL_LOG_CATEGORY_INPUT)
            {
                return "Input";
            }
            if (this == SDL_LOG_CATEGORY_TEST)
            {
                return "Test";
            }
            if (this == SDL_LOG_CATEGORY_GPU)
            {
                return "GPU";
            }
            return $"Unknown({Value})";
        }
    }

    public static SDL_LogCategory SDL_LOG_CATEGORY_APPLICATION => new(0);
    public static SDL_LogCategory SDL_LOG_CATEGORY_ERROR => new(1);
    public static SDL_LogCategory SDL_LOG_CATEGORY_ASSERT => new(2);
    public static SDL_LogCategory SDL_LOG_CATEGORY_SYSTEM => new(3);
    public static SDL_LogCategory SDL_LOG_CATEGORY_AUDIO => new(4);
    public static SDL_LogCategory SDL_LOG_CATEGORY_VIDEO => new(5);
    public static SDL_LogCategory SDL_LOG_CATEGORY_RENDER => new(6);
    public static SDL_LogCategory SDL_LOG_CATEGORY_INPUT => new(7);
    public static SDL_LogCategory SDL_LOG_CATEGORY_TEST => new(8);
    public static SDL_LogCategory SDL_LOG_CATEGORY_GPU => new(9);

    public readonly record struct SDL_LogPriority(int Value)
    {
        [SuppressMessage(
            "Style",
            "IDE0046:Convert to conditional expression",
            Justification = "Consecutive ternary operators are less readable than consecutive if statements."
        )]
        public static SDL_LogPriority FromLogger(ILogger logger)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                return SDL_LOG_PRIORITY_TRACE;
            }
            if (logger.IsEnabled(LogLevel.Debug))
            {
                return SDL_LOG_PRIORITY_DEBUG;
            }
            if (logger.IsEnabled(LogLevel.Information))
            {
                return SDL_LOG_PRIORITY_INFO;
            }
            if (logger.IsEnabled(LogLevel.Warning))
            {
                return SDL_LOG_PRIORITY_WARN;
            }
            if (logger.IsEnabled(LogLevel.Error))
            {
                return SDL_LOG_PRIORITY_ERROR;
            }
            if (logger.IsEnabled(LogLevel.Critical))
            {
                return SDL_LOG_PRIORITY_CRITICAL;
            }

            return SDL_LOG_PRIORITY_INVALID;
        }

        [SuppressMessage(
            "Style",
            "IDE0046:Convert to conditional expression",
            Justification = "Consecutive ternary operators are less readable than consecutive if statements."
        )]
        public LogLevel ToLogLevel()
        {
            if (this == SDL_LOG_PRIORITY_TRACE)
            {
                return LogLevel.Trace;
            }
            if (this == SDL_LOG_PRIORITY_DEBUG)
            {
                return LogLevel.Debug;
            }
            if (this == SDL_LOG_PRIORITY_INFO)
            {
                return LogLevel.Information;
            }
            if (this == SDL_LOG_PRIORITY_WARN)
            {
                return LogLevel.Warning;
            }
            if (this == SDL_LOG_PRIORITY_ERROR)
            {
                return LogLevel.Error;
            }
            if (this == SDL_LOG_PRIORITY_CRITICAL)
            {
                return LogLevel.Critical;
            }
            return LogLevel.None;
        }
    }

    public static SDL_LogPriority SDL_LOG_PRIORITY_INVALID => new(0);
    public static SDL_LogPriority SDL_LOG_PRIORITY_TRACE => new(1);
    public static SDL_LogPriority SDL_LOG_PRIORITY_VERBOSE => new(2);
    public static SDL_LogPriority SDL_LOG_PRIORITY_DEBUG => new(3);
    public static SDL_LogPriority SDL_LOG_PRIORITY_INFO => new(4);
    public static SDL_LogPriority SDL_LOG_PRIORITY_WARN => new(5);
    public static SDL_LogPriority SDL_LOG_PRIORITY_ERROR => new(6);
    public static SDL_LogPriority SDL_LOG_PRIORITY_CRITICAL => new(7);

    [LibraryImport(LibSdl3)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SDL_SetLogPriorities(SDL_LogPriority priority);

    [LibraryImport(LibSdl3)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SDL_SetLogOutputFunction(
        delegate* unmanaged[Cdecl]<void*, SDL_LogCategory, SDL_LogPriority, byte*, void> callback,
        void* userdata
    );

    #endregion // SDL_log.h

    #region SDL_main.h

    [LibraryImport(LibSdl3)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void SDL_SetMainReady();

    #endregion // SDL_main.h
}
