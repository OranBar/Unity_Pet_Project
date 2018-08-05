#define RUNLOCAL

/** Code by Oran Bar **/

//The max characters that can be put into the error stream is 1028

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Schema;

#if UNITY_EDITOR
using OranUnityUtils;
using UnityEngine;
#endif

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/

class Player
{
    static void Main(string[] args)
    {
        LaPulzellaD_Orleans giovannaD_Arco = new LaPulzellaD_Orleans();

        giovannaD_Arco.ParseInputs_Begin();

        int turn = 0;
        
        while (true)
        {
            turn++;
            giovannaD_Arco.ParseInputs_Turn();
            if (turn == 1)
            {
                Console.Error.WriteLine("Game Info");
                Console.Error.WriteLine(giovannaD_Arco.game.Encode()+"\n");
            }
            Console.Error.WriteLine(giovannaD_Arco.currGameState.Encode());
            InfluenceMap runMap = new InfluenceMap();
            InfluenceMap buildMap = new InfluenceMap();
            TurnAction move = giovannaD_Arco.think(out runMap, out buildMap);
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
}

public class GameState
{
    public List<Site> sites = new List<Site>();
    public List<Unit> units = new List<Unit>();
    public int money;
    public int touchedSiteId;

    public int numUnits => units.Count;

    public Unit MyQueen => units.First(u => u.owner == Owner.Friendly && u.unitType == UnitType.Queen);
    public Unit EnemyQueen => units.First(u => u.owner == Owner.Enemy && u.unitType == UnitType.Queen);

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
    public static int MAX_CONCURRENT_MINES = 0, MAX_BARRACKSES_KNIGHTS = 1, MAX_BARRACKSES_ARCER = 0, MAX_BARRACKSES_GIANT = 0, MAX_TOWERS = 0;
    public static int GIANT_COST = 140, KNIGHT_COST = 80, ARCHER_COST = 100;
    public static int ENEMY_CHECK_RANGE = 340, TOO_MANY_UNITS_NEARBY = 2;
    public static int INFLUENCEMAP_SQUARELENGTH = 25;
    public static int QUEEN_MOVEMENT = 60;
    
    public GameInfo game;

    public GameState currGameState;
    public GameState prevGameState = null;

    private int buildOrder_stateMachine_stateIndex = -1;

    
    public TurnAction think(out InfluenceMap turnInfluenceMap, out InfluenceMap buildInfluenceMap)
    {
        buildInfluenceMap = null;
        TurnAction chosenMove = new TurnAction();
        Unit myQueen = currGameState.MyQueen;

        turnInfluenceMap = CreateInfluenceMap();

        var bestTileToRun = UnscaledBestInBox(myQueen, QUEEN_MOVEMENT * 10, turnInfluenceMap);
#if UNITY_EDITOR
//        Debug.Log("Queen Danger is "+turnInfluenceMap[myQueen.pos.x/INFLUENCEMAP_SQUARELENGTH, myQueen.pos.y/INFLUENCEMAP_SQUARELENGTH]);
        
        UnityEngine.Debug.Log("BestTile is ("+bestTileToRun.Item1+", "+bestTileToRun.Item2+") with amount = "+turnInfluenceMap[bestTileToRun.Item1/INFLUENCEMAP_SQUARELENGTH, bestTileToRun.Item2/INFLUENCEMAP_SQUARELENGTH]);
#endif
        
//        Console.Error.WriteLine("My Queen "+currGameState.MyQueen);
//        Console.Error.WriteLine("touchedSiteId "+currGameState.touchedSiteId);

        
        Site touchedSite = null;
        if (currGameState.touchedSiteId != -1)
        {
            touchedSite = currGameState.sites[currGameState.touchedSiteId];
        }
        
        IEnumerable<Site> mySites = currGameState.sites.Where(s => s.owner == Owner.Friendly);

        // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
        Site closestUnbuiltSite = SortSites_ByDistance_WithBiasTowardsCenter(myQueen.pos, currGameState.sites)
            .Where(s => s.owner == Owner.Neutral 
                        && currGameState.units.Where(u => u.owner == Owner.Enemy).Count(u => u.DistanceTo(s) < ENEMY_CHECK_RANGE) < TOO_MANY_UNITS_NEARBY)
            .FirstOrDefault();

//        Site targetMoveSite = ChooseNextSite();
        
        List<Site> closestUnbuiltMines = SortSites_ByDistance_WithBiasTowardsEdges(myQueen.pos, currGameState.sites)
            .Where(s => s.structureType == StructureType.None && s.owner == Owner.Neutral && IsSiteMinedOut(s.siteId) == false)
            .ToList();
        
        int owned_knight_barrackses = mySites.Count(ob => ob.structureType == StructureType.Barracks && ob.CreepsType == UnitType.Knight);
        int owned_archer_barrackses = mySites.Count(ob => ob.structureType == StructureType.Barracks && ob.CreepsType == UnitType.Archer);
        int owned_giant_barrackses = mySites.Count(ob => ob.structureType == StructureType.Barracks && ob.CreepsType == UnitType.Giant);
        int owned_mines = mySites.Count(ob => ob.structureType == StructureType.Mine && ob.gold > 0);
        int owned_towers = mySites.Count(ob => ob.structureType == StructureType.Tower && ob.owner == Owner.Friendly);
        
//        Console.Error.WriteLine($"owned_knight_barrackses {owned_knight_barrackses} " +
//                                $"owned_archer_barrackses {owned_archer_barrackses} " +
//                                $"owned_giant_barrackses {owned_giant_barrackses} " +
//                                $"owned_mines {owned_mines} " +
//                                $"owned_towers {owned_towers} ");
//
//        Console.Error.WriteLine(
//            $"MAX_CONCURRENT_MINES {MAX_CONCURRENT_MINES} " +
//            $"MAX_BARRACKSES_KNIGHTS {MAX_BARRACKSES_KNIGHTS} " +
//            $"MAX_BARRACKSES_ARCER  {MAX_BARRACKSES_ARCER} " +
//            $"MAX_BARRACKSES_GIANT {MAX_BARRACKSES_GIANT} " +
//            $"MAX_TOWERS {MAX_TOWERS} "
//        );
                            
        
        
        int total_owner_barrackses = owned_archer_barrackses + owned_giant_barrackses + owned_knight_barrackses;
        
        bool touchingNeutralSite = touchedSite != null && touchedSite.owner == Owner.Neutral;
        bool touchingMyMine = touchedSite != null && touchedSite.owner == Owner.Friendly &&
                              touchedSite.structureType == StructureType.Mine;
        
        bool touchingMyTower = touchedSite != null && touchedSite.owner == Owner.Friendly &&
                               touchedSite.structureType == StructureType.Tower;
        
        bool prev_touchingMyMine = touchedSite != null && touchedSite.owner == Owner.Friendly &&
                                   touchedSite.structureType == StructureType.Mine;

        var enemyUnitsInMyQueenRange =
            currGameState.units.Count(u => u.owner == Owner.Enemy && myQueen.DistanceTo(u) <= ENEMY_CHECK_RANGE);

        int owned_giants = currGameState.units.Count(u => u.owner == Owner.Friendly && u.unitType == UnitType.Giant);
        
        
        //If we are touching a site, we do something with it
        if (touchingNeutralSite)
        {
            //Build
            var chosenBuildMove = ChoseBuildMove(closestUnbuiltMines, touchedSite, owned_mines, owned_knight_barrackses, owned_archer_barrackses, owned_giant_barrackses, owned_towers, myQueen);
            chosenMove.queenAction = chosenBuildMove;
        }
        else //Run
        if (enemyUnitsInMyQueenRange >= TOO_MANY_UNITS_NEARBY)
        {
            Console.Error.WriteLine("Running from enemies");
            chosenMove.queenAction = new Move(bestTileToRun.Item1, bestTileToRun.Item2);
//            Move moveToAngle = RunToAngle(myQueen);
//            chosenMove.queenAction = RunToClosestTowerOrAngle(myQueen);
        }
        else if (touchingMyMine && IsMineMaxed(touchedSite) == false)
        {
            //Empower Mine
            chosenMove.queenAction = new BuildMine(currGameState.touchedSiteId);
        }
        else if (touchingMyTower && touchedSite.param1 <= 700 && owned_towers >= 2)
        {
            //Emppower Tower
            chosenMove.queenAction = new BuildTower(currGameState.touchedSiteId);
        }
        
        if(chosenMove.queenAction is Wait)
        {
            double squareLength = INFLUENCEMAP_SQUARELENGTH; //Maximum common divisor between 60, 100, 75, 50 (movement speeds)

            int mapWidth = (int) Math.Ceiling(1920 / squareLength)+1;
            int mapHeight = (int) Math.Ceiling(1000 / squareLength)+1;
            double minInfluence = -40;
            double maxInfluence = 40;
            buildInfluenceMap = new InfluenceMap(mapWidth, mapHeight, minInfluence, maxInfluence, new EuclideanDistanceSqr());
            
            
            
            foreach (var site in currGameState.sites.Where(s => s.owner == Owner.Neutral))
            {
                int sitePosX = (int) Math.Ceiling(site.pos.x / squareLength);
                int sitePosY = (int) Math.Ceiling(site.pos.y / squareLength);


                double influenceValue = 1;// + (site.pos.Distance(myQueen.pos) / 500);  
//                if (owned_mines < MAX_CONCURRENT_MINES)
//                {
//                    influenceValue = influenceValue + (site.pos.Distance(new Position(960, 500)) / 200);
//                }
//                else
//                {
//                    influenceValue = influenceValue - (site.pos.Distance(new Position(960, 500)) / 400);
//                }
                
                int siteRadius = (int) Math.Floor(GetSiteInfo(site).radius / squareLength);
                
                
                buildInfluenceMap.applyInfluence(sitePosX, sitePosY, influenceValue, siteRadius+1, 0, 0);
                buildInfluenceMap.applyInfluence(sitePosX, sitePosY, -influenceValue, siteRadius, 0, 0);
            }

            Func<Site, bool> isBusyAttackingEnemies = t => currGameState.units
                .Any(u1 => u1.owner == Owner.Friendly && u1.DistanceTo(t) < t.param2);
                
            
            var enemyTowers = currGameState.sites
                .Where(u => u.owner == Owner.Enemy && u.structureType == StructureType.Tower)
                .Where(t => isBusyAttackingEnemies(t) == false);
            
            foreach (var tower in enemyTowers)
            {
//                int towerPosX = (int) Math.Ceiling(tower.pos.x / squareLength);
//                int towerPosY = (int) Math.Ceiling(tower.pos.y / squareLength);
                double towerInfluence = 10;
                //4 is health decay per turn
                int decayedDistance = (int) Math.Ceiling((tower.param2 - 4) / squareLength);
//            int decayedDistance = 3;

//                mapForBuilding.applyInfluence(towerPosX, towerPosY, -towerInfluence, 3, decayedDistance, 0.8);
//                mapForBuilding.applyInfluence(towerPosX, towerPosY, towerInfluence, 1, 0, 0);
//                
                ScaleAndApplyInfluence(tower, -towerInfluence, 3, decayedDistance, 0.95, ref buildInfluenceMap);
                ScaleAndApplyInfluence(tower, towerInfluence, 1,0,0, ref buildInfluenceMap);
            }
            
            
            var bestSiteForBuilding = UnscaledBestInBox(myQueen, QUEEN_MOVEMENT * 12, buildInfluenceMap);
            Console.Error.WriteLine("best site for building is "+bestSiteForBuilding.Item1+", "+bestSiteForBuilding.Item2);

            if (bestSiteForBuilding.Item1 + bestSiteForBuilding.Item2 == 0)
            {
                chosenMove.queenAction = new Wait();
            }
            else
            {
                chosenMove.queenAction = new Move(bestSiteForBuilding.Item1, bestSiteForBuilding.Item2);
            }

            if (closestUnbuiltMines.FirstOrDefault() != null && owned_mines < MAX_CONCURRENT_MINES)
            {
                //Go To Next Mine (Tries to filter our the mined out sites.
//                chosenMove.queenAction = new Move(GetSiteInfo(closestUnbuiltMines.First()).pos);
            }
            else if (total_owner_barrackses < MAX_BARRACKSES_KNIGHTS + MAX_BARRACKSES_ARCER + MAX_BARRACKSES_GIANT || owned_towers < MAX_TOWERS)
            {
                //Go to next closest site
//                chosenMove.queenAction = new Move(GetSiteInfo(closestUnbuiltSite).pos);
                //Run to angle if close to enemies. Running takes priority, so we do the computations last
            }
            else
            {
                buildOrder_stateMachine_stateIndex++;

                if (buildOrder_stateMachine_stateIndex == 0)
                {
//                    MAX_BARRACKSES_ARCER++;
                    MAX_CONCURRENT_MINES++;
                    MAX_CONCURRENT_MINES++;
//                    MAX_CONCURRENT_MINES++;
//                    MAX_BARRACKSES_KNIGHTS++;
//                    MAX_BARRACKSES_KNIGHTS++;
//                    MAX_BARRACKSES_GIANT++;
                    MAX_TOWERS++;
                    MAX_TOWERS++;
                    MAX_TOWERS++;
                    MAX_TOWERS++;

                }
                else if (buildOrder_stateMachine_stateIndex == 1)
                {
                    MAX_BARRACKSES_GIANT++;
                    MAX_BARRACKSES_KNIGHTS++;
                    
                }
                else if (buildOrder_stateMachine_stateIndex == 2){
                    MAX_CONCURRENT_MINES++;
                }
            }
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

        Unit enemyQueen = currGameState.EnemyQueen;
        
        IEnumerable<Site> myIdleBarracses = mySites
            .Where(site => site.structureType == StructureType.Barracks && site.param1 == 0)
            .OrderBy(s2 => s2.pos.DistanceSqr(enemyQueen.pos));
        
        List<Site> barracksesToTrainFrom = new List<Site>();
        
        if (myIdleBarracses.Any())
        {
            //Train

//            DecideUnitsToTrain();

            if (owned_giants >= 1)
            {
                myIdleBarracses = myIdleBarracses.Where(b => b.CreepsType != UnitType.Giant);
            }
            
            int remainingGold = currGameState.money;
            foreach (var barraks in myIdleBarracses)
            {
                int cost = 0;
                switch (barraks.CreepsType)
                {
                    case UnitType.Knight:
                        cost += KNIGHT_COST;
                        break;
                    case UnitType.Archer:
                        cost += ARCHER_COST;
                        break;
                    case UnitType.Giant:
                        cost += GIANT_COST;
                        break;
                }
                
                if (remainingGold >= cost)
                {
                    barracksesToTrainFrom.Add(barraks);
                    remainingGold -= cost;
                }
                else
                {
                    break;
                }
            }
            chosenMove.trainAction = new Train(barracksesToTrainFrom); 
        }
        else
        {
            //Wait
            chosenMove.trainAction = new HaltTraing();
        }

        return chosenMove;
    }

//    private Site ChooseNextSite()
//    {
//        Site closestUnbuiltSite = SortSites_ByDistance(currGameState.MyQueen.pos, currGameState.sites)
//                    .Where(s => s.owner == Owner.Neutral 
//                                && currGameState.units.Where(u => u.owner == Owner.Enemy).Count(u => u.DistanceTo(s) < ENEMY_CHECK_RANGE) < TOO_MANY_UNITS_NEARBY)
//                    .FirstOrDefault();
//
//        return closestUnbuiltSite;
//
//    }

    private InfluenceMap CreateInfluenceMap()
    {
//        double squareLength = (60 * 0.4);
        double squareLength = INFLUENCEMAP_SQUARELENGTH; //Maximum common divisor between 60, 100, 75, 50 (movement speeds)

        int mapWidth = (int) Math.Ceiling(1920 / squareLength)+1;
        int mapHeight = (int) Math.Ceiling(1000 / squareLength)+1;
        double minInfluence = -40;
        double maxInfluence = 40;
        InfluenceMap map = new InfluenceMap(mapWidth, mapHeight, minInfluence, maxInfluence, new EuclideanDistanceSqr());

        var enemyUnits = currGameState.units
            .Where(u => u.owner == Owner.Enemy);

        var myTowers = currGameState.sites
            .Where(u => u.owner == Owner.Friendly && u.structureType == StructureType.Tower);
        
        var enemyTowers = currGameState.sites
            .Where(u => u.owner == Owner.Enemy && u.structureType == StructureType.Tower);
        /*
        foreach (var tower in myTowers)
        {
            double towerInfluence = 40;
//            int decayedDistance = (int)Math.Floor((int)Math.Ceiling(tower.param2 / squareLength) * 1.6);
//            int decayedDistance = 3;
            int decayedDistance = (int) Math.Ceiling((tower.param2 - 4) / squareLength);

            ScaleAndApplyInfluence(tower, towerInfluence, 3, decayedDistance, 0.7, ref map);
            ScaleAndApplyInfluence(tower, -towerInfluence, 1, 0, 0, ref map);
            
        }
        
        foreach (var tower in enemyTowers)
        {
            double towerInfluence = 40;
            int decayedDistance = (int) Math.Ceiling(tower.param2 / squareLength);
//            int decayedDistance = 3;

            ScaleAndApplyInfluence(tower, -towerInfluence, 3, decayedDistance, 0.7, ref map);
            ScaleAndApplyInfluence(tower, towerInfluence, 1, 0, 0, ref map);
        }
*/
        if (enemyUnits.Any())
        {
            Unit enemy = enemyUnits.First();
            Unit myQueen = currGameState.MyQueen;
            List<Position> enemyToQueenLine = map.useVisionLine(enemy.pos.x, enemy.pos.y, myQueen.pos.x, myQueen.pos.y);

            Vector2_OB side1Pos = (new Vector2_OB(myQueen.pos.x, myQueen.pos.y) - new Vector2_OB(enemy.pos.x, enemy.pos.y)).Normalize().Orthogonal() * 10; 
            Vector2_OB side2Pos = (new Vector2_OB(myQueen.pos.x, myQueen.pos.y) - new Vector2_OB(enemy.pos.x, enemy.pos.y)).Normalize().Orthogonal() * -10;
            
            List<Position> enemytoQueenSide1 = map.useVisionLine(enemy.pos.x, enemy.pos.y, (int)side1Pos.X, (int)side1Pos.Y); 
            List<Position> enemytoQueenSide2 = map.useVisionLine(enemy.pos.x, enemy.pos.y, (int)side2Pos.X, (int)side2Pos.Y);
            
            //Expand line 
            double influence = 10;
            foreach (var pos in enemyToQueenLine)
            {
                if (map.isObstacle(pos.x / INFLUENCEMAP_SQUARELENGTH, pos.y/ INFLUENCEMAP_SQUARELENGTH))
                {
                    influence = influence * 0.66;
                }
                ScaleAndApplyInfluence(pos, influence, 1, 0, 0, ref map);
            }

            influence = -10;
            
            foreach (var pos in enemytoQueenSide1)
            {
                if (map.isObstacle(pos.x/ INFLUENCEMAP_SQUARELENGTH, pos.y/ INFLUENCEMAP_SQUARELENGTH))
                {
                    influence = influence * 0.66;
                }
                ScaleAndApplyInfluence(pos, influence, 1, 0, 0, ref map);
            }
            
//            foreach (var pos in enemytoQueenSide2)
//            {
//                if (map.isObstacle(pos.x, pos.y))
//                {
//                    influence = influence * 0.66;
//                }
//                    
//                ScaleAndApplyInfluence(pos, influence, 1, 0, 0, ref map);
//            }

        }
        
//        //Enemy units influence
//        foreach (var enemy in enemyUnits)
//        {
//            double enemyInfluence = GetEnemyInfluence(enemy);
//            ScaleAndApplyInfluence(enemy, enemyInfluence, GetEnemyInfluenceRadius(enemy), GetEnemyInfluenceRadius(enemy), 0.5, ref map);
//        }
        
       
        
//        //My Queen
//        Unit myQueen = currGameState.MyQueen;
//        int myQueenPosX = (int) Math.Ceiling(myQueen.pos.x / squareLength);
//        int myQueenPosY = (int) Math.Ceiling(myQueen.pos.y / squareLength);

//        map.applyInfluence(myQueenPosX, myQueenPosY, 10.0, 0, 0, 0.0);

        
        return map;

    }
    
    private void ScaleAndApplyInfluence(Site site, double amount, int fullDistance, int decayedDistance, double distanceDecay, ref InfluenceMap map)
    {
        ScaleAndApplyInfluence(site.pos, amount, fullDistance, decayedDistance, distanceDecay, ref map);
    }
    
    private void ScaleAndApplyInfluence(Unit unit, double amount, int fullDistance, int decayedDistance, double distanceDecay, ref InfluenceMap map)
    {
        ScaleAndApplyInfluence(unit.pos, amount, fullDistance, decayedDistance, distanceDecay, ref map);
    }
    
    private void ScaleAndApplyInfluence(Position pos, double amount, int fullDistance, int decayedDistance, double distanceDecay, ref InfluenceMap map)
    {
        ScaleAndApplyInfluence(pos.x, pos.y, amount, fullDistance, decayedDistance, distanceDecay, ref map);
    }

    private void ScaleAndApplyInfluence(int x, int y, double amount, int fullDistance, int decayedDistance, double distanceDecay, ref InfluenceMap map)
    {
        int scaledPosX = (int) Math.Ceiling(x*1.0 / INFLUENCEMAP_SQUARELENGTH);
        int scaledPosY = (int) Math.Ceiling(y*1.0 / INFLUENCEMAP_SQUARELENGTH);
        
        map.applyInfluence(scaledPosX ,scaledPosY ,amount,fullDistance,decayedDistance,distanceDecay);
        
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
        int x1 = Math.Max(0 , pos.x - range);
        int x2 = Math.Min(1920, pos.x + range);
        
        int y1 = Math.Max(0 , pos.y - range);
        int y2 = Math.Min(1000, pos.y + range);
        
        
        return UnscaledBestInBox(x1, y1, x2, y2, map);
    }
    
    private Tuple<int, int> UnscaledBestInBox(int x1, int y1, int x2, int y2, InfluenceMap map)
    {
        x1 = x1 / INFLUENCEMAP_SQUARELENGTH;
        y1 = y1 / INFLUENCEMAP_SQUARELENGTH;
        x2 = x2 / INFLUENCEMAP_SQUARELENGTH;
        y2 = y2 / INFLUENCEMAP_SQUARELENGTH;
        
        var bestCell = map.selectBestInBox(x1, y1, x2, y2);

        bestCell = Tuple.Create(bestCell.Item1 * INFLUENCEMAP_SQUARELENGTH, bestCell.Item2 * INFLUENCEMAP_SQUARELENGTH);

        return bestCell;
    }
    
    private double GetEnemyInfluence(Unit enemy)
    {
        switch (enemy.unitType)
        {
            case UnitType.Queen:
                return 0;
            case UnitType.Knight:
                return -10;
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
//        return 2;
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
        gameState.Decode(currGameState.Encode());

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
            currGameState.units.Count(u => u.owner == Owner.Friendly && u.unitType == UnitType.Giant);
        
        int myKnightsCount = 
            currGameState.units.Count(u => u.owner == Owner.Friendly && u.unitType == UnitType.Knight);
        
        int myArchersCount =
            currGameState.units.Count(u => u.owner == Owner.Friendly && u.unitType == UnitType.Archer);
        
        int enemyTowersCount =
            currGameState.sites.Count(s => s.owner == Owner.Enemy && s.structureType == StructureType.Tower);
        
        int enemyArchersCount =
            currGameState.units.Count(u => u.owner == Owner.Enemy && u.unitType == UnitType.Archer);

        int enemyUnitsCount =
            currGameState.units.Count(u => u.owner == Owner.Enemy);

        
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
    private IAction ChoseBuildMove(List<Site> closestUnbuiltMines, Site touchedSite, int owned_mines,
        int owned_knight_barrackses, int owned_archer_barrackses, int owned_giant_barrackses, int owned_towers, Unit myQueen)
    {
        IAction chosenBuildMove = null;
        
        //touchedSite == closestUnbuiltMine
        bool siteHasGold = touchedSite.gold > 0;

        if (owned_mines < MAX_CONCURRENT_MINES /*&& currGameState.touchedSiteId == closestUnbuiltMines.First().siteId*/
                                               && siteHasGold)
        {
            //chosenMove.queenAction = new BuildMine(currGameState.touchedSiteId);
            chosenBuildMove = new BuildMine(currGameState.touchedSiteId);
        }
        else if (owned_archer_barrackses < MAX_BARRACKSES_ARCER)
        {
            //chosenMove.queenAction = new BuildBarracks(currGameState.touchedSiteId, BarracksType.Archer);
            chosenBuildMove = new BuildBarracks(currGameState.touchedSiteId, BarracksType.Archer);
        }
        else if (owned_knight_barrackses < MAX_BARRACKSES_KNIGHTS)
        {
            //chosenMove.queenAction = new BuildBarracks(currGameState.touchedSiteId, BarracksType.Knight);
            chosenBuildMove = new BuildBarracks(currGameState.touchedSiteId, BarracksType.Knight);
        }
        else if (owned_towers < MAX_TOWERS)
        {
            chosenBuildMove = new BuildTower(currGameState.touchedSiteId);
        }
        else if (owned_giant_barrackses < MAX_BARRACKSES_GIANT)
        {
            //chosenMove.queenAction = new BuildBarracks(currGameState.touchedSiteId, BarracksType.Archer);
            chosenBuildMove = new BuildBarracks(currGameState.touchedSiteId, BarracksType.Giant);
        }
        else if (siteHasGold == false)
        {
            chosenBuildMove = new BuildTower(currGameState.touchedSiteId);
        }
        else
        {
            chosenBuildMove = new Wait();
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
            currGameState.sites
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
        return game.minedOutSites_ids.Contains(siteId);
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
        return game.sites[siteId];
    }
    
    public void ParseInputs_Turn()
    {
        prevGameState = currGameState;
        currGameState = new GameState();
        
        var inputs = Console.ReadLine().Split(' ');
        currGameState.money = int.Parse(inputs[0]);
        currGameState.touchedSiteId = int.Parse(inputs[1]); // -1 if none
        for (int i = 0; i < game.numSites; i++)
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
            currGameState.sites.Add(site);

            site.pos = game.sites[site.siteId].pos;
            
            if (gold == 0 /*&& prevGameState != null && prevGameState.sites[siteId].gold > 0*/)
            {
                game.minedOutSites_ids.Add(site.siteId);
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
            currGameState.units.Add(unit);
        }
    }
    
    public void ParseInputs_Begin()
    {
        game = new GameInfo();
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
            game.sites[siteId] = newSiteInfo;
        }
    }

//    public string InitGameInfo(string l1, string l2)
//    {
//        game = new GameInfo();
//        string[] inputs;
//        
//        int numSites = int.Parse(l1);
//        for (int i = 0; i < numSites; i++)
//        {
//            inputs = l2.Split(' ');
//            int siteId = int.Parse(inputs[0]);
//            int x = int.Parse(inputs[1]);
//            int y = int.Parse(inputs[2]);
//            int radius = int.Parse(inputs[3]);
//        
//            SiteInfo newSiteInfo = new SiteInfo(siteId, new Position(x,y), radius);
//            game.sites[siteId] = newSiteInfo;
//        }
//    }
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
	protected double[][] _influenceMap;
    protected bool[,] _obstacles;
	protected int width, height;

	private double minInfluence, maxInfluence;

	public int getWidth()
	{
		return width;
	}

	public int getHeight()
	{
		return height;
	}

	public DistanceFunc computeDistanceFunc;


	public double this[int x, int y] {
		get {
			return get(x, y);
		}
		set {
			_influenceMap[x][y] = value;
		}
	}

	public InfluenceMap()
	{
			
	}
	
	public InfluenceMap(int width, int height, double minInfluence, double maxInfluence, DistanceFunc computeDistanceFunc)
	{
		this.width = width;
		this.height = height;
		this._influenceMap = new double[width][];
		for (int i = 0; i < width; i++)
		{
			this._influenceMap[i] = new double[height];
		}
	    this._obstacles = new bool[width, height];
		this.computeDistanceFunc = computeDistanceFunc;
		this.minInfluence = minInfluence;
		this.maxInfluence = maxInfluence;
		myHashset = new HashSet<XAndY>();
	}

    public void AddObstacle(int x, int y)
    {
        _obstacles[x, y] = true;
    }

    public bool isObstacle(int x, int y)
    {
        return _obstacles[x, y];
    }
    
	private bool isInBounds(int x, int y)
	{
		return x >= 0 && x < width && y>=0 && y < height;
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
		var amount = _influenceMap[x][y];
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
				if (xNeighbour >= 0 && xNeighbour <= width - 1
				&& yNeighbour >= 0 && yNeighbour <= height - 1)
				{
					neighbours.Add(new XAndY( xNeighbour, yNeighbour ));
				}
			}
		}
		return neighbours;
	}


	//public int[][] getNeighbours(int x, int y)
	//{
	//	int noOfNeighbours = 8;
	//	if (x == 0 || x == width - 1)
	//	{
	//		noOfNeighbours -= 3;
	//	}
	//	if (y == 0 || y == width - 1)
	//	{
	//		if (noOfNeighbours < 8)
	//		{
	//			noOfNeighbours -= 2;
	//		}
	//		noOfNeighbours -= 3;
	//	}

	//	int currNeighbours = 0;
	//	int[][] neighbours = new int[noOfNeighbours][];
	//	for (int i = -1; i <= 1; i++)
	//	{
	//		for (int j = -1; j <= 1; j++)
	//		{
	//			if (i == 0 && j == 0) { continue; }

	//			int xNeighbour = x - i, yNeighbour = y - j;
	//			if (xNeighbour >= 0 && xNeighbour <= width - 1
	//			&& yNeighbour >= 0 && yNeighbour <= height - 1)
	//			{
	//				neighbours[currNeighbours] = new int[] { xNeighbour, yNeighbour };
	//				currNeighbours++;
	//			}
	//		}
	//	}
	//	return neighbours;
	//}

	private HashSet<XAndY> myHashset;

	public void applyInfluence(int x, int y, double amount, int fullDistance, int decayedDistance, double distanceDecay)
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

//	private int TrySetAmount(int x, int y, double amount)
//	{
//		try
//		{
//			_influenceMap[x][y] += amount;
//			return 1;
////			return true;
//		}
//		catch { return 0; } //out of map
//	}

	/// FullAmount Distance is the distance where the full amount will be applied. Then, reducedAmountDistance is the distance that will suffer from decay
	//public void applyInfluence(int x, int y, double amount, int fullAmountDistance, int reducedAmountDistance, double distanceDecay)
	//{
	//	List <XAndY> alreadyExplored = new List<XAndY>();
	//	applyInfluenceRecursive(x, y, amount, fullAmountDistance, reducedAmountDistance, distanceDecay, alreadyExplored);
	//}
	public void applyInfluenceRecursive(int x, int y, double amount, int fullAmountDistance, int reducedAmountDistance, double distanceDecay, List<XAndY> alreadyExplored)
	{
		if (fullAmountDistance < 0 && reducedAmountDistance < 0) { throw new Exception("Error"); }

		try
		{
			double foo = _influenceMap[x][y];
		}
		catch 
		{
			//End of map
			return;
		}


		//if (fullAmountDistance > 0)
		//{
		//	//System.err.println("x "+x+ " y "+y);
		//	_influenceMap[x][y] = amount;
		//	myHashset.Clear();
		//	myHashset.AddRange(getNeighbours(x, y));
		//	myHashset.ExceptWith(alreadyExplored);

		//	foreach (int[] neighbour in getNeighbours(x, y))
		//	{
		//		applyInfluenceRecursive(neighbour[0], neighbour[1], amount, fullAmountDistance - 1, reducedAmountDistance, distanceDecay);
		//	}
		//}

		//if (reducedAmountDistance > 0)
		//{
		//	_influenceMap[x][y] = amount;
		//	foreach (int[] neighbour in getNeighbours(x, y))
		//	{
		//		applyInfluenceRecursive(neighbour[0], neighbour[1], amount * distanceDecay, fullAmountDistance, reducedAmountDistance - 1, distanceDecay);
		//	}
		//}

		//System.err.println("x "+x+ " y "+y);


		if(alreadyExplored.Contains(new XAndY(x, y))==false)
		{
			_influenceMap[x][y] += amount;
			alreadyExplored.Add(new XAndY(x,y));
		}
		myHashset = new HashSet<XAndY>();
		foreach (var neighbour in getNeighbours(x,y))
		{
			myHashset.Add(neighbour);
		}
//		myHashset.AddRange(getNeighbours(x, y));
		myHashset.ExceptWith(alreadyExplored);
		

		foreach (XAndY neighbour in myHashset)
		{
			if (fullAmountDistance > 0)
			{
				applyInfluenceRecursive(neighbour.x, neighbour.y, amount, fullAmountDistance - 1, reducedAmountDistance, distanceDecay, alreadyExplored);

			}
			else if(reducedAmountDistance > 0)
			{
				applyInfluenceRecursive(neighbour.x, neighbour.y, amount * distanceDecay, fullAmountDistance, reducedAmountDistance - 1, distanceDecay, alreadyExplored);

			}
		}
	}

	public void applyInfluence(double amount, int fullAmountDistance, int reducedAmountDistance, double distanceDecay, params int[] points)
	{
		if (points.Length % 2 == 1)
		{
		    throw new Exception("invalid number of points args");
		}

		int noOfPoints = points.Length / 2;

		amount /= noOfPoints;

		for (int i = 0; i < noOfPoints; i++)
		{
			int pointX = points[(i * 2)];
			int pointY = points[(i * 2) + 1];

			applyInfluence(pointX, pointY, amount, fullAmountDistance, reducedAmountDistance, distanceDecay);
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
    public List<Position> useVisionLine(int y1, int x1, int y2, int x2)
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

}

/** Vector2_OB Class
 * 
 * Author: Oran Bar
 */
[Serializable]
public class Vector2_OB : IEquatable<Vector2_OB>
{
    #region Static Variables
    public static double COMPARISON_TOLERANCE = 0.0000001;

    private readonly static Vector2_OB zeroVector = new Vector2_OB(0);
    private readonly static Vector2_OB unitVector = new Vector2_OB(1);

    public static Vector2_OB Zero
    {
        get { return zeroVector; }
        private set { }
    }
    public static Vector2_OB One
    {
        get { return unitVector; }
        private set { }
    }
    #endregion

    public virtual double X { get; set; }
    public virtual double Y { get; set; }

    public Vector2_OB(double val)
    {
        this.X = val;
        this.Y = val;
    }

    public Vector2_OB(double x, double y)
    {
        this.X = x;
        this.Y = y;
    }

    public Vector2_OB(Vector2_OB v)
    {
        this.X = v.X;
        this.Y = v.Y;
    }

    #region Operators
    public static Vector2_OB operator +(Vector2_OB v1, Vector2_OB v2)
    {
        return new Vector2_OB(v1.X + v2.X, v1.Y + v2.Y);
    }

    public static Vector2_OB operator -(Vector2_OB v1, Vector2_OB v2)
    {
        return new Vector2_OB(v1.X - v2.X, v1.Y - v2.Y);
    }

    public static Vector2_OB operator *(Vector2_OB v1, double mult)
    {
        return new Vector2_OB(v1.X * mult, v1.Y * mult);
    }

    public static bool operator ==(Vector2_OB a, Vector2_OB b)
    {
        // If both are null, or both are same instance, return true.
        if (System.Object.ReferenceEquals(a, b))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }

        // Return true if the fields match:
        return a.Equals(b);
    }

    public static bool operator !=(Vector2_OB a, Vector2_OB b)
    {
        return (a == b) == false;
    }
    #endregion

    #region Object Class Overrides
    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        return Equals(obj as Vector2_OB);
    }

    public bool Equals(Vector2_OB other)
    {
        if ((object)other == null)
        {
            return false;
        }
        if (Math.Abs(X - other.X) > COMPARISON_TOLERANCE)
        {
            return false;
        }
        if (Math.Abs(Y - other.Y) > COMPARISON_TOLERANCE)
        {
            return false;
        }
        return true;

    }


    public override int GetHashCode()
    {
        unchecked
        {
            return 17 * X.GetHashCode() + 23 * Y.GetHashCode();
        }
    }


    public override string ToString()
    {
        return String.Format("[{0}, {1}] ", X, Y);
    }
    #endregion

    #region Vector2_OB Methods
    public static double Distance(Vector2_OB v1, Vector2_OB v2)
    {
        return Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2));
    }

    public static double DistanceSquared(Vector2_OB v1, Vector2_OB v2)
    {
        return Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2);
    }

