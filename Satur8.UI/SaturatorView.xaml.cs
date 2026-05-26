using Microsoft.Extensions.DependencyInjection;
using Satur8.Persistence.Services;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Satur8.UI
{
    /// <summary>
    /// Логика взаимодействия для SaturatorView.xaml
    /// </summary>
    public partial class SaturatorView : UserControl
    {
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern int ToUnicode(uint wVirtKey, uint wScanCode, byte[] lpKeyState,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags);

        [DllImport("user32.dll")]
        private static extern bool GetKeyboardState(byte[] lpKeyState);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private IntPtr _hookId = IntPtr.Zero;
        private LowLevelKeyboardProc? _proc;

        public SaturatorView()
        {
            InitializeComponent();
            DataContext = new SaturatorViewModel();

            EventManager.RegisterClassHandler(typeof(TextBox),
                UIElement.GotFocusEvent, new RoutedEventHandler(Input_GotFocus));
            EventManager.RegisterClassHandler(typeof(TextBox),
                UIElement.LostFocusEvent, new RoutedEventHandler(Input_LostFocus));

            EventManager.RegisterClassHandler(typeof(PasswordBox),
                UIElement.GotFocusEvent, new RoutedEventHandler(Input_GotFocus));
            EventManager.RegisterClassHandler(typeof(PasswordBox),
                UIElement.LostFocusEvent, new RoutedEventHandler(Input_LostFocus));

            Unloaded += (s, e) => RemoveHook();
        }

        private void Input_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TextBox || e.OriginalSource is PasswordBox)
            {
                _proc = HookCallback;
                using var curProcess = Process.GetCurrentProcess();
                using var curModule = curProcess.MainModule!;
                _hookId = SetWindowsHookEx(
                    WH_KEYBOARD_LL, _proc,
                    GetModuleHandle(curModule.ModuleName!), 0);
            }
        }

        private void Input_LostFocus(object sender, RoutedEventArgs e)
        {
            RemoveHook();
        }

        private void RemoveHook()
        {
            if (_hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookId);
                _hookId = IntPtr.Zero;
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Key key = KeyInterop.KeyFromVirtualKey(vkCode);

                var focused = Keyboard.FocusedElement;

                if (focused is TextBox tb)
                {
                    if (key == Key.Back)
                    {
                        int caret = tb.CaretIndex;
                        if (tb.SelectionLength > 0)
                            tb.SelectedText = "";
                        else if (caret > 0)
                        {
                            tb.Text = tb.Text.Remove(caret - 1, 1);
                            tb.CaretIndex = caret - 1;
                        }
                        return (IntPtr)1;
                    }

                    if (key == Key.Delete)
                    {
                        int caret = tb.CaretIndex;
                        if (tb.SelectionLength > 0)
                            tb.SelectedText = "";
                        else if (caret < tb.Text.Length)
                            tb.Text = tb.Text.Remove(caret, 1);
                        tb.CaretIndex = caret;
                        return (IntPtr)1;
                    }

                    string ch = GetCharsFromKeys((uint)vkCode, 0);
                    if (!string.IsNullOrEmpty(ch) && !char.IsControl(ch[0]))
                    {
                        int caret = tb.CaretIndex;
                        tb.SelectedText = ch;
                        tb.CaretIndex = caret + ch.Length;
                        return (IntPtr)1;
                    }
                }

                else if (focused is PasswordBox pb)
                {
                    if (key == Key.Back)
                    {
                        if (pb.Password.Length > 0)
                            pb.Password = pb.Password[..^1];
                        return (IntPtr)1;
                    }

                    if (key == Key.Delete)
                    {
                        return (IntPtr)1;
                    }

                    string ch = GetCharsFromKeys((uint)vkCode, 0);
                    if (!string.IsNullOrEmpty(ch) && !char.IsControl(ch[0]))
                    {
                        pb.Password += ch;
                        return (IntPtr)1;
                    }
                }
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private string GetCharsFromKeys(uint keys, uint scanCode)
        {
            var buf = new StringBuilder(256);
            var state = new byte[256];
            GetKeyboardState(state);
            int result = ToUnicode(keys, scanCode, state, buf, buf.Capacity, 0);
            return result > 0 ? buf.ToString() : string.Empty;
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SaturatorViewModel vm)
                vm.ShowAccountPanel = true;

            Dispatcher.BeginInvoke(() =>
            {
                LoginBox.Focus();
                Keyboard.Focus(LoginBox);
            }, System.Windows.Threading.DispatcherPriority.Input);
        }

        private void CloseAccountPanel_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SaturatorViewModel vm)
                vm.ShowAccountPanel = false;

            RemoveHook();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SaturatorViewModel vm) return;

            var login = LoginBox.Text.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                vm.AuthError = "Заполните все поля.";
                return;
            }

            vm.IsLoading = true;
            vm.AuthError = string.Empty;

            try
            {
                var auth = PluginService.Services.GetRequiredService<AuthService>();

                var result = await auth.LoginAsync(login, password);

                if (result.Success)
                {
                    vm.LoggedInAs = "@" + result.User!.Login;
                    vm.IsLoggedIn = true;
                    vm.ShowAccountPanel = true;
                    LoginBox.Text = string.Empty;
                    PasswordBox.Password = string.Empty;
                }
                else
                {
                    vm.AuthError = result.Error ?? "Ошибка входа.";
                }
            }
            catch (Exception ex)
            {
                vm.AuthError = "Ошибка подключения: " + ex.Message;
            }
            finally
            {
                vm.IsLoading = false;
            }
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SaturatorViewModel vm) return;

            var login = LoginBox.Text.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                vm.AuthError = "Заполните все поля.";
                return;
            }

            vm.IsLoading = true;
            vm.AuthError = string.Empty;

            try
            {
                var auth = PluginService.Services.GetRequiredService<AuthService>();

                var result = await auth.RegisterAsync(login, password, new CancellationToken());

                if (result.Success)
                {
                    vm.LoggedInAs = "@" + result.User!.Login;
                    vm.IsLoggedIn = true;
                    vm.ShowAccountPanel = true;
                    LoginBox.Text = string.Empty;
                    PasswordBox.Password = string.Empty;
                }
                else
                {
                    vm.AuthError = result.Error ?? "Ошибка регистрации.";
                }
            }
            //catch (Exception ex)
            //{
            //    vm.AuthError = "Ошибка подключения: " + ex.Message;
            //}
            catch (Exception ex)
            {
                var msg = ex.Message;
                var inner = ex.InnerException;
                while (inner != null)
                {
                    msg += " → " + inner.Message;
                    inner = inner.InnerException;
                }
                vm.AuthError = msg;
            }
            finally
            {
                vm.IsLoading = false;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SaturatorViewModel vm) return;
            vm.IsLoggedIn = false;
            vm.LoggedInAs = string.Empty;
            vm.AuthError = string.Empty;
            vm.ShowAccountPanel = false;
        }
    }
}