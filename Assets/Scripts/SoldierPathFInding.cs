using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;

    private float detectionRadius = 10f; // Port�e de d�tection

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        InvokeRepeating(nameof(FindNearestTarget), 0f, 0.5f); // met � jour la cible r�guli�rement
    }

    void Update()
    {
        if (target != null && agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);

            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < 1f)
            {
                Destroy(target.gameObject);
                Destroy(gameObject);
            }
        }
    }

    void FindNearestTarget()
    {
        GameObject[] allSoldiers = GameObject.FindGameObjectsWithTag("Soldier");

        float closestDist = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject s in allSoldiers)
        {
            if (s == gameObject) continue; // pas soi-m�me

            float d = Vector3.Distance(transform.position, s.transform.position);
            if (d < closestDist && d <= detectionRadius)
            {
                closestDist = d;
                closest = s.transform;
            }
        }

        if (closest != null)
        {
            target = closest;
        }
    }
}
