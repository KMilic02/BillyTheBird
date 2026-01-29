#if UNITY_EDITOR
using UnityEngine;

public partial class Player : MonoBehaviour
{
    void debugUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Entering debug mode");
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Exiting debug mode");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        //Time.timeScale = Input.GetKey(KeyCode.C) ? 5f : 1f;
        //Time.timeScale = Input.GetKey(KeyCode.V) ? 0.2f : 1f;
    }
    
    void OnGUI()
    {
        GUILayout.TextField(
            "1 - enable cursor, 2 - disable cursor"
        );

        GUILayout.TextField(
            $"Can dash: {canDash}"
        );

        GUILayout.TextField(
            $"Is Dashing: {playerState == State.Dashing}"
        );

        GUILayout.TextField(
            $"Glide remaining: {glideDurationLeft}"
        );

        GUILayout.TextField(
            $"Health: {health}"
        );
        
        if (GUILayout.Button("Add feathers"))
        {
            addFeather();
            Debug.Log($"Feathers {feathers}");
        }

        if (GUILayout.Button("Add Seed"))
        {
            addSeeds(1);
            Debug.Log($"Seeds {seeds}");
        }
        
        if (GUILayout.Button("Add 10 Seeds"))
        {
            addSeeds(10);
            Debug.Log($"Seeds {seeds}");
        }
    }

    void OnDrawGizmos()
    {
        if (playerCollider == null)
            return;
        
        var groundCheckPosition = playerCollider.bounds.center - Vector3.up * playerCollider.bounds.extents.y;
        var groundCheckExtents = playerCollider.bounds.extents.x;

        Matrix4x4 matrix = Matrix4x4.TRS(groundCheckPosition, Quaternion.Euler(0.0f, 0.0f, transform.rotation.y), Vector3.one);
        
        Gizmos.matrix = matrix;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero,
            new Vector3(groundCheckExtents, 0.02f, groundCheckExtents));
        
    }
}
#endif