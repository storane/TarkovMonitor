using System.Diagnostics;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Nodes;

namespace TarkovMonitor
{
    public class PrintScreen
    {
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        private int screenIndex = GetTarkovScreenIndex();

        public PrintScreen() { }

        public static int GetTarkovScreenIndex()
        {
            try
            {
                var configPath = Environment.ExpandEnvironmentVariables(@"%AppData%\Battlestate Games\Escape From Tarkov\Settings\Graphics.ini");
                using var file = new FileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(file, Encoding.UTF8);

                var json = JsonNode.Parse(reader.ReadToEnd());
                var activeDisplay = ((int)json["DisplaySettings"]["Display"]);

                var message = $"Active display number - {activeDisplay}.";
                Debug.WriteLine(message);
                return activeDisplay;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unable to query Escape From Tarkov graphic settings.", e);
                return 0;
            }
        }

        public string? SaveScreenshot(IntPtr hWnd, string screenshotPath)
        {
            if (hWnd == IntPtr.Zero)
            {
                Debug.WriteLine("Unable to capture screenshot", hWnd);
                return null;
            }
            SetForegroundWindow(hWnd);

            var index = screenIndex;
            if (Properties.Settings.Default.monitorIndex != -1)
            {
                index = Properties.Settings.Default.monitorIndex;
                Debug.WriteLine("Saved index = ", index);
            }

            var screen = Screen.AllScreens.ElementAtOrDefault(index);
            if (screen == null)
            {
                Debug.WriteLine("Screen index out of bounds", index);
                return null;
            }
            var bounds = screen.Bounds;

            var position = new Vector2(bounds.X, bounds.Y);
            using var bitmap = new Bitmap(bounds.Size.Width, bounds.Size.Height, PixelFormat.Format24bppRgb);
            try
            {
                using var gfx = Graphics.FromImage(bitmap);
                gfx.CopyFromScreen((int)position.X, (int)position.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unable to capture screenshot", e);
                return null;
            }
            var imgName = DateTime.Now.ToString("yyyy-MM-dd[HH-mm-ss]") + ".jpeg";
            var fullPath = Path.Combine(screenshotPath, imgName);
            bitmap.Save(fullPath);

            return fullPath;
        }
    }
}
