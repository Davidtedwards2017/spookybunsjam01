using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilites;

public class RoofSpawner : MonoBehaviour
{
    public List<RoofSection> SectionPrefabs;
    public int MaxCountInFront = 2;
    public int MaxCountBehind = 2;
    public int RoofSectionLimit = 5;
    public int SpawnCount = 0;

    public RoofSection currentSection;

    private FilteredRandom<RoofSection> RandomSection;

    // Start is called before the first frame update
    void Start()
    {
        RandomSection = new FilteredRandom<RoofSection>(SectionPrefabs, 2);

        currentSection = SectionPrefabs[0].Spawn(Vector3.zero, "|" + SpawnCount);
        SpawnNewSection(RandomSection.GetNextRandom());
        SpawnNewSection(RandomSection.GetNextRandom());
    }

    private void SpawnNewSection(RoofSection nextSectionPrefab)
    {
        var newSection = nextSectionPrefab.Spawn(GetLastSection(out int count), "|" + (++SpawnCount));
        newSection.OnPlayerEnterRoofSection.AddListener(OnSectionEntered);
        newSection.OnPlayerExitRoofSection.AddListener(OnSectionExited);
    }

    private RoofSection GetLastSection(out int count)
    {
        count = 1;
        var section = currentSection;
        while(section.NextSection != null)
        {
            count++;
            section = section.NextSection;
        }

        return section;
    }

    private RoofSection GetFirstSection(out int count)
    {
        count = 1;
        var section = currentSection;
        while (section.PreviousSection != null)
        {
            count++;
            section = section.PreviousSection;
        }

        return section;
    }

    private void OnSectionEntered(RoofSection section)
    {
        currentSection = section;

        var last = GetLastSection(out int count);
        while(count < MaxCountInFront)
        {
            SpawnNewSection(RandomSection.GetNextRandom());
            last = GetLastSection(out count);
        }
    }

    private void OnSectionExited(RoofSection section)
    {
        var first = GetFirstSection(out int count);
        if (count > MaxCountBehind)
        { 
            Destroy(first.gameObject);
        }
    }
}
