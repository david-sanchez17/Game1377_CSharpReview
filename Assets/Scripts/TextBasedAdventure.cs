using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;
using static TextBasedAdventure;


public class TextBasedAdventure : MonoBehaviour
{
    private const int START_ROW = 0;
    private const int START_COL = 0;

    private const int STARTING_HEALTH = 10;
    private const int ENEMY_DAMAGE = 1;
    private const int ITEM_HEAL_AMOUNT = 2;

    private const int TELEPORTER_PAIR_SIZE = 2;

    [System.Serializable]
    public struct Room
    {
        public string Name;
        public TileType Type;
        public bool Visited;
    }

    [System.Serializable]
    public struct Position
    {
        public int Row;
        public int Col;
    }

    public enum TileType
    {
        Invalid,
        Empty,
        Item,
        Enemy,
        Exit,
        Blockade,
        Teleporter
    }

    private Room[,] dungeon =
    {
        {
            new Room { Name = "Dark Cave", Type = TileType.Empty },
            new Room { Name = "Mossy Tunnel", Type = TileType.Item },
            new Room { Name = "Rock Wall", Type = TileType.Blockade },
            new Room { Name = "Blue Portal", Type = TileType.Teleporter }
        },

        {
            new Room { Name = "Bone Chamber", Type = TileType.Enemy },
            new Room { Name = "Flooded Hall", Type = TileType.Empty },
            new Room { Name = "Storage Room", Type = TileType.Empty },
            new Room { Name = "Treasure Nook", Type = TileType.Item }
        },

        {
            new Room { Name = "Goblin Den", Type = TileType.Empty },
            new Room { Name = "Armory", Type = TileType.Enemy },
            new Room { Name = "Iron Gate", Type = TileType.Exit },
            new Room { Name = "Quiet Hall", Type = TileType.Empty }
        },

        {
            new Room { Name = "Red Portal", Type = TileType.Teleporter },
            new Room { Name = "Library", Type = TileType.Empty },
            new Room { Name = "Barricade", Type = TileType.Blockade },
            new Room { Name = "Throne Room", Type = TileType.Enemy }
        }
    };

    private string[,] tileDescriptions =
    {
        {

private Position[] teleporterLocations =
{
        new Position { Row = 0, Col = 3 },
        new Position { Row = 3, Col = 0 }
    };

private int playerRow = START_ROW;
private int playerCol = START_COL;
private int playerHealth = STARTING_HEALTH;

private void Start()
{
    ValidateTeleporters();
    OutputTileInformation();
}

private void Update()
{
    if (Input.GetKeyDown(KeyCode.L))
    {
        Look();
        return;
    }

    if (Input.GetKeyDown(KeyCode.T))
    {
        UseTeleporter();
        return;
    }

    bool keyPressed = HandleInput(out int newRow, out int newCol);

    if (!keyPressed)
    {
        return;
    }

    SetPlayerPosition(newRow, newCol);
    OutputTileInformation();
}
  
private void OutputTileInformation()
{
    Room currentRoom = dungeon[playerRow, playerCol];

    Debug.Log("You are in: " + currentRoom.Name);

    if (!currentRoom.Visited)
    {
        Debug.Log(tileDescriptions[playerRow, playerCol]);

        currentRoom.Visited = true;
        dungeon[playerRow, playerCol] = currentRoom;
    }

    switch (currentRoom.Type)
        {
            case TileType.Empty:
                Debug.Log("There is nothing here.");
                break;

            case TileType.Item:
                Debug.Log("You found an item.");
                ItemPickup();
                break;

            case TileType.Enemy:
                Debug.Log("A goblin attacks you!");
                EncounterEnemy();
                break;

            case TileType.Exit:
                Debug.Log("You found the exit!");
                break;

            case TileType.Blockade:
                Debug.Log("A blockade is here. You cannot pass.");
                break;

            case TileType.Teleporter:
                Debug.Log("A teleporter stands before you. Press 'T' to use it.");
                break;

            default:
                Debug.LogError("Invalid tile type");
                break;
        }

}
    private void Look()
    {
        Debug.Log(tileDescriptions[playerRow, playerCol]);
    }
    private void EncounterEnemy()
    {
        PlayerTakeDamage(ENEMY_DAMAGE);
    }
}
