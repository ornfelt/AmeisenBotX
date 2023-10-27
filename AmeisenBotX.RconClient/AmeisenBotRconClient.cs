using AmeisenBotX.RconClient.Enums;
using AmeisenBotX.RconClient.Messages;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AmeisenBotX.RconClient
{
    /// <summary>
    /// Represents a client for communicating with the AmeisenBot RCON server.
    /// </summary>
    public class AmeisenBotRconClient
    {
        ///<summary>
        /// Creates a new instance of the AmeisenBotRconClient class with the specified parameters.
        ///</summary>
        ///<param name="endpoint">The endpoint of the AmeisenBot RCON server.</param>
        ///<param name="name">The name of the client.</param>
        ///<param name="wowRace">The race of the client's character in World of Warcraft.</param>
        ///<param name="wowGender">The gender of the client's character in World of Warcraft.</param>
        ///<param name="wowClass">The class of the client's character in World of Warcraft.</param>
        ///<param name="wowRole">The role of the client's character in World of Warcraft.</param>
        ///<param name="image">Optional. The image associated with the client.</param>
        ///<param name="guid">Optional. The GUID of the client.</param>
        ///<param name="validateCertificate">Optional. Determines whether to validate the server's certificate.</param>
        public AmeisenBotRconClient(string endpoint, string name, string wowRace, string wowGender, string wowClass, string wowRole, string image = "", string guid = "", bool validateCertificate = false)
        {
            Endpoint = endpoint;

            KeepaliveEnpoint = new($"{Endpoint}/api/keepalive");
            RegisterEnpoint = new($"{Endpoint}/api/register");
            DataEnpoint = new($"{Endpoint}/api/data");
            ImageEnpoint = new($"{Endpoint}/api/image");
            ActionEnpoint = new($"{Endpoint}/api/action");

            if (!validateCertificate)
            {
                HttpClientHandler handler = new()
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
                };

                HttpClient = new(handler) { Timeout = TimeSpan.FromSeconds(1) };
            }
            else
            {
                HttpClient = new() { Timeout = TimeSpan.FromSeconds(1) };
            }

            Guid = guid.Length > 0 ? guid : System.Guid.NewGuid().ToString();

            RegisterMessage = new()
            {
                Guid = Guid,
                Name = name,
                Race = wowRace,
                Gender = wowGender,
                Class = wowClass,
                Role = wowRole,
                Image = image
            };
        }

        /// <summary>
        /// Gets or sets the endpoint string for the code.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the Guid value.
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance 
        /// needs to be registered.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance needs to be registered; 
        ///   otherwise, <c>false</c>.
        /// </value>
        public bool NeedToRegister { get; private set; } = true;

        /// <summary>
        /// Gets or sets the list of pending actions.
        /// </summary>
        /// <value>The list of pending actions.</value>
        public List<ActionType> PendingActions { get; private set; } = new();

        /// <summary>
        /// Gets or sets the RegisterMessage property.
        /// </summary>
        public RegisterMessage RegisterMessage { get; }

        /// <summary>
        /// Gets the URI endpoint for the action.
        /// </summary>
        private Uri ActionEnpoint { get; }

        /// <summary>
        /// Gets or sets the data endpoint URI.
        /// </summary>
        private Uri DataEnpoint { get; }

        /// <summary>
        /// Gets or sets the HttpClient object used for making HTTP requests.
        /// </summary>
        private HttpClient HttpClient { get; set; }

        /// <summary>
        /// Gets or sets the private Uri representing the ImageEnpoint property.
        /// </summary>
        private Uri ImageEnpoint { get; }

        /// <summary>
        /// Gets the URI of the keepalive endpoint.
        /// </summary>
        private Uri KeepaliveEnpoint { get; }

        ///<summary>
        ///Gets or sets the URI for the registration endpoint.
        ///</summary>
        private Uri RegisterEnpoint { get; }

        /// <summary>
        /// Sends a keep-alive message to the specified endpoint using HTTP POST request.
        /// If the response is successful, returns true. Otherwise, sets the NeedToRegister property to true and returns false.
        /// </summary>
        public bool KeepAlive()
        {
            using StringContent content = new(JsonSerializer.Serialize(new KeepAliveMessage() { Guid = Guid }), Encoding.UTF8, "application/json");
            HttpResponseMessage dataResponse = HttpClient.PostAsync(KeepaliveEnpoint, content).Result;

            if (dataResponse.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                NeedToRegister = true;
                return false;
            }
        }

        /// <summary>
        /// Retrieves pending actions from the specified endpoint using HttpClient.GetAsync and deserializes the response content into a list of ActionType objects using JsonSerializer.Deserialize. 
        /// If the operation is successful (response status code is 2xx), assigns the deserialized list to the PendingActions property and returns true. 
        /// Otherwise, sets the NeedToRegister flag to true and returns false.
        /// </summary>
        public bool PullPendingActions()
        {
            HttpResponseMessage dataResponse = HttpClient.GetAsync(ActionEnpoint).Result;

            if (dataResponse.IsSuccessStatusCode)
            {
                PendingActions = JsonSerializer.Deserialize<List<ActionType>>(dataResponse.Content.ReadAsStringAsync().Result, new JsonSerializerOptions() { AllowTrailingCommas = true, NumberHandling = JsonNumberHandling.AllowReadingFromString });
                return true;
            }
            else
            {
                NeedToRegister = true;
                return false;
            }
        }

        /// <summary>
        /// Register a user by sending a POST request to the specified RegisterEnpoint with the serialized RegisterMessage object as the content.
        /// </summary>
        /// <returns>True if the request was successful and false otherwise.</returns>
        public bool Register()
        {
            using StringContent content = new(JsonSerializer.Serialize(RegisterMessage), Encoding.UTF8, "application/json");
            HttpResponseMessage registerResponse = HttpClient.PostAsync(RegisterEnpoint, content).Result;

            NeedToRegister = false;

            if (registerResponse.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sends the data message to the data endpoint and returns a boolean value indicating whether the operation was successful or not.
        /// </summary>
        /// <param name="dataMessage">The data message to be sent.</param>
        /// <returns>True if the data message was sent successfully, False otherwise.</returns>
        public bool SendData(DataMessage dataMessage)
        {
            if (dataMessage == null)
            {
                return false;
            }

            dataMessage.Guid = Guid;

            using StringContent content = new(JsonSerializer.Serialize(dataMessage), Encoding.UTF8, "application/json");
            HttpResponseMessage dataResponse = HttpClient.PostAsync(DataEnpoint, content).Result;

            if (dataResponse.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                NeedToRegister = true;
                return false;
            }
        }

        /// <summary>
        /// Sends an image to the specified endpoint using a POST request.
        /// </summary>
        /// <param name="image">The image to be sent.</param>
        /// <returns>True if the image was successfully sent, False otherwise.</returns>
        public bool SendImage(string image)
        {
            using StringContent content = new(JsonSerializer.Serialize(new ImageMessage() { Guid = Guid, Image = image }), Encoding.UTF8, "application/json");
            HttpResponseMessage dataResponse = HttpClient.PostAsync(ImageEnpoint, content).Result;

            if (dataResponse.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                NeedToRegister = true;
                return false;
            }
        }
    }
}