using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;
using static TextBasedAdventure;

//Maybe I'm not built for this coding stuff

public class TextBasedAdventure : MonoBehaviour
{
    private const int START_ROW = 0;
    private const int START_COL = 0;

    private const int STARTING_HEALTH = 10;
    private const int ENEMY_DAMAGE = 1;
    private const int ITEM_HEAL_AMOUNT = 2;

    //Teleports must be in pairs, so this constant is used to validate the teleporter list and to loop through it when teleporting.
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
            "You hear distant growling in the darkness.",
            "Rusted weapons lie abandoned here.",
            "A large iron gate leads outside.",
            "The silence in this hall is unsettling."
        },
        {
            "The stench of decay fills the air.",
            "A strange liquid drips from the ceiling.",
            "You see a glint of something shiny in the corner.",
            "The walls are covered in ancient markings."
        },
        {
            "You hear scratching noises nearby.",
            "Something, something, Kings Field 4 reference.",
            "The exit is locked tight. You need to find a key.",
            "A cold breeze sends shivers down your spine."
        },
        {
            "The silence here is deafening.",
            "Bookshelves line the walls, filled with dusty Grimoires.",
            "A barricade blocks your path forward.",
            "The throne room is eerily quiet, but you sense danger."
        }
    };

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

        //Next time, remember your code when you get called on by professor so you dont look like a complete dumbass
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
    private void ItemPickup()
    {
        PlayerHeal(ITEM_HEAL_AMOUNT);
    }
    private void PlayerHeal (int amount)
    {
        playerHealth += amount;
        Debug.Log("Your health is now: " + playerHealth);
    }
    private void PlayerTakeDamage (int amount)
    {
        playerHealth -= amount;
        Debug.Log("Your health is now: " + playerHealth);
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            Debug.Log("You have died. Game Over.");
   
        }
    }
    private void SetPlayerPosition(int newRow, int newCol)
    {
        if (!CheckIfNewPositionInTileBounds(newRow, newCol))
        {
            Debug.Log("Cannot go that way.");
            return;
        }
        if (dungeon[newRow, newCol].Type == TileType.Blockade)
        {
            Debug.Log("A blockade prevents movement.");
            return;
        }

        playerRow = newRow;
        playerCol = newCol;
    }

    private bool CheckIfNewPositionInTileBounds(int newRow, int newCol)
    {
       bool rowInBounds = newRow >= 0 && newRow < dungeon.GetLength(0);

        bool colInBounds = newCol >= 0 && newCol < dungeon.GetLength(1);

        return rowInBounds && colInBounds;
    }
    private bool HandleInput(out int newRow, out int newCol)
    {
        newRow = playerRow;
        newCol = playerCol;

        bool keyPressed = true;

        if (Input.GetKeyDown(KeyCode.W))
        {
            newRow--;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            newRow++;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            newCol--;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            newCol++;
        }
        else
        {
            keyPressed = false;
        }

        return keyPressed;
    }

    private void UseTeleporter()
    {
        if (dungeon[playerRow, playerCol].Type != TileType.Teleporter)
        {
            Debug.Log("There is no teleporter here.");
            return;
        }
        for (int i = 0; i < teleporterLocations.Length; i += TELEPORTER_PAIR_SIZE)
        {
            Position first = teleporterLocations[i];
            Position second = teleporterLocations[i + 1];

            bool atFirst = playerRow == first.Row && playerCol == first.Col;

            bool atSecond = playerRow == second.Row && playerCol == second.Col;

            if (atFirst)
            {
                playerRow = second.Row;
                playerCol = second.Col;

                Debug.Log("Teleported!");
                OutputTileInformation();
                return;
            }

            if (atSecond)
            {
                playerRow = first.Row;
                playerCol = first.Col;

                Debug.Log("Teleported!");
                OutputTileInformation();
                return;
            }
        }
    }

    private void ValidateTeleporters()
    {
        if (teleporterLocations.Length % TELEPORTER_PAIR_SIZE != 0)
        {
            Debug.LogError("Teleporters must be in pairs.");
            return;
        }

        for (int i = 0; i < teleporterLocations.Length; i++)
        {
            Position location = teleporterLocations[i];

            if (dungeon[location.Row, location.Col].Type != TileType.Teleporter)
            {
                Debug.LogError("Teleporter list does not match dungeon.");
            }
        }
    }
}