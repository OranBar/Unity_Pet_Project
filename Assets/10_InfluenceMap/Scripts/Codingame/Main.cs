﻿using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class Main : MonoBehaviour
{

    public InfluenceMapVisualizer visualizer;
    
	 private string GAMESTATE_ENC =
            "24|0.xx0.1.xx0.|1.244.1.1.0.684.475.0.|2.xx0.1.xx0.|3.xxxxxx0.|4.xx0.1.xx0.|5.181.1.0.0.1.x0.|6.288.2.1.0.764.498.0.|7.xx0.1.xx0.|8.xxxxxx0.|9.xx0.1.xx0.|10.222.2.xxxx0.|11.xxxxxx0.|12.xxxxxx0.|13.xx2.1.2.0.0.|14.294.4.1.0.724.483.0.|15.xx0.1.xx0.|16.xxxxxx0.|17.227.2.xxxx0.|18.xx0.1.xx0.|19.151.2.0.0.2.x0.|20.xxxxxx0.|21.xxxxxx0.|22.189.1.0.0.1.x0.|23.xx0.1.xx0.|8|413.760.0.x27.|467.765.1.0.13.|453.726.1.0.9.|844.448.1.0.22.|772.548.1.0.22.|834.528.1.0.22.|731.545.1.0.18.|828.293.1.x40.|245|17|";
    
    private string GAMEINFO_ENC = 
        "24|0.1575.391.89.|1.345.609.89.|2.879.164.73.|3.1041.836.73.|4.1726.845.65.|5.194.155.65.|6.614.731.70.|7.1306.269.70.|8.802.849.61.|9.1118.151.61.|10.172.824.82.|11.1748.176.82.|12.828.623.74.|13.1092.377.74.|14.592.506.61.|15.1328.494.61.|16.1497.150.60.|17.423.850.60.|18.1751.591.79.|19.169.409.79.|20.616.166.76.|21.1304.834.76.|22.422.338.86.|23.1498.662.86.|";

        
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
