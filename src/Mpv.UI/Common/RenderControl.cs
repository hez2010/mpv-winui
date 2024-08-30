﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using WinRT;

namespace MpvWinUI.Common;

public unsafe class RenderControl : OpenGLRenderControlBase<FrameBuffer>
{
    private SwapChainPanel _swapChainPanel;

    public ContextSettings Setting { get; set; } = new ContextSettings();
    public RenderContext Context { get; private set; }

    public event EventHandler Ready;
    public event Action<TimeSpan> Render;

    public double ScaleX => _swapChainPanel?.CompositionScaleX ?? 1;
    public double ScaleY => _swapChainPanel?.CompositionScaleY ?? 1;

    public RenderControl()
    {
        SizeChanged += OnSizeChanged;
        Unloaded += OnUnloaded;
    }

    public override void Initialize()
    {
        if (Context == null)
        {
            base.Initialize();
            Context = new RenderContext(Setting);
            _swapChainPanel = new SwapChainPanel();
            _swapChainPanel.CompositionScaleChanged += OnCompositionScaleChanged;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;
            Content = _swapChainPanel;

            if (!TryLoadFrameBuffer())
            {
                UpdateFrameBufferSize();
            }

            Ready?.Invoke(this, EventArgs.Empty);
        }
    }

    public int GetBufferHandle()
        => FrameBuffer.GLFrameBufferHandle;

    protected override void Draw()
    {
        FrameBuffer.Begin();
        Render?.Invoke(_stopwatch.Elapsed - _lastFrameStamp);
        FrameBuffer.End();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Release();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (Context != null && e.NewSize.Width > 0 && e.NewSize.Height > 0)
        {
            if (!TryLoadFrameBuffer())
            {
                UpdateFrameBufferSize();
            }
        }
    }

    private void OnCompositionScaleChanged(SwapChainPanel sender, object args)
        => UpdateFrameBufferSize();

    private void UpdateFrameBufferSize()
        => FrameBuffer?.UpdateSize((int)ActualWidth, (int)ActualHeight, ScaleX, ScaleY);

    private bool TryLoadFrameBuffer()
    {
        if (FrameBuffer != null)
        {
            return false;
        }

        FrameBuffer = new FrameBuffer(Context, (int)ActualWidth, (int)ActualHeight, ScaleX, ScaleY);
        _swapChainPanel.As<ISwapChainPanelNative>().SetSwapChain(FrameBuffer.SwapChainHandle);
        return true;
    }
}
