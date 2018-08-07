using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class Main : MonoBehaviour
{

    public InfluenceMapVisualizer visualizer;
 
    private string GAMEINFO_ENC =
        "24|0.1217.768.65.|1.703.232.65.|2.689.822.88.|3.1231.178.88.|4.155.385.65.|5.1765.615.65.|6.1677.389.87.|7.243.611.87.|8.456.178.84.|9.1464.822.84.|10.205.155.65.|11.1715.845.65.|12.150.850.60.|13.1770.150.60.|14.480.651.62.|15.1440.349.62.|16.1032.589.67.|17.888.411.67.|18.971.171.81.|19.949.829.81.|20.415.421.71.|21.1505.579.71.|22.1256.515.78.|23.664.485.78.|";

    private string GAMESTATE_ENC =
        "24|0.xxxxxx0.|1.266.3.1.0.336.333.0.|2.xxxxxx0.|3.xxxxxx0.|4.239.2.2.0.4.0.0.|5.xx0.1.xx0.|6.xx0.1.xx0.|7.232.1.0.0.1.x0.|8.236.1.1.0.256.297.0.|9.xx1.1.184.256.0.|10.4.3.0.0.3.x0.|11.xx0.1.xx0.|12.191.3.0.0.3.x0.|13.xxxxxx0.|14.211.3.1.0.412.367.0.|15.xx2.1.3.0.0.|16.xxxxxx0.|17.310.5.1.0.260.295.0.|18.xxxxxx0.|19.xxxxxx0.|20.241.3.1.0.384.356.0.|21.xx0.1.xx0.|22.xx1.1.336.336.0.|23.286.3.1.0.312.324.0.|3|1476.455.0.0.5.|250.750.0.x27.|1522.479.1.x24.|9|x";

    private string PULZELLAINFO_ENC = "1.1.0.0.6.x140.";

    private void Start()
    {
        RunTurn();
    }

    [Button]
    public void RunTurn()
    {
        LaPulzellaD_Orleans giovannaD_Arco = new LaPulzellaD_Orleans();
        giovannaD_Arco.Decode(PULZELLAINFO_ENC);
        giovannaD_Arco.game = new GameState();
        giovannaD_Arco.game.Decode(GAMESTATE_ENC);
        giovannaD_Arco.gameInfo = new GameInfo();
        giovannaD_Arco.gameInfo.Decode(GAMEINFO_ENC);

        foreach (var site in giovannaD_Arco.game.sites)
        {
            site.pos = giovannaD_Arco.gameInfo.sites[site.siteId].pos;
        }

        InfluenceMap map = new InfluenceMap();
        InfluenceMap buildMap = new InfluenceMap();
        TurnAction action = giovannaD_Arco.think(out map, out buildMap);
        
        Debug.Log("chosen action "+action.queenAction);
        
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
