using Avalonia;

namespace AvaVibeTweak
{
    public static class AppBuilderExtensions
    {
        public static AppBuilder UseAvaVibeTweak(this AppBuilder builder)
        {
            return builder.AfterSetup(_ =>
            {
                VibeOverlayManager.Initialize();
            });
        }
    }
}
