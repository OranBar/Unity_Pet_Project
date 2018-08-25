#define RUNLOCAL

/** Code by Oran Bar **/

//The max characters that can be put into the error stream is 1028

using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditorInternal;
using OranUnityUtils;
using UnityEngine;
#endif

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/

public delegate double PropagationFunction(double amount, double distance, double maxDistance);

class Player
{
    static void Main(string[] args)
    {
        LaPulzellaD_Orleans giovannaD_Arco = new LaPulzellaD_Orleans();

        giovannaD_Arco.ParseInputs_Begin();
        giovannaD_Arco.InitializeInfluenceMaps();
        
        int turn = 0;
        
        while (true)
        {
            turn = turn + 2;
            giovannaD_Arco.turn = turn;
            
            giovannaD_Arco.ParseInputs_Turn();
//            Stopwatch sw = new Stopwatch();
//            sw.Start();
            Console.Error.WriteLine(giovannaD_Arco.game.Encode()+"-"+giovannaD_Arco.Encode());
            if (turn == 2)
            {
//                Console.Error.WriteLine("Game Info");
                Console.Error.WriteLine(giovannaD_Arco.gameInfo.Encode());
            }
            
//            Console.Error.WriteLine(giovannaD_Arco.Encode());
//            sw.Stop();
//            Console.Error.WriteLine("Encoding ={0}",sw.Elapsed);

            Tuple<int, int> chosenMove;
            TurnAction move = giovannaD_Arco.think(out chosenMove);
            Console.Error.WriteLine(chosenMove.Item1+"."+chosenMove.Item2);
            move.PrintMove();
        }
    }
}

public class Position
{
    public int x, y;

    public Position()
    {
        
    }
    
    public Position(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return $"({x},{y})";
    }

    public double DistanceSqr(Position distanceTo)
    {
        return Math.Pow((this.x - distanceTo.x), 2) + Math.Pow((this.y - distanceTo.y), 2);
    }
    
    public double DistanceTo(Position distanceTo)
    {
        return Math.Sqrt(Math.Pow((this.x - distanceTo.x), 2) + Math.Pow((this.y - distanceTo.y), 2));
    }
}

#region IActions

public abstract class IAction
{
    public abstract string ToString_Impl();
    public string message;

    public IAction(string message = "")
    {
        this.message = message;
    }

    public override string ToString()
    {
        if (message != "")
        {
            return ToString_Impl() +" "+ message;
        }
        else
        {
            return ToString_Impl();
        }

    }
}

public class TurnAction : IAction
{
    public IAction queenAction;
    public IAction trainAction;

    public TurnAction()
    {
        this.message = "";
        queenAction = new Wait();
        trainAction = new HaltTraing();
    }
    
    public TurnAction(string message) : base(message)
    {
        this.message = "";
    }

    public void PrintMove()
    {
        Console.WriteLine(queenAction.ToString());
        Console.WriteLine(trainAction.ToString());
    }
    
    public override string ToString_Impl()
    {
        return "Invalid";
    }

    public override string ToString()
    {
        return ToString_Impl();
    }
}

#region Train Actions

public class HaltTraing : IAction
{
    public string actionName = "TRAIN";

    public HaltTraing(string message = "") : base(message)
    {
        if(message != ""){throw new ArgumentException();}
    }

    
    public override string ToString_Impl()
    {
        return this.actionName;
    }
}

public class Train : IAction
{
    public string actionName = "TRAIN";
    public List<int> siteIds;

    public Train(string message = "") : base(message){}

    public Train(IEnumerable<Site> baraccksesToTrainFrom)
    {
        siteIds = baraccksesToTrainFrom.Select(s => s.siteId).ToList();
    }

    public override string ToString_Impl()
    {
        string result = this.actionName;
        foreach (var siteId in siteIds)
        {
            result += " " + siteId;
        }

        return result;
    }
}

#endregion

#region QueenActions

public class Wait : IAction
{
    public string actionName = "WAIT";

    public Wait(string message = "") : base(message){}

    public override string ToString_Impl()
    {
        return this.actionName;
    }
}

public class Move : IAction
{
    public string actionName = "MOVE";
    public Position targetPos;

    public Move(Position pos, string message = "") : base(message)
    {
        targetPos = pos;
    }
    
    public Move(int x, int y, string message = "") : base(message)
    {
        targetPos = new Position(x,y);
    }
    
    public override string ToString_Impl()
    {
        return actionName + " " + targetPos.x +" " + targetPos.y;
    }
}

public class BuildBarracks : IAction
{
    public int siteId;
    public string barracks_type;

    public BuildBarracks(int siteId, BarracksType barracksType, string message = "") : base(message)
    {
        this.siteId = siteId;
        switch (barracksType)
        {
            case BarracksType.Knight:
                this.barracks_type = "KNIGHT";
                break;
            case BarracksType.Archer:
                this.barracks_type = "ARCHER";
                break;
            case BarracksType.Giant:
                this.barracks_type = "GIANT";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(barracksType), barracksType, null);
        }
    }

    public override string ToString_Impl()
    {
        return $"BUILD {siteId} BARRACKS-{barracks_type}";
    }
}

public class BuildMine : IAction
{
    public int siteId;

    public BuildMine(int siteId, string message = "") : base(message)
    {
        this.siteId = siteId;
    }

    public override string ToString_Impl()
    {
        return $"BUILD {siteId} MINE";
    }
}

public class BuildTower : IAction
{
    public int siteId;

    public BuildTower(int siteId, string message = "") : base(message)
    {
        this.siteId = siteId;
    }

    public override string ToString_Impl()
    {
        return $"BUILD {siteId} TOWER";
    }
}

public enum BarracksType
{
    Knight = 0,
    Archer = 1,
    Giant = 2
}

#endregion

#endregion


#region Game Structures

public class Unit
{
    public Position pos;
    public Owner owner;
    public UnitType unitType;
    public int health;

    public Unit()
    {
        
    }
    
    public Unit(int x, int y, int owner, int unitType, int health)
    {
        this.pos = new Position(x, y);
        this.owner = (Owner) owner;
        this.unitType = (UnitType) unitType;
        this.health = health;
    }

    public double DistanceTo(Site other)
    {
        return Math.Sqrt(Math.Pow(this.pos.x - other.pos.x, 2) + Math.Pow(this.pos.y - other.pos.y, 2));
    }
    
    public double DistanceTo(Unit other)
    {
        return Math.Sqrt(Math.Pow(this.pos.x - other.pos.x, 2) + Math.Pow(this.pos.y - other.pos.y, 2));
    }
    

    public string Encode()
    {
        StringEncoderBuilder result = new StringEncoderBuilder(".");
        if(pos == null){ pos = new Position(); }
        result.Append(pos.x);
        result.Append(pos.y);
        result.Append((int) owner);
        result.Append((int) unitType);
        result.Append(health);
        return result.Build();
    }

    public void Decode(string encoded)
    {
        encoded = encoded.Replace("x", "-1.");
        String[] values = encoded.Split('.');
        pos = new Position();
        pos.x = int.Parse(values[0]);
        pos.y = int.Parse(values[1]);
        owner = (Owner) int.Parse(values[2]);
        unitType = (UnitType) int.Parse(values[3]);
        health = int.Parse(values[4]);
    }

    public override string ToString()
    {
        return $"Pos: {pos} - Owner: {owner} - UnitType {unitType} - health {health}";
    }

    public double DistanceSqr(Position sitePos)
    {
        return pos.DistanceSqr(sitePos);
    }
}

public class GameState
{
    public List<Site> sites = new List<Site>();
    public List<Unit> units = new List<Unit>();
    public int money;
    public int touchedSiteId;

    public int EnemyUnitsInRangeOfMyQueen(int range) => units.Count(u => u.owner == Owner.Enemy && MyQueen.DistanceTo(u) <= range);
    public int EnemyUnitsInRangeOf(Position p, int range) => units.Count(u => u.owner == Owner.Enemy && p.DistanceTo(u.pos) <= range);
    public int AlliedTowersInRangeOf(Position p, int range) => sites.Count(s => s.owner == Owner.Friendly && p.DistanceTo(s.pos) <= range);
    
    //Properties
    public List<Site> MySites {get;private set;}
    public List<Site> EnemySites {get;private set;}
    public List<Unit> EnemyUnits {get;private set;}
    public List<Unit> MyUnits {get;private set;}
    public Site TouchedSite {get;private set;}  
    public Unit MyQueen {get;private set;}
    public Unit EnemyQueen {get;private set;}
    public int UnitsCount {get;private set;}
    public int Owned_knight_barrackses {get;private set;}
    public int Owned_archer_barrackses {get;private set;}
    public int Owned_giant_barrackses {get;private set;}
    public int Owned_mines {get;private set;}
    public int Owned_towers {get;private set;}
    public int Total_owned_barrackses {get;private set;}
    public bool TouchingNeutralSite {get;private set;}
    public bool TouchingMyMine {get;private set;}
    public bool TouchingMyTower {get;private set;}
    public bool Prev_touchingMyMine {get;private set;}
    
    public int Owned_giants {get;private set;}
    public List<Site> EnemyTowers { get; set; }
    public List<Site> MyTowers { get; set; }

    /**
     * Call this if the properties are null or 0
     */
    public void PreLoadProperties()
    {
        MySites = sites.Where(s => s.owner == Owner.Friendly).ToList();
        EnemySites = sites.Where(s => s.owner == Owner.Enemy).ToList();
        EnemyUnits = units.Where(u => u.owner == Owner.Enemy && u.unitType != UnitType.Queen).ToList();
        MyUnits = units.Where(u => u.owner == Owner.Friendly && u.unitType != UnitType.Queen).ToList();
        TouchedSite = (touchedSiteId != -1)? sites[touchedSiteId] : null;  
        MyQueen = units.First(u => u.owner == Owner.Friendly && u.unitType == UnitType.Queen);
        EnemyQueen = units.First(u => u.owner == Owner.Enemy && u.unitType == UnitType.Queen);
        UnitsCount = units.Count;
        Owned_knight_barrackses = MySites.Count(ob => ob.structureType == StructureType.Barracks && ob.CreepsType == UnitType.Knight);
        Owned_archer_barrackses = MySites.Count(ob => ob.structureType == StructureType.Barracks && ob.CreepsType == UnitType.Archer);
        Owned_giant_barrackses = MySites.Count(ob => ob.structureType == StructureType.Barracks && ob.CreepsType == UnitType.Giant);
        Owned_mines = MySites.Count(ob => ob.structureType == StructureType.Mine && ob.gold > 0);
        Owned_towers = MySites.Count(ob => ob.structureType == StructureType.Tower && ob.owner == Owner.Friendly);
        Total_owned_barrackses = Owned_archer_barrackses + Owned_giant_barrackses + Owned_knight_barrackses;
        TouchingNeutralSite = TouchedSite != null && TouchedSite.owner == Owner.Neutral;
        TouchingMyMine = TouchedSite != null && TouchedSite.owner == Owner.Friendly && TouchedSite.structureType == StructureType.Mine;
        TouchingMyTower = TouchedSite != null && TouchedSite.owner == Owner.Friendly && TouchedSite.structureType == StructureType.Tower;
        Prev_touchingMyMine = TouchedSite != null && TouchedSite.owner == Owner.Friendly && TouchedSite.structureType == StructureType.Mine;
        Owned_giants = units.Count(u => u.owner == Owner.Friendly && u.unitType == UnitType.Giant);
        EnemyTowers = sites.Where(t => t.owner == Owner.Enemy && t.structureType == StructureType.Tower).ToList();
        MyTowers = sites.Where(t => t.owner == Owner.Friendly && t.structureType == StructureType.Tower).ToList();
    }
    
    public string Encode()
    {
        StringEncoderBuilder result = new StringEncoderBuilder("|");
        int noOfSites = sites.Count;
        result.Append(noOfSites);
        for (int i = 0; i < sites.Count; i++)
        {
            result.Append(sites[i].Encode());
        }
        int noOfUnits = units.Count;
        result.Append(noOfUnits);
        for (int i = 0; i < units.Count; i++)
        {
            result.Append(units[i].Encode());
        }

        result.Append(money);
        result.Append(touchedSiteId);
        return result.Build();
    }
    public void Decode(string encoded)
    {
        encoded = encoded.Replace("x", "-1.");
        encoded = encoded.Remove(encoded.Length - 1);
        
        String[] values = encoded.Split('|');
        int noOfSites = int.Parse(values[0]);
        for (int i = 1; i < noOfSites+1; i++)
        {
            var site = new Site();
            site.Decode(values[i]);
            sites.Add(site);
        }
        int noOfUnits = int.Parse(values[noOfSites+1]);
        for (int i = 0; i < noOfUnits; i++)
        {
            var unit = new Unit();
            unit .Decode(values[noOfSites+2+i]);
            units.Add(unit);
        }

        this.money = int.Parse(values[values.Length - 2]);
        this.touchedSiteId= int.Parse(values.Last());

        PreLoadProperties();
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"Money = {money} - touchingSiteId = {touchedSiteId} - touchedSite = {sites.FirstOrDefault(s=>touchedSiteId == s.siteId)}\n" );
        sb.Append("Sites: \n");
        sites.ForEach(s => sb.Append(s+"\n"));
        
        sb.Append("Units: \n");
        sites.ForEach(s => sb.Append(s+"\n"));

        return sb.ToString();
    }
}

public enum UnitType
{
    Queen = -1,
    Knight = 0,
    Archer = 1,
    Giant = 2
}

public enum StructureType
{
    None = -1,
    Mine = 0,
    Tower = 1,
    Barracks = 2
}

public enum Owner
{
    Neutral = -1,
    Friendly = 0,
    Enemy = 1
}

public class GameInfo
{
    public Dictionary<int, SiteInfo> sites = new Dictionary<int, SiteInfo>();
    public HashSet<int> minedOutSites_ids = new HashSet<int>();    //Info passed into Site
    
    //Properties
    public int numSites => sites.Count;
    
    public string GetSites_ToString()
    {
        return sites.Aggregate("", (agg, x) => agg + "\n"+ x.ToString());
    }

    public string Encode()
    {
        StringEncoderBuilder sb = new StringEncoderBuilder("|");
        sb.Append(sites.Count);
        for (int i = 0; i < sites.Count; i++)
        {
            sb.Append(sites[i].Encode());
        }
        return sb.Build();
    }

    public void Decode(string encoded)
    {
        encoded = encoded.Replace("x", "-1.");
        var values = encoded.Split('|');
        int sitesCount = int.Parse(values[0]);
        for (int i = 1; i < sitesCount+1; i++)
        {
            SiteInfo site = new SiteInfo();
            site.Decode(values[i]);
            sites[site.siteId] = site;
        }
    }
    
}

public class SiteInfo
{
    public int siteId;
    public Position pos;
    public int radius;

    public SiteInfo()
    {
            
    }
    public SiteInfo(int siteId, Position pos, int radius)
    {
        this.siteId = siteId;
        this.pos = pos;
        this.radius = radius;
    }

    public override string ToString()
    {
        return $"Site {siteId} - Pos: {pos} - Rad: {radius}";
    }

    public string Encode()
    {
        StringEncoderBuilder sb = new StringEncoderBuilder(".");
        sb.Append(siteId);
        sb.Append(pos.x);
        sb.Append(pos.y);
        sb.Append(radius);
        return sb.Build();
    }
    
    public void Decode(string encoded)
    {
        encoded = encoded.Replace("x", "-1.");
        var values = encoded.Split('.');
        this.siteId = int.Parse(values[0]);
        pos = new Position(int.Parse(values[1]), int.Parse(values[2]));
        radius = int.Parse(values[3]);
    }
}

public class Site
{
    public int siteId;
    public int gold;
    public int maxMineSize;
    public StructureType structureType = StructureType.None;
    public Owner owner;
    public int param1;
    public int param2;
    public bool isMinedOut;

    public UnitType CreepsType => (UnitType) param2;

    public Position pos;

    public Site()
    {
        
    }
    
    public Site(int siteId, int gold, int maxMineSize, int structureType, int owner, int param1, int param2)
    {
        this.siteId = siteId;
        this.gold = gold;
        this.maxMineSize = maxMineSize;
        this.structureType = (StructureType) structureType;
        this.owner = (Owner) owner;
        this.param1 = param1;
        this.param2 = param2;
    }
    
    public override string ToString()
    {
        return $"Site {siteId} - gold: {gold} - maxMineSize: {maxMineSize} - structureType: {structureType} - owner: {owner} - param1: {param1} - creepsType: {CreepsType} - param2: {param2}";
    }

    public string Encode()
    {
        StringEncoderBuilder result = new StringEncoderBuilder(".");
        result.Append(siteId);
        result.Append(gold);
        result.Append(maxMineSize);
        result.Append((int) structureType);
        result.Append((int) owner);
        result.Append(param1);
        result.Append(param2);
        result.Append(isMinedOut);
        return result.Build();
    }
    
    public void Decode(string encoded)
    {
        encoded = encoded.Replace("x", "-1.");
        String[] values = encoded.Split('.');
        siteId = int.Parse(values[0]);
        gold = int.Parse(values[1]);
        maxMineSize = int.Parse(values[2]);
        structureType = (StructureType)(int.Parse(values[3]));
        owner = (Owner)(int.Parse(values[4]));
        param1 = int.Parse(values[5]);
        param2 = int.Parse(values[6]);
        isMinedOut = values[7] == "1";
    }

}

public class StringEncoderBuilder
{
    private StringBuilder result = new StringBuilder();
    private string delimiter;

    public StringEncoderBuilder(string delimiter)
    {
        this.delimiter = delimiter;
    }

    public void Append(bool b)
    {
        if (b)
        {
            result.Append("1"+delimiter);
        }
        else
        {
            result.Append("0"+delimiter);
        }
    }
    
    public void Append(int i)
    {
        if (i == -1)
        {
            result.Append("x");
        }
        else
        {
            result.Append(i+delimiter);
        }
    }
    
    public void Append(string s)
    {

        if (s == "-1")
        {
            result.Append("x");
        }
        else
        {
            result.Append(s+delimiter);
        }
    }

    public string Build()
    {
        return result.ToString();
    }
}



#endregion


public class LaPulzellaD_Orleans
{
    public static int MAX_CONCURRENT_MINES = 1,
        MAX_BARRACKSES_KNIGHTS = 1,
        MAX_BARRACKSES_ARCER = 0,
        MAX_BARRACKSES_GIANT = 0,
        MAX_TOWERS = 6;
        

    public static int GIANT_COST = 140, KNIGHT_COST = 80, ARCHER_COST = 100;
    public static int ENEMY_CHECK_RANGE = 260, TOO_MANY_UNITS_NEARBY = 2;
    public static int INFLUENCEMAP_SQUARELENGTH = 20;
    public static int QUEEN_MOVEMENT = 60;
    public static int MAX_DISTANCE = 4686400;
    public static int HEAL_TOWER = 800 / 3;
    public static int TOWERCOUNT_GIANT_TRIGGER = 4;

    //Try using amount numbers bigger than radius. And then we think about changing this function to something more like..... 
    public static PropagationFunction linearDecay => (amount, distance, decay) => amount - (distance * decay); //If amount is less than distance, values will become negative towards the end! No good.    
    
    public static PropagationFunction linearPropagation => (amount, distance, maxDistance) => amount - amount * (distance / maxDistance);
    public static PropagationFunction polynomial2Propagation => (amount, distance, maxDistance) => (1 - Math.Pow(distance / maxDistance, 2)) * amount;
    public static PropagationFunction polynomial4Propagation => (amount, distance, maxDistance) => (1 - Math.Pow(distance / maxDistance, 4)) * amount;
//    public static PropagationFunction polynomial2Propagation => (amount, distance, maxDistance) => amount - amount * Math.Pow(distance*distance/ maxDistance, 2) ;
//    public static PropagationFunction polynomial4Propagation => (amount, distance, maxDistance) => amount - amount * Math.Pow(distance) / maxDistance;

    
    public static Position CENTER = new Position(960, 500);
    
    public GameInfo gameInfo;

    public GameState game;
    public GameState prevGame = null;

    public int turn;

    private int buildOrder_stateMachine_stateIndex = -1;

    private InfluenceMap _survivorModeMap = new InfluenceMap(1920, 1000, -120, 120, new EuclideanDistance(), INFLUENCEMAP_SQUARELENGTH);

    private InfluenceMap cacheMap_SitesAndObstacles;
    private List<long> totals = new List<long>();

    public InfluenceMap SurvivorModeMap
    {
        get { return _survivorModeMap; }
        set { _survivorModeMap = value; }
    }

    public LaPulzellaD_Orleans()
    {
    }
    
    public string Encode()
    {
        StringEncoderBuilder sb = new StringEncoderBuilder(".");
        sb.Append(MAX_CONCURRENT_MINES);
        sb.Append(MAX_BARRACKSES_KNIGHTS);
        sb.Append(MAX_BARRACKSES_ARCER);
        sb.Append(MAX_BARRACKSES_GIANT);
        sb.Append(MAX_TOWERS);
        sb.Append(buildOrder_stateMachine_stateIndex);
        sb.Append(turn);
        return sb.Build();
    }