    public double Distance(Vector2_OB other)
    {
        return Vector2_OB.Distance(this, other);
    }

    public double DistanceSquared(Vector2_OB other)
    {
        return Vector2_OB.DistanceSquared(this, other);
    }

	public Vector2_OB Closest(params Vector2_OB[] vectors) {
		return vectors.ToList().OrderBy( v1 => this.DistanceSquared(v1) ).First();
	}

	public double Length()
    {
        return Math.Sqrt(X * X + Y * Y);
    }

    public double LengthSquared()
    {
        return X * X + Y * Y;
    }

    public Vector2_OB Normalize()
    {
        double length = LengthSquared();
        return new Vector2_OB(X / length, Y / length);
    }

    public double Dot(Vector2_OB v)
    {
        return X * v.X + Y * v.Y;
    }

    public double Cross(Vector2_OB v)
    {
        return X * v.Y + Y * v.X;
    }

    public Vector2_OB Orthogonal()
    {
        return new Vector2_OB(-Y, X);
    }
    
    //TODO: test
    public double AngleTo(Vector2_OB v)
    {
        return this.Dot(v) / (this.Length() + v.Length());
    }

    public Vector2_OB ScalarProjectionOn(Vector2_OB v)
    {
        return v.Normalize() * this.Dot(v);
    }

	public double AngleInDegree() {
		return AngleInRadians() * (180.0 / Math.PI);
	}

	public double AngleInRadians() {
		return Math.Atan2(Y, X);
	}
    #endregion
}

