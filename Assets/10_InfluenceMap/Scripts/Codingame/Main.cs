using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using OranUnityUtils;
using UnityEngine.Profiling;

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
    private int squareLength;


    private void Start()
    {
        RunTurn();
    }

    [Button]
    public void ReRunSimulation()
    {
        RunTurn();
    }

    public bool runTurn;

    private void Update()
    {
        if (runTurn)
        {
            Profiler.BeginSample("------------------------------------------------ Run Turn - BEGIN");
            RunTurn();
            Profiler.EndSample();
            runTurn = false;
        }
    }


    public void RunTurn()
    {
//        Profiler.BeginSample("Run Turn - BEGIN");
        var tmp = gameState_and_pulzella_encoded.Split('-');
        gameState_encoded = tmp[0];
        pulzella_encoded = tmp[1];

        LaPulzellaD_Orleans giovannaD_Arco = new LaPulzellaD_Orleans();
//        try
//        {
            giovannaD_Arco.Decode(pulzella_encoded);
            giovannaD_Arco.game = new GameState();
            giovannaD_Arco.game.Decode(gameState_encoded);
            giovannaD_Arco.gameInfo = new GameInfo();
            giovannaD_Arco.gameInfo.Decode(gameInfo_encoded);

            giovannaD_Arco.InitializeInfluenceMaps();
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

        TurnAction action = giovannaD_Arco.think();
        
//        Profiler.EndSample();
        
        Debug.Log("chosen action "+action.queenAction);

//        giovannaD_Arco.SurvivorModeMap.ResetMapToZeroes();
        int xIndex, yIndex;

        var myQueenPosition = giovannaD_Arco.game.MyQueen.pos;
        squareLength = LaPulzellaD_Orleans.INFLUENCEMAP_SQUARELENGTH;
        xIndex = (int) Math.Ceiling(myQueenPosition.x / squareLength*1.0);
        yIndex = (int) Math.Ceiling(myQueenPosition.y / squareLength*1.0);
//        giovannaD_Arco.SurvivorModeMap.ApplyInfluence_Range_Unscaled(myQueenPosition.x, myQueenPosition.y, 10, 2, 5, LaPulzellaD_Orleans.linearPropagation);
        
//        giovannaD_Arco.SurvivorModeMap.ApplyInfluence_Range_Unscaled(giovannaD_Arco.game.sites.First().pos.x, giovannaD_Arco.game.sites.First().pos.y, 10, 1, 25, LaPulzellaD_Orleans.linearPropagation);
        
        
        visualizer.SetMyQueenPosition(new Position(xIndex, yIndex));
        foreach (var enemy in giovannaD_Arco.game.EnemyUnits.Where(u=>u.unitType != UnitType.Queen))
        {
            var myEnemyPosition = enemy.pos;
            xIndex = (int) Math.Ceiling(myEnemyPosition.x / squareLength*1.0);
            yIndex = (int) Math.Ceiling(myEnemyPosition.y / squareLength*1.0);
            
            visualizer.SetMyEnemyPosition(new Position(xIndex, yIndex));
        }
        
        var enemyQueenPosition = giovannaD_Arco.game.EnemyQueen.pos;
        xIndex = (int) Math.Ceiling(enemyQueenPosition.x / squareLength*1.0);
        yIndex = (int) Math.Ceiling(enemyQueenPosition.y / squareLength*1.0);
        
        
        visualizer.SetEnemyQueenPosition(new Position(xIndex, yIndex));
        
        visualizer.SetNewInfluenceMap(giovannaD_Arco.SurvivorModeMap);
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
