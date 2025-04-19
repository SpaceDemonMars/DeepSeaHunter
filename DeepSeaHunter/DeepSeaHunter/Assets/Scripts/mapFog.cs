using UnityEngine;
using UnityEngine.UI;

public class MapFog : MonoBehaviour
{
    public Image fogImage;
    private Texture2D fogTexture;
    private RectTransform fogRectTransform;

    private void Start()
    {
        fogRectTransform = fogImage.GetComponent<RectTransform>();

        fogTexture = new Texture2D(512, 512, TextureFormat.ARGB32, false);
        fogTexture.filterMode = FilterMode.Point;

        Color32[] colors = new Color32[fogTexture.width * fogTexture.height];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = new Color32(0, 0, 0, 255);

        fogTexture.SetPixels32(colors);
        fogTexture.Apply();

        fogImage.sprite = Sprite.Create(fogTexture, new Rect(0, 0, fogTexture.width, fogTexture.height), new Vector2(0.5f, 0.5f));
    }

    public void RevealArea(Vector2 worldPosition, float radius)
    {
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(fogRectTransform, worldPosition, null, out localPosition);

        float mapWidth = fogRectTransform.rect.width;
        float mapHeight = fogRectTransform.rect.height;

        int texX = Mathf.FloorToInt((localPosition.x / mapWidth + 0.5f) * fogTexture.width);
        int texY = Mathf.FloorToInt((localPosition.y / mapHeight + 0.5f) * fogTexture.height);

        int pixelRadius = Mathf.FloorToInt(radius * fogTexture.width / mapWidth);

        for (int y = -pixelRadius; y <= pixelRadius; y++)
        {
            for (int x = -pixelRadius; x <= pixelRadius; x++)
            {
                int px = texX + x;
                int py = texY + y;

                if (px >= 0 && px < fogTexture.width && py >= 0 && py < fogTexture.height)
                {
                    float dist = Mathf.Sqrt(x * x + y * y);
                    if (dist <= pixelRadius)
                    {
                        Color32 pixelColor = fogTexture.GetPixel(px, py);
                        pixelColor.a = 0;
                        fogTexture.SetPixel(px, py, pixelColor);
                    }
                }
            }
        }

        fogTexture.Apply();
    }
}