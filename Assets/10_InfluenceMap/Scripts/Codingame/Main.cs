using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class Main : MonoBehaviour
{

    public InfluenceMapVisualizer visualizer;
    
    //Turn 84
    private string GAMESTATE_ENC =
        "24|0.xxxxxx0.|1.xxxxxx0.|2.xxxxxx0.|3.xxxxxx0.|4.xx0.1.xx0.|5.226.1.2.0.4.0.0.|6.xxxxxx0.|7.xxxxxx0.|8.xxxxxx0.|9.xxxxxx0.|10.xxxxxx0.|11.214.1.xxxx0.|12.xxxxxx0.|13.xxxxxx0.|14.xxxxxx0.|15.xxxxxx0.|16.xxxxxx0.|17.xxxxxx0.|18.214.3.xxxx0.|19.xxxxxx0.|20.236.1.xxxx0.|21.xxxxxx0.|22.xxxxxx0.|23.xxxxxx0.|2|220.298.0.x70.|1687.705.1.x70.|20|x";
    
    //Turn 94
//	 private string GAMESTATE_ENC =
//            "24|0.xx0.1.xx0.|1.244.1.1.0.656.465.0.|2.xx0.1.xx0.|3.xxxxxx0.|4.xx0.1.xx0.|5.174.1.0.0.1.x0.|6.288.2.1.0.736.489.0.|7.xx0.1.xx0.|8.xxxxxx0.|9.xx0.1.xx0.|10.222.2.2.0.9.2.0.|11.xxxxxx0.|12.xxxxxx0.|13.xx2.1.0.0.0.|14.294.4.1.0.696.474.0.|15.xx0.1.xx0.|16.xxxxxx0.|17.227.2.1.0.372.349.0.|18.xx0.1.xx0.|19.137.2.0.0.2.x0.|20.xx0.1.xx0.|21.xxxxxx0.|22.182.1.0.0.1.x0.|23.xx0.1.xx0.|11|282.796.0.x18.|332.799.1.0.2.|666.544.1.0.16.|582.646.1.0.12.|641.611.1.0.20.|547.626.1.0.12.|1086.282.1.0.25.|1015.434.1.0.25.|1076.471.1.0.25.|998.396.1.0.25.|587.315.1.x40.|133|10|";

    private string GAMEINFO_ENC =
        "24|0.679.166.76.|1.1241.834.76.|2.566.646.72.|3.1354.354.72.|4.1754.834.76.|5.166.166.76.|6.944.178.84.|7.976.822.84.|8.687.413.78.|9.1233.587.78.|10.1497.551.63.|11.423.449.63.|12.1536.175.85.|13.384.825.85.|14.872.591.76.|15.1048.409.76.|16.1748.329.82.|17.172.671.82.|18.422.206.81.|19.1498.794.81.|20.179.413.80.|21.1741.587.80.|22.1202.162.72.|23.718.838.72.|";


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
