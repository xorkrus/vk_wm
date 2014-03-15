using System;

namespace Galssoft.VKontakteWM.Notification.NfnsWithSoftKeys
{
  public class SpinnerClickEventArgs : EventArgs
  {
    private bool forward;

    public SpinnerClickEventArgs(bool forward)
    {
      this.forward = forward;
    }

    // Возвращает true если нажата > кнопка прокрутки
    // Возвращает true если нажата < кнопка прокрутки
    public bool Forward { get { return forward; } }
  }

  public delegate void SpinnerClickEventHandler(object sender, SpinnerClickEventArgs e);
}
