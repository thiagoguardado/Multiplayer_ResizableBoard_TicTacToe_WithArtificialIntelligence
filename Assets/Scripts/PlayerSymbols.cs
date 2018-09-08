using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Symbols", menuName = "Player Symbols",order =0)]
public class PlayerSymbols : ScriptableObject {

    public List<SymbolAndSprite> possiblePlayerSprites;

}

[System.Serializable]
public class SymbolAndSprite
{
    public PlayerSymbol playerSymbol;
    public Sprite playerSprite;

    public SymbolAndSprite(PlayerSymbol playerSymbol, Sprite playerSprite)
    {
        this.playerSymbol = playerSymbol;
        this.playerSprite = playerSprite;
    }
}
