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

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.Extensions.Logging;
using static Vitimiti.GameFramework.NativeInterop.Ffi;

namespace Vitimiti.GameFramework.Internals;

internal sealed partial class SdlLogger(ILogger<SdlLogger> logger) : IDisposable
{
    private delegate void LogOutput(
        SDL_LogCategory category,
        SDL_LogPriority priority,
        string message
    );

    private readonly ILogger<SdlLogger> _logger = logger;

    private GCHandle _logOutputFunctionHandle;
    private bool _disposedValue;

    public void Initialize()
    {
        ObjectDisposedException.ThrowIf(_disposedValue, this);
        SDL_SetLogPriorities(SDL_LogPriority.FromLogger(_logger));
        LogOutput logOutputFunction = (category, priority, message) =>
        {
            var level = priority.ToLogLevel();
            Log(_logger, level, category, message);
        };

        _logOutputFunctionHandle = GCHandle.Alloc(logOutputFunction);
        unsafe
        {
            SDL_SetLogOutputFunction(
                &LogOutputFunction,
                (void*)GCHandle.ToIntPtr(_logOutputFunctionHandle)
            );
        }
    }

    private void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            // No managed resources to dispose of in this class, but if there were any, they would be disposed of here.
        }

        if (_logOutputFunctionHandle.IsAllocated)
        {
            _logOutputFunctionHandle.Free();
        }

        _disposedValue = true;
    }

    ~SdlLogger()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void LogOutputFunction(
        void* userdata,
        SDL_LogCategory category,
        SDL_LogPriority priority,
        byte* message
    )
    {
        if (userdata is null)
        {
            return;
        }

        var handle = GCHandle.FromIntPtr((nint)userdata);
        if (!handle.IsAllocated || handle.Target is not LogOutput callback)
        {
            return;
        }

        var messageString = Utf8StringMarshaller.ConvertToManaged(message) ?? string.Empty;
        callback(category, priority, messageString);
    }

    [LoggerMessage(EventId = 9000, Message = "[{Category}] {Message}")]
    private static partial void Log(
        ILogger logger,
        LogLevel logLevel,
        SDL_LogCategory category,
        string message
    );
}
