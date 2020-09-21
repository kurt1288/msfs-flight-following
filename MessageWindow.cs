using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MSFSFlightFollowing
{
   public class MessageWindow : IDisposable
   {
      public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      struct WNDCLASS
      {
         public uint style;
         public IntPtr lpfnWndProc;
         public int cbClsExtra;
         public int cbWndExtra;
         public IntPtr hInstance;
         public IntPtr hIcon;
         public IntPtr hCursor;
         public IntPtr hbrBackground;
         [MarshalAs(UnmanagedType.LPWStr)]
         public string lpszMenuName;
         [MarshalAs(UnmanagedType.LPWStr)]
         public string lpszClassName;
      }

      [StructLayout(LayoutKind.Sequential)]
      public struct MSG
      {
         public IntPtr hwnd;
         public uint message;
         public IntPtr wParam;
         public IntPtr lParam;
         public uint time;
      }

      [DllImport("user32.dll", SetLastError = true)]
      static extern UInt16 RegisterClassW([In] ref WNDCLASS lpWndClass);

      [DllImport("user32.dll", SetLastError = true)]
      static extern IntPtr CreateWindowExW(
         UInt32 dwExStyle,
         [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
         [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
         UInt32 dwStyle,
         Int32 x,
         Int32 y,
         Int32 nWidth,
         Int32 nHeight,
         IntPtr hWndParent,
         IntPtr hMenu,
         IntPtr hInstance,
         IntPtr lpParam
      );

      [DllImport("user32.dll", SetLastError = true)]
      static extern IntPtr DefWindowProcW(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

      [DllImport("user32.dll", SetLastError = true)]
      static extern bool DestroyWindow(IntPtr hWnd);

      [DllImport("user32.dll")]
      public static extern sbyte GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

      [DllImport("user32.dll")]
      public static extern IntPtr DispatchMessage(ref MSG lpmsg);

      [DllImport("user32.dll")]
      public static extern bool TranslateMessage(ref MSG lpMsg);

      private const int ERROR_CLASS_ALREADY_EXISTS = 1410;

      private bool disposed = false;
      public IntPtr Hwnd { get; private set; }

      public void Dispose()
      {
         if (!disposed)
         {
            // Dispose unmanaged resources
            if (Hwnd != IntPtr.Zero)
            {
               DestroyWindow(Hwnd);
               Hwnd = IntPtr.Zero;
            }
            disposed = true;
         }
         GC.SuppressFinalize(this);
      }

      static MessageWindow instance = null;
      public static MessageWindow GetWindow()
      {
         if (instance == null)
            instance = new MessageWindow();
         return instance;
      }

      private MessageWindow()
      {
         var className = Assembly.GetExecutingAssembly().GetName().Name;
         wndProcDelegate = CustomWndProc;
         WNDCLASS windClass = new WNDCLASS { lpszClassName = className, lpfnWndProc = Marshal.GetFunctionPointerForDelegate(wndProcDelegate) };
         UInt16 classAtom = RegisterClassW(ref windClass);
         int lastError = Marshal.GetLastWin32Error();
         if (classAtom == 0 && lastError != ERROR_CLASS_ALREADY_EXISTS)
            throw new System.Data.DuplicateNameException();
         Hwnd = CreateWindowExW(0, className, "MessagePump", 0, 0, 0, 10, 10, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
      }

      private IntPtr CustomWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
      {
         WndProcHandle?.Invoke(hWnd, msg, wParam, lParam);
         return DefWindowProcW(hWnd, msg, wParam, lParam);
      }

      public static void MessageLoop()
      {
         var window = GetWindow(); // Ensure an instance has been created.
         var msg = new MSG();
         int r;
         // Standard WIN32 message loop
         while ((r = GetMessage(out msg, IntPtr.Zero, 0, 0)) > 0)
         {
            TranslateMessage(ref msg);
            DispatchMessage(ref msg);
         }
      }

      public event WndProc WndProcHandle;

      private WndProc wndProcDelegate;
   }
}
