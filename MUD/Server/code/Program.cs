using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

using MessageTypes;


namespace Server
{
    class Program
    {
        static Dictionary<String,Socket> clientDictionary = new Dictionary<String,Socket>();
        static int clientID = 1;

        static List<Player> playerList = new List<Player>();

        static Dungeon dungeon = new Dungeon();

        static bool firstPlayerHasConnect = false;

        static string DisplayPlayerLocations()
        {
            string print = "";
            for(int i = 0; i < playerList.Count; i++)
            {
                print = print + "\n " + playerList[i].name + " is currently located at " + playerList[i].currRoom.Name;
            }
            return print;

        }

        static void Rename(Socket currPlayer, string newName)
        {
            for(int i = 0; i < playerList.Count; i++)
            {
                if(currPlayer == playerList[i].owner)
                {
                    SendChatMessage(playerList[i].name + " is now called " + newName);
                    playerList[i].name = newName;
                }
            }
        }

        static void OnConnect()
        {
            if(!firstPlayerHasConnect)
            {
                dungeon.Init();

                firstPlayerHasConnect = true;
            }
        }

        static void SendClientName(Socket s, String clientName)
        {
            ClientNameMsg nameMsg = new ClientNameMsg();
            nameMsg.name = clientName;

            MemoryStream outStream = nameMsg.WriteData();

            s.Send(outStream.GetBuffer() );
        }

        static void SendClientList()
        {
            ClientListMsg clientListMsg = new ClientListMsg();

            lock (clientDictionary)
            {
                foreach (KeyValuePair<String, Socket> s in clientDictionary)
                {
                    clientListMsg.clientList.Add(s.Key);
                }

                MemoryStream outStream = clientListMsg.WriteData();

                foreach (KeyValuePair<String, Socket> s in clientDictionary)
                {
                    s.Value.Send(outStream.GetBuffer());
                }
            }
        }

        static void SendChatMessage(String msg)
        {
            PublicChatMsg chatMsg = new PublicChatMsg();

            chatMsg.msg = msg;

            MemoryStream outStream = chatMsg.WriteData();

            lock (clientDictionary)
            {            
                foreach (KeyValuePair<String,Socket> s in clientDictionary)
                {
                    try
                    {
                        s.Value.Send(outStream.GetBuffer());
                    }
                    catch (System.Exception)
                    {
                    	
                    }                    
                }
            }
        }

        static void SendPrivateMessage(Socket s, String from, String msg)
        {
            PrivateChatMsg chatMsg = new PrivateChatMsg();
            chatMsg.msg = msg;
            chatMsg.destination = from;
            MemoryStream outStream = chatMsg.WriteData();

            try
            {
                s.Send(outStream.GetBuffer());
            }
            catch (System.Exception)
            {

            }
        }

        static Socket GetSocketFromName(String name)
        {
            lock (clientDictionary)
            {
                return clientDictionary[name];
            }
        }

        static String GetNameFromSocket(Socket s)
        {
            lock (clientDictionary)
            {
                foreach (KeyValuePair<String, Socket> o in clientDictionary)
                {
                    if (o.Value == s)
                    {
                        return o.Key;
                    }
                }
            }

            return null;
        }

        static void RemoveClientBySocket(Socket s)
        {
            string name = GetNameFromSocket(s);

            if (name != null)
            {
                lock (clientDictionary)
                {
                    clientDictionary.Remove(name);
                }
            }
        }

        static void Attack(string[] input, Player currPlayer)
        {
            int damageToDeal = 20;
            bool enemyIsValid = false;
            string enemy = input[1];

            for(int i = 0; i < currPlayer.currRoom.enemyList.Count; i++)
            {
                if(enemy == currPlayer.currRoom.enemyList[i].GetName())
                {
                    enemyIsValid = true;
                    damageToDeal = currPlayer.GetDamage();
                    currPlayer.currRoom.enemyList[i].enemyHealth.TakeHealth(damageToDeal);
                    string message = currPlayer.name + " attacked " + currPlayer.currRoom.enemyList[i].GetName() + " for " + damageToDeal + ". ";
                    SendChatMessage(message);
                    string deadMessage = "";
                    string enemiesInRoom = "";
                    string enemyAttackBack = "";

                    if(currPlayer.currRoom.enemyList[i].enemyHealth.IsDead())
                    {
                        deadMessage = currPlayer.currRoom.enemyList[i].GetName() + " has been slain! ";

                        currPlayer.currRoom.enemyList.Remove(currPlayer.currRoom.enemyList[i]);

                        if(currPlayer.currRoom.enemyList.Count > 0)
                        {
                            enemiesInRoom = "The enemies in the room are now: ";
                            for (int j = 0; j < currPlayer.currRoom.enemyList.Count; j++)
                            {
                                enemiesInRoom = enemiesInRoom + currPlayer.currRoom.enemyList[j].GetName() + " ";
                            }
                        }
                        else
                        {
                            enemiesInRoom = "There are no more enemies in the room.";
                        }
                        

                        
                    }
                    else
                    {
                        int damage = currPlayer.currRoom.enemyList[i].GetRandomDamgeInRange();
                        currPlayer.currRoom.enemyList[i].EnemyAttack(currPlayer, damage);
                        enemyAttackBack = currPlayer.name + "has been hit by " + currPlayer.currRoom.enemyList[i].GetName() + " for " + damage + ". Player now has " + currPlayer.playerHealth.GetHealth() + " health. " ;
                        if(currPlayer.playerHealth.IsDead())
                        {
                            SendChatMessage(currPlayer.name + " has died and has been sent back to the start.");
                            currPlayer.playerHealth.SetHealth(100);
                            currPlayer.currRoom = dungeon.roomMap["Outside the cave"];
                        }
                        //SendChatMessage(currPlayer.name + "has been hit by " + currPlayer.currRoom.enemyList[i].GetName() + " for " + damage);
                    }
                    SendChatMessage(deadMessage + enemiesInRoom + enemyAttackBack);
                }
            }

            if(!enemyIsValid)
            {
                SendPrivateMessage(currPlayer.owner, "Server", "That enemy is not valid");
            }
        }


        static void MoveRoom(Player currPlayer, string[] input)
        {
            if(input[1].ToLower() == "north" && currPlayer.currRoom.north != null)
            {
                //SendChatMessage(currPlayer.name + " has left the " + currPlayer.currRoom.Name + " and entered the " + currPlayer.currRoom.north);
                currPlayer.currRoom = dungeon.roomMap[currPlayer.currRoom.north];
                
            } else if(input[1].ToLower() == "south" && currPlayer.currRoom.south != null)
            {
                //SendChatMessage(currPlayer.name + " has left the " + currPlayer.currRoom.Name + " and entered the " + currPlayer.currRoom.south);
                currPlayer.currRoom = dungeon.roomMap[currPlayer.currRoom.south];
            }
            else if (input[1].ToLower() == "east" && currPlayer.currRoom.east != null)
            {
                //SendChatMessage(currPlayer.name + " has left the " + currPlayer.currRoom.Name + " and entered the " + currPlayer.currRoom.east);
                currPlayer.currRoom = dungeon.roomMap[currPlayer.currRoom.east];
            }
            else if (input[1].ToLower() == "west" && currPlayer.currRoom.west != null)
            {
                //SendChatMessage(currPlayer.name + " has left the " + currPlayer.currRoom.Name + " and entered the " + currPlayer.currRoom.west);
                currPlayer.currRoom = dungeon.roomMap[currPlayer.currRoom.west];
            }
            else
            {
                SendPrivateMessage(currPlayer.owner, "Server", "ERROR , you cannot go this direction");
            }
            string enemies = "";

            if(currPlayer.currRoom.enemyList.Count > 0)
            {
                enemies = "You see several enemies in the room: ";
                for(int i = 0; i < currPlayer.currRoom.enemyList.Count; i++)
                {
                    enemies = enemies + currPlayer.currRoom.enemyList[i].GetName() + " ";
                }
            }

            string directions = "You can go ";
            if(currPlayer.currRoom.north != null)
            {
                directions = directions + " north";
            }
            if (currPlayer.currRoom.south != null)
            {
                directions = directions + " south";
            }
            if (currPlayer.currRoom.east != null)
            {
                directions = directions + " east or ";
            }
            if (currPlayer.currRoom.west != null)
            {
                directions = directions + " west.";
            }
            SendPrivateMessage(currPlayer.owner, "Server", "You are currently in " + currPlayer.currRoom.Name + ". " + currPlayer.currRoom.description + enemies +  directions);
        }

