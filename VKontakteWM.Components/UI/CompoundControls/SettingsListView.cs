using System.Drawing;

namespace Galssoft.VKontakteWM.Components.UI.CompoundControls
{
    /// <summary>
    /// Класс с элементами настроек
    /// </summary>
    public abstract partial class SettingsListView : KineticListView<SettingsListViewItems>
    {
        #region Constrictors

        protected SettingsListView(KineticControlsSetting settings)
            : base(KineticControlScrollType.Vertical, settings)
        {

        }

        protected SettingsListView()
            : this(null)
        {
        }

        #endregion

        #region Actions

        protected override void OnItemSelected(int index, Point mousePosition)
        {
            if (index < 0)
                return;

            int sPoint = Width - ((ExNativeItemData)NativeItems[index]).WorkAreaWidth;

            if (mousePosition.X >= sPoint)
            {
                ((ExNativeItemData)NativeItems[index]).ClickOn(mousePosition.X - sPoint);
            }
        }

        protected override void DrawItemOn(GDI.Gdi g, NativeItemData nativeItem, Rectangle rItem, int nItem)
        {
            ExNativeItemData item = (ExNativeItemData)nativeItem;
            Rectangle itemrect = new Rectangle(rItem.Right - item.WorkAreaWidth, rItem.Top, item.WorkAreaWidth, rItem.Height);
            item.DrawItemOn(g, itemrect);
        }

        #endregion
    }
}
