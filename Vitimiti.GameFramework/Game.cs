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
using Vitimiti.GameFramework.Internals;

namespace Vitimiti.GameFramework;

public class Game(ILoggerFactory? loggerFactory = null) : IDisposable
{
    private SdlContext? _sdlContext = new(loggerFactory);

    private bool _disposedValue;

    [MemberNotNull(nameof(_sdlContext))]
    public void Run()
    {
        ObjectDisposedException.ThrowIf(_disposedValue, this);
        Initialize();
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
