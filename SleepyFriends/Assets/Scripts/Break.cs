using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Break : ICommand
{
    private Tilemap currentBreakableBlocksTilemap;
    private TileBase[] breakableTiles;
    private Tilemap BreakableBlocksTilemap = GameObject.Find("BreakableBlocks").GetComponent<Tilemap>();

    public Break(TileBase[] breakableTiles, Tilemap currentBreakableBlocksTilemap) {
        this.breakableTiles = breakableTiles;
        this.currentBreakableBlocksTilemap = currentBreakableBlocksTilemap;
    }

    public void Execute() {

    }

    public void Undo() {
        BreakableBlocksTilemap.SetTilesBlock(currentBreakableBlocksTilemap.cellBounds, breakableTiles);
    }
}
