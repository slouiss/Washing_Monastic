using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject topDoor;
    [SerializeField] GameObject bottomDoor;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    public Vector2Int RoomIndex { get; set; }

    public void OpenDoor(Vector2Int direction){

        if(direction == Vector2.up){
            topDoor.SetActive(true);
        }

        if(direction == Vector2.down){
            bottomDoor.SetActive(true);
        }

        if(direction == Vector2.left){
            leftDoor.SetActive(true);
        }

        if(direction == Vector2.right){
            rightDoor.SetActive(true);
        }
    }
}