using System;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI.Controls;

namespace Galssoft.VKontakteWM.CustomControlls
{
    sealed class ClickableGraphicsImage : GraphicsImage
    {
        public ClickableGraphicsImage()
        {
            Focusable = true;
        }

        public ClickableGraphicsImage(IImage alphaChannelImage, bool stretch) : base(alphaChannelImage, stretch)
        {
            Focusable = true;
        }

        public override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);
            OnClick(EventArgs.Empty);
        }
    }
}
