using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoofSection : MonoBehaviour
{
    public Transform StartAnchor;
    public Transform EndAnchor;
    public BoxCollider2D Collider;

    private const float HEIGHT = 300;

    public RoofSection NextSection;
    public RoofSection PreviousSection;


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

    public RoofSection Spawn(Vector3 position, string suffix)
    {
        position = position - StartAnchor.position;

        var go = Instantiate(gameObject, position, Quaternion.identity);
        go.name = gameObject.name + suffix;
        return go.GetComponent<RoofSection>();
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
