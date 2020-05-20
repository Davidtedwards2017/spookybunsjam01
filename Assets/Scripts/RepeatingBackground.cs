using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilites;

public class RepeatingBackground : MonoBehaviour
{
    public Camera mainCamera;
    public BackgroundPiece prefab;

    private BackgroundPiece[] Pieces;
    private Vector2 screenBounds;

    private Vector3 lastPosition;

    private const int size = 3;

    [Range(-1,1)]
    public float parallaxEffect;

    public Vector3 StartSpawnOffset;


    // Start is called before the first frame update
    void Start()
    {

        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        lastPosition = mainCamera.transform.position;

        Pieces = new BackgroundPiece[3];
        
        Pieces[1] = Spawn(prefab, mainCamera.transform.position + StartSpawnOffset, "B");
        Pieces[0] = SpawnToLeftOf(prefab, Pieces[1], Vector3.zero,  "A");
        Pieces[2] = SpawnToRightOf(prefab, Pieces[1], Vector3.zero, "C");
    }

    private void LateUpdate()
    {
        var delta = mainCamera.transform.position - lastPosition;

        var displacement = new Vector3(delta.x * parallaxEffect, 0, 0);
        foreach (var piece in Pieces)
        {
            piece.transform.position += displacement;
        }

        if (delta.x > 0) //camera moving right so check left most piece
        {
            var leftMost = Pieces[0];
            var screenPos = mainCamera.WorldToViewportPoint(leftMost.GetRightPoint());
            if(screenPos.x < 0)
            {
                Pieces = Pieces.ReverseShift();

                leftMost.transform.position = Pieces[size - 2].GetRightPoint() + new Vector3(leftMost.Width() / 2, 0, 0);
                Pieces[size - 1] = leftMost;
            }
        }          
        else if(delta.x < 0) //camera moving left so check right most piece
        {
            var rightMost = Pieces[size - 1];
            var screenPos = mainCamera.WorldToViewportPoint(rightMost.GetLeftPoint());
            if (screenPos.x > 1)
            {
                Pieces = Pieces.Shift();

                rightMost.transform.position = Pieces[size - 2].GetLeftPoint() - new Vector3(rightMost.Width() / 2, 0, 0);
                Pieces[0] = rightMost;
            }
        }
        
        lastPosition = mainCamera.transform.position;
    }
    
    private BackgroundPiece Spawn(BackgroundPiece prefab, Vector3 position, string suffix)
    {
        var instance = Instantiate(prefab, position, Quaternion.identity);
        //instance.transform.SetParent(transform);
        instance.gameObject.name = prefab.name + "|" + suffix;
        return instance;
    }

    private BackgroundPiece SpawnToLeftOf(BackgroundPiece prefab, BackgroundPiece piece, Vector3 offset, string suffix)
    {
        var spawnPoint = (piece.GetLeftPoint() - prefab.GetRightPoint()) + offset;
        return Spawn(prefab, spawnPoint, suffix);
    }

    private BackgroundPiece SpawnToRightOf(BackgroundPiece prefab, BackgroundPiece piece, Vector3 offset, string suffix)
    {
        var spawnPoint = (piece.GetRightPoint() - prefab.GetLeftPoint()) + offset;
        return Spawn(prefab, spawnPoint, suffix);
    }
}
