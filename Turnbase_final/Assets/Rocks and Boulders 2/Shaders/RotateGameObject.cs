using UnityEngine;

public class RotateGameObject : MonoBehaviour
{
    public float rot_speed_x = 0;
    public float rot_speed_y = 0;
    public float rot_speed_z = 0;
    public bool local = false;

    void FixedUpdate()
    {
        if (local)
        {
            // หมุนใน local space
            transform.Rotate(Time.fixedDeltaTime * rot_speed_x,
                             Time.fixedDeltaTime * rot_speed_y,
                             Time.fixedDeltaTime * rot_speed_z,
                             Space.Self);
        }
        else
        {
            // หมุนใน world space
            transform.Rotate(Time.fixedDeltaTime * rot_speed_x,
                             Time.fixedDeltaTime * rot_speed_y,
                             Time.fixedDeltaTime * rot_speed_z,
                             Space.World);
        }
    }
}
