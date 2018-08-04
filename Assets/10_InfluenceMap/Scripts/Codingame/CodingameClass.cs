#define RUNLOCAL

/** Code by Oran Bar **/

//The max characters that can be put into the error stream is 1028

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Schema;

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
            InfluenceMap map = new InfluenceMap();
            TurnAction move = giovannaD_Arco.think(out map);
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
    public UnitType creepsType;
    public bool isMinedOut;

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
        this.creepsType = (UnitType) param2;
    }
    
    public override string ToString()
    {
        return $"Site {siteId} - gold: {gold} - maxMineSize: {maxMineSize} - structureType: {structureType} - owner: {owner} - param1: {param1} - creepsType: {creepsType}";
    }

    public string Encode()
    {
        StringEncoderBuilder result = new StringEncoderBuilder(".");
        result.Append(siteId);
        result.Append(gold);
        result.Append(maxMineSize);
        result.Append((int) structureType);
        result.Append((int) owner);
        result.Append((param1));
        result.Append((int) creepsType);
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
        creepsType = (UnitType)(int.Parse(values[6]));
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
    public static int MAX_CONCURRENT_MINES = 3, MAX_BARRACKSES_KNIGHTS = 0, MAX_BARRACKSES_ARCER = 0, MAX_BARRACKSES_GIANT = 1, MAX_TOWERS = 4;
    public static int GIANT_COST = 140, KNIGHT_COST = 80, ARCHER_COST = 100;
    public static int ENEMY_CHECK_RANGE = 200, TOO_MANY_UNITS_NEARBY = 2;
    public static int INFLUENCEMAP_SQUARELENGTH = 10;
    
    public GameInfo game;

    public GameState currGameState;
    public GameState prevGameState = null;

    private bool flag = true;

    
    public TurnAction think(out InfluenceMap turnInfluenceMap)
    {
        TurnAction chosenMove = new TurnAction();
        Unit myQueen = currGameState.MyQueen;

        turnInfluenceMap = CreateInfluenceMap();
        
        Site touchedSite = null;
        if (currGameState.touchedSiteId != -1)
        {
            touchedSite = currGameState.sites[currGameState.touchedSiteId];
        }
        
        IEnumerable<Site> mySites = currGameState.sites.Where(s => s.owner == Owner.Friendly);

        
        // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
        Site closestUnbuiltSite = SortSites_ByDistance(myQueen.pos, currGameState.sites)
            .Where(s => s.owner == Owner.Neutral 
                        && currGameState.units.Where(u => u.owner == Owner.Enemy).Count(u => u.DistanceTo(s) < ENEMY_CHECK_RANGE) < TOO_MANY_UNITS_NEARBY)
            .FirstOrDefault();

        Site targetMoveSite = closestUnbuiltSite;
        
        List<Site> closestUnbuiltMines = SortSites_ByDistance(myQueen.pos, currGameState.sites)
            .Where(s => s.structureType == StructureType.None && s.owner == Owner.Neutral && IsSiteMinedOut(s.siteId) == false)
            .ToList();
        
        int owned_knight_barrackses = mySites.Count(ob => ob.structureType == StructureType.Barracks && ob.creepsType == UnitType.Knight);
        int owned_archer_barrackses = mySites.Count(ob => ob.structureType == StructureType.Barracks && ob.creepsType == UnitType.Archer);
        int owned_giant_barrackses = mySites.Count(ob => ob.structureType == StructureType.Barracks && ob.creepsType == UnitType.Giant);
        int owned_mines = mySites.Count(ob => ob.structureType == StructureType.Mine && ob.gold > 0);
        int owned_towers = mySites.Count(ob => ob.structureType == StructureType.Tower && ob.owner == Owner.Friendly);
        
        int total_owner_barrackses = owned_archer_barrackses + owned_giant_barrackses + owned_knight_barrackses;
        
        bool touchingNeutralSite = touchedSite != null && touchedSite.owner == Owner.Neutral;
        bool touchingMyMine = touchedSite != null && touchedSite.owner == Owner.Friendly &&
                              touchedSite.structureType == StructureType.Mine;
        
        bool touchingMyTower = touchedSite != null && touchedSite.owner == Owner.Friendly &&
                               touchedSite.structureType == StructureType.Tower;
        
        bool prev_touchingMyMine = touchedSite != null && touchedSite.owner == Owner.Friendly &&
                                   touchedSite.structureType == StructureType.Mine;

        //If we are touching a site, we do something with it
        if (touchingNeutralSite)
        {
            //Build
            var chosenBuildMove = ChoseBuildMove(closestUnbuiltMines, touchedSite, owned_mines, owned_knight_barrackses, owned_archer_barrackses, owned_giant_barrackses, owned_towers, myQueen);
            chosenMove.queenAction = chosenBuildMove;
        }
        else if (touchingMyMine && IsMineMaxed(touchedSite) == false)
        {
            //Empower Mine
            chosenMove.queenAction = new BuildMine(currGameState.touchedSiteId);
        }
        else if (touchingMyTower && touchedSite.param1 <= 700)
        {
            //Emppower Tower
            chosenMove.queenAction = new BuildTower(currGameState.touchedSiteId);
        }
        
        if(chosenMove.queenAction is Wait)
        {
            if (closestUnbuiltMines.FirstOrDefault() != null && owned_mines < MAX_CONCURRENT_MINES)
            {
                //Go To Next Mine (Tries to filter our the mined out sites.
                chosenMove.queenAction = new Move(GetSiteInfo(closestUnbuiltMines.First()).pos);
            }
            else if (total_owner_barrackses < MAX_BARRACKSES_KNIGHTS + MAX_BARRACKSES_ARCER + MAX_BARRACKSES_GIANT || owned_towers < MAX_TOWERS)
            {
                //Go to next closest site
                chosenMove.queenAction = new Move(GetSiteInfo(closestUnbuiltSite).pos);
                //Run to angle if close to enemies. Running takes priority, so we do the computations last
            }
            else
            {
                if (flag)
                {
                    flag = false;
//                    MAX_CONCURRENT_MINES++;
//                    MAX_BARRACKSES_ARCER++;
                    MAX_BARRACKSES_KNIGHTS++;
//                    MAX_BARRACKSES_KNIGHTS++;
//                    MAX_BARRACKSES_GIANT++;
                    MAX_TOWERS++;
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

        IEnumerable<Site> myIdleBarracses = mySites
            .Where(site => site.structureType == StructureType.Barracks && site.param1 == 0);
        List<Site> barracksesToTrainFrom = new List<Site>();
        
        if (myIdleBarracses.Any())
        {
            //Train

            DecideUnitsToTrain();
            
            int remainingGold = currGameState.money;
            foreach (var barraks in myIdleBarracses)
            {
                int cost = 0;
                switch (barraks.creepsType)
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

    private InfluenceMap CreateInfluenceMap()
    {
//        double squareLength = (60 * 0.4);
        double squareLength = INFLUENCEMAP_SQUARELENGTH; //Maximum common divisor between 60, 100, 75, 50 (movement speeds)

        int mapWidth = (int) Math.Ceiling(1980 / squareLength);
        int mapHeight = (int) Math.Ceiling(1020 / squareLength);
        double minInfluence = -100;
        double maxInfluence = 100;
        InfluenceMap map = new InfluenceMap(mapWidth, mapHeight, minInfluence, maxInfluence, new EuclideanDistanceSqr());

        var enemyUnits = currGameState.units
            .Where(u => u.owner == Owner.Enemy);

        //Enemy units influence
        foreach (var enemy in enemyUnits)
        {
            Position enemyPos = enemy.pos;
            double enemyInfluence = GetEnemyInfluence(enemy);
            map.applyInfluence(enemyPos.x, enemyPos.y, enemyInfluence, GetEnemyInfluenceRadius(enemy), 0, 0);
        }

        return map;

    }

    private double GetEnemyInfluence(Unit enemy)
    {
        switch (enemy.unitType)
        {
            case UnitType.Queen:
                return 0;
            case UnitType.Knight:
                return 5;
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
        
        Func<double, double, double> archersEvaluation = (archers, enemyUnits) => archers * 3.0 - enemyUnits;
        Func<double, double, double> giantsEvaluation = (giants, enemyTowers) => giants * 3.0 - enemyTowers;
        Func<double, double, double, double> knightsEvaluation = (knights, enemyArchers, enemyTowers) => ((knights / 4)- enemyArchers)/2 + (knights - enemyTowers)/2;

        List<Site> sitesThatCanTrainGiants = gameState.sites.Where(s =>
            s.owner == Owner.Friendly && s.structureType == StructureType.Barracks && s.creepsType == UnitType.Giant)
            .ToList();
        
        List<Site> sitesThatCanTrainKnights = gameState.sites.Where(s =>
                s.owner == Owner.Friendly && s.structureType == StructureType.Barracks && s.creepsType == UnitType.Knight)
            .ToList();

        List<Site> sitesThatCanTrainArchers = gameState.sites.Where(s =>
                s.owner == Owner.Friendly && s.structureType == StructureType.Barracks && s.creepsType == UnitType.Archer)
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

        if (owned_mines < MAX_CONCURRENT_MINES && currGameState.touchedSiteId == closestUnbuiltMines.First().siteId
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

    public List<Site> SortSites_ByDistance(Position startPosition, List<Site> siteList)
    {
        IOrderedEnumerable<Site> sortedSitesStream = 
            siteList
                .OrderBy(s => startPosition.DistanceSqr(GetSiteInfo(s.siteId).pos));

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