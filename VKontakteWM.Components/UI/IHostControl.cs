using System.Drawing;

namespace Galssoft.VKontakteWM.Components.UI
{
    public interface IHostControl
    {
        void Invalidate(Rectangle rectangle);

        int Left { get; set; }
        int Top { get; set; }
    }
}