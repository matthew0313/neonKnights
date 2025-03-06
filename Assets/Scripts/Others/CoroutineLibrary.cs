using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using System.Text;

public class CoroutineLibrary
{
    public static IEnumerator<float> Typewrite(string text, float interval, float longInterval, Action<string> onStringUpdate, Action onFinish = null)
    {
        StringBuilder sb = new();
        int i = 0;
        while (i < text.Length)
        {
            if (text[i] == '<')
            {
                int tagStart = i;
                int tagEnd = text.Length;
                for (int k = tagStart; k < text.Length; k++)
                {
                    if (text[k] == '>')
                    {
                        tagEnd = k;
                    }
                }
                if (tagEnd < text.Length)
                {
                    sb.Append(text.Substring(tagStart, tagEnd - tagStart + 1));
                    i += tagEnd - tagStart + 1;
                }
            }

            sb.Append(text[i]);
            onStringUpdate.Invoke(sb.ToString());

            if (text[i] == '.' || text[i] == ',' || text[i] == '!' || text[i] == '?' || text[i] == '\n') yield return Timing.WaitForSeconds(longInterval);
            else yield return Timing.WaitForSeconds(interval);
            i++;
        }
        onFinish?.Invoke();
    }
}