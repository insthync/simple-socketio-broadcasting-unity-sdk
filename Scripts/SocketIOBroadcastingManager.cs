using Cysharp.Threading.Tasks;
using SocketIOClient;
using System;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace SimpleSocketIOBroadcastingSDK
{
    public class SocketIOBroadcastingManager : MonoBehaviour
    {
        private static SocketIOBroadcastingManager instance;
        public static SocketIOBroadcastingManager Instance
        {
            get
            {
                return instance;
            }
        }

        public string serviceAddress = "http://localhost:8212";
        public event Action<SocketIOResponse> onMsg;
        private SocketIO client;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[" + nameof(SocketIOBroadcastingManager) + "] Awaken");
        }

        private async void OnDestroy()
        {
            await Disconnect();
        }

        public async Task Connect()
        {
            await Disconnect();
            await UniTask.SwitchToMainThread();
            Debug.Log("[" + nameof(SocketIOBroadcastingManager) + "] Connecting to " + serviceAddress);
            client = new SocketIO(serviceAddress, new SocketIOOptions()
            {
                Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
            });
            client.On("msg", OnMsg);
            // Always accept SSL
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, policyErrors) => true;
            await client.ConnectAsync();
            await UniTask.SwitchToMainThread();
            Debug.Log("[" + nameof(SocketIOBroadcastingManager) + "] Connected to " + serviceAddress);
        }

        public async Task Disconnect()
        {
            Debug.Log("[" + nameof(SocketIOBroadcastingManager) + "] Disconnecting");
            if (client != null && client.Connected)
                await client.DisconnectAsync();
            await UniTask.SwitchToMainThread();
            Debug.Log("[" + nameof(SocketIOBroadcastingManager) + "] Disconnected");
            client = null;
        }

        private async void OnMsg(SocketIOResponse response)
        {
            await UniTask.SwitchToMainThread();
            if (onMsg != null)
                onMsg.Invoke(response);
        }

        public async Task BroadcastAll(object data)
        {
            await client.EmitAsync("all", data);
            await UniTask.SwitchToMainThread();
        }

        public async Task BroadcastOther(object data)
        {
            await client.EmitAsync("other", data);
            await UniTask.SwitchToMainThread();
        }
    }
}
