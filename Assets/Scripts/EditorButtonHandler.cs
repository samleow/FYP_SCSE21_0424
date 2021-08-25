
using UnityEngine;

public enum TileType : int
{
    NONE,
    WALL,
    POINT,
    POWERUP,
    ERASE
}

public class EditorButtonHandler : MonoBehaviour
{
    private TileType _selectedTile = TileType.NONE;

    // param type -> TileType
    public void SelectTile(int type)
    {
        /*switch (type)
        {
            case TileType.NONE:
                break;
            case TileType.WALL:
                break;
            case TileType.POINT:
                break;
            case TileType.POWERUP:
                break;
        }*/

        _selectedTile = (TileType)type;

        Debug.Log("Tile Selected : " + _selectedTile);
    }

}
