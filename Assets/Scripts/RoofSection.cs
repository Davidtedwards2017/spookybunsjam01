using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class RoofSection : MonoBehaviour
{
    public Transform StartAnchor;
    public Transform EndAnchor;
    public BoxCollider2D Collider;

    private const float HEIGHT = 300;

    public RoofSection NextSection;
    public RoofSection PreviousSection;

    private BoxCollider2D[] RoofColliders;


    [Header("Events")]
    [Space]

    public RoofSectionEvent OnPlayerEnterRoofSection = new RoofSectionEvent();
    public RoofSectionEvent OnPlayerExitRoofSection = new RoofSectionEvent();


    public class RoofSectionEvent : UnityEvent<RoofSection> { }


    public RoofSection Spawn(RoofSection fromSection, string suffix)
    {
        var section = Spawn(fromSection.GetEndPosition(), suffix);
        fromSection.NextSection = section;
        section.PreviousSection = fromSection;
        return section;
    }

    private void Awake()
    {
        RoofColliders = GetComponentsInChildren<BoxCollider2D>().Where(col => !col.isTrigger).ToArray();
    }

    public RoofSection Spawn(Vector3 position, string suffix)
    {
        position = position - StartAnchor.position;

        var go = Instantiate(gameObject, position, Quaternion.identity);
        go.name = gameObject.name + suffix;
        return go.GetComponent<RoofSection>();
    }

    public bool GetSpawnEdge(out Vector3 leftPoint, out Vector3 rightPoint)
    {
        leftPoint = Vector3.zero;
        rightPoint = Vector3.zero;

        if (!RoofColliders.Any())
        {
            return false;
        }


        var col = RoofColliders.First();

        var top = col.bounds.center.y + col.bounds.extents.y;
        var left = col.bounds.center.x + col.bounds.extents.x;
        var right = col.bounds.center.x - col.bounds.extents.x;


        leftPoint = new Vector3(left, top);
        rightPoint = new Vector3(right, top);

        return true;
    }

    private void Update()
    {
        Vector3 left;
        Vector3 right;
        if(GetSpawnEdge(out left, out right))
        {
            Debug.DrawLine(left, right, Color.red);
        }
    }

    public Vector3 GetEndPosition()
    {
        return EndAnchor.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Collider.bounds.center, Collider.bounds.size);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(StartAnchor.position, EndAnchor.position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Player") return;
        if(OnPlayerEnterRoofSection != null)
        {
            OnPlayerEnterRoofSection.Invoke(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Player") return;
        if (OnPlayerExitRoofSection != null)
        {
            OnPlayerExitRoofSection.Invoke(this);
        }
    }

    private void OnDestroy()
    {
        if(NextSection != null && NextSection == this)
        {
            NextSection.PreviousSection = null;
        }

        if(PreviousSection != null && PreviousSection == this)
        {
            PreviousSection.NextSection = null;
        }
    }
}
