using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Galssoft.VKontakteWM.Components.GDI;

namespace Galssoft.VKontakteWM.Components.UI.Controls
{
    public class ExtendedProgressBar : UIControl
    {
        [DefaultValue(100)]
        public int Maximum { get; set; }

        private int _value;
        [DefaultValue(0)]
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnInvalidate();
                }
            }
        }

        public IImage EmptyImage
        {
            get
            {
                return _emptyImage;
            }
            set
            {
                _emptyImage = value;
                if (_emptyImageBitmap != null)
                    _emptyImageBitmap.Dispose();
                _emptyImageBitmap = CreateBitmapFromIImage(_emptyImage);
            }
        }
        private IImage _emptyImage;
        private Image _emptyImageBitmap;

        public IImage FullImage
        {
            get
            {
                return _fullImage;
            }
            set
            {
                _fullImage = value;
                if (_fullImageBitmap != null)
                    _fullImageBitmap.Dispose();
                _fullImageBitmap = CreateBitmapFromIImage(_fullImage);
            }
        }
        private IImage _fullImage;
        private Image _fullImageBitmap;

        private Bitmap CreateBitmapFromIImage(IImage image)
        {
            Bitmap result = null;
            if (image != null)
            {
                ImageInfo ii;
                image.GetImageInfo(out ii);
                result = new Bitmap((int)ii.Width, (int)ii.Height);
                using (Graphics gr = Graphics.FromImage(result))
                {
                    IntPtr hdc = gr.GetHdc();
                    Rectangle rect = new Rectangle(0, 0, result.Width, result.Height);
                    image.Draw(hdc, ref rect, IntPtr.Zero);
                    gr.ReleaseHdc(hdc);
                }
            }
            return result;
        }

        /// <summary>
        /// Радиус скруглений. Если 0, скруглений нет.
        /// Не может быть больше половины от меньшего размера.
        /// </summary>
        private int _radius;
        [DefaultValue(0)]
        public int RoundCornersRadius
        {
            get
            {
                return _radius;
            }
            set
            {
                int limit = Math.Min(Width, Height) / 2;
                if (value > limit)
                    _radius = limit;
                else if (value < 0)
                    _radius = 0;
                else
                    _radius = value;
            }
        }

        public string Text
        {
            get;
            set;
        }

        public Font Font
        {
            get;
            set;
        }

        /// <summary>
        /// Нужно ли обводить края кнопки
        /// </summary>
        [DefaultValue(false)]
        public bool NeedBorder { get; set; }

        [DefaultValue(false)]
        public bool SwitchTextColor { get; set; }

        public ExtendedProgressBar()
        {

        }

        /// <summary>
        /// Отрисовка
        /// </summary>
        protected override void OnRender(Gdi gMem, Rectangle clipRect)
        {
            using (Bitmap bmp = new Bitmap(clipRect.Width, clipRect.Height))
            {
                using (Graphics bgr = Graphics.FromImage(bmp)) // Рисуем все в буфере для скорости
                {
                    SizeF textSize = string.IsNullOrEmpty(Text) ? new SizeF(0, 0) : bgr.MeasureString(Text, Font);
                    Color textColor = SwitchTextColor ? BackColor : ForeColor;

                    // Отрисовка фона			
                    if (EmptyImage != null)
                        bgr.DrawImage(_emptyImageBitmap,
                                      new Rectangle(0, 0, clipRect.Width, clipRect.Height),
                                      new Rectangle(0, 0, _emptyImageBitmap.Width, _emptyImageBitmap.Height),
                                      GraphicsUnit.Pixel);

                    // Отрисовка прогресса
                    if (FullImage != null)
                        bgr.DrawImage(_fullImageBitmap,
                                      new Rectangle(0, 0, clipRect.Width * Value / Maximum, clipRect.Height),
                                      new Rectangle(0, 0, _fullImageBitmap.Width * Value / Maximum, _fullImageBitmap.Height),
                                      GraphicsUnit.Pixel);

                    Pen borderPen = new Pen(SwitchTextColor ? Color.FromArgb(88, 97, 114 /*93, 103, 121*/) : Color.Black);

                    if (NeedBorder)
                    {
                        // Обводка прямых краев

                        bgr.DrawLine(borderPen, RoundCornersRadius, 0, Width - RoundCornersRadius, 0);
                        bgr.DrawLine(borderPen, Width - 1, RoundCornersRadius - 2, Width - 1,
                                     Height - RoundCornersRadius + 2);
                        bgr.DrawLine(borderPen, RoundCornersRadius - 3, Height - 1, Width + 3 - RoundCornersRadius,
                                     Height - 1);
                        bgr.DrawLine(borderPen, 0, RoundCornersRadius, 0, Height - RoundCornersRadius);
                    }

                    //Отрисовка текста

                    float textLeft = (clipRect.Width - textSize.Width) / 2;
                    float textTop = (clipRect.Height - textSize.Height) / 2;

                    if (!string.IsNullOrEmpty(Text))
                        bgr.DrawString(Text, Font, new SolidBrush(textColor),
                                       new RectangleF(textLeft, textTop, textSize.Width, textSize.Height));

                    // Скругление углов

                    if (RoundCornersRadius > 0)
                    {
                        // Необходимо скругление - вычисление точек границы вырезаемой области
                        int N = 100;
                        List<Point> points = new List<Point>(N + 1);
                        Point[] pointsArray = new Point[N];
                        points.Add(new Point(0, 0));

                        // Скругление верхнего левого угла кнопки

                        for (int i = 0; i < N; i++)
                        {
                            int x = i * RoundCornersRadius / (N - 1);
                            int y = (int)(RoundCornersRadius -
                                           Math.Round(Math.Sqrt(RoundCornersRadius * RoundCornersRadius -
                                                                (x - RoundCornersRadius) * (x - RoundCornersRadius))));

                            points.Add(new Point(x, y));
                        }
                        bgr.FillPolygon(new SolidBrush(Parent.BackColor), points.ToArray());

                        if (NeedBorder)
                        {
                            // Обводка верхнего левого угла кнопки

                            points.CopyTo(1, pointsArray, 0, N);
                            bgr.DrawLines(borderPen, pointsArray);
                        }

                        // Скругление верхнего правого угла кнопки

                        for (int i = 0; i < N + 1; i++)
                        {
                            points[i] = new Point(clipRect.Width - points[i].X, points[i].Y);
                        }
                        bgr.FillPolygon(new SolidBrush(Parent.BackColor), points.ToArray());

                        if (NeedBorder)
                        {
                            // Обводка верхнего правого угла кнопки

                            points.CopyTo(1, pointsArray, 0, N);
                            bgr.DrawLines(borderPen, pointsArray);
                        }

                        // Скругление нижнего правого угла кнопки

                        for (int i = 0; i < N + 1; i++)
                        {
                            points[i] = new Point(points[i].X, clipRect.Height - points[i].Y);
                        }
                        bgr.FillPolygon(new SolidBrush(Parent.BackColor), points.ToArray());

                        if (NeedBorder)
                        {
                            // Обводка нижнего правого угла кнопки

                            points.CopyTo(1, pointsArray, 0, N);
                            bgr.DrawLines(borderPen, pointsArray);
                        }

                        // Скругление нижнего левого угла кнопки

                        for (int i = 0; i < N + 1; i++)
                        {
                            points[i] = new Point(clipRect.Width - points[i].X, points[i].Y);
                        }
                        bgr.FillPolygon(new SolidBrush(Parent.BackColor), points.ToArray());

                        if (NeedBorder)
                        {
                            // Обводка нижнего левого угла кнопки

                            points.CopyTo(1, pointsArray, 0, N);
                            bgr.DrawLines(borderPen, pointsArray);
                        }
                    }
                }
                gMem.DrawImage(bmp, clipRect.Left, clipRect.Top);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_fullImageBitmap != null)
            {
                _fullImageBitmap.Dispose();
                _fullImageBitmap = null;
            }
            if (_emptyImageBitmap != null)
            {
                _emptyImageBitmap.Dispose();
                _emptyImageBitmap = null;
            }
        }
    }
}
