using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class LiftPlatform : MonoBehaviour
{
    [Header("Lift Settings")]
    public Vector3 moveOffset = new Vector3(0, 5f, 0);  // ระยะทางที่จะยก
    public float moveSpeed = 2f;                        // ความเร็วการยก
    public float delayBeforeMove = 1.5f;                // เวลาก่อนเริ่มยก

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isPlayerOn = false;
    private bool isMoving = false;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + moveOffset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPlayerOn && !isMoving)
        {
            isPlayerOn = true;
            StartCoroutine(StartLiftAfterDelay());
            // ให้ Player ยึดติดกับ Platform
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOn = false;
            // ถอน parent เพื่อให้ Player หลุดออกจากลิฟต์
            other.transform.SetParent(null);
        }
    }

    IEnumerator StartLiftAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeMove);
        isMoving = true;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }

    // Debug Gizmos ช่วยเห็นตำแหน่งเป้าหมายใน Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + moveOffset);
        Gizmos.DrawWireCube(transform.position + moveOffset, transform.localScale);
    }
}
