using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using OranUnityUtils;
using UnityEngine.Profiling;
using UnityEngineInternal.Input;

public class Main : MonoBehaviour
{

    [BoxGroup("Codingame Encoded Turn Info")]
    public string gameInfo_encoded;
    [BoxGroup("Codingame Encoded Turn Info")]
    [ResizableTextArea]
    public string gameState_and_pulzella_encoded;
    
    [Button]
    public void ReRunSimulation()
    {
        ParseGame();
        turnToRun = 1;
        RunTargetTurn();
    }
    
    [Space]
    
    [SerializeField] [ReadOnly]
    private string gameState_encoded;
    [SerializeField] [ReadOnly]
    private string pulzella_encoded;
    
    public InfluenceMapVisualizer visualizer;
    private int squareLength;

    public Slider turnSlider;
    public Text sliderTurnLabel;

    private void Start()
    {
        ParseGame();
        RunTargetTurn();
    }

    public string path;
    
    public string gameToParse;

    private DropdownList<string> GetHtmlFiles()
    {
        var result = new DropdownList<string>();
        Directory.GetFiles(path).Where(f => Path.GetExtension(f)==".html").ForEach(f1 => result.Add(f1, f1));
        return result;
    }
    
    string gameLog;

    List<EncodedTurnAndChosenTile> parsedGame_turns = new List<EncodedTurnAndChosenTile>();

    public class EncodedTurnAndChosenTile
    {
        public string encoded_turn;
        public Position chosenTile;

        public EncodedTurnAndChosenTile(string encodedTurn, Position chosenTile)
        {
            encoded_turn = encodedTurn;
            this.chosenTile = chosenTile;
        }
    }
    
    [Button("Parse Game")]
    private void ParseGame()
    {
        parsedGame_turns.Clear();
        gameLog = File.ReadAllText(path+"\\"+gameToParse+".csv");

        var lines = gameLog.Split('\n');
        
        gameInfo_encoded = lines[0];

        for (int i = 1; i < (lines.Length - 1); i=i+2)
        {
            var strPosition = lines[i + 1];
            var str_positions_split = strPosition.Split('.');
            var chosenMove = new Position(int.Parse(str_positions_split[0]), int.Parse(str_positions_split[1]));

            var gameState_encoded = lines[i];
            
            EncodedTurnAndChosenTile tmp = new EncodedTurnAndChosenTile(gameState_encoded , chosenMove);
            parsedGame_turns.Add(tmp);
        }

        turnSlider.minValue = 1; 
        turnSlider.maxValue = (parsedGame_turns.Count * 2);    //max value is all my turns, so it's half of the max turns. *
        turnSlider.onValueChanged.AddListener(sliderValue =>
        {
            RunTargetTurn(sliderValue);
            sliderTurnLabel.text = (turnSlider.value * 2) + "/" + (turnSlider.maxValue);    // * That's why we multiply by 2
        });

        sliderTurnLabel.text = "2/" +turnSlider.maxValue;
        Debug.Log("Game Parsed");
    }

    public int turnToRun;
    
    [Button()]
    private void RunTargetTurn()
    {
        //First line is game info. we skip it
        gameState_and_pulzella_encoded = parsedGame_turns[turnToRun].encoded_turn;
        Debug.Log("Turn "+(turnToRun*2)+" loaded");
        RunTurn();
    }

    public void RunNextTurn()
    {
        if (turnToRun <= parsedGame_turns.Count - 1)
        {
            //This calls RunTargetTurn
            turnSlider.value +=1;
        }
    }
    
    public void RunPreviousTurn()
    {
        if (turnToRun >= 0)
        {
            //This calls RunTargetTurn
            turnSlider.value -= 1;
        }
    }
    
