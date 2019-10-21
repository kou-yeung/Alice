using UnityEditor.UI;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class CreatedUI
{
    static CreatedUI()
    {
        MenuOptions.OnCreatedButton += OnCreatedButton;
        MenuOptions.OnCreatedScrollRect += OnCreatedScrollRect;
    }

    static void OnCreatedButton(GameObject obj)
    {
        //btn.gameObject.AddComponent<ButtonSE>();
    }

    static void OnCreatedScrollRect(GameObject obj)
    {
        var scrollrect = obj.GetComponent<ScrollRect>();
        scrollrect.horizontalScrollbarSpacing = -20;
        scrollrect.verticalScrollbarSpacing = -20;

        var horizontal = scrollrect.horizontalScrollbar.GetComponent<RectTransform>();
        var hpos = horizontal.localPosition;
        hpos.y -= 20;
        horizontal.localPosition = hpos;

        var vertical = scrollrect.verticalScrollbar.GetComponent<RectTransform>();
        var vpos = vertical.localPosition;
        vpos.x += 20;
        vertical.localPosition = vpos;
    }
}