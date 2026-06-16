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
using Vitimiti.GameFramework.Internals;

namespace Vitimiti.GameFramework;

public class Game(ILoggerFactory? loggerFactory = null) : IDisposable
{
    private static int _isRunning; // 0 = false, 1 = true

    private int _ownsRunLock; // 0 = false, 1 = true
    private SdlContext? _sdlContext = new(loggerFactory);

    private bool _disposedValue;

    [MemberNotNull(nameof(_sdlContext))]
    public void Run()
    {
        ObjectDisposedException.ThrowIf(_disposedValue, this);
        if (Interlocked.CompareExchange(ref _isRunning, 1, 0) != 0)
        {
            throw new InvalidOperationException(
                $"Cannot run {nameof(Game)} because another {nameof(Game)} instance is already running."
            );
        }

        try
        {
            Interlocked.Exchange(ref _ownsRunLock, 1);
            Initialize();
        }
        catch
        {
            // Roll back lock ownership on startup failure.
            Interlocked.Exchange(ref _ownsRunLock, 0);
            Interlocked.Exchange(ref _isRunning, 0);
            throw;
        }
    }

    [MemberNotNull(nameof(_sdlContext))]
    private void Initialize()
    {
        ObjectDisposedException.ThrowIf(_disposedValue, this);
        if (_sdlContext is null)
        {
            throw new InvalidOperationException(
                $"Cannot initialize {nameof(Game)} because the {nameof(SdlContext)} is null."
            );
        }

        _sdlContext.Initialize();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            _sdlContext?.Dispose();
        }

        _sdlContext = null;
        _disposedValue = true;

        // Only release global lock if this instance acquired it.
        if (Interlocked.Exchange(ref _ownsRunLock, 0) == 1)
        {
            Interlocked.Exchange(ref _isRunning, 0);
        }
    }

    ~Game()
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