    private void RunTargetTurn(float turn)
    {
        //First line is game info. we skip it
        turnToRun = (int) turn - 1;
        RunTargetTurn();
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.LeftArrow))
        {
            RunPreviousTurn();
        }
        else if(Input.GetKeyUp(KeyCode.RightArrow))
        {
            RunNextTurn();
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

        Position chosenTile = null;
        TurnAction action = giovannaD_Arco.think(out chosenTile);
        
//        Profiler.EndSample();
        
        Debug.Log("chosen action "+action.queenAction);
        
        //Test----------------------------------------------------
//        giovannaD_Arco.SurvivorModeMap.ResetMapToZeroes();
//        var myQueenPosition = giovannaD_Arco.game.MyQueen.pos;
//        var enemmyQueenPosition = giovannaD_Arco.game.EnemyQueen.pos;
//
//        giovannaD_Arco.SurvivorModeMap.ApplyInfluence_Range_Unscaled(myQueenPosition.x, myQueenPosition.y, 10, 5, 10, LaPulzellaD_Orleans.linearDecay);
//        
//        giovannaD_Arco.SurvivorModeMap.ApplyInfluence_Range_Unscaled(enemmyQueenPosition .x, enemmyQueenPosition .y, 10, 0, 20, LaPulzellaD_Orleans.linearDecay);
        //----------------------------------------------------
        
        
        visualizer.HighlightsOff();
        
        visualizer.SetNewInfluenceMap(giovannaD_Arco.SurvivorModeMap);
        visualizer.UpdateCells_AccordingToInfluence();
        
        visualizer.SetSquareHighlights(giovannaD_Arco.game, chosenTile, parsedGame_turns[turnToRun].chosenTile);
        
        
//        int xIndex, yIndex;
//
//        var myQueenPosition = giovannaD_Arco.game.MyQueen.pos;
//        squareLength = LaPulzellaD_Orleans.INFLUENCEMAP_SQUARELENGTH;
//        xIndex = (int) Math.Ceiling(myQueenPosition.x / squareLength*1.0);
//        yIndex = (int) Math.Ceiling(myQueenPosition.y / squareLength*1.0);
////        giovannaD_Arco.SurvivorModeMap.ApplyInfluence_Range_Unscaled(myQueenPosition.x, myQueenPosition.y, 10, 2, 5, LaPulzellaD_Orleans.linearPropagation);
//        
////        giovannaD_Arco.SurvivorModeMap.ApplyInfluence_Range_Unscaled(giovannaD_Arco.game.sites.First().pos.x, giovannaD_Arco.game.sites.First().pos.y, 2, 1, 0, LaPulzellaD_Orleans.linearPropagation);
//
//        visualizer.HighlightTile(new Position(chosenTile.Item1, chosenTile.Item2), Color.green);
//        
//        visualizer.SetChosenTile(new Position(chosenTile.Item1, chosenTile.Item2));
//        visualizer.SetMyQueenPosition(new Position(xIndex, yIndex));
//        foreach (var enemy in giovannaD_Arco.game.EnemyUnits.Where(u=>u.unitType != UnitType.Queen))
//        {
//            var myEnemyPosition = enemy.pos;
//            xIndex = (int) Math.Ceiling(myEnemyPosition.x / squareLength*1.0);
//            yIndex = (int) Math.Ceiling(myEnemyPosition.y / squareLength*1.0);
//            
//            visualizer.SetMyEnemyPosition(new Position(xIndex, yIndex));
//        }
//        
//        var enemyQueenPosition = giovannaD_Arco.game.EnemyQueen.pos;
//        xIndex = (int) Math.Ceiling(enemyQueenPosition.x / squareLength*1.0);
//        yIndex = (int) Math.Ceiling(enemyQueenPosition.y / squareLength*1.0);
//        
//        
//        visualizer.SetEnemyQueenPosition(new Position(xIndex, yIndex));
//        
//        visualizer.SetNewInfluenceMap(giovannaD_Arco.SurvivorModeMap);
//        visualizer.UpdateCells_AccordingToInfluence();
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
