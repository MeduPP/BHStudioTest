using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNetManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        Player.ResetPlayerNumbers();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        Player.ResetPlayerNumbers();
    }
}