        static void CommandStates(string[] input, Socket chatClient)
        {
            switch(input[0])
            {
                case "help":
                    SendPrivateMessage(chatClient, "Server", "The commands are: " );
                    SendPrivateMessage(chatClient, "Server", "Go [direction] - Moves in given direction; Say [Text to say] - writes to all players; rename [New name] - gives yourself a new name; players - displays all current players names and locations");
                    break;
                case "attack":
                    for (int i = 0; i < playerList.Count; i++)
                    {
                        if (playerList[i].owner == chatClient)
                        {
                            Attack(input, playerList[i]);
                        }
                    }
                    break;
                case "go":
                    string direction = input[1];

                    for(int i = 0; i < playerList.Count; i++)
                    {
                        if(playerList[i].owner == chatClient)
                        {
                            MoveRoom(playerList[i], input);
                        }
                    }
                    break;
                case "say":
                    string outputLine = GetNameFromSocket(chatClient) + ": ";
                    for(int i = 1; i < input.Length; i++)
                    {
                        outputLine = outputLine + input[i] + ' ';
                    }
                    SendChatMessage(outputLine);
                    break;
                case "rename":
                    Rename(chatClient, input[1]);
                    break;
                case "players":
                    SendPrivateMessage(chatClient, "Server", DisplayPlayerLocations());
                    break;
                default:
                    SendPrivateMessage(chatClient, "Server", "ERROR");
                    SendPrivateMessage(chatClient, "Server", "That is not a form of input");
                    break;
            }
        }

        static void receiveClientProcess(Object o)
        {
            bool bQuit = false;

            Socket chatClient = (Socket)o;

            Console.WriteLine("client receive thread for " + GetNameFromSocket(chatClient));

            SendClientList();

            while (bQuit == false)
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    int result;

                    result = chatClient.Receive(buffer);

                    if (result > 0)
                    {
                        MemoryStream stream = new MemoryStream(buffer);
                        BinaryReader read = new BinaryReader(stream);

                        Msg m = Msg.DecodeStream(read);

                        if (m != null)
                        {
                            switch (m.mID)
                            {
                                case PublicChatMsg.ID:
                                    {
                                        PublicChatMsg publicMsg = (PublicChatMsg)m;

                                        String formattedMsg = "<" + GetNameFromSocket(chatClient)+"> " + publicMsg.msg;
                                        Console.WriteLine("Public message - " + formattedMsg);

                                        //Split the input string into command and after command
                                        String[] commandWord = publicMsg.msg.Split(' ');

                                        CommandStates(commandWord , chatClient);
                                    }
                                    break;

                                case PrivateChatMsg.ID:
                                    {
                                        PrivateChatMsg privateMsg = (PrivateChatMsg)m;

                                        String formattedMsg = "PRIVATE <" + GetNameFromSocket(chatClient) + "> " + privateMsg.msg;

                                        Console.WriteLine("private chat - " + formattedMsg + "to " + privateMsg.destination);

                                        SendPrivateMessage(GetSocketFromName(privateMsg.destination), GetNameFromSocket(chatClient), formattedMsg);

                                        formattedMsg = "<" + GetNameFromSocket(chatClient) + "> --> <" +privateMsg.destination+"> " + privateMsg.msg;
                                        SendPrivateMessage(chatClient, "", formattedMsg);
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                    }                   
                }
                catch (Exception)
                {
                    bQuit = true;

                    String output = "Lost client: " + GetNameFromSocket(chatClient);
                    Console.WriteLine(output);
                    SendChatMessage(output);

                    RemoveClientBySocket(chatClient);

                    SendClientList();
                }
            }
        }

        static void Main(string[] args)
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8500));
            serverSocket.Listen(32);

            bool bQuit = false;

            Console.WriteLine("Server");

            while (!bQuit)
            {
                Socket serverClient = serverSocket.Accept();

                Thread myThread = new Thread(receiveClientProcess);
                myThread.Start(serverClient);

                lock (clientDictionary)
                {
                    String clientName = "client" + clientID;
                    clientDictionary.Add(clientName, serverClient);

                    OnConnect();

                    playerList.Add(new Player(serverClient,clientName, dungeon.roomMap.ElementAt(0).Value));

                    Player currPlayer = null;
                    for(int i = 0; i < playerList.Count; i++)
                    {
                        if(playerList[i].owner == serverClient)
                        {
                            currPlayer = playerList[i];
                        }
                    }
                    SendPrivateMessage(currPlayer.owner, "Server", "You start in " + currPlayer.currRoom.Name + ". " + currPlayer.currRoom.description + " " + DisplayPlayerLocations());
                    
                    SendClientName(serverClient, clientName);
                    Thread.Sleep(500);
                    SendClientList();

                    clientID++;
                }
                
            }
        }
    }
}
