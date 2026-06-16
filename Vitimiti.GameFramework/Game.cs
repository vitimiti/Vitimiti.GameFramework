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
using static Vitimiti.GameFramework.NativeInterop.Ffi;

namespace Vitimiti.GameFramework;

public abstract partial class Game(ILoggerFactory? loggerFactory = null) : IDisposable
{
    private static int _isRunning; // 0 = false, 1 = true

    private readonly ILogger<Game> _logger =
        loggerFactory?.CreateLogger<Game>() ?? NullLoggerFactory.Instance.CreateLogger<Game>();

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

    private void SetUnhandledExceptionHandlers()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogUnhandledException(_logger, ex);
                try
                {
                    if (
                        !SDL_ShowSimpleMessageBox(
                            SDL_MESSAGEBOX_ERROR,
                            "Unhandled Exception",
                            ex.ToString(),
                            SDL_Window.Null
                        )
                    )
                    {
                        LogUnableToShowMessageBox(_logger, SDL_GetError());
                    }
                }
                catch (Exception ex2)
                {
                    LogExceptionWhenShowingMessageBox(_logger, ex2);
                }
            }
            else
            {
                var exceptionObjectString =
                    e.ExceptionObject?.ToString() ?? "Unknown unhandled exception object.";

                LogUnknownUnhandledException(_logger, exceptionObjectString);
                try
                {
                    if (
                        !SDL_ShowSimpleMessageBox(
                            SDL_MESSAGEBOX_ERROR,
                            "Unknown Unhandled Exception",
                            exceptionObjectString,
                            SDL_Window.Null
                        )
                    )
                    {
                        LogUnableToShowMessageBox(_logger, SDL_GetError());
                    }
                }
                catch (Exception ex2)
                {
                    LogExceptionWhenShowingMessageBox(_logger, ex2);
                }
            }
        };
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

        SetUnhandledExceptionHandlers();
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

    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Critical,
        Message = "Unhandled exception occurred."
    )]
    private static partial void LogUnhandledException(ILogger logger, Exception exception);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Critical,
        Message = "Unknown unhandled exception object occurred: {Obj}."
    )]
    private static partial void LogUnknownUnhandledException(ILogger logger, object obj);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Error,
        Message = "Unable to show message box: {Error}"
    )]
    private static partial void LogUnableToShowMessageBox(ILogger logger, string error);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Error,
        Message = "Exception when showing message box."
    )]
    private static partial void LogExceptionWhenShowingMessageBox(
        ILogger logger,
        Exception exception
    );
}