    public void Decode(string encoded)
    {
        encoded = encoded.Replace("x", "-1.");
        String[] values = encoded.Split('.');
        MAX_CONCURRENT_MINES = int.Parse(values[0]);
        MAX_BARRACKSES_KNIGHTS = int.Parse(values[1]);
        MAX_BARRACKSES_ARCER = int.Parse(values[2]);
        MAX_BARRACKSES_GIANT = int.Parse(values[3]);
        MAX_TOWERS  = int.Parse(values[4]);
        buildOrder_stateMachine_stateIndex = int.Parse(values[5]);
        turn = int.Parse(values[6]);
    }

    
    public TurnAction think(out Tuple<int,int> chosenTile)
    {
//        this.SurvivorModeMap.ResetMapToZeroes();
        TurnAction chosenMove = new TurnAction();
        GameState g = game;

        chosenTile = null;
        var bestMove = SurvivorMode(g, out chosenTile);
        bool isSafeToBuild = g.EnemyUnitsInRangeOfMyQueen(ENEMY_CHECK_RANGE) < 2;
        
        //If we are touching a site, we do something with it
        //Second condition makes him less likely to build when enemies are close
//        if (g.EnemyUnitsInRangeOfMyQueen(ENEMY_CHECK_RANGE) > 2)
//        {
//            //TODO: look for move in close squares. not far away.
//            chosenMove.queenAction = bestMove;
//            Console.Error.WriteLine("Running Away");
//        }
        var myQueenPosition = game.MyQueen.pos;
        var squareLength = LaPulzellaD_Orleans.INFLUENCEMAP_SQUARELENGTH;
        int queenX= (int) Math.Ceiling(myQueenPosition.x / squareLength*1.0);
        int queenY = (int) Math.Ceiling(myQueenPosition.y / squareLength*1.0);
        
        
        if (g.TouchingNeutralSite)
        {
            //Build
            var chosenBuildMove = ChoseBuildMove(g);
            chosenMove.queenAction = chosenBuildMove;
            Console.Error.WriteLine("Build Neutral Site");
        }
        else if (SurvivorModeMap[queenX, queenY] >= 0)
        {
            //Do nothing. Will trigger if(chosenMove.queenAction is Wait) as next instruction, picking the best move since it's too dangerous to stay put 
        }
        else if (g.TouchingMyTower && g.Owned_towers > MAX_TOWERS && IsSiteMinedOut(g.touchedSiteId) == false && isSafeToBuild)
        {
            //Switch tower with mine
            chosenMove.queenAction = new BuildMine(g.touchedSiteId);
        }
        else if (g.TouchingMyMine && IsMineMaxed(g.TouchedSite) == false && isSafeToBuild)
        {
            //Empower Mine
            Console.Error.WriteLine("Empower Mine!");
            chosenMove.queenAction = new BuildMine(game.touchedSiteId);
        }
        else if (g.TouchingMyTower && g.TouchedSite.param1 <= 300 + 145 * g.Owned_towers && g.TouchedSite.param1 <= 700 && isSafeToBuild)
        {
            //Empower Tower
            Console.Error.WriteLine("Empower Tower!");
            chosenMove.queenAction = new BuildTower(game.touchedSiteId);
        }
        
        if(chosenMove.queenAction is Wait)
        {
            chosenMove.queenAction = bestMove;
//            chosenMove.queenAction = Wood1Strategy(g, out buildInfluenceMap);
            Console.Error.WriteLine($"SurivorMode move is {chosenMove.queenAction}");
        }
        
        //Run to angle if close to enemies. Running takes priority, so we do the computations last
//        var enemyUnitsInMyQueenRange =
//            currGameState.units.Count(u => u.owner == Owner.Enemy && myQueen.DistanceTo(u) <= ENEMY_CHECK_RANGE);
//
//        //Run
//        if (enemyUnitsInMyQueenRange >= TOO_MANY_UNITS_NEARBY)
//        {
////            Move moveToAngle = RunToAngle(myQueen);
////            chosenMove.queenAction = RunToClosestTowerOrAngle(myQueen);
//        }

        Unit enemyQueen = game.EnemyQueen;

        chosenMove.trainAction = ChoseTrainAction(g);

        return chosenMove;
    }

    private IAction ChoseTrainAction(GameState g)
    {
        IEnumerable<Site> myIdleBarracses = g.MySites
            .Where(site => site.structureType == StructureType.Barracks && site.param1 == 0)
            .OrderBy(s2 => s2.pos.DistanceSqr(g.EnemyQueen.pos));
        
        List<Site> barracksesToTrainFrom = new List<Site>();
        
        if (myIdleBarracses.Any() )
        {
            //Train
            
            int remainingGold = game.money;

                
            if (g.Owned_giant_barrackses >= 1 && (g.Owned_giants < 1 && g.EnemySites.Count(s => s.structureType==StructureType.Tower) >= TOWERCOUNT_GIANT_TRIGGER))
            {
                if (remainingGold > GIANT_COST)
                {
                    var giant_Rax = myIdleBarracses.First(b => b.CreepsType == UnitType.Giant);
                    barracksesToTrainFrom.Add(giant_Rax);
                    remainingGold -= GIANT_COST;
                    myIdleBarracses = myIdleBarracses.Where(b => b.CreepsType != UnitType.Giant);
                }
                else
                {
                    //Wait for money to build giants
                    return new HaltTraing();
                }
            }
            
            foreach (var barraks in myIdleBarracses)
            {
                int cost = 0;
                switch (barraks.CreepsType)
                {
                    
                    case UnitType.Knight:
                        cost += KNIGHT_COST;
                        break;
                    case UnitType.Giant:
                        cost += GIANT_COST;
                        break;
                    case UnitType.Archer:
                        cost += ARCHER_COST;
                        break;
                }
                
                if (remainingGold >= cost)
                {
                    barracksesToTrainFrom.Add(barraks);
                    remainingGold -= cost;
                }
            }
            return new Train(barracksesToTrainFrom); 
        }
        else
        {
            //Wait
            return  new HaltTraing();
        }
    }

    private double NormalizeDistance(double distance)
    {
        double result = 0;
        result = distance / MAX_DISTANCE;
        return result;
    }
    
    private IAction SurvivorMode(GameState g, out Tuple<int, int> chosenTile)
    {
        double squareLength = INFLUENCEMAP_SQUARELENGTH; //Maximum common divisor between 60, 100, 75, 50 (movement speeds)
        
        var influenceMap = SurvivorModeMap;
        influenceMap.ResetMapToZeroes();

        int searchRange = QUEEN_MOVEMENT * 2;
        double favorCloseSitesOverOpenSquares = 5;
        
        Stopwatch total = new Stopwatch();
        
        
        //var move = UnscaledBestInBox(g.MyQueen, searchRange, influenceMap);
        Stopwatch sw = new Stopwatch();
        //Avoid enemy towers!!
        total.Start();
        sw.Start();

        foreach (var tower in g.EnemyTowers)
        {
            int siteRadius = GetRadius(tower);
            int towerRange = (int) Math.Ceiling(tower.param2 / squareLength);
                
//            ScaleAndApplyInfluence_Range(tower.pos, -25, towerRange - siteRadius, 0, linearPropagation, influenceMap);
            influenceMap.ApplyInfluence_Range_Unscaled(tower.pos.x, tower.pos.y, -25, towerRange - siteRadius, 0, linearDecay, true);
//            ScaleAndApplyInfluence_Circle(tower.pos, -25, towerRange, 0, linearPropagation, influenceMap);
//            ScaleAndApplyInfluence_Circle(tower.pos, -25, 3, 0, linearPropagation, influenceMap);

        }
        sw.Stop();
        Console.Error.WriteLine("Enemy Towers={0}",sw.ElapsedMilliseconds);
        sw.Reset();
        
        sw.Start();

        int enemyThreat = g.EnemyUnits.Count(); /** g.AlliedTowersInRangeOf(tower.pos, tower.param2)*/
        int maxTowerRange = 800;
        
        foreach (var tower in g.MyTowers)
        {
            int siteRadius = GetRadius(tower);
            int towerRange = (int) Math.Ceiling(tower.param2 / squareLength);
        
            //Heal my towers!
            double towerHp = tower.param1;
//            if (towerHp < HEAL_TOWER)
//            {
                var towerHp_norm = 1 - (towerHp / 800);     //[0,1]
                // Towers covering a tower make it better. If there are more enemies, then it should be even better

                influenceMap.ApplyInfluence_Range_Unscaled(tower.pos.x, tower.pos.y, enemyThreat/2, 2, towerRange - siteRadius + 2, linearDecay);

                if (towerHp_norm <= 0.3)
                {
                    influenceMap.ApplyInfluence_Range_Unscaled(tower.pos.x, tower.pos.y, towerHp_norm + favorCloseSitesOverOpenSquares, 1, 0, linearDecay);
                    influenceMap.ApplyInfluence_Range_Unscaled(tower.pos.x, tower.pos.y, towerHp_norm, 0, 5 - siteRadius + (int)(towerHp_norm * 10), linearDecay);
                }
//                else if (towerHp_norm <= 0.6)
//                {
//                    influenceMap.ApplyInfluence_Range_Unscaled(tower.pos.x, tower.pos.y, towerHp_norm * favorCloseSitesOverOpenSquares, 1, 0, linearPropagation);
//                }
            
            
                
//                ScaleAndApplyInfluence_Circle(tower.pos, towerHp_norm, siteRadius+1, 10, polynomial2Propagation,  influenceMap);
//                ScaleAndApplyInfluence_Circle(tower.pos, towerHp_norm * favorCloseSitesOverOpenSquares, siteRadius+1, 0, linearPropagation, ref buildInfluenceMap);

//            }
//            ScaleAndApplyInfluence_Circle(tower.pos, -100, siteRadius, towerRange - siteRadius + 1, polynomial2Propagation, ref buildInfluenceMap);
            
            //Towers should make all nearby sites more influencial, because now I have control over it.
            //ScaleAndApplyInfluence_Circle(tower.pos, 10, 0, towerRange, linearPropagation, ref buildInfluenceMap);
        }
        sw.Stop();
        Console.Error.WriteLine("My Towers={0}",sw.ElapsedMilliseconds);
        sw.Reset();
        
       
        sw.Start();

        //Enemy units influence
        foreach (var enemy in g.EnemyUnits)
        {
            double enemyInfluence = GetEnemyInfluence(enemy);
//            ScaleAndApplyInfluence_Range(enemy.pos, enemyInfluence, 1, GetEnemyInfluenceRadius(enemy) *4, linearPropagation, influenceMap);
            influenceMap.ApplyInfluence_Range_Unscaled(enemy.pos.x, enemy.pos.y, -20, 2, 18, linearDecay);

        }
        sw.Stop();
        Console.Error.WriteLine("Enemy Units ={0}",sw.ElapsedMilliseconds);
        sw.Reset();
        
//        List<Site> influencingSites = g.sites.Where(s => s.owner==Owner.Neutral).ToList();

//        if (g.Owned_mines < 3 && g.Owned_towers >= 5 + g.Owned_mines)
//        {
//            influencingSites.    
//        }
//        
        sw.Start();
        foreach (var site in g.sites)
        {
//            int siteRadius = (int) Math.Floor(GetSiteInfo(site).radius / squareLength);
            

            
            if (site.structureType != StructureType.Mine && site.owner == Owner.Friendly)
            {
//                ScaleAndApplyInfluence_Circle(site.pos, -100, siteRadius, 0, polynomial2Propagation, influenceMap);
            }
            if (site.structureType == StructureType.Mine && site.owner == Owner.Friendly)
            {
//                ScaleAndApplyInfluence_Circle(site.pos, -120, siteRadius+1, 0, polynomial2Propagation, influenceMap);
                influenceMap.ApplyInfluence_Range_Unscaled(site.pos.x, site.pos.y, enemyThreat * 2, 0, 3, linearDecay);
            }

//            if (site.structureType == StructureType.Barracks && site.owner == Owner.Enemy && site.param1 != 0)
//            {
//                ScaleAndApplyInfluence_Circle(site.pos, -25 * (6 - site.param1) , siteRadius, 8, linearPropagation, ref buildInfluenceMap);
//            }
            else if(site.owner == Owner.Neutral)
            {
                //TODO: I only need to do this once, then copy it every time. It will cost much less than computing it again every time. Unless the influence values change with the time
                double influence = 0;
            
                double distanceToCenter = CENTER.DistanceSqr(site.pos); 

                double distanceToCenter_norm = 1 - NormalizeDistance(distanceToCenter);

                influence = (1.75 + 1 * distanceToCenter_norm) * 2;

                int decayRange = 12;
                influence = decayRange + influence;
                
            
                //TODO: maybe do siteRadius and siteRadius-1
                
//                influenceMap.ApplyInfluence_Range_Unscaled(site.pos.x, site.pos.y, influence/2 * favorCloseSitesOverOpenSquares, siteRadius+1, 7, polynomial2Propagation);
                influenceMap.ApplyInfluence_Range_Unscaled(site.pos.x, site.pos.y, influence * 2, 1, 0, linearDecay);
                influenceMap.ApplyInfluence_Range_Unscaled(site.pos.x, site.pos.y, influence, 1, decayRange, linearDecay);

            }
        }
        sw.Stop();
        Console.Error.WriteLine("Sites ={0}",sw.ElapsedMilliseconds);
        sw.Reset();
        
        total.Stop();
        totals.Add(total.ElapsedMilliseconds);
        Console.Error.WriteLine("Total ={0}, Average = {1}",total.ElapsedMilliseconds, totals.Average());
        
        

        
    //Favor center
//        if (turn < 70)
//        {
//            ScaleAndApplyInfluence_Circle(new Position(960,200), .7, 6, 30, linearPropagation, ref buildInfluenceMap);
//            ScaleAndApplyInfluence_Circle(new Position(960,300), .7, 6, 30, linearPropagation, ref buildInfluenceMap);
//            ScaleAndApplyInfluence_Circle(new Position(960,400), .7, 6, 30, linearPropagation, ref buildInfluenceMap);
//            
//        }

        
        if (turn >= 100)
        {
            searchRange *= 3;
        }
        
        sw.Start();
        var survivorModeChosenSite = UnscaledBestInBox(g.MyQueen, searchRange, influenceMap);
        sw.Stop();
        Console.Error.WriteLine("Select best in bos ={0}",sw.Elapsed);
        sw.Reset();

        chosenTile = Tuple.Create(survivorModeChosenSite.Item1/INFLUENCEMAP_SQUARELENGTH, survivorModeChosenSite.Item2/INFLUENCEMAP_SQUARELENGTH);
#if UNITY_EDITOR
        UnityEngine.Debug.Log("Survivor Mode tile is is ("+chosenTile.Item1+", "+chosenTile.Item2+") with amount = "+influenceMap[chosenTile.Item1, chosenTile.Item2]);
#endif

        return new Move(survivorModeChosenSite.Item1, survivorModeChosenSite.Item2);

    }

  

