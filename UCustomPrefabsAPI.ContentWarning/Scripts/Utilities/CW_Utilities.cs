using Photon.Pun;
using Steamworks;
namespace UCustomPrefabsAPI.ContentWarning
{
    public static class CW_Utilities
    {
        public static bool IsPlayer(Player player)
        {
            return PlayerHandler.instance.players.Contains(player);
        }
        public static bool TryGetPlayerWithSteamID(CSteamID steamID, out Player player)
        {
            player = null;
            if (PlayerHandler.instance)
                foreach (var punplayer in PhotonNetwork.PlayerList)
                {
                    if (!SteamAvatarHandler.TryGetSteamIDForPlayer(punplayer, out var PUNsteamID))
                        continue;
                    if (PUNsteamID == steamID)
                    {
                        foreach (var active_player in PlayerHandler.instance.players)
                        {
                            if (punplayer.ActorNumber == active_player.photonView.OwnerActorNr)
                            {
                                player = active_player;
                                return true;
                            }
                        }
                    }
                }
            return false;
        }
        public static bool TryGetSteamIDWithPlayer(Player player, out CSteamID steamID)
        {
            steamID = default;
            if (player)
                foreach (var punplayer in PhotonNetwork.PlayerList)
                {
                    if (punplayer.ActorNumber == player.photonView.OwnerActorNr)
                    {
                        return SteamAvatarHandler.TryGetSteamIDForPlayer(punplayer, out steamID);
                    }
                }
            return false;
        }
        public static bool TryGetPlayerWithActorNumber(int actorNumber, out Player player)
        {
            player = null;
            foreach (var activePlayer in PlayerHandler.instance.players)
            {
                if (activePlayer.photonView.OwnerActorNr != actorNumber)
                    continue;
                player = activePlayer;
                return true;
            }
            return false;
        }
    }
}
