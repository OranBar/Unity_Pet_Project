using System;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using OranUnityUtils;

public class Main : MonoBehaviour
{

    [BoxGroup("Codingame Encoded Turn Info")]
    public string gameInfo_encoded;
    [BoxGroup("Codingame Encoded Turn Info")]
    [ResizableTextArea]
    public string gameState_and_pulzella_encoded;
    
    [Space]
    
    [ShowNonSerializedField] [ReadOnly]
    private string gameState_encoded;
    [ShowNonSerializedField] [ReadOnly]
    private string pulzella_encoded;
    
    public InfluenceMapVisualizer visualizer;
 
//    private string GAMEINFO_ENC =
//        "24|0.337.826.84.|1.1583.174.84.|2.423.199.65.|3.1497.801.65.|4.1512.561.66.|5.408.439.66.|6.773.605.87.|7.1147.395.87.|8.878.363.61.|9.1042.637.61.|10.649.312.77.|11.1271.688.77.|12.1749.575.72.|13.171.425.72.|14.658.833.77.|15.1262.167.77.|16.168.168.78.|17.1752.832.78.|18.509.651.65.|19.1411.349.65.|20.914.831.76.|21.1006.169.76.|22.152.650.62.|23.1768.350.62.|";
//
//    private string GAMESTATE_ENC =
//        "24|0.xxxxxx0.|1.xxxxxx0.|2.213.1.2.0.0.0.0.|3.xx0.1.xx0.|4.xxxxxx0.|5.224.3.xxxx0.|6.xxxxxx0.|7.xxxxxx0.|8.xxxxxx0.|9.xxxxxx0.|10.xxxxxx0.|11.xxxxxx0.|12.xxxxxx0.|13.226.3.xxxx0.|14.xxxxxx0.|15.xxxxxx0.|16.215.1.0.0.1.x0.|17.xx0.1.xx0.|18.xxxxxx0.|19.xxxxxx0.|20.xxxxxx0.|21.xxxxxx0.|22.xxxxxx0.|23.xxxxxx0.|2|327.200.0.x90.|1580.754.1.x90.|104|2|";
//
//    private string PULZELLAINFO_ENC = "1.1.0.0.6.x10.";

    private void Start()
    {
        RunTurn();
    }

    [Button]
    public void ReRunSimulation()
    {
        Start();
    }
    
    
    public void RunTurn()
    {
        var tmp = gameState_and_pulzella_encoded.Split('-');
        gameState_encoded = tmp[0];
        pulzella_encoded = tmp[1];
//        
//        Debug.Assert(gameInfo_encoded.Contains("x") == false);
//        Debug.Assert(gameState_encoded.Contains("x"));
//        Debug.Assert(pulzella_encoded.Length <= 15);
//        
        LaPulzellaD_Orleans giovannaD_Arco = new LaPulzellaD_Orleans();
//        try
//        {
            giovannaD_Arco.Decode(pulzella_encoded);
            giovannaD_Arco.game = new GameState();
            giovannaD_Arco.game.Decode(gameState_encoded);
            giovannaD_Arco.gameInfo = new GameInfo();
            giovannaD_Arco.gameInfo.Decode(gameInfo_encoded);

//        }
//        catch (FormatException)
//        {
//            gameInfo_encoded = gameInfo_encoded.Remove(gameInfo_encoded.Length-1);
//            gameState_encoded = gameState_encoded.Remove(gameState_encoded.Length-1);
//            pulzella_encoded = pulzella_encoded.Remove(pulzella_encoded.Length-1);
//            
//            giovannaD_Arco.Decode(pulzella_encoded);
//            giovannaD_Arco.game = new GameState();
//            giovannaD_Arco.game.Decode(gameState_encoded);
//            giovannaD_Arco.gameInfo = new GameInfo();
//            giovannaD_Arco.gameInfo.Decode(gameInfo_encoded);
//
//        }

        foreach (var site in giovannaD_Arco.game.sites)
        {
            site.pos = giovannaD_Arco.gameInfo.sites[site.siteId].pos;
        }

        InfluenceMap map = new InfluenceMap();
        InfluenceMap buildMap = new InfluenceMap();
        TurnAction action = giovannaD_Arco.think(out map, out buildMap);
        
        Debug.Log("chosen action "+action.queenAction);
        
        var myQueenPosition = giovannaD_Arco.game.MyQueen.pos;
        int xIndex = myQueenPosition.x / LaPulzellaD_Orleans.INFLUENCEMAP_SQUARELENGTH;
        int yIndex = myQueenPosition.y / LaPulzellaD_Orleans.INFLUENCEMAP_SQUARELENGTH;
        

        visualizer.SetMyQueenPosition(new Position(xIndex, yIndex));
        visualizer.SetNewInfluenceMap(buildMap);
        visualizer.UpdateCells();
    }
    
//    public void RunTurn(string gameState_Enc, string gameInfo_Enc)
//    {
//        LaPulzellaD_Orleans giovannaD_Arco = new LaPulzellaD_Orleans();
//        giovannaD_Arco.currGameState = new GameState();
//        giovannaD_Arco.currGameState.Decode(gameState_Enc);
//        giovannaD_Arco.game = new GameInfo();
//        giovannaD_Arco.game.Decode(gameInfo_Enc);
//
//        foreach (var site in giovannaD_Arco.currGameState.sites)
//        {
//            site.pos = giovannaD_Arco.game.sites[site.siteId].pos;
//        }
//        
//        InfluenceMap map = new InfluenceMap();
//        giovannaD_Arco.think(out map);
//    }
    
    
}