    private int GetRadius(Site site)
    {
        int siteRadius = (int) Math.Floor(GetSiteInfo(site).radius / (double)INFLUENCEMAP_SQUARELENGTH);
        return siteRadius;
    }   
    private void ScaleAndApplyInfluence_Diamond(Site site, double amount, int fullDistance, int decayedDistance, double distanceDecay, InfluenceMap map)
    {
        ScaleAndApplyInfluence_Diamond(site.pos, amount, fullDistance, decayedDistance, distanceDecay, map);
    }
    
    private void ScaleAndApplyInfluence_Diamond(Unit unit, double amount, int fullDistance, int decayedDistance, double distanceDecay, InfluenceMap map)
    {
        ScaleAndApplyInfluence_Diamond(unit.pos, amount, fullDistance, decayedDistance, distanceDecay, map);
    }
    
    private void ScaleAndApplyInfluence_Diamond(Position pos, double amount, int fullDistance, int decayedDistance, double distanceDecay, InfluenceMap map)
    {
        ScaleAndApplyInfluence_Diamond(pos.x, pos.y, amount, fullDistance, decayedDistance, distanceDecay, map);
    }
    
//    private void ScaleAndApplyInfluence_Range(Position pos, double amount, int fullDistance, int decayedDistance, PropagationFunction propagationFunc, InfluenceMap map)
//    {
//        ScaleAndApplyInfluence_Range(pos.x, pos.y, amount, fullDistance,decayedDistance,propagationFunc, map);     
//    }

    private void ScaleAndApplyInfluence_Circle(Position pos, double amount, int fullDistance, int decayedDistance, PropagationFunction propagationFunc, InfluenceMap map)
    {
        ScaleAndApplyInfluence_Circle(pos.x, pos.y, amount, fullDistance,decayedDistance,propagationFunc, map);        
    }

    private void ScaleAndApplyInfluence_Circle(int x, int y, double amount, int fullDistance, int decayedDistance, PropagationFunction propagationFunc, InfluenceMap map)
    {
        int scaledPosX = (int) Math.Ceiling(x*1.0 / INFLUENCEMAP_SQUARELENGTH);
        int scaledPosY = (int) Math.Ceiling(y*1.0 / INFLUENCEMAP_SQUARELENGTH);
        
        map.ApplyInfluence_Circle(scaledPosX ,scaledPosY ,amount,fullDistance,decayedDistance, propagationFunc);
        
    }
    
//    private void ScaleAndApplyInfluence_Range(int x, int y, double amount, int fullDistance, int decayedDistance, PropagationFunction propagationFunc, InfluenceMap map, int initialRadiusSkip = 0)
//    {
//        int scaledPosX = (int) Math.Ceiling(x*1.0 / INFLUENCEMAP_SQUARELENGTH);
//        int scaledPosY = (int) Math.Ceiling(y*1.0 / INFLUENCEMAP_SQUARELENGTH);
//        
//        map.ApplyInfluence_Range(scaledPosX ,scaledPosY ,amount,fullDistance,decayedDistance, propagationFunc, initialRadiusSkip );
//        
//    }
    
    private void ScaleAndApplyInfluence_Diamond(int x, int y, double amount, int fullDistance, int decayedDistance, double distanceDecay, InfluenceMap map)
    {
        int scaledPosX = (int) Math.Ceiling(x*1.0 / INFLUENCEMAP_SQUARELENGTH);
        int scaledPosY = (int) Math.Ceiling(y*1.0 / INFLUENCEMAP_SQUARELENGTH);
        
        map.ApplyInfluence_Diamond(scaledPosX ,scaledPosY ,amount,fullDistance,decayedDistance,distanceDecay);
        
    }
    
    //range is length of a square's vertex to position in center
    private Tuple<int, int> UnscaledBestInBox(Site site, int range, InfluenceMap map)
    {
        return UnscaledBestInBox(site.pos, range, map);
    }
    
    //range is length of a square's vertex to position in center
    private Tuple<int, int> UnscaledBestInBox(Unit unit, int range, InfluenceMap map)
    {
        return UnscaledBestInBox(unit.pos, range, map);
    }

    //range is length of a square's vertex to position in center
    private Tuple<int, int> UnscaledBestInBox(Position pos, int range, InfluenceMap map)
    {
        int x1 = Math.Max(1 , pos.x - range);
        int x2 = Math.Min(1919, pos.x + range);
        
        int y1 = Math.Max(1 , pos.y - range);
        int y2 = Math.Min(998, pos.y + range);
        
        
        return UnscaledBestInBox(x1, y1, x2, y2, map);
    }
    
    private Tuple<int, int> UnscaledBestInBox(int x1, int y1, int x2, int y2, InfluenceMap map, bool returnResultUnscaled = true)
    {
        x1 = x1 / INFLUENCEMAP_SQUARELENGTH;
        y1 = y1 / INFLUENCEMAP_SQUARELENGTH;
        x2 = x2 / INFLUENCEMAP_SQUARELENGTH;
        y2 = y2 / INFLUENCEMAP_SQUARELENGTH;
        
        var bestCell = map.selectBestInBox(x1, y1, x2, y2);
        if (returnResultUnscaled)
        {
            bestCell = Tuple.Create(bestCell.Item1 * INFLUENCEMAP_SQUARELENGTH, bestCell.Item2 * INFLUENCEMAP_SQUARELENGTH);
        }
        else
        {
            bestCell = Tuple.Create(bestCell.Item1, bestCell.Item2);
        }

        return bestCell;
    }
    
