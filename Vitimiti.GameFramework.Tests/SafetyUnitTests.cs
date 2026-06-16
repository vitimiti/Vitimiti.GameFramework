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

namespace Vitimiti.GameFramework.Tests;

public class SafetyUnitTests
{
    private class TestMainLoop : Game;

    private TestMainLoop? _testMainLoop;

    [Fact]
    public void OnlyOneRunningInstanceIsValid()
    {
        _testMainLoop = new TestMainLoop();
        Assert.NotNull(_testMainLoop);
        _testMainLoop.Run();
        Assert.Throws<InvalidOperationException>(() => _testMainLoop.Run());
        _testMainLoop.Dispose();
    }

    [Fact]
    public void ExecutionAfterDisposeIsInvalid()
    {
        _testMainLoop = new TestMainLoop();
        Assert.NotNull(_testMainLoop);
        _testMainLoop.Dispose();
        Assert.Throws<ObjectDisposedException>(() => _testMainLoop.Run());
    }
}
