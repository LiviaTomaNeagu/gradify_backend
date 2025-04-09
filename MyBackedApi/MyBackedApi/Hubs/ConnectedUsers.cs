using System.Collections.Concurrent;

namespace MyBackedApi.Hubs
{

    public static class ConnectedUsers
    {
        public static ConcurrentDictionary<string, string> UserConnections { get; } = new();
    }

}
