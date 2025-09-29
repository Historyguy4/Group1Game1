using UnityEngine;

public class ColorAndSize : MonoBehaviour
{
    [SerializeField] Color colorA;
    [SerializeField] Color colorB;

    [SerializeField] float minScale = 0.9f;
    [SerializeField] float maxScale = 1.1f;

    [SerializeField] float speed = 1f;

    [SerializeField] SpriteRenderer sr;
    private Vector3 baseScale;

    void Awake()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time * speed, 1f);

        sr.color = Color.Lerp(colorA, colorB, t);

        float s = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = baseScale * s;
    }
}
