using DpsLibs.Data;
using Timensit_API.Classes;
using Timensit_API.Controllers.Users;
using Timensit_API.Models.Common;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
    [Route("signalr")]
    [EnableCors("TimensitPolicy")]
    public class ThongBaoHub : Hub
    {
        private NCCConfig _config;
        private UserController user ;
        public ThongBaoHub(IOptions<NCCConfig> config, IHostingEnvironment hostingEnvironment)
        {
            _config = config.Value;
            //user = new UserController(config, configLogin, hostingEnvironment, accessor);
        }
        private static Dictionary<string, string> ConnectedClients = new Dictionary<string, string>();

        public async Task SendMessage(string infoToken)
        {
            TokenRequesModel infoDataCon = JsonConvert.DeserializeObject<TokenRequesModel>(infoToken);
            string Token = infoDataCon.Token;
            string idUser = infoDataCon.UserID.ToString();
            string message = infoDataCon.Value.ToString();
            string clientID = checkClientIDWithToken(Token);
            await Clients.Client(clientID).SendAsync("receiveMessage", clientID, message);
            //await Clients.All.SendAsync("receiveMessage", clientID, message);
        }
        public override Task OnConnectedAsync()
        {
            var id = Context.ConnectionId;

            return base.OnConnectedAsync();
        }
        public Task OnConnectedTokenAsync(string infoToken)
        {
            try
            {
                var id = Context.ConnectionId;
                TokenRequesModel infoDataCon = JsonConvert.DeserializeObject<TokenRequesModel>(infoToken);
                string Token = infoDataCon.Token;
                string idUser = infoDataCon.UserID.ToString();
                string clientId = Context.ConnectionId;
                if (!ConnectedClients.Keys.Contains(Token))
                {
                    ConnectedClients.Add(Token, clientId);
                }
                else
                {
                    ConnectedClients[Token] = clientId;
                }
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    Hashtable has = new Hashtable();
                    has.Add("Token", Token);
                    has.Add("TimeConnect", DateTime.Now);
                    has.Add("TimeTokenConnect", DateTime.Now);
                    has.Add("ConnectionId", clientId);

                    string sql_ = @" IF EXISTS (SELECT * FROM SignalR_Connect c WHERE c.Token=@Token)
		                                BEGIN
			                                DELETE SignalR_Connect WHERE Token =@Token 
		                                END ";

                    cnn.ExecuteNonQuery(sql_, new SqlConditions { { "Token", Token } });
                    if (cnn.Insert(has, "SignalR_Connect") == 1)
                    {
                    }
                }
            }catch(Exception ex)
            {
                return base.OnDisconnectedAsync(ex);
            }
             return base.OnConnectedAsync();
        }
        public Task ReconnectToken(string infoToken)
        {
            try
            {
                TokenRequesModel infoDataCon = JsonConvert.DeserializeObject<TokenRequesModel>(infoToken);
                string Token = infoDataCon.Token;
                string idUser = infoDataCon.UserID.ToString();
                string clientID = checkClientIDWithToken(Token);
                if (!ConnectedClients.ContainsKey(Token))
                {
                    ConnectedClients[Token] = clientID;
                    using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                    {
                        Hashtable has = new Hashtable();
                        has.Add("Token", Token);
                        has.Add("TimeTokenConnect", DateTime.Now);
                        SqlConditions cond = new SqlConditions();
                        cond.Add("ConnectionId", clientID);
                        if (cnn.Update(has, cond, "SignalR_Connect") == 1)
                        {

                        }
                    }

                }
                //return base.OnConnected();

            }
            catch (Exception ex)
            {

                return base.OnDisconnectedAsync(ex);
            }
            return base.OnConnectedAsync();
        }
        public Task onDisconnectToken(string infoToken)
        {
            try
            {
                TokenRequesModel infoDataCon = JsonConvert.DeserializeObject<TokenRequesModel>(infoToken);
                string Token = infoDataCon.Token;
                string idUser = infoDataCon.UserID.ToString();
                string clientID = checkClientIDWithToken(Token);
                if (!ConnectedClients.ContainsKey(Token))
                {
                    ConnectedClients[Token] = clientID;
                    using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                    {
                        SqlConditions val = new SqlConditions();
                        val.Add("Token", Token);
                        cnn.Delete(val, "SignalR_Connect");
                        ConnectedClients.Remove(Token);                        
                    }
                }
            }
            catch (Exception ex)
            {

                return base.OnDisconnectedAsync(ex);
            }
            return base.OnDisconnectedAsync(new Exception());
        }
        //public async Task ReceiveMessage(string type)
        //{
        //    await Clients.All.SendAsync("recieveMessaged", type);
        //}       
        private string checkClientIDWithToken(string token)
        {
            string clientId = "";
            ConnectedClients.TryGetValue(token, out clientId);
            return clientId;
        }
        private class TokenRequesModel
        {
            public string Token { get; set; } = "";
            public long UserID { get; set; } = 0;
            public string Value { get; set; } = "";
        }
    }   
}