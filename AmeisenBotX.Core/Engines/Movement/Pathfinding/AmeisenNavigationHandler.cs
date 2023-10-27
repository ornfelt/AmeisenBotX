using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Pathfinding.Enums;
using AnTCP.Client;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AmeisenBotX.Core.Engines.Movement.Pathfinding
{
    /// Represents a navigation handler for Ameisen Pathfinding.
    /// Implements the IPathfindingHandler interface.
    /// </summary>
    public class AmeisenNavigationHandler : IPathfindingHandler
    {
        /// <summary>
        /// Initializes a new instance of the AmeisenNavigationHandler class with the specified IP address and port.
        /// </summary>
        public AmeisenNavigationHandler(string ip, int port)
        {
            Client = new(ip, port);
            ConnectionWatchdog = new(ObserveConnection);
            ConnectionWatchdog.Start();
        }

        /// <summary>
        /// Gets the private AnTcpClient object used for communication.
        /// </summary>
        private AnTcpClient Client { get; }

        /// <summary>
        /// Gets or sets the connection watchdog Thread object. 
        /// </summary>
        private Thread ConnectionWatchdog { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the program should exit.
        /// </summary>
        private bool ShouldExit { get; set; }

        /// <summary>
        /// Retrieves a path from the current client's location to a specified target location on a specified map.
        /// </summary>
        /// <param name="mapId">The ID of the map on which the path is requested.</param>
        /// <param name="origin">The starting location of the path.</param>
        /// <param name="target">The target location to reach.</param>
        /// <returns>An IEnumerable of Vector3 objects representing the path from the origin to the target. If the client is not connected, an empty array is returned.</returns>
        public IEnumerable<Vector3> GetPath(int mapId, Vector3 origin, Vector3 target)
        {
            try
            {
                return Client.IsConnected ? Client.Send((byte)EMessageType.PATH, (mapId, origin, target, 2)).AsArray<Vector3>() : Array.Empty<Vector3>();
            }
            catch
            {
                return Array.Empty<Vector3>();
            }
        }

        /// <summary>
        /// This method returns a random point on the map.
        /// If the client is connected, it sends a message of type RANDOM_POINT and returns the received data as a Vector3.
        /// If the client is not connected, it returns Vector3.Zero.
        /// If an exception occurs, it returns Vector3.Zero.
        /// </summary>
        public Vector3 GetRandomPoint(int mapId)
        {
            try
            {
                return Client.IsConnected ? Client.Send((byte)EMessageType.RANDOM_POINT, mapId).As<Vector3>() : Vector3.Zero;
            }
            catch
            {
                return Vector3.Zero;
            }
        }

        /// <summary>
        /// Gets a random point around the specified origin within a maximum radius on the specified map.
        /// </summary>
        /// <param name="mapId">The ID of the map.</param>
        /// <param name="origin">The origin point.</param>
        /// <param name="maxRadius">The maximum radius.</param>
        /// <returns>A Vector3 representing the random point around the origin, or Vector3.Zero if an exception occurs.</returns>
        public Vector3 GetRandomPointAround(int mapId, Vector3 origin, float maxRadius)
        {
            try
            {
                return Client.IsConnected ? Client.Send((byte)EMessageType.RANDOM_POINT_AROUND, (mapId, origin, maxRadius)).As<Vector3>() : Vector3.Zero;
            }
            catch
            {
                return Vector3.Zero;
            }
        }

        ///<summary>
        ///Moves an object along the surface of a map from its origin to a target location.
        ///</summary>
        public Vector3 MoveAlongSurface(int mapId, Vector3 origin, Vector3 target)
        {
            try
            {
                return Client.IsConnected ? Client.Send((byte)EMessageType.MOVE_ALONG_SURFACE, (mapId, origin, target)).As<Vector3>() : Vector3.Zero;
            }
            catch
            {
                return Vector3.Zero;
            }
        }

        /// <summary>
        /// Stops the process by setting the ShouldExit property to true and joining the ConnectionWatchdog thread.
        /// </summary>
        public void Stop()
        {
            ShouldExit = true;
            ConnectionWatchdog.Join();
        }

        /// <summary>
        /// Periodically checks if the client is connected and attempts to connect if not.
        /// </summary>
        private void ObserveConnection()
        {
            while (!ShouldExit)
            {
                if (!Client.IsConnected)
                {
                    try
                    {
                        Client.Connect();
                    }
                    catch
                    {
                        // ignored, will happen when we cant connect
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}