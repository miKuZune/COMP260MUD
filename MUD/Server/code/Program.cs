using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Data.SQLite;

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

        static SQLiteConnection conn = null;

        static void CreateNewDatabase(String DBName)
        {
            try
            {
                SQLiteConnection.CreateFile(DBName);

                conn = new SQLiteConnection("Data Source =" + DBName + ";Version=3;FailIfMissing=True");

                SQLiteCommand command;

                conn.Open();
                List<string> dbVars = new List<string>();
                bool noMoreDBVariables = false;
                while(!noMoreDBVariables)
                {
                    Console.WriteLine("Enter the sql code for the variable. Type 'exit' when you are done and hit enter.");
                    string newVar = Console.ReadLine();
                    if (newVar == "exit")
                    {
                        noMoreDBVariables = true;
                    }else { dbVars.Add(newVar); }
                }

                string sqlCommand = "create table table_" + DBName + "(";
                for(int i = 0; i < dbVars.Count; i++)
                {
                    sqlCommand += dbVars[i];
                    if (i < dbVars.Count - 1){ sqlCommand += ","; }
                }
                sqlCommand += ")";

                Console.WriteLine(sqlCommand);

                command = new SQLiteCommand(sqlCommand, conn);
                command.ExecuteNonQuery();
                Console.WriteLine("Database " + DBName + " has been created");
            }catch(Exception e)
            {
                Console.WriteLine("Failed to create database: " + e);
            }
        }

        static void OpenDatabase(String DBname)
        {
            conn = new SQLiteConnection("Data Source=" + DBname + ";Version=3;FailIfMissing=True"); ;

            try
            {
                conn.Open();
                Console.WriteLine("Database connection to " + DBname + " established");
            }catch(Exception e)
            {
                Console.WriteLine("Failed to open database: " + e);
            }
        }

        static void AddEntry(String DBName)
        {
            try
            {
                //Read columns in database and ask for vaiables to fill these.
                SQLiteCommand comm = new SQLiteCommand("select * from table_" + DBName, conn);
                SQLiteDataReader read = comm.ExecuteReader();
                List<String> entryVars = new List<String>();
                List<String> columnNames = new List<String>();

                for(int i = 0; i < read.FieldCount; i++)
                {
                    columnNames.Add(read.GetName(i));
                    Console.WriteLine("Please enter "+ read.GetName(i));
                    entryVars.Add(Console.ReadLine());
                }

                comm = new SQLiteCommand("select * from table_" + DBName + " where " + columnNames[0] + " == '" + entryVars[0] + "'", conn);
                read = comm.ExecuteReader();

                if(!read.HasRows)
                {
                    try
                    {
                        String sql = "insert into table_" + DBName + " (";
                        for(int i = 0; i < columnNames.Count; i++)
                        {
                            sql += columnNames[i];
                            if (i < columnNames.Count - 1) { sql += ", "; }
                        }
                        sql += ") values";
                        sql += "(";
                        for(int i = 0; i < entryVars.Count; i++)
                        {
                            sql += "'" + entryVars[i] + "'";
                            if (i < entryVars.Count - 1) { sql += ","; }
                        }

                        sql += ")";

                        comm = new SQLiteCommand(sql, conn);
                        comm.ExecuteNonQuery();

                    }catch (Exception e)
                    {
                        Console.WriteLine("Could not add to database : " + e);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create entry: " + e);
            }
        }

        static void DisplayAllEntries(String dbName)
        {
            try
            {
                //Read columns
                SQLiteCommand comm = new SQLiteCommand("select * from table_" + dbName, conn);
                SQLiteDataReader read = comm.ExecuteReader();
                List<String> columnNames = new List<String>();

                for (int i = 0; i < read.FieldCount; i++)
                {
                    columnNames.Add(read.GetName(i));
                }

                Console.WriteLine("");
                SQLiteCommand command = new SQLiteCommand("select * from table_" + dbName + " order by name asc", conn);
                SQLiteDataReader reader = command.ExecuteReader();



                while(reader.Read())
                {
                    String output = null;
                    for(int i = 0; i < columnNames.Count; i++)
                    {
                        output += columnNames[i] + ": " + reader[columnNames[i]] + "\t";
                    }

                    Console.WriteLine(output);
                }
                reader.Close();
                Console.WriteLine("");
            } catch (Exception e)
            {
                Console.WriteLine("Unable to display current entires : " + e);
            }
        }

        static void DeleteEntry(string dbName)
        {
            try
            {
                Console.WriteLine("Enter the name of the entry you wish to delete");
                string name = Console.ReadLine();

                SQLiteCommand command = new SQLiteCommand("delete from table_" + dbName + " where name == '" + name + "'", conn);
                SQLiteDataReader reader = command.ExecuteReader();

                if(reader.RecordsAffected > 0)
                {
                    Console.WriteLine("Deleted: " + name);
                }else
                {
                    Console.WriteLine("Not in DB: " + name);
                }

            }catch(Exception e)
            {
                Console.WriteLine("Failed to delete entry: " + e);
            }
        }

        //Returns a string containg players names and locations in the dungeon.
        static string DisplayPlayerLocations()
        {
            string print = "";
            for(int i = 0; i < playerList.Count; i++)
            {
                print = print + playerList[i].name + " is currently located at " + playerList[i].currRoom.Name;
            }
            return print;

        }

        //Changes the player name, and sends a message to everyone that this change has occured.
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

        //Checks if the enemy to attack is valid, if so attacks that enemy. Also handles enemy death and enemies attacking the player back.
        static void Attack(string[] input, Player currPlayer)
        {
            //Holds the parameter for the enemy to attack
            string enemy = input[1].ToLower();
            //Used to send a message to the player if no enemy is found.
            bool enemyIsValid = false;

            //Goes through the enemies in the room to find the one the player typed, if valid.
            for(int i = 0; i < currPlayer.currRoom.enemyList.Count; i++)
            {
                if(enemy == currPlayer.currRoom.enemyList[i].GetName().ToLower())
                {
                    //Holds the messages which will be sent to players as a result of the attack.
                    string playerAttackMessage = "";
                    string deadMessage = "";
                    string enemyAttackBack = "";

                    enemyIsValid = true;
                    AttackManager AM = new AttackManager(currPlayer, currPlayer.currRoom.enemyList[i]);

                    playerAttackMessage = AM.PlayerAttack(currPlayer.GetDamage());
                    SendChatMessage(playerAttackMessage);

                    //If enemy is killed prints to the player
                    if(currPlayer.currRoom.enemyList[i].enemyHealth.IsDead())
                    {
                        deadMessage = AM.OnEnemyDead();
                    }
                    //If the enemy is not killed they will attack the player back.
                    else
                    {
                        int damage = currPlayer.currRoom.enemyList[i].GetRandomDamgeInRange();

                        enemyAttackBack = AM.EnemyAttackPlayer(damage);

                        //If the player dies from this attack a message is sent to everyone, the player's health is then reset and they are sent to the start of the dungeon.
                        if (currPlayer.playerHealth.IsDead())
                        {
                            SendChatMessage( deadMessage + enemyAttackBack + AM.OnPlayerDeath());
                            currPlayer.currRoom = dungeon.roomMap["Outside the cave"];
                        }
                    }
                    SendChatMessage(deadMessage + enemyAttackBack);
                }
            }

            //Printed if the input is not valid.
            if(!enemyIsValid)
            {
                SendPrivateMessage(currPlayer.owner, "Server", "That enemy is not valid");
            }
        }

        //Sends private messages to everyone in the same room as the player
        static void SpeakToPeopleInRoom(string[] input, Player currPlayer)
        {
            string wordsToSay = "" + currPlayer.name + ": ";

            for(int i = 1; i < input.Length; i++)
            {
                wordsToSay = wordsToSay + input[i];
            }

            for(int i = 0; i < playerList.Count; i++)
            {
                if(playerList[i].currRoom == currPlayer.currRoom && playerList[i].owner != currPlayer.owner)
                {
                    SendPrivateMessage(playerList[i].owner, "" + currPlayer.name, wordsToSay);
                }
            }
        }

        //Moves the player from their location to a new room, if the input is valid
        static void MoveRoom(Player currPlayer, string[] input)
        {
            MoveRoomManager MRM = new MoveRoomManager(currPlayer, input);

            //Checks if there are any enemies in the room and stops the player from leaving if there are.
            if(MRM.CheckForEnemiesInRoom())
            {
                SendPrivateMessage(currPlayer.owner, "Server", "You cannot leave while there are enemies in the room");
                return;
            }

            //Checks if the input parameter is valid. IF valid moves the player to the next room. If not valid sends a private error message to the client.
            string validRoomMessage = MRM.MoveRoom(dungeon.roomMap);
            if(validRoomMessage != null){SendPrivateMessage(currPlayer.owner, "Server", "ERROR , you cannot go this direction");}

            string sendString = "You are currently in " + currPlayer.currRoom.Name + AddIndent() +  currPlayer.currRoom.description + AddIndent() + MRM.ListEnemiesInRoom() + AddIndent() + MRM.GetPlayersInRoom(playerList) + AddIndent() + MRM.GetPossibleDirections();

            SendPrivateMessage(currPlayer.owner ,"Server" ,sendString);
        }

        //Returns a string to indent a message.
        static string AddIndent()
        {
            string indent = "       ";

            return indent;
        }
        
        //Takes players input and performs appropriate commands if valid.    
        static void CommandStates(string[] input, Socket chatClient)
        {
            switch(input[0])
            {
                case "help":
                    //Gives the player a message containing all the possible commands
                    SendPrivateMessage(chatClient, "Server", "The commands are: " );
                    SendPrivateMessage(chatClient, "Server", "Go [direction] - Moves in given direction; Say [Text to say] - writes to all players; rename [New name] - gives yourself a new name; players - displays all current players names and locations; attack [enemy name] - attacks the enemy chosen");
                    break;
                case "attack":
                    //Gets the current player then trys to attack the chosen enemy.
                    Attack(input, GetCurrentPlayer(chatClient));
                    break;
                case "go":
                    //Gets the current player and trys to move them in the given direction.
                    MoveRoom(GetCurrentPlayer(chatClient), input);
                    break;
                case "say":
                    //Gets the current player then gives the message to all over players in the same room.
                    SpeakToPeopleInRoom(input, GetCurrentPlayer(chatClient));
                    break;
                case "rename":
                    //Changes the users name to their input.
                    Rename(chatClient, input[1]);
                    break;
                case "players":
                    //Gets a list of players and player locations and displays them to the user.
                    SendPrivateMessage(chatClient, "Server", DisplayPlayerLocations());
                    break;
                default:
                    //The command from the player is invalid so an error message is shown.
                    SendPrivateMessage(chatClient, "Server", "ERROR");
                    SendPrivateMessage(chatClient, "Server", "That is not a valid form of input");
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

        static Player GetCurrentPlayer(Socket serverClient)
        {
            Player thisPlayer = null;

            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i].owner == serverClient)
                {
                    thisPlayer = playerList[i];
                }
            }

            return thisPlayer;
        }

        static String AskForDBName()
        {
            Console.WriteLine("Please input the database name");
            string dbName = Console.ReadLine();

            return dbName;
        }

        static void Main(string[] args)
        {
            bool finishedWithDatabase = false;
            string dbName = null;
            while (!finishedWithDatabase)
            {
                //Allow access to the database
                Console.WriteLine("You have started the server.");
                Console.WriteLine("You currently can perform maintanenece on the database.");
                Console.WriteLine("Here are the options");
                Console.WriteLine("1 - create database");
                Console.WriteLine("2 - Open database");
                Console.WriteLine("3 - Add entry to database");
                Console.WriteLine("4 - Display all entries");
                Console.WriteLine("5 - Delete entry");
                Console.WriteLine("Exit - Finish with database");

                String action = Console.ReadLine();


                switch (action)
                {
                    //Create database
                    case "1":
                        dbName = AskForDBName();
                        CreateNewDatabase(dbName);
                    break;
                    case "2":
                        dbName = AskForDBName();
                        OpenDatabase(dbName);
                        break;
                    case "3":
                        AddEntry(dbName);
                        break;
                    case "4":
                        DisplayAllEntries(dbName);
                        break;
                    case "5":
                        DeleteEntry(dbName);
                        break;
                    case "exit":
                        finishedWithDatabase = true;
                        break;
                    default:
                        Console.WriteLine("That was not valid. Please try again.");
                    break;
                }
            }

            //Do server stuff
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //165.227.230.143
            //127.0.0.1
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8500));
            serverSocket.Listen(32);

            bool bQuit = false;

            Console.WriteLine("Server Online");

            while (!bQuit)
            {
                //Accepts a new client.
                Socket serverClient = serverSocket.Accept();

                Thread myThread = new Thread(receiveClientProcess);
                myThread.Start(serverClient);

                lock (clientDictionary)
                {
                    String clientName = "client" + clientID;
                    clientDictionary.Add(clientName, serverClient);

                    OnConnect();
                    //Adds the new player to the list of players.
                    playerList.Add(new Player(serverClient,clientName, dungeon.roomMap.ElementAt(0).Value));


                    //Goes through each player to get the current player.
                    Player currPlayer = null;
                    currPlayer = GetCurrentPlayer(serverClient);
                    //Sends a starting message to the new player
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
