using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Xna.Framework;
using Org.Libsdl.App;
using SDL = SDL3.SDL;
using System;

namespace GPUInstancing.Android
{
    [Activity(
        Label = "@string/app_name",
        MainLauncher = true,
        Icon = "@drawable/icon",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.SensorLandscape,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize | ConfigChanges.ScreenLayout
    )]
    public class Activity1 : SDLActivity
    {
        protected override string[] GetLibraries()
        {
            return ["SDL3"];
        }

        protected override void Main()
        {
            // Enable high DPI "Retina" support. Trust us, you'll want this.
            SDL.SDL_SetHint("FNA_GRAPHICS_ENABLE_HIGHDPI", "1");

            // Keep mouse and touch input separate.
            SDL.SDL_SetHint(SDL.SDL_HINT_MOUSE_TOUCH_EVENTS, "0");
            SDL.SDL_SetHint(SDL.SDL_HINT_TOUCH_MOUSE_EVENTS, "0");
            SDL.SDL_SetHint(SDL.SDL_HINT_PEN_TOUCH_EVENTS, "0");
            SDL.SDL_SetHint("FNA3D_FORCE_DRIVER", "Vulkan");
            SDL.SDL_SetHint("FNA3D_OPENGL_FORCE_ES3", "1");

            SDL.SDL_RunApp(0, IntPtr.Zero, FakeMain, IntPtr.Zero);
        }

        static int FakeMain(int argc, IntPtr argv)
        {
            RealMain();
            return 0;
        }

		static void RealMain()
		{
			using (var game = new Game1())
				game.Run();
		}

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);

            if (hasFocus)
                SetImmersive();
        }

        private void SetImmersive()
        {
            if (System.OperatingSystem.IsAndroidVersionAtLeast(30))
            {
                Window.SetDecorFitsSystemWindows(false);
                Window.InsetsController.SystemBarsBehavior = (int)WindowInsetsControllerBehavior.ShowTransientBarsBySwipe;
                //NO Color Type error.
                //Window.SetNavigationBarColor(Color.Transparent);
                Window.InsetsController.Hide(WindowInsets.Type.SystemBars());
            }
            else
            {
#pragma warning disable CS0618
                this.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(SystemUiFlags.LayoutStable | SystemUiFlags.LayoutHideNavigation | SystemUiFlags.LayoutFullscreen | SystemUiFlags.HideNavigation | SystemUiFlags.Fullscreen | SystemUiFlags.ImmersiveSticky);
#pragma warning restore CS0618
            }
        }
    }
}
