using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class Main : MonoBehaviour
{

    public InfluenceMapVisualizer visualizer;
    
    //Turn 84
    private string GAMESTATE_ENC =
        "24|0.xxxxxx0.|1.xx2.1.1.0.0.|2.206.1.0.0.1.x0.|3.xxxxxx0.|4.xx0.1.xx0.|5.221.2.1.0.760.497.0.|6.xxxxxx0.|7.xxxxxx0.|8.xxxxxx0.|9.xxxxxx0.|10.165.2.0.0.2.x0.|11.xxxxxx0.|12.xx0.1.xx0.|13.xxxxxx0.|14.246.1.1.0.124.208.0.|15.xxxxxx0.|16.xx0.1.xx0.|17.xxxxxx0.|18.210.1.xxxx0.|19.xx1.1.268.304.0.|20.246.1.2.0.0.0.0.|21.xx0.1.xx0.|22.xxxxxx0.|23.xx1.1.292.317.0.|10|442.322.0.0.23.|313.438.0.0.23.|406.332.0.0.23.|296.346.0.0.23.|325.388.0.x45.|739.368.1.0.21.|625.467.1.0.21.|694.385.1.0.12.|733.507.1.0.21.|891.734.1.x43.|23|18|";
    
    //Turn 94
//	 private string GAMESTATE_ENC =
//            "24|0.xx0.1.xx0.|1.244.1.1.0.656.465.0.|2.xx0.1.xx0.|3.xxxxxx0.|4.xx0.1.xx0.|5.174.1.0.0.1.x0.|6.288.2.1.0.736.489.0.|7.xx0.1.xx0.|8.xxxxxx0.|9.xx0.1.xx0.|10.222.2.2.0.9.2.0.|11.xxxxxx0.|12.xxxxxx0.|13.xx2.1.0.0.0.|14.294.4.1.0.696.474.0.|15.xx0.1.xx0.|16.xxxxxx0.|17.227.2.1.0.372.349.0.|18.xx0.1.xx0.|19.137.2.0.0.2.x0.|20.xx0.1.xx0.|21.xxxxxx0.|22.182.1.0.0.1.x0.|23.xx0.1.xx0.|11|282.796.0.x18.|332.799.1.0.2.|666.544.1.0.16.|582.646.1.0.12.|641.611.1.0.20.|547.626.1.0.12.|1086.282.1.0.25.|1015.434.1.0.25.|1076.471.1.0.25.|998.396.1.0.25.|587.315.1.x40.|133|10|";

    private string GAMEINFO_ENC =
        "24|0.849.450.66.|1.1071.550.66.|2.170.497.80.|3.1750.503.80.|4.1490.833.77.|5.430.167.77.|6.1392.168.78.|7.528.832.78.|8.632.589.76.|9.1288.411.76.|10.169.826.79.|11.1751.174.79.|12.1057.837.73.|13.863.163.73.|14.364.666.64.|15.1556.334.64.|16.1267.701.74.|17.653.299.74.|18.431.435.85.|19.1489.565.85.|20.178.223.85.|21.1742.777.85.|22.1123.191.89.|23.797.809.89.|";


    private void Start()
    {
        RunTurn();
    }

    [Button]
    public void RunTurn()
    {
        LaPulzellaD_Orleans giovannaD_Arco = new LaPulzellaD_Orleans();
        giovannaD_Arco.currGameState = new GameState();
        giovannaD_Arco.currGameState.Decode(GAMESTATE_ENC);
        giovannaD_Arco.game = new GameInfo();
        giovannaD_Arco.game.Decode(GAMEINFO_ENC);

        foreach (var site in giovannaD_Arco.currGameState.sites)
        {
            site.pos = giovannaD_Arco.game.sites[site.siteId].pos;
        }

        InfluenceMap map = new InfluenceMap();
        InfluenceMap buildMap = new InfluenceMap();
        giovannaD_Arco.think(out map, out buildMap);

        visualizer.InflMap = map;
        
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
