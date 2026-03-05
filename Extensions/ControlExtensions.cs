using Godot;

namespace BaseLib.Extensions;

public static class ControlExtensions
{
    /// <summary>
    /// Draws the area of a Control.
    /// An easy way to use for debugging is by adding it to the Control's Draw event.
    /// eg. control.Draw += control.DrawDebug;
    /// </summary>
    /// <param name="item"></param>
    public static void DrawDebug(this Control item)
    {
        item.DrawRect(new Rect2(0, 0, item.Size), new Color(1, 1, 1, 0.5f));
    }
    public static void DrawDebug(this Control artist, Control child)
    {
        artist.DrawRect(new Rect2(child.Position, child.Size), new Color(1, 1, 1, 0.5f));
    }
}
