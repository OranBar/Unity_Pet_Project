using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class Main : MonoBehaviour
{

    public InfluenceMapVisualizer visualizer;
 
    private string GAMEINFO_ENC =
        "24|0.666.848.62.|1.1254.152.62.|2.601.416.71.|3.1319.584.71.|4.1231.825.85.|5.689.175.85.|6.1682.820.90.|7.238.180.90.|8.962.823.87.|9.958.177.87.|10.1126.408.88.|11.794.592.88.|12.1549.614.63.|13.371.386.63.|14.1373.353.67.|15.547.647.67.|16.1505.155.65.|17.415.845.65.|18.152.407.62.|19.1768.593.62.|20.1642.380.85.|21.278.620.85.|22.155.845.65.|23.1765.155.65.|";

    private string GAMESTATE_ENC =
        "24|0.xxxxxx0.|1.xxxxxx0.|2.253.2.1.0.352.342.0.|3.253.2.1.1.100.192.0.|4.xxxxxx0.|5.109.4.0.0.4.x0.|6.xx2.1.0.0.0.|7.95.1.0.0.1.x0.|8.xxxxxx0.|9.244.4.0.0.4.x0.|10.319.3.0.0.3.x0.|11.187.3.0.0.3.x0.|12.xx1.1.72.163.0.|13.242.3.1.0.368.348.0.|14.297.4.xxxx0.|15.297.4.1.0.220.272.0.|16.xxxxxx0.|17.201.1.1.0.188.253.0.|18.249.3.2.0.4.0.0.|19.xx1.1.56.147.0.|20.xxxxxx0.|21.200.3.1.0.92.191.0.|22.231.3.1.0.160.234.0.|23.xxxxxx0.|10|1010.425.0.0.16.|1034.498.0.0.16.|1082.508.0.0.16.|981.493.0.0.16.|290.362.0.0.24.|246.491.0.0.24.|324.456.0.0.24.|294.419.0.0.24.|1133.527.0.x88.|1516.505.1.x57.|18|10|";

    private string PULZELLAINFO_ENC = "1.1.0.0.6.x230.";

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
        
        visualizer.InflMap = buildMap;
        
        visualizer.UpdateInfluenceColor();

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
