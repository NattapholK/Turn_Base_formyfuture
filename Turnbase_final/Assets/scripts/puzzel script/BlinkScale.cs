using UnityEngine;
using System.Collections;

public class BlinkScale : MonoBehaviour
{
    [Header("Scale Settings")]
    public float scaleYMin = 0.3f;     // ค่าย่อเล็กสุดของแกน Y
    public float scaleSpeed = 5f;      // ความเร็วในการย่อ/ขยาย
    public float blinkInterval = 2f;   // เวลาพักระหว่างแต่ละรอบ (วินาที)

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        StartCoroutine(BlinkLoop());
    }

    IEnumerator BlinkLoop()
    {
        while (true)
        {
            // ย่อแกน Y ลง
            yield return StartCoroutine(ScaleYTo(scaleYMin));

            // รอแป๊บหนึ่ง (ช่วงตาปิด)
            yield return new WaitForSeconds(0.1f);

            // ขยายกลับ
            yield return StartCoroutine(ScaleYTo(originalScale.y));

            // รอระหว่างการกระพริบ
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    IEnumerator ScaleYTo(float targetY)
    {
        Vector3 scale = transform.localScale;
        float startY = scale.y;
        float t = 0f;

        while (Mathf.Abs(scale.y - targetY) > 0.01f)
        {
            t += Time.deltaTime * scaleSpeed;
            scale.y = Mathf.Lerp(startY, targetY, t);
            transform.localScale = scale;
            yield return null;
        }

        scale.y = targetY;
        transform.localScale = scale;
    }
}
