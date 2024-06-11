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
            topDoor.SetActive(false);
        }

        if(direction == Vector2.down){
            bottomDoor.SetActive(false);
        }

        if(direction == Vector2.left){
            leftDoor.SetActive(false);
        }

        if(direction == Vector2.right){
            rightDoor.SetActive(false);
        }
    }
}