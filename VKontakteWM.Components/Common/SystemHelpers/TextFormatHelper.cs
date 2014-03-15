using System;

using System.Collections.Generic;
using System.Text;
using Galssoft.VKontakteWM.Components.GDI;
using System.Drawing;

namespace Galssoft.VKontakteWM.Components.Common.SystemHelpers
{
    public static class TextFormatHelper
    {
        /// <summary>
        /// Функция "распила" текста на строчки
        /// </summary>
        /// <param name="text">Исходный текст</param>
        /// <param name="width">Ширина области отображения текста</param>
        /// <param name="count">Количество выводимых строк (если значение не ограничено указать 0)</param>
        /// <param name="g">Графический контекст</param>
        /// <returns></returns>
        public static List<string> CutTextToLines(string inputText, int textWidth, int linesCount, Gdi g)
        {
            List<string> result = new List<string>();

            if (linesCount == 0)
            {
                linesCount = int.MaxValue;
            }

            inputText = inputText.Replace("\r", string.Empty);
            string[] lines = inputText.Split('\n'); // первый распил по символам переноса строки

            int currentLine;
            int maxChars;
            string text;
            string outtext;
            int ipreviouscol;
            int icol;
            int[] extents;
            Size size;
            bool isSpace; // признак: пробел
            bool isOver; // признак: нужна новая строка

            currentLine = 0;

            try
            {
                foreach (string line in lines)
                {
                    currentLine++;

                    text = line;
                    icol = line.Length;
                    ipreviouscol = 0;

                    do
                    {
                        text = line.Substring(ipreviouscol);

                        size = g.GetTextExtent(text, textWidth, out extents, out maxChars);

                        isSpace = false;
                        isOver = false;

                        if (text.Length > maxChars)
                        {
                            isOver = true;

                            // ищем первый пробел по которому можем обрезать
                            int iSpace = text.LastIndexOf(' ', maxChars, maxChars);

                            if (iSpace > -1) // он есть
                            {
                                isSpace = true;

                                icol = iSpace;
                            }
                            else // его нет
                            {
                                isSpace = false;

                                icol = maxChars;
                            }

                            outtext = text.Substring(0, icol); // обрезаем кусок текста до пробела
                        }
                        else
                        {
                            isOver = false;

                            outtext = text; // берем весь текст
                        }

                        // если в конце строки был пробел, то это необходимо учесть на следующей итерации
                        if (isSpace)
                        {
                            ipreviouscol += (icol + 1);
                        }
                        else
                        {
                            ipreviouscol += icol;
                        }

                        // если набрали требуемое в linesCount количество строк, необходимо прервать фукцию обработки
                        // если к этому моменту остался необработанный текст, необходимо добавить символы "..."
                        if (linesCount > (result.Count + 1))
                        {
                            result.Add(outtext);
                        }
                        else
                        {
                            if (isOver || (lines.Length != currentLine))
                            {
                                outtext = outtext.Remove(outtext.Length - 3, 3); // удаляем последние 3 символа чтобы "..." гарантировано влезло
                                outtext = outtext.Trim();

                                outtext += "...";
                            }

                            result.Add(outtext);

                            throw new Exception();
                        }
                    }
                    while (text.Length > maxChars);
                }
            }
            catch // чтобы выйти из всех циклов
            {
                //
            }

            return result;
        }
    }
}
