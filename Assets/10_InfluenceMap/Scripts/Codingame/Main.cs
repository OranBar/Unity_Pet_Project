using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class Main : MonoBehaviour
{

    public InfluenceMapVisualizer visualizer;
    
	 private string GAMESTATE_ENC =
            "24|0.xx0.1.xx0.|1.244.1.1.0.656.465.0.|2.xx0.1.xx0.|3.xxxxxx0.|4.xx0.1.xx0.|5.174.1.0.0.1.x0.|6.288.2.1.0.736.489.0.|7.xx0.1.xx0.|8.xxxxxx0.|9.xx0.1.xx0.|10.222.2.2.0.9.2.0.|11.xxxxxx0.|12.xxxxxx0.|13.xx2.1.0.0.0.|14.294.4.1.0.696.474.0.|15.xx0.1.xx0.|16.xxxxxx0.|17.227.2.1.0.372.349.0.|18.xx0.1.xx0.|19.137.2.0.0.2.x0.|20.xx0.1.xx0.|21.xxxxxx0.|22.182.1.0.0.1.x0.|23.xx0.1.xx0.|11|282.796.0.x18.|332.799.1.0.2.|666.544.1.0.16.|582.646.1.0.12.|641.611.1.0.20.|547.626.1.0.12.|1086.282.1.0.25.|1015.434.1.0.25.|1076.471.1.0.25.|998.396.1.0.25.|587.315.1.x40.|133|10|";
    
    private string GAMEINFO_ENC = 
        "24|0.1575.391.89.|1.345.609.89.|2.879.164.73.|3.1041.836.73.|4.1726.845.65.|5.194.155.65.|6.614.731.70.|7.1306.269.70.|8.802.849.61.|9.1118.151.61.|10.172.824.82.|11.1748.176.82.|12.828.623.74.|13.1092.377.74.|14.592.506.61.|15.1328.494.61.|16.1497.150.60.|17.423.850.60.|18.1751.591.79.|19.169.409.79.|20.616.166.76.|21.1304.834.76.|22.422.338.86.|23.1498.662.86.|";


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
        giovannaD_Arco.think(out map);

        visualizer.InflMap = map;
        visualizer.UpdateInfluenceColor();
    }
    
    public void RunTurn(string gameState_Enc, string gameInfo_Enc)
    {
        LaPulzellaD_Orleans giovannaD_Arco = new LaPulzellaD_Orleans();
        giovannaD_Arco.currGameState = new GameState();
        giovannaD_Arco.currGameState.Decode(gameState_Enc);
        giovannaD_Arco.game = new GameInfo();
        giovannaD_Arco.game.Decode(gameInfo_Enc);

        foreach (var site in giovannaD_Arco.currGameState.sites)
        {
            site.pos = giovannaD_Arco.game.sites[site.siteId].pos;
        }
        
        InfluenceMap map = new InfluenceMap();
        giovannaD_Arco.think(out map);
    }
    
    
}
