using Godot;
using System;
using System.Runtime.InteropServices;

public partial class WindowOverlay : Node2D
{
	// WinAPI constants
	const int GWL_EXSTYLE = -20;
	const int WS_EX_LAYERED = 0x80000;
	const int WS_EX_TRANSPARENT = 0x20;

	// WinAPI imports
	[DllImport("user32.dll")]
	static extern IntPtr GetActiveWindow();

	[DllImport("user32.dll")]
	static extern int GetWindowLong(IntPtr hWnd, int nIndex);

	[DllImport("user32.dll")]
	static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

	public override void _Ready()
	{
		// --- lo que YA tenías ---
		var screen = DisplayServer.ScreenGetSize();
		DisplayServer.WindowSetSize(new Vector2I(screen.X, screen.Y - 1)); // workaround 1px
		DisplayServer.WindowSetPosition(Vector2I.Zero);

		// --- NUEVO: click-through ---
		EnableClickThrough();
	}

	void EnableClickThrough()
	{
		var hwnd = GetActiveWindow();
		if (hwnd == IntPtr.Zero)
			return;

		int style = GetWindowLong(hwnd, GWL_EXSTYLE);
		style |= WS_EX_LAYERED;      // mantiene transparencia
		style |= WS_EX_TRANSPARENT; // CLICK-THROUGH

		SetWindowLong(hwnd, GWL_EXSTYLE, style);
	}
}
