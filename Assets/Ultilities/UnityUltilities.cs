using TMPro;
using UnityEngine;

public static class UnityUltilities
{
    private static Camera _cam = Camera.main;

    public static Vector3 GetMousePosition()
    {
        Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    public static TextMeshPro CreateWorldText(string content, Vector3 worldPositon,int textSize, Color color)
    {
        GameObject obj = new GameObject("Debug Text");
        TextMeshPro tmp = obj.AddComponent<TextMeshPro>();
        tmp.alignment = TextAlignmentOptions.Center;

        tmp.text = content;
        tmp.transform.position = worldPositon;
        tmp.color = color;
        tmp.fontSize = textSize;

        return tmp;
    }
}
