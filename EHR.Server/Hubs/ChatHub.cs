using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace EHR.Server.Hubs
{
    public class ChatHub : Hub
    {
        private static RoomManager roomManager = new RoomManager();

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string mrn = roomManager.FindRoomByConnectionId(Context.ConnectionId).MRN;
            Clients.Group(mrn).SendAsync("Left", Context.ConnectionId);
            roomManager.RemoveFromAnyRoom(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        //public async Task CreateRoom(string mrn)
        //{
        //    RoomInfo roomInfo = roomManager.CreateRoom(mrn);
        //    if (roomInfo != null)
        //    {
        //        await Groups.AddToGroupAsync(Context.ConnectionId, roomInfo.MRN);
        //        await Clients.Caller.SendAsync("Created", roomInfo.MRN);
        //    }
        //    else
        //    {
        //        await Clients.Caller.SendAsync("Error", "error occurred when creating a new room.");
        //    }
        //}

        public async Task Join(string MRN, string username, string uri)
        {
            if(roomManager.AddToRoom(MRN, Context.ConnectionId, username, uri))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, MRN);
                await Clients.Caller.SendAsync("Joined", 
                    roomManager.FindRoom(MRN).Participants.Select(t => t.Item1).ToArray(),
                    roomManager.FindRoom(MRN).Participants.Select(t => t.Item2).ToArray(),
                    roomManager.FindRoom(MRN).Participants.Select(t => t.Item3).ToArray());
                await Clients.Group(MRN).SendAsync("Ready", Context.ConnectionId, username, uri);
            }
            else
            {
                await Clients.Caller.SendAsync("Error", "error occurred when joining the room.");
            }
            
        }

        public async Task Leave(string MRN, string ConnId)
        {
            roomManager.RemoveFromRoom(MRN, ConnId);
            await Clients.Group(MRN).SendAsync("Left", ConnId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, MRN);
        }

        public async Task SendMessage(string MRN, object message)//This is a fallback method, it has not been implemented on clients
        {
            await Clients.OthersInGroup(MRN).SendAsync("ServerMessage", message);
        }

        public async Task SendOneMessage(string MRN, object message, string partnerConnId)
        {
            await Clients.Client(partnerConnId).SendAsync("ServerMessage", message);
            //throw new NotImplementedException();
        }

        //Deprecated
        /// <summary>
        /// Room management for WebRTCHub
        /// </summary>
        public class RoomManager
        {
            private int nextRoomId;
            /// <summary>
            /// Room List (key:RoomId)
            /// </summary>
            private ConcurrentDictionary<int, RoomInfo> rooms;

            public RoomManager()
            {
                nextRoomId = 1;
                rooms = new ConcurrentDictionary<int, RoomInfo>();
            }

            public RoomInfo CreateRoom(string MRN)
            {
                rooms.TryRemove(nextRoomId, out _);

                //create new room info
                var roomInfo = new RoomInfo
                {
                    RoomId = nextRoomId.ToString(),
                    MRN = MRN,
                    Participants = new List<Tuple<string, string, string>>()
                };

                bool result = rooms.TryAdd(nextRoomId, roomInfo);

                if (result)
                {
                    nextRoomId++;
                    return roomInfo;
                }
                else
                {
                    return null;
                }
            }

            public bool AddToRoom(string mrn, string id, string username, string uri)
            {
                bool result = true;
                if (!rooms.Any(r => r.Value.MRN == mrn))
                {
                    if(CreateRoom(mrn) == null)
                    {
                        result = false;
                    }
                }

                if (result)
                    rooms.SingleOrDefault(r => r.Value.MRN == mrn).Value.Participants.Add(new Tuple<string, string, string>(id, username, uri));
                return result;
            }

            public RoomInfo FindRoom(string mrn)
            {
                return rooms.SingleOrDefault(r => r.Value.MRN == mrn).Value;
            }

            public RoomInfo FindRoomByConnectionId(string id)
            {
                return rooms.FirstOrDefault(r => r.Value.Participants.Any(p => p.Item1 == id)).Value;
            }

            public void RemoveFromRoom(string mrn, string id)
            {
                rooms.SingleOrDefault(r => r.Value.MRN == mrn).Value.Participants.Remove(
                    rooms.SingleOrDefault(rm => rm.Value.MRN == mrn).Value.Participants.SingleOrDefault(p=>p.Item1 == id));
            }

            public void RemoveFromAnyRoom(string id)
            {
                foreach(var room in rooms)
                {
                    room.Value.Participants.Remove(room.Value.Participants.SingleOrDefault(p => p.Item1 == id));
                }
            }

            public void DeleteRoom(int roomId)
            {
                rooms.TryRemove(roomId, out _);
            }

            public List<RoomInfo> GetAllRoomInfo()
            {
                return rooms.Values.ToList();
            }
        }

        public class RoomInfo
        {
            public string RoomId { get; set; }
            public string MRN { get; set; }
            public List<Tuple<string, string, string>> Participants { get; set; }
        }

    }
}
