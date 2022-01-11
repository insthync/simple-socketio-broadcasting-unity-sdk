using SocketIOClient;
using System;
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
                if (!instance)
                    new GameObject("_SocketIOBroadcastingManager").AddComponent<SocketIOBroadcastingManager>();
                return instance;
            }
        }

        public string serviceAddress = "http://localhost:8212";
        public event Action<SocketIOResponse> onMsg;
        private SocketIO client;

        private void Awake()
        {
            if (instance)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private async void OnDestroy()
        {
            await Disconnect();
        }

        public async Task Connect()
        {
            await Disconnect();
            client = new SocketIO(serviceAddress);
            client.On("msg", OnMsg);
            await client.ConnectAsync();
        }

        public async Task Disconnect()
        {
            if (client != null && client.Connected)
                await client.DisconnectAsync();
            client = null;
        }

        private void OnMsg(SocketIOResponse response)
        {
            if (onMsg != null)
                onMsg.Invoke(response);
        }

        public async Task BroadcastAll(object data)
        {
            await client.EmitAsync("all", data);
        }

        public async Task BroadcastOther(object data)
        {
            await client.EmitAsync("other", data);
        }
    }
}
