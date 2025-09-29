using UnityEngine;

public class UIFloating : MonoBehaviour
{
    [SerializeField] Vector2 amplitude = new Vector2(6f, 6f);
    [SerializeField] float speed = 0.5f;
    [SerializeField] RectTransform rect;
    [SerializeField] Vector2 startPos;
    [SerializeField] float seedX;
    [SerializeField] float seedY;

    void Awake()
    {
        //Start the seed
        startPos = rect.anchoredPosition;
        seedX = Random.value * 1000f;
        seedY = Random.value * 1000f;
    }

    //Use this to do some cool floaty effect on UI elements
    void Update()
    {
        float t = Time.time * speed;

        float x = (Mathf.PerlinNoise(seedX, t) - 0.5f) * 2f * amplitude.x;
        float y = (Mathf.PerlinNoise(seedY, t) - 0.5f) * 2f * amplitude.y;

        rect.anchoredPosition = startPos + new Vector2(x, y);
    }
}
