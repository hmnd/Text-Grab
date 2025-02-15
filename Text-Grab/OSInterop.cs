﻿using System;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;

static class OSInterop
{
    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(int smIndex);
    public const int SM_CMONITORS = 80;

    [DllImport("user32.dll")]
    public static extern bool SystemParametersInfo(int nAction, int nParam, ref RECT rc, int nUpdate);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] MONITORINFOEX info);

    [DllImport("user32.dll")]
    public static extern IntPtr MonitorFromWindow(HandleRef handle, int flags);

    [DllImport("user32.dll")]
    public static extern bool ClipCursor(ref RECT lpRect);

    [DllImport("user32.dll")]
    public static extern bool ClipCursor([In()] IntPtr lpRect);

    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
        public int width { get { return right - left; } }
        public int height { get { return bottom - top; } }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Auto)]
    public class MONITORINFOEX
    {
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));
        public RECT rcMonitor = new RECT();
        public RECT rcWork = new RECT();
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] szDevice = new char[32];
        public int dwFlags;
    }
}

static class WPFExtensionMethods
{
    public static Point GetAbsolutePosition(this Window w)
    {
        if (w.WindowState != WindowState.Maximized)
            return new Point(w.Left, w.Top);

        Int32Rect r;
        bool multimonSupported = OSInterop.GetSystemMetrics(OSInterop.SM_CMONITORS) != 0;
        if (!multimonSupported)
        {
            OSInterop.RECT rc = new OSInterop.RECT();
            OSInterop.SystemParametersInfo(48, 0, ref rc, 0);
            r = new Int32Rect(rc.left, rc.top, rc.width, rc.height);
        }
        else
        {
            WindowInteropHelper helper = new WindowInteropHelper(w);
            IntPtr hmonitor = OSInterop.MonitorFromWindow(new HandleRef(null, helper.EnsureHandle()), 2);
            OSInterop.MONITORINFOEX info = new OSInterop.MONITORINFOEX();
            OSInterop.GetMonitorInfo(new HandleRef(null, hmonitor), info);
            r = new Int32Rect(info.rcMonitor.left, info.rcMonitor.top, info.rcMonitor.width, info.rcMonitor.height);
        }
        return new Point(r.X, r.Y);
    }
}