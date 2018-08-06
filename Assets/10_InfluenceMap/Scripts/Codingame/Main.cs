using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class Main : MonoBehaviour
{

    public InfluenceMapVisualizer visualizer;
 
    private string GAMEINFO_ENC =
        "24|0.394.151.61.|1.1526.849.61.|2.1035.826.84.|3.885.174.84.|4.1036.385.84.|5.884.615.84.|6.464.589.84.|7.1456.411.84.|8.1591.638.69.|9.329.362.69.|10.180.587.90.|11.1740.413.90.|12.1760.162.70.|13.160.838.70.|14.658.758.70.|15.1262.242.70.|16.696.424.87.|17.1224.576.87.|18.1515.165.75.|19.405.835.75.|20.626.171.81.|21.1294.829.81.|22.1763.839.67.|23.157.161.67.|";

    private string GAMESTATE_ENC =
        "24|0.202.2.1.0.424.372.0.|1.xx0.1.xx0.|2.xx1.1.196.263.0.|3.xxxxxx0.|4.xxxxxx0.|5.xxxxxx0.|6.xxxxxx0.|7.xxxxxx0.|8.xx0.1.xx0.|9.212.1.2.0.0.0.0.|10.xxxxxx0.|11.xxxxxx0.|12.xxxxxx0.|13.xxxxxx0.|14.xxxxxx0.|15.xxxxxx0.|16.252.3.1.0.484.402.0.|17.xx1.1.372.354.0.|18.xxxxxx0.|19.xxxxxx0.|20.271.3.1.0.460.391.0.|21.xx1.1.344.340.0.|22.xx0.1.xx0.|23.184.1.0.0.1.x0.|3|1126.742.0.0.4.|627.328.0.x95.|1147.790.1.x85.|44|16|";

    private string PULZELLAINFO_ENC = "1.1.0.0.6.x96.";

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
