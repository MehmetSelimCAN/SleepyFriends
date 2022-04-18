using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Move : ICommand
{
    private Vector3 direction = Vector3.zero;
    private Transform objectToMove;

    public Move(Transform objectToMove, Vector3 direction) {
        this.objectToMove = objectToMove;
        this.direction = direction;
    }

    public void Execute() {
        objectToMove.position += direction;
    }

    public void Undo() {
        objectToMove.position -= direction;
    }
}
