using System.Collections;
using UnityEngine;

public static class Ease
{
    public static float OutQuad(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }

    public static float OutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3) + c1 * Mathf.Pow(t - 1f, 2);
    }
}
public class MenuPanel : MonoBehaviour
{
    [SerializeField] RectTransform menu;
    [SerializeField] Vector2 offscreen = new Vector2(-600f, 0f);
    [SerializeField] Vector2 onscreen = Vector2.zero;
    [SerializeField] float duration = 0.4f;

    void Start()
    {
        menu.anchoredPosition = offscreen;
        StartCoroutine(SlideIn(menu, offscreen, onscreen, duration));
    }

    IEnumerator SlideIn(RectTransform panel, Vector2 from, Vector2 to,
                        float duration)
    {
        for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
        {
            float t = elapsed / duration;          // 0 to 1, linear in time
            float p = Ease.OutQuad(t);             // reshaped progress
            panel.anchoredPosition = Vector2.LerpUnclamped(from, to, p);
            yield return null;                     // wait one frame
        }
        panel.anchoredPosition = to;               // land exactly on the end
    }
}