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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using static Vitimiti.GameFramework.NativeInterop.Ffi;

namespace Vitimiti.GameFramework.Internals;

internal sealed partial class SdlContext : IDisposable
{
    private SdlLogger? _sdlLogger;
    private bool _disposedValue;

    public SdlContext(ILoggerFactory? loggerFactory = null)
    {
        SDL_SetMainReady();
        _sdlLogger = new SdlLogger(
            loggerFactory?.CreateLogger<SdlLogger>()
                ?? NullLoggerFactory.Instance.CreateLogger<SdlLogger>()
        );
    }

    [MemberNotNull(nameof(_sdlLogger))]
    public void Initialize()
    {
        ObjectDisposedException.ThrowIf(_disposedValue, this);
        if (_sdlLogger is null)
        {
            throw new InvalidOperationException(
                $"Cannot initialize {nameof(SdlContext)} because the {nameof(SdlLogger)} is null."
            );
        }

        _sdlLogger.Initialize();
    }

    private void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            _sdlLogger?.Dispose();
        }

        SDL_Quit();
        _sdlLogger = null;

        _disposedValue = true;
    }

    ~SdlContext()
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
}
