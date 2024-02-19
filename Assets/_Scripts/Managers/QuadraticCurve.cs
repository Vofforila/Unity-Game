using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadraticCurve : MonoBehaviour
{
    public Transform A;
    public Transform B;
    public Transform C;

    public Vector3 Evaluate(float t)
    {
        Vector3 ac = Vector3.Lerp(A.position, C.position, t);
        Vector3 cb = Vector3.Lerp(C.position, B.position, t);

        return Vector3.Lerp(ac, cb, t);
    }

    private void OnDrawGizmos()
    {
        if (A == null || B == null || C == null)
        {
            return;
        }

        for (int i = 0; i < 20; i++)
            Gizmos.DrawWireSphere(Evaluate(i / 20f), 0.5f);
    }
}