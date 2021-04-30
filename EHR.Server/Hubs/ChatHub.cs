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
        //room manager to manage patient based chat rooms
        private static RoomManager roomManager = new RoomManager();

        //new connection
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        //on disconnect for any reason remove them from rooms
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

        //on join add to room and notify participants, also send list to caller
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

        //leave a room, remove from room and notify
        public async Task Leave(string MRN, string ConnId)
        {
            roomManager.RemoveFromRoom(MRN, ConnId);
            await Clients.Group(MRN).SendAsync("Left", ConnId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, MRN);
        }

        //This is a fallback method, it has not been implemented on clients
        //It will send messages to all chatters
        public async Task SendMessage(string MRN, object message)
        {
            await Clients.OthersInGroup(MRN).SendAsync("ServerMessage", message);
        }

        //Send message to 1 participant, this is used as a fallback on the client
        public async Task SendOneMessage(string MRN, object message, string partnerConnId)
        {
            await Clients.Client(partnerConnId).SendAsync("ServerMessage", message);
        }

        /// Room management for Chat
        public class RoomManager
        {
            private int nextRoomId;
            /// Room List (key:RoomId)
            private ConcurrentDictionary<int, RoomInfo> rooms;

            //key increments, its not really used though
            public RoomManager()
            {
                nextRoomId = 1;
                rooms = new ConcurrentDictionary<int, RoomInfo>();
            }

            //create a new room
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

            //add someone to a room
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

            //find a room, return the room by patient info
            public RoomInfo FindRoom(string mrn)
            {
                return rooms.SingleOrDefault(r => r.Value.MRN == mrn).Value;
            }

            //find a room by a participant connection id
            public RoomInfo FindRoomByConnectionId(string id)
            {
                return rooms.FirstOrDefault(r => r.Value.Participants.Any(p => p.Item1 == id)).Value;
            }

            //remove a connection id from a room
            public void RemoveFromRoom(string mrn, string id)
            {
                rooms.SingleOrDefault(r => r.Value.MRN == mrn).Value.Participants.Remove(
                    rooms.SingleOrDefault(rm => rm.Value.MRN == mrn).Value.Participants.SingleOrDefault(p=>p.Item1 == id));
            }

            //remove a connection id from all rooms
            public void RemoveFromAnyRoom(string id)
            {
                foreach(var room in rooms)
                {
                    room.Value.Participants.Remove(room.Value.Participants.SingleOrDefault(p => p.Item1 == id));
                }
            }

            //delete a room
            public void DeleteRoom(int roomId)
            {
                rooms.TryRemove(roomId, out _);
            }

            //return all room info
            public List<RoomInfo> GetAllRoomInfo()
            {
                return rooms.Values.ToList();
            }
        }

        //room info class
        public class RoomInfo
        {
            public string RoomId { get; set; }
            public string MRN { get; set; }
            public List<Tuple<string, string, string>> Participants { get; set; }
        }

    }
}