    private double GetEnemyInfluence(Unit enemy)
    {
        switch (enemy.unitType)
        {
            case UnitType.Queen:
                return 0;
            case UnitType.Knight:
                return -5;
            case UnitType.Archer:
                return 0;
            case UnitType.Giant:
                return 0;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private int GetEnemyInfluenceRadius(Unit enemy)
    {
        double squareLength = INFLUENCEMAP_SQUARELENGTH; //Maximum common divisor between 60, 100, 75, 50 (movement speeds)
        //return 2;
        switch (enemy.unitType)
        {
            case UnitType.Queen:
                return (int)(60 / INFLUENCEMAP_SQUARELENGTH);
            case UnitType.Knight:
                return (int) (100 / INFLUENCEMAP_SQUARELENGTH);
            case UnitType.Archer:
                return (int) (236 / INFLUENCEMAP_SQUARELENGTH); //200 is range, 36 is 1/2 of move range 
            case UnitType.Giant:
                return (int) (50 / INFLUENCEMAP_SQUARELENGTH);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IAction DecideUnitsToTrain()
    {
        //Copy game state
        GameState gameState = new GameState();
        gameState.Decode(this.game.Encode());

        int availableMoney = gameState.money;
        
        Func<double, double, double> archersEvaluation = (archers, enemyUnits) => enemyUnits - (archers * 3.0) ;
        Func<double, double, double> giantsEvaluation = (giants, enemyTowers) =>  enemyTowers - (giants * 10.0) ;
        Func<double, double, double, double> knightsEvaluation = (knights, enemyArchers, enemyTowers) => (enemyArchers - (knights / 4))/2 + (enemyTowers - (knights/4))/2;

        List<Site> sitesThatCanTrainGiants = gameState.sites.Where(s =>
            s.owner == Owner.Friendly && s.structureType == StructureType.Barracks && s.CreepsType == UnitType.Giant)
            .ToList();
        
        List<Site> sitesThatCanTrainKnights = gameState.sites.Where(s =>
                s.owner == Owner.Friendly && s.structureType == StructureType.Barracks && s.CreepsType == UnitType.Knight)
            .ToList();

        List<Site> sitesThatCanTrainArchers = gameState.sites.Where(s =>
                s.owner == Owner.Friendly && s.structureType == StructureType.Barracks && s.CreepsType == UnitType.Archer)
            .ToList();
        
        double deltaMoreGiants = 0, deltaMoreKnights = 0, deltaMoreArchers = 0;
        
        int myGiantsCount =
            this.game.units.Count(u => u.owner == Owner.Friendly && u.unitType == UnitType.Giant);
        
        int myKnightsCount = 
            this.game.units.Count(u => u.owner == Owner.Friendly && u.unitType == UnitType.Knight);
        
        int myArchersCount =
            this.game.units.Count(u => u.owner == Owner.Friendly && u.unitType == UnitType.Archer);
        
        int enemyTowersCount =
            this.game.sites.Count(s => s.owner == Owner.Enemy && s.structureType == StructureType.Tower);
        
        int enemyArchersCount =
            this.game.units.Count(u => u.owner == Owner.Enemy && u.unitType == UnitType.Archer);

        int enemyUnitsCount =
            this.game.units.Count(u => u.owner == Owner.Enemy);

        
        if( availableMoney >= GIANT_COST && sitesThatCanTrainGiants.Count > 0)
        {
            
            double currentGiantScore = giantsEvaluation(myGiantsCount, enemyTowersCount);
            double scoreWithAdditionalGiant = giantsEvaluation(myGiantsCount+1, enemyTowersCount);
            deltaMoreGiants = scoreWithAdditionalGiant - currentGiantScore;

        }
        
        if( availableMoney >= KNIGHT_COST && sitesThatCanTrainKnights.Count > 0)
        {
            double currentKnightScore = knightsEvaluation(myKnightsCount, enemyArchersCount, enemyTowersCount);
            double scoreWithAdditionalKnight = knightsEvaluation(myKnightsCount+4, enemyArchersCount, enemyTowersCount);

            deltaMoreKnights = scoreWithAdditionalKnight - currentKnightScore;
        }
        
        if( availableMoney >= ARCHER_COST && sitesThatCanTrainArchers.Count > 0)
        {
            double currrentArcherScore = archersEvaluation(myArchersCount, enemyUnitsCount);
            double scoreWithAdditionalArcher = archersEvaluation(myArchersCount+2, enemyUnitsCount);
            
            deltaMoreArchers = scoreWithAdditionalArcher - currrentArcherScore;
        }

//        IOrderedEnumerable<(double, string)> orderedTrainActions = new List<(double, string)>
//            {
//                (scoreWithAdditionalKnight, "KNIGHT"),
//                (scoreWithAdditionalArcher, "ARCHER"),
//                (scoreWithAdditionalGiant, "GIANT")
//            }
//            .OrderBy(el => el.Item1);

        var orderedTrainActions = new double[]
        {
            (deltaMoreKnights),
            (deltaMoreArchers),
            (deltaMoreGiants)
        };
        
        orderedTrainActions.ToList().ForEach(Console.Error.WriteLine);

        return null;
    }

    /**
     * Returns WAIT if Alexander can't mine anymore and has enough buildings.
     */
    private IAction ChoseBuildMove(GameState g)
    {
        IAction chosenBuildMove = null;
        
        //touchedSite == closestUnbuiltMine
        bool siteHasGold = g.TouchedSite.gold > 0;

        bool isSafeToBuild = g.EnemyUnitsInRangeOfMyQueen(ENEMY_CHECK_RANGE) < 2;

        if (g.Owned_mines < MAX_CONCURRENT_MINES /*&& currGameState.touchedSiteId == closestUnbuiltMines.First().siteId*/
                                               && siteHasGold && isSafeToBuild )
        {
            //chosenMove.queenAction = new BuildMine(currGameState.touchedSiteId);
            chosenBuildMove = new BuildMine(g.touchedSiteId);
        }
        else if (g.Owned_archer_barrackses < MAX_BARRACKSES_ARCER)
        {
            //chosenMove.queenAction = new BuildBarracks(currGameState.touchedSiteId, BarracksType.Archer);
            chosenBuildMove = new BuildBarracks(game.touchedSiteId, BarracksType.Archer);
        }
        else if (g.Owned_knight_barrackses < MAX_BARRACKSES_KNIGHTS)
        {
            //chosenMove.queenAction = new BuildBarracks(currGameState.touchedSiteId, BarracksType.Knight);
            chosenBuildMove = new BuildBarracks(game.touchedSiteId, BarracksType.Knight);
        }
        else if (g.Owned_towers < MAX_TOWERS)
        {
            chosenBuildMove = new BuildTower(game.touchedSiteId);
        }
        else if (g.Owned_giant_barrackses < MAX_BARRACKSES_GIANT)
        {
            //chosenMove.queenAction = new BuildBarracks(currGameState.touchedSiteId, BarracksType.Archer);
            chosenBuildMove = new BuildBarracks(game.touchedSiteId, BarracksType.Giant);
        }
        else if (siteHasGold == false)
        {
            chosenBuildMove = new BuildTower(game.touchedSiteId);
        }
        else
        {
            //If enemies are close, no point in building a mine. Let's build a tower instead.
            //By default build towers. #SurvivorMode
            if (g.EnemyUnitsInRangeOf(g.TouchedSite.pos, 400) >= 2)
            {
                chosenBuildMove = new BuildTower(g.touchedSiteId);
            }
            else
            {
                if (g.Owned_mines >= 3 && g.Owned_giant_barrackses < 1)
                {
                    chosenBuildMove = new BuildBarracks(game.touchedSiteId, BarracksType.Giant);
                }
                else
                {
                    chosenBuildMove = new BuildMine(game.touchedSiteId);
                }
            }
            
            
//            chosenBuildMove = new Wait();

        }

        return chosenBuildMove;
    }

    private IAction RunToAngle(Unit myQueen)
    {
        Position[] angles = {new Position(0, 0), new Position(1920, 1000)};
        Position targetAngle;

        targetAngle = angles.OrderBy(a => myQueen.pos.DistanceSqr(a)).First();
        return new Move(targetAngle);
    }
    
    private IAction RunToClosestTowerOrAngle(Unit myQueen)
    {
        Site closestTower =
            game.sites
                .Where(s => s.structureType == StructureType.Tower && s.owner == Owner.Friendly)
                .OrderByDescending(s1 => myQueen.DistanceTo(s1))
                .FirstOrDefault();

        if (closestTower != null)
        {
            return new Move(closestTower.pos);    
        }
        else
        {
            return RunToAngle(myQueen);
        }
    }

    private bool IsSiteMinedOut(int siteId)
    {
        return gameInfo.minedOutSites_ids.Contains(siteId);
    }

    public List<Site> SortSites_ByDistance_WithBiasTowardsCenter(Position startPosition, List<Site> siteList)
    {
        IOrderedEnumerable<Site> sortedSitesStream = 
            siteList
                .OrderBy(s => startPosition.DistanceSqr(GetSiteInfo(s.siteId).pos) + s.pos.DistanceSqr(new Position(960, 500)));

        return sortedSitesStream.ToList();
    }
    
    public List<Site> SortSites_ByDistance_WithBiasTowardsEdges(Position startPosition, List<Site> siteList)
    {
        IOrderedEnumerable<Site> sortedSitesStream = 
            siteList
                .OrderBy(s => startPosition.DistanceSqr(GetSiteInfo(s.siteId).pos) - s.pos.DistanceSqr(new Position(960, 500)));

//        sortedSitesStream = sortedSitesStream
//            .Where(s => s.pos.Distance())
        
        return sortedSitesStream.ToList();
    }
    

    private bool IsMineMaxed(Site site)
    {
        return site.param1 == site.maxMineSize;
    }

    public SiteInfo GetSiteInfo(Site site)
    {
        return GetSiteInfo(site.siteId);
    }
    
    public SiteInfo GetSiteInfo(int siteId)
    {
        return gameInfo.sites[siteId];
    }
    
    public void ParseInputs_Turn()
    {
        prevGame = game;
        game = new GameState();
        
        var inputs = Console.ReadLine().Split(' ');
        game.money = int.Parse(inputs[0]);
        game.touchedSiteId = int.Parse(inputs[1]); // -1 if none
        for (int i = 0; i < gameInfo.numSites; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int siteId = int.Parse(inputs[0]);
            int gold = int.Parse(inputs[1]); // used in future leagues
            int maxMineSize = int.Parse(inputs[2]); // used in future leagues
            int structureType = int.Parse(inputs[3]); // -1 = No structure, 2 = Barracks
            int owner = int.Parse(inputs[4]); // -1 = No structure, 0 = Friendly, 1 = Enemy
            int param1 = int.Parse(inputs[5]);
            int param2 = int.Parse(inputs[6]);

           
            Site site = new Site(siteId, gold, maxMineSize, structureType, owner, param1, param2);
            site.isMinedOut = IsSiteMinedOut(site.siteId);
            game.sites.Add(site);

            site.pos = gameInfo.sites[site.siteId].pos;
            
            if (gold == 0 /*&& prevGameState != null && prevGameState.sites[siteId].gold > 0*/)
            {
                gameInfo.minedOutSites_ids.Add(site.siteId);
//                game.minedOutSites_ids.ToList().ForEach(Console.Error.WriteLine);
            }
        }
        
        int numUnits = int.Parse(Console.ReadLine());
        for (int i = 0; i < numUnits; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int x = int.Parse(inputs[0]);
            int y = int.Parse(inputs[1]);
            int owner = int.Parse(inputs[2]);
            int unitType = int.Parse(inputs[3]); // -1 = QUEEN, 0 = KNIGHT, 1 = ARCHER
            int health = int.Parse(inputs[4]);
            
            Unit unit = new Unit(x,y,owner,unitType,health);
            game.units.Add(unit);
        }
        
        game.PreLoadProperties();
        
    }
    
    public void ParseInputs_Begin()
    {
        gameInfo = new GameInfo();
        string[] inputs;
        int numSites = int.Parse(Console.ReadLine());
        
        for (int i = 0; i < numSites; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int siteId = int.Parse(inputs[0]);
            int x = int.Parse(inputs[1]);
            int y = int.Parse(inputs[2]);
            int radius = int.Parse(inputs[3]);
            
            SiteInfo newSiteInfo = new SiteInfo(siteId, new Position(x,y), radius);
            gameInfo.sites[siteId] = newSiteInfo;
        }
        Console.Error.WriteLine();
    }
    
    /**
     * Only call asfter gameInfo was set
     */
    public void InitializeInfluenceMaps()
    {
        int xIndex, yIndex;
    
        foreach (var site in gameInfo.sites.Values)
        {
            xIndex = (int) Math.Ceiling(site.pos.x / LaPulzellaD_Orleans.INFLUENCEMAP_SQUARELENGTH*1.0);
            yIndex = (int) Math.Ceiling(site.pos.y / LaPulzellaD_Orleans.INFLUENCEMAP_SQUARELENGTH*1.0);
            int range = (int) Math.Ceiling(site.radius / LaPulzellaD_Orleans.INFLUENCEMAP_SQUARELENGTH * 1.0);

            SurvivorModeMap.AddObstacle(xIndex, yIndex, range);        
        }

        SurvivorModeMap.Init_AfterSettingObstacles();
    }
}

public interface DistanceFunc
{
	double computeDistance(int x1, int y1, int x2, int y2);
}

public class EuclideanDistanceSqr : DistanceFunc {

	public double computeDistance(int x1, int y1, int x2, int y2)
	{
		return Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2);
	}
}

public class EuclideanDistance : DistanceFunc {

    public double computeDistance(int x1, int y1, int x2, int y2)
    {
        return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
    }
}

public class ManhattanDistance : DistanceFunc {

    public double computeDistance(int x1, int y1, int x2, int y2)
    {
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
    }
}

public struct XAndY
{
	public int x, y;

	public XAndY(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}

public class InfluenceMap
{
    public class CellAndDistance
    {
        public readonly InfluenceMapCell cell;
        public readonly double distance;

        public CellAndDistance(InfluenceMapCell cell, double distance)
        {
            this.cell = cell;
            this.distance = distance;
        }
    }
    
    public class InfluenceMapCell
    {
        public int x, y;
        
        public CellAndDistance[] neighboursAndDistance = null;
        public CellAndDistance[] neighbourObstaclesAndDistance = null;

        public HashSet<InfluenceMapCell> GetNeighbours => new HashSet<InfluenceMapCell>(neighboursAndDistance.Select(nAd => nAd.cell));
    }
    
    public InfluenceMapCell[,] influenceMapCells;
	protected double[,] _influenceMap;
    protected bool[,] isObstacle;

    public bool IsObstacle(int x, int y) => isObstacle[x, y];
    
    protected int gridWidth, gridHeight;

	public double minInfluence, maxInfluence;
    private double maxDistance_EuclSqr, maxDistance_Eucl, maxDistance_Manh;
    
    
    public int actualWidth, actualHeight;
    public int unit;
    

	public DistanceFunc computeDistanceFunc;


	public double this[int x, int y] {
		get {
			return get(x, y);
		}
		set {
			_influenceMap[x,y] = value;
		}
	}

    public InfluenceMap()
    {
        
    }
    
	public InfluenceMap(InfluenceMap mapToCopy)
	{
	    this._influenceMap = mapToCopy._influenceMap.Clone() as double[,];
	    this.isObstacle = mapToCopy.isObstacle.Clone() as bool[,];
	    this.influenceMapCells = mapToCopy.influenceMapCells;    //It's okay to copy as reference since the maps are the same, and the cells don't contain any information particular to the instance
	    this.gridWidth = mapToCopy.gridWidth;
	    this.gridHeight = mapToCopy.gridHeight;
	    this.minInfluence = mapToCopy.minInfluence;
	    this.maxInfluence = mapToCopy.maxInfluence;
	    this.computeDistanceFunc = mapToCopy.computeDistanceFunc;
	    this.unit = mapToCopy.unit;
	    this.actualWidth = mapToCopy.actualWidth;
	    this.actualHeight = mapToCopy.actualHeight;
	    myHashset = new HashSet<XAndY>();
	    
        this.maxDistance_EuclSqr = mapToCopy.maxDistance_EuclSqr;
        this.maxDistance_Eucl = mapToCopy.maxDistance_Eucl;
        this.maxDistance_Manh = mapToCopy.maxDistance_Manh;
	}
    
    public InfluenceMap(int actualWidth, int actualHeight, double minInfluence, double maxInfluence, DistanceFunc computeDistanceFunc, int unit)
    {
        this.unit = unit;
        this.actualWidth = actualWidth;
        this.actualHeight = actualHeight;
        this.gridWidth = Unitize(actualWidth) ;
        this.gridHeight = Unitize(actualHeight);
        this._influenceMap = new double[gridWidth,gridHeight];
        this.influenceMapCells = new InfluenceMapCell[gridWidth,gridHeight];
        this.isObstacle = new bool[gridWidth,gridHeight];
		
        this.computeDistanceFunc = computeDistanceFunc;
        this.minInfluence = minInfluence;
        this.maxInfluence = maxInfluence;
        myHashset = new HashSet<XAndY>();
	    
        this.maxDistance_EuclSqr = actualWidth * actualWidth + actualHeight * actualHeight;
        this.maxDistance_Eucl = Math.Sqrt(maxDistance_EuclSqr);
//        this.maxDistance_Manh = new ManhattanDistance().computeDistance(0, 0, gridWidth, gridHeight);
        this.maxDistance_Manh = 50;

    }
    
    public int Unitize(int coord)
    {
        int unitizedCoord = (int) Math.Round(1.0*coord / unit);
        return unitizedCoord;
    }
    
    public Position Unitize(Position pos)
    {
        return new Position(Unitize(pos.x), Unitize(pos.y));
    }

    /**
     * Call this to refresh the distances based on all the added obstacles so far
     */
    public void Init_AfterSettingObstacles()
    {
        PrecomputeDistances();
    }
    
    private void PrecomputeDistances()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                InfluenceMapCell currCell = new InfluenceMapCell();
                currCell.x = x;
                currCell.y = y;
                influenceMapCells[x, y] = currCell;
            }
        }

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                InfluenceMapCell currCell = influenceMapCells[x, y];

                List<CellAndDistance> neighboursAndDistance = new List<CellAndDistance>();
                List<CellAndDistance> neighbourObstaclesAndDistance = new List<CellAndDistance>();
                
                List<XAndY> neighbours = getNeighbours(x, y);
                foreach (var neighbour in neighbours)
                {
                    InfluenceMapCell neighbourCell = influenceMapCells[neighbour.x, neighbour.y];
                    var distanceAndCell = new CellAndDistance(neighbourCell, computeDistanceFunc.computeDistance(x,y,neighbour.x, neighbour.y));

                    if (isObstacle[neighbour.x, neighbour.y])
                    {
                        neighbourObstaclesAndDistance.Add(distanceAndCell);
                    }
                    else
                    {
                        neighboursAndDistance.Add(distanceAndCell);
                    }
                }

                currCell.neighboursAndDistance = neighboursAndDistance.OrderBy(nAd => nAd.distance).ToArray();
                currCell.neighbourObstaclesAndDistance = neighbourObstaclesAndDistance.OrderBy(nAd => nAd.distance).ToArray();
            }
        }
    }

    //TODO
    HashSet<InfluenceMapCell> visited = new HashSet<InfluenceMapCell>();
    List<CellAndDistance> frontier = new List<CellAndDistance>();
    
    public void ApplyInfluence_Range_Unscaled(int xPos, int yPos, double amount, int fullDistance, int decayDistance, PropagationFunction decayedDistanceFunc, bool ignoreObstacles = false)
    {
        InfluenceMapCell startCell = influenceMapCells[Unitize(xPos), Unitize(yPos)];

//        HashSet<InfluenceMapCell> visited = new HashSet<InfluenceMapCell>();
//        List<CellAndDistance> frontier = new List<CellAndDistance>();
        //TODO
        visited.Clear();
        frontier.Clear();

        double distanceToStartCell = new Position(xPos, yPos).DistanceTo(new Position(startCell.x * unit, startCell.y * unit));
        frontier.Add(new CellAndDistance(startCell,distanceToStartCell));
        visited.Add(startCell);

        int steps = 0, obstacleSteps = 0;
        
        Stopwatch sw = new Stopwatch();
//        sw.Start();
        while (frontier.Any(f => isObstacle[f.cell.x, f.cell.y]))
        {
            obstacleSteps++;
            var currFrontierCellInfo = frontier.First(f => isObstacle[f.cell.x, f.cell.y]);
            frontier.Remove(currFrontierCellInfo);
            
            InfluenceMapCell currCell = currFrontierCellInfo.cell;
            double distance = currFrontierCellInfo.distance;
            
            if (currCell.neighboursAndDistance.Length != 0)
            {
                for (var index = 0; index < currCell.neighboursAndDistance.Length; index++)
                {
                    var neighbourAndDistance = currCell.neighboursAndDistance[index];
                    if (visited.Contains(neighbourAndDistance.cell) == false)
                    {
                        var neighbour = neighbourAndDistance.cell;
                        var distanceToCell = neighbourAndDistance.distance;


                        var newFrontierCandidate = new CellAndDistance(neighbour, distance + distanceToCell);
                        visited.Add(neighbour);
                        frontier.Add(newFrontierCandidate);
                    }
                }
            }
            else
            {
                for (var index = 0; index < currCell.neighbourObstaclesAndDistance.Length; index++)
                {
                    var neighbourAndDistance = currCell.neighbourObstaclesAndDistance[index];
                    if (visited.Contains(neighbourAndDistance.cell) == false)
                    {
                        var neighbour = neighbourAndDistance.cell;
                        var distanceToCell = neighbourAndDistance.distance;


                        var newFrontierCandidate = new CellAndDistance(neighbour, distance + distanceToCell);
                        visited.Add(neighbour);
                        frontier.Add(newFrontierCandidate);
                    }
                }
            }
        }

//        sw.Stop();
//        Console.Error.WriteLine("Find nonObstacles={0}",sw.Elapsed);
//        sw.Reset();
        
        //TODO: ricalcola ristanze delle celle nella frontiera
        frontier = frontier.Select(f => new CellAndDistance(f.cell, 0.0)).ToList();

//        foreach (var f in frontier)
//        {
//            _influenceMap[f.Item1.x, f.Item1.y] += 10;
//        }

        
        sw.Start();
        while (frontier.Count > 0)
        {
            steps++;
            
            var currFrontierCellInfo = GetBestFrontierCell(frontier, true);
            
            InfluenceMapCell currCell = currFrontierCellInfo.cell;
            double distance = currFrontierCellInfo.distance;

            double cellAmount = amount;
            
            if (distance > fullDistance)
            {
                double linearDecay = 0.7;
                cellAmount = decayedDistanceFunc(amount, distance-fullDistance, linearDecay);
            }

            if (isObstacle[currCell.x, currCell.y] == false)
            {
                _influenceMap[currCell.x, currCell.y] += cellAmount;
            }
            
            foreach (var neighbourAndDistance in currCell.neighboursAndDistance)
            {
                if (visited.Contains(neighbourAndDistance.cell) == false)
                {
                    var neighbour = neighbourAndDistance.cell;
                    var distanceToCell = neighbourAndDistance.distance;

                    if (distanceToCell + distance <= fullDistance + decayDistance)
                    {
                        var newFrontierCandidate = new CellAndDistance(neighbour, distance + distanceToCell);
                        visited.Add(neighbour);
                        frontier.Add(newFrontierCandidate);
                    }
                }
            }

            if (ignoreObstacles)
            {
                foreach (var neighbourAndDistanceObst in currCell.neighbourObstaclesAndDistance)
                {
                    if (visited.Contains(neighbourAndDistanceObst.cell) == false)
                    {
                        var neighbour = neighbourAndDistanceObst.cell;
                        var distanceToCell = neighbourAndDistanceObst.distance;

                        if (distanceToCell + distance <= fullDistance + decayDistance)
                        {
                            var newFrontierCandidate = new CellAndDistance(neighbour, distance + distanceToCell);
                            visited.Add(neighbour);
                            frontier.Add(newFrontierCandidate);
                        }
                    }
                }    
            }
        }
        
        sw.Stop();
//        Console.Error.WriteLine("Fill time={0}",sw.Elapsed);
        #if UNITY_EDITOR
//        UnityEngine.Debug.LogFormat("Fill time={0}",sw.Elapsed);
        #endif
        sw.Reset();
        
//        sw.Stop();
//        Console.Error.WriteLine("Steps ={0} ObstacleSteps={1}",steps, obstacleSteps);
//        #if UNITY_EDITOR
//        UnityEngine.Debug.LogFormat("Steps ={0} ObstacleSteps={1}",steps, obstacleSteps);
//        #endif
//        sw.Reset();
    }

    private CellAndDistance GetBestFrontierCell(List<CellAndDistance> frontier, bool removeCellFromCollection = false)
    {
        CellAndDistance bestSoFar = frontier.First();
        int bestIndex = 0;
        int frontierCardinality = frontier.Count;
        for (var i = 0; i < frontierCardinality; i++)
        {
            var candidate = frontier[i];
            if (candidate.distance < bestSoFar.distance)
            {
                bestSoFar = candidate;
                bestIndex = i;
            }
        }

        if (removeCellFromCollection)
        {
            frontier.RemoveAt(bestIndex);
        }
        
        return bestSoFar;
    }
    
//    public void ApplyInfluence_Range(int xPos, int yPos, double amount, int fullDistance, int decayDistance, PropagationFunction decayedDistanceFunc, int initialRadiusSkip = 0)
//    {
//        InfluenceMapCell startCell = influenceMapCells[xPos, yPos];
//
//        HashSet<InfluenceMapCell> visited = new HashSet<InfluenceMapCell>();
//        List<Tuple<InfluenceMapCell, double>> frontier = new List<Tuple<InfluenceMapCell, double>>();
//
////        if (isObstacle[startCell.x, startCell.y])
//        if(initialRadiusSkip != 0)
//        {
//            //We need to do a radius scan and put in the frontier some squares that are not obstacles. The closest ones to this center
//            var squaresAndDistanceAround = GetSquaresInRange_WithDistance_Unscaled(startCell.x, startCell.y, initialRadiusSkip+10);
//            double closestOutsideRange = squaresAndDistanceAround.Where(sAd => isObstacle[sAd.Item1.x,sAd.Item1.y]==false).Min(sAd1 => sAd1.Item2);
//
//            int flag = 0;
//            foreach (var newFrontierMember in squaresAndDistanceAround.Where(sAd => sAd.Item2 == closestOutsideRange))
//            {
//                var currCell = influenceMapCells[newFrontierMember.Item1.x, newFrontierMember.Item1.y];
//                frontier.Add(Tuple.Create(currCell, newFrontierMember.Item2));
//                flag++;
//            }
//
//            if (flag > 0)
//            {
//                Console.Error.WriteLine("A new frontier with "+flag+" tiles was created. The closestRange was "+closestOutsideRange);
//            }
//        }
//        else
//        {
//            frontier.Add(Tuple.Create(startCell,0.0));
//        }
//        
//        visited.Add(startCell);
//        while (frontier.Count > 0)
//        {
//            var currFrontierCellInfo = frontier.OrderBy(f => f.Item2).First();
//            frontier.Remove(currFrontierCellInfo);
//            
//            InfluenceMapCell currCell = currFrontierCellInfo.Item1;
//            double distance = currFrontierCellInfo.Item2;
//
//            double cellAmount = amount;
//            if (distance > fullDistance)
//            {
//                cellAmount = decayedDistanceFunc(amount, distance, maxDistance_Manh);
//            }
//            
//            _influenceMap[currCell.x, currCell.y] += cellAmount;
//            
//            foreach (var neighbourAndDistance in currCell.neighboursAndDistance.OrderBy(nAd => nAd.distance))
//            {
//                if (visited.Contains(neighbourAndDistance.Item1) == false)
//                {
//                    var neighbour = neighbourAndDistance.Item1;
//                    var distanceToCell = neighbourAndDistance.Item2;
//
//                    if (distanceToCell + distance <= fullDistance + decayDistance)
//                    {
//                        var newFrontierCandidate = Tuple.Create(neighbour, distance + distanceToCell);
//                        visited.Add(neighbour);
//                        frontier.Add(newFrontierCandidate);
//                    }
//                }
//            }
//        }
//    }
//    

    public void AddObstacle(int x, int y, int range)
    {
        foreach (var square in GetSquaresInRange(x, y, range))
        {
            isObstacle[square.x, square.y] = true;
            _influenceMap[square.x, square.y] = -5;
        }
    }

    public void ResetMapToZeroes()
    {
        _influenceMap = new double[gridWidth, gridHeight];
    }
    

	private bool isInBounds(int x, int y)
	{
		return x >= 0 && x < gridWidth && y>=0 && y < gridHeight;
	}

	/**
	 * Returns 1 if the amount was correctly set, 0 if the x,y were out of the map's bounds. 
	 */
	private int SetAmount_IfInBounds(int x, int y, double amount)
	{
		if (isInBounds(x, y))
		{
			this[x, y] = amount;
			return 1;
		}

		return 0;
	}
	
	/**
	 * Returns 1 if the amount was correctly set, 0 if the x,y were out of the map's bounds. 
	 */
	private int AddAmount_IfInBounds(int x, int y, double amount)
	{
		if (isInBounds(x, y))
		{
			var currAmount = this[x, y];
			this[x, y] = currAmount + amount;
			return 1;
		}

		return 0;
	}

	public double get(int x, int y)
	{
		var amount = _influenceMap[x,y];
		var clampedAmount = Clamp(amount, minInfluence, maxInfluence);
		return clampedAmount;
	}
	
	public double Clamp(double value, double min, double max)  
    {  
        return (value < min) ? min : (value > max) ? max : value;  
    }

	public List<XAndY> getNeighbours(int x, int y)
	{
		List<XAndY> neighbours = new List<XAndY>();
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (i == 0 && j == 0) { continue; }

				int xNeighbour = x - i, yNeighbour = y - j;
				if (xNeighbour >= 0 && xNeighbour <= gridWidth - 1
				&& yNeighbour >= 0 && yNeighbour <= gridHeight - 1)
				{
					neighbours.Add(new XAndY( xNeighbour, yNeighbour ));
				}
			}
		}
		return neighbours;
	}

	private HashSet<XAndY> myHashset;

	public void ApplyInfluence_Diamond(int x, int y, double amount, int fullDistance, int decayedDistance, double distanceDecay)
	{
		AddAmount_IfInBounds(x, y, amount);

		for (int range = 1; range <= fullDistance + decayedDistance; range++)
		{
			if (range > fullDistance)
			{
				amount *= distanceDecay;
			}
			AddAmount_IfInBounds(x + range, y + 0, amount);
			AddAmount_IfInBounds(x - range, y + 0, amount);
			
			AddAmount_IfInBounds(x + 0, y + range, amount);
			AddAmount_IfInBounds(x + 0, y - range, amount);

			for (int diagonal = 1; diagonal < range; diagonal++)
			{
				double influenceToApply = amount;
				//we fill the rombo by mirroring 1/4 of the shape. (an edge connected to the center)
				int fails = 0;
				fails += AddAmount_IfInBounds(x + range - diagonal, y - diagonal, amount);
				fails += AddAmount_IfInBounds(x + range - diagonal, y + diagonal, amount);
				fails += AddAmount_IfInBounds(x - range + diagonal, y + diagonal, amount);
				fails += AddAmount_IfInBounds(x - range + diagonal, y - diagonal, amount);
				//if(fails == 4) { break; }
				//_influenceMap[x + range - diagonal][y - diagonal] = amount;
				//_influenceMap[x + range - diagonal][y + diagonal] = amount;
				//_influenceMap[x + range + diagonal][y + diagonal] = amount;
				//_influenceMap[x + range + diagonal][y - diagonal] = amount;
			}
		}
	}

    public void ApplyInfluence_Circle(int xPos, int yPos, double amount, int fullDistance, int decayDistance,
        PropagationFunction decayedDistanceFunc)
    {
        int xCenter = xPos;
        int yCenter = yPos;
        int radius = fullDistance + decayDistance;

        foreach (var pos in GetSquaresInRange(xPos, yPos, radius))
        {
            int x = pos.x;
            int y = pos.y;

            if (isObstacle[x, y])
            {
                continue;
            }
            
            var distanceSqr = (x - xCenter) * (x - xCenter) + (y - yCenter) * (y - yCenter);
            double manhattanDistance = Math.Abs(x - xCenter) + Math.Abs(y - yCenter);
            double cellAmount = amount;

            if (manhattanDistance > fullDistance)
            {
//                    double mahnattanDistance = Math.Abs(x - xCenter) + Math.Abs(y - yCenter);
//                    cellAmount = decayedDistanceFunc(amount, Math.Sqrt(distanceSqr) - fullDistance, decayDistance);
                cellAmount = decayedDistanceFunc(amount, distanceSqr, LaPulzellaD_Orleans.MAX_DISTANCE);
            }
            
            AddAmount_IfInBounds(x, y, cellAmount);
            
            
//                int xSym = xCenter - (x - xCenter);
//                int ySym = yCenter - (y - yCenter);

//                AddAmount_IfInBounds(xSym, y, cellAmount);
//                AddAmount_IfInBounds(x, ySym, cellAmount);
//                AddAmount_IfInBounds(xSym, ySym, cellAmount);

        }
    }


    
    public List<Position> GetSquaresInRange (int xPos, int yPos, int radius)
    {
        List<Position> results = new List<Position>();
            
        int x, y;

        for (y = -radius; y <= radius; y++)
        {
            for (x = -radius; x <=  radius; x++)
            {
//                double distanceSqr = (x * x) + (y * y);
                double distance = computeDistanceFunc.computeDistance(0, 0, x, y);
//                if (distanceSqr <= (radius * radius))
                if (distance <= radius)

                {
                    if (x + xPos < 0 || x + xPos >= gridWidth || y + yPos < 0 || y + yPos >= gridHeight)
                    {
                        continue;
                    }
                    
                    results.Add(new Position(xPos+x,yPos+y));
                }
            }
        }

        return results;

    }
    
    //This should be able to correctly handle the squares. Returns unscaled positions
    public List<Tuple<Position,double>> GetSquaresInRange_WithDistance_Unscaled(int xCenter, int yCenter, int radius)
    {
        List<Tuple<Position,double>> results = new List<Tuple<Position,double>>();

        Position currSquare;
        double radiusSqr = radius * radius;
//        int x, y;
        //Adding unit/2 is the same as rounding up the distance. This is to make up for stepping by unit
        for (int x = xCenter; x <= xCenter - radius +unit/2; x=x+unit)
//        for (int x = xCenter - radius ; x <= xCenter; x++)
        {
            for (int y = yCenter; y <= yCenter - radius + unit/2; y=y+unit)
            {
                // we don't have to take the square root, it's slow
                double distanceSqr = (x - xCenter) * (x - xCenter) + (y - yCenter) * (y - yCenter); 
                if (distanceSqr < radiusSqr) 
                {
                    int xSym = xCenter - (x - xCenter);
                    int ySym = yCenter - (y - yCenter);

                    double distance = Math.Sqrt(distanceSqr);
                    
                    results.Add(Tuple.Create(new Position(x,y), distance));
                    results.Add(Tuple.Create(new Position(x,ySym), distance));
                    results.Add(Tuple.Create(new Position(xSym,y), distance));
                    results.Add(Tuple.Create(new Position(xSym,ySym), distance));
                    
//                    AddAmount_IfInBounds(x, y, amount);
//                    AddAmount_IfInBounds(x, ySym, amount);
//                    AddAmount_IfInBounds(xSym, y, amount);
//                    AddAmount_IfInBounds(xSym, ySym, amount);
                    // (x, y), (x, ySym), (xSym , y), (xSym, ySym) are in the circle
                }
            }
        }        
//        for (y = -radius; y <= radius+unit/2; y = y+unit)
//        {
//            for (x = -radius; x <=  radius+unit/2; x = x+unit)
//            {
//                double distanceSqr = (x * x) + (y * y);
////                double distance = computeDistanceFunc.computeDistance(0, 0, x, y);
//                if (distanceSqr <= (radius * radius) )
////                if (distance <= radius)
//
//                {
//                    if (x + xPos < 0 || x + xPos >= gridWidth * unit || y + yPos < 0 || y + yPos >= gridHeight * unit)
//                    {
//                        continue;
//                    }
//                    
//                    results.Add(Tuple.Create(new Position(xPos+x,yPos+y), Math.Sqrt(distanceSqr)));
//                }
//            }
//        }

        return results;

    }
    



    public Tuple<int, int> selectBestInCircle(int x1, int y1, int radius)
    {
        int xCenter = x1;
        int yCenter = y1;

        Tuple<int, int> bestSquare = Tuple.Create(x1, y1);
        double bestSquareInflValue = _influenceMap[bestSquare.Item1,bestSquare.Item2];
        
        for (int x = xCenter - radius ; x <= xCenter; x++)
        {
            for (int y = yCenter - radius ; y <= yCenter; y++)
            {
                // we don't have to take the square root, it's slow
                if ((x - xCenter)*(x - xCenter) + (y - yCenter)*(y - yCenter) <= radius*radius) 
                {
                    int xSym = xCenter - (x - xCenter);
                    int ySym = yCenter - (y - yCenter);

                    
                    if (_influenceMap[x,y] > bestSquareInflValue)
                    {
                        bestSquare = Tuple.Create(x,y);
                        bestSquareInflValue = _influenceMap[x,y];
                    }
                    if (_influenceMap[x,ySym] > bestSquareInflValue)
                    {
                        bestSquare = Tuple.Create(x,ySym);
                        bestSquareInflValue = _influenceMap[x,ySym];
                    }
                    if (_influenceMap[xSym,y] > bestSquareInflValue)
                    {
                        bestSquare = Tuple.Create(xSym,y);
                        bestSquareInflValue = _influenceMap[xSym,y];
                    }
                    if (_influenceMap[xSym,ySym] > bestSquareInflValue)
                    {
                        bestSquare = Tuple.Create(xSym,ySym);
                        bestSquareInflValue = _influenceMap[xSym,ySym];
                    }
//                    AddAmount_IfInBounds(x, y, amount);
//                    AddAmount_IfInBounds(x, ySym, amount);
//                    AddAmount_IfInBounds(xSym, y, amount);
//                    AddAmount_IfInBounds(xSym, ySym, amount);
                    // (x, y), (x, ySym), (xSym , y), (xSym, ySym) are in the circle
                }
            }
        }

        return bestSquare;
    }
    
    

	public void applyInfluenceStars(int x, int y, double amount, int fullDistance, int decayedDistance, double distanceDecay)
	{
		AddAmount_IfInBounds(x, y, amount);

		for (int range = 1; range < fullDistance + decayedDistance; range++)
		{
			if (range > fullDistance)
			{
				amount *= distanceDecay;
			}
			AddAmount_IfInBounds(x + range, y + 0, amount);
			AddAmount_IfInBounds(x + 0, y + range, amount);
			AddAmount_IfInBounds(x - range, y + 0, amount);
			AddAmount_IfInBounds(x + 0, y - range, amount);
		}
	}

	
	//Use two opposite corners, where x1<x2 or y1<y2. They will define the bounds of the search. 
	//The tile with highest score is selected. If multiple bests, it will select the last
	public Tuple<int, int> selectBestInBox(int x1, int y1, int x2, int y2)
	{
		//TODO: handle casse where x1>x2 or y1>y2?
		double currBestScore = double.MinValue;
		Tuple<int, int> currBest = Tuple.Create(-1, -1);
		for (int currX = x1; currX <= x2; currX++)
		{
			for (int currY = y1; currY <= y2; currY++)
			{
				if (get(currX, currY) > currBestScore)
				{
					currBestScore = get(currX, currY);
					currBest = Tuple.Create(currX, currY);
				}
			}
		}
		return currBest;
	}

        // use Bresenham-like algorithm to print a line from (y1,x1) to (y2,x2)
    // The difference with Bresenham is that ALL the points of the line are
    //   printed, not only one per x coordinate.
    // Principles of the Bresenham's algorithm (heavily modified) were taken from:
    //   http://www.intranet.ca/~sshah/waste/art7.html
    public List<Position> GetPointsInLine(int y1, int x1, int y2, int x2)
    {
        List<Position> result = new List<Position>();
        int i; // loop counter
        int ystep, xstep; // the step on y and x axis
        int error; // the error accumulated during the increment
        int errorprev; // *vision the previous value of the error variable
        int y = y1, x = x1; // the line points
        int ddy, ddx; // compulsory variables: the double values of dy and dx
        int dx = x2 - x1;
        int dy = y2 - y1;
        result.Add(new Position(y1, x1)); // first point
        
        // NB the last point can't be here, because of its previous point (which has to be verified)
        if (dy < 0)
        {
            ystep = -1;
            dy = -dy;
        }
        else
            ystep = 1;

        if (dx < 0)
        {
            xstep = -1;
            dx = -dx;
        }
        else
            xstep = 1;

        ddy = 2 * dy; // work with double values for full precision
        ddx = 2 * dx;
        if (ddx >= ddy)
        {
            // first octant (0 <= slope <= 1)
            // compulsory initialization (even for errorprev, needed when dx==dy)
            errorprev = error = dx; // start in the middle of the square
            for (i = 0; i < dx; i++)
            {
                // do not use the first point (already done)
                x += xstep;
                error += ddy;
                if (error > ddx)
                {
                    // increment y if AFTER the middle ( > )
                    y += ystep;
                    error -= ddx;
                    // three cases (octant == right->right-top for directions below):
                    if (error + errorprev < ddx) // bottom square also
                        result.Add(new Position(y - ystep, x));
                    else if (error + errorprev > ddx) // left square also
                        result.Add(new Position(y, x - xstep));
                    else
                    {
                        // corner: bottom and left squares also
                        result.Add(new Position(y - ystep, x));
                        result.Add(new Position(y, x - xstep));
                    }
                }

                result.Add(new Position(y, x));
                errorprev = error;
            }
        }
        else
        {
            // the same as above
            errorprev = error = dy;
            for (i = 0; i < dy; i++)
            {
                y += ystep;
                error += ddx;
                if (error > ddy)
                {
                    x += xstep;
                    error -= ddy;
                    if (error + errorprev < ddy)
                        result.Add(new Position(y, x - xstep));
                    else if (error + errorprev > ddy)
                        result.Add(new Position(y - ystep, x));
                    else
                    {
                        result.Add(new Position(y, x - xstep));
                        result.Add(new Position(y - ystep, x));
                    }
                }

                result.Add( new Position(y, x));
                errorprev = error;
            }
        }
      // assert ((y == y2) && (x == x2));  // the last point (y2,x2) has to be the same with the last point of the algorithm
        return result;
    }


    public Tuple<int, int> SelectBestInBox_Unscaled(int x1, int y1, int x2, int y2)
    {
        //TODO: handle casse where x1>x2 or y1>y2?
		double currBestScore = double.MinValue;
		Tuple<int, int> currBest = Tuple.Create(-1, -1);
		for (int currX = x1; currX <= x2; currX++)
		{
			for (int currY = y1; currY <= y2; currY++)
			{
				if (get(currX/unit, currY/unit) > currBestScore)
				{
					currBestScore = get(currX/unit, currY/unit);
					currBest = Tuple.Create(currX/unit, currY/unit);
				}
			}
		}
		return currBest;
    }
}
