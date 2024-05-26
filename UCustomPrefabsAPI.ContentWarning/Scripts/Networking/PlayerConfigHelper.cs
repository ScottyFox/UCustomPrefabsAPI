using ExitGames.Client.Photon;
using Photon.Pun;
namespace UCustomPrefabsAPI.ContentWarning.Networking
{
    public class PlayerConfigHelper : MonoBehaviourPunCallbacks
    {
        public const string PlayerDataPrefix = "ucp_pd";
        private string currentConfig = string.Empty;
        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            UpdateTemplates(targetPlayer);
        }
        public void UpdatePlayerData()
        {
            if (photonView.IsMine)
            {
                Hashtable hash = new Hashtable
                {
                    { PlayerDataPrefix, Plugin.localPlayerConfig.Value }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
            UpdateTemplates(photonView.Owner);
        }
        public static string GetPlayerData()
        {
            return Plugin.localPlayerConfig.Value;
        }
        public static void SetPlayerData(string data)
        {
            Plugin.localPlayerConfig.Value = data;
        }
        public void UpdateTemplates(Photon.Realtime.Player targetPlayer)
        {
            if (targetPlayer != photonView.Owner)
                return;
            if (!targetPlayer.CustomProperties.TryGetValue(PlayerDataPrefix, out var data))
                return;
            var tokens = (string)data;
            if (tokens == currentConfig)
                return;
            if (string.IsNullOrWhiteSpace(tokens))
                return;
            if (!transform.GetComponent<Player>())
                return;
            InstanceManager.RemoveInstancesFromTarget(transform);
            foreach (var token in tokens.Split(','))
                InstanceManager.Register($"{targetPlayer.ActorNumber}:{token}", token, transform);
            currentConfig = tokens;
        }
        public void Start()
        {
            UpdatePlayerData();
        }
    }
}
