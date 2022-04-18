using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    private RaycastHit2D checkWall;
    private RaycastHit2D checkSleepy;
    private RaycastHit2D checkBreakableBlock;

    private LayerMask wallLayer;
    private LayerMask gameLayer;
    private LayerMask breakableBlocksLayer;

    private Tilemap BreakableBlocksTileMap;

    private Tile brokeTile;
    private Tile brokeTileWithShadow;
    private Tile halfBrokeTileWithShadow;

    private Sprite awakePlayerSprite;

    /*private Vector2 pos1;
    private Vector2 pos2;
    private bool canMove = true; mobile */

    private List<Move> moveCommandList = new List<Move>();
    private List<Unite> uniteCommandList = new List<Unite>();
    private List<Break> tilemapCommandList = new List<Break>();

    private void Awake() {
        brokeTile = Resources.Load<Tile>("BreakableBlocks/BrokeBlock");
        brokeTileWithShadow = Resources.Load<Tile>("BreakableBlocks/BrokeBlockWithShadow");
        halfBrokeTileWithShadow = Resources.Load<Tile>("BreakableBlocks/BreakableBlockWithShadow");
    }

    void Start()
    {
        //canMove = true; mobile
        wallLayer = LayerMask.GetMask("Wall");
        gameLayer = LayerMask.GetMask("GameLayer");

        breakableBlocksLayer = LayerMask.GetMask("BreakableBlocksLayer");
        BreakableBlocksTileMap = GameObject.Find("BreakableBlocks").GetComponent<Tilemap>();

        awakePlayerSprite = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;

        CheckSleepyPlayers();
    }

    void Update()
    {
        #region PC Movement
        if (Input.GetKeyDown(KeyCode.A)) {
            Move(new Vector2(-0.5f, 0f));
        }

        else if (Input.GetKeyDown(KeyCode.D)) {
            Move(new Vector2(0.5f, 0f));
        }

        else if (Input.GetKeyDown(KeyCode.W)) {
            Move(new Vector2(0f, 0.5f));
        }

        else if (Input.GetKeyDown(KeyCode.S)) {
            Move(new Vector2(0f, -0.5f));
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            UndoCommand();
        }
        #endregion
/*
        #region Mobile Movement
        Touch touch1 = Input.GetTouch(0);

        if(IsPointerOverUIObject()) {
            canMove = false;
        }

        else if (!IsPointerOverUIObject()) {
            if (touch1.phase == TouchPhase.Began) {
                pos1 = touch1.position;
                canMove = false;
            }

            if (touch1.phase == TouchPhase.Ended) {
                pos2 = touch1.position;
                canMove = true;
            }

            if (canMove) {
                if (Mathf.Abs(pos1.x - pos2.x) >= Mathf.Abs(pos1.y - pos2.y) && pos1.x - pos2.x >= 1) {
                    Move(new Vector2(-0.5f, 0f));
                }

                else if (Mathf.Abs(pos1.x - pos2.x) >= Mathf.Abs(pos1.y - pos2.y) && pos1.x - pos2.x < -1) {
                    Move(new Vector2(0.5f, 0f));
                }

                else if (Mathf.Abs(pos1.y - pos2.y) > Mathf.Abs(pos1.x - pos2.x) && pos1.y - pos2.y >= 1) {
                    Move(new Vector2(0f, -0.5f));
                }

                else if (Mathf.Abs(pos1.y - pos2.y) > Mathf.Abs(pos1.x - pos2.x) && pos1.y - pos2.y < -1) {
                    Move(new Vector2(0f, 0.5f));
                }
            }
        }

        #endregion
*/
    }

    /*private bool IsPointerOverUIObject() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }  mobile */

    public void AddCommand(ICommand uniteCommand, ICommand moveCommand, ICommand tilemapCommand) {
        uniteCommandList.Add(uniteCommand as Unite);
        moveCommandList.Add(moveCommand as Move);
        tilemapCommandList.Add(tilemapCommand as Break);
        moveCommand.Execute();
    }

    public void UndoCommand() {
        if (uniteCommandList.Count == 0) {
            return;
        }

        uniteCommandList[uniteCommandList.Count - 1].Undo();
        uniteCommandList.RemoveAt(uniteCommandList.Count - 1);

        moveCommandList[moveCommandList.Count - 1].Undo();
        moveCommandList.RemoveAt(moveCommandList.Count - 1);

        tilemapCommandList[tilemapCommandList.Count - 1].Undo();
        tilemapCommandList.RemoveAt(tilemapCommandList.Count - 1);

    }

    public void Move(Vector2 direction) {
        foreach (Transform child in transform) {
            checkWall = Physics2D.Raycast(child.position, direction, 0.25f, wallLayer);

            if (checkWall == true) {
                GameManager.canMove = false;
                break;
            }
        }

        if (GameManager.canMove) {
            TileBase[] currentBreakableTiles = BreakableBlocksTileMap.GetTilesBlock(BreakableBlocksTileMap.cellBounds);
            ICommand unite = new Unite(transform, transform.childCount);
            ICommand movement = new Move(transform, direction);
            ICommand tilemap = new Break(currentBreakableTiles, BreakableBlocksTileMap);
            AddCommand(unite, movement, tilemap);

            foreach (Transform child in transform) {
                if (direction == new Vector2(0.5f, 0f)) {       //Right
                    child.Find("WalkingEffect").GetComponent<Animator>().SetTrigger("WalkingEffectD");
                    continue;
                }
                else if (direction == new Vector2(-0.5f, 0f)) { //Left
                    child.Find("WalkingEffect").GetComponent<Animator>().SetTrigger("WalkingEffectA");
                    continue;
                }
                else if (direction == new Vector2(0f, 0.5f)) {  //Up
                    child.Find("WalkingEffect").GetComponent<Animator>().SetTrigger("WalkingEffectW");
                    continue;
                }
                else if (direction == new Vector2(0f, -0.5f)) { //Down
                    child.Find("WalkingEffect").GetComponent<Animator>().SetTrigger("WalkingEffectS");
                    continue;
                }
            }

            CheckSleepyPlayers();
        }

        GameManager.canMove = true;

        StartCoroutine(CheckBreakableBlocks());
    }

    public IEnumerator CheckBreakableBlocks() {
        yield return new WaitForSeconds(0f);
        foreach (Transform child in transform) {
            checkBreakableBlock = Physics2D.Raycast(child.position, Vector3.forward, 10f, breakableBlocksLayer);
            Vector3Int tilePosition = BreakableBlocksTileMap.WorldToCell(child.position);
            TileBase currentTile;
            if (checkBreakableBlock) {
                currentTile = checkBreakableBlock.transform.GetComponent<Tilemap>().GetTile(tilePosition);
                if (currentTile != brokeTile && currentTile != brokeTileWithShadow) {
                    if (currentTile == halfBrokeTileWithShadow) {
                        BreakableBlocksTileMap.SetTile(tilePosition, brokeTileWithShadow);
                    }
                    else {
                        BreakableBlocksTileMap.SetTile(tilePosition, brokeTile);
                    }
                }
                else if (currentTile == brokeTile || currentTile == brokeTileWithShadow) {
                    //Death animation
                    Destroy(child.gameObject);
                }
            }
        }

    }

    public void CheckSleepyPlayers() {
        foreach (Transform child in transform) {
            checkSleepy = Physics2D.Raycast(child.position + new Vector3(0f, +0.26f, 0f), Vector3.up, 0.24f, gameLayer);

            if (checkSleepy && checkSleepy.transform.CompareTag("SleepyPlayer")) {
                Transform sleepyPlayer = checkSleepy.transform;
                sleepyPlayer.SetParent(transform);
                sleepyPlayer.tag = "Player";
                sleepyPlayer.GetChild(1).gameObject.SetActive(false); //Sleep effect
                sleepyPlayer.GetComponent<Animator>().SetTrigger("WakeyWakey");
                sleepyPlayer.GetComponent<SpriteRenderer>().sprite = awakePlayerSprite;
            }

            checkSleepy = Physics2D.Raycast(child.position + new Vector3(0f,-0.26f,0f), Vector3.down, 0.24f, gameLayer);

            if (checkSleepy && checkSleepy.transform.CompareTag("SleepyPlayer")) {
                Transform sleepyPlayer = checkSleepy.transform;
                sleepyPlayer.SetParent(transform);
                sleepyPlayer.tag = "Player";
                sleepyPlayer.GetChild(1).gameObject.SetActive(false); //Sleep effect
                sleepyPlayer.GetComponent<Animator>().SetTrigger("WakeyWakey");
                sleepyPlayer.GetComponent<SpriteRenderer>().sprite = awakePlayerSprite;
            }

            checkSleepy = Physics2D.Raycast(child.position + new Vector3(-0.26f, 0f, 0f), Vector3.left, 0.24f, gameLayer);

            if (checkSleepy && checkSleepy.transform.CompareTag("SleepyPlayer")) {
                Transform sleepyPlayer = checkSleepy.transform;
                sleepyPlayer.SetParent(transform);
                sleepyPlayer.tag = "Player";
                sleepyPlayer.GetChild(1).gameObject.SetActive(false); //Sleep effect
                sleepyPlayer.GetComponent<Animator>().SetTrigger("WakeyWakey");
                sleepyPlayer.GetComponent<SpriteRenderer>().sprite = awakePlayerSprite;
            }

            checkSleepy = Physics2D.Raycast(child.position + new Vector3(+0.26f, 0f, 0f), Vector3.right, 0.24f, gameLayer);

            if (checkSleepy && checkSleepy.transform.CompareTag("SleepyPlayer")) {
                Transform sleepyPlayer = checkSleepy.transform;
                sleepyPlayer.SetParent(transform);
                sleepyPlayer.tag = "Player";
                sleepyPlayer.GetChild(1).gameObject.SetActive(false); //Sleep effect
                sleepyPlayer.GetComponent<Animator>().SetTrigger("WakeyWakey");
                sleepyPlayer.GetComponent<SpriteRenderer>().sprite = awakePlayerSprite;
            }
        }
    }

}
