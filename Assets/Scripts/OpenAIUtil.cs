using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace OpenAI
{
    static class OpenAIUtil
    {
        private const int TimeOut = 0;
        private static UnityWebRequest _postRequest;

        public static string ReceivedMessage = "";

        static string CreateChatRequestBody(string prompt)
        {
            var msg = new OpenAI.RequestMessage();
            msg.role = "user";
            msg.content = prompt;

            var req = new OpenAI.Request();
            req.model = "gpt-3.5-turbo";
            req.messages = new[] { msg };

            return JsonUtility.ToJson(req);
        }

        public static async Task InvokeChat(string apiKey, string prompt)
        {
            // POST
            _postRequest = UnityWebRequest.Put(OpenAI.Api.Url, CreateChatRequestBody(prompt));

            // Request timeout setting
            _postRequest.timeout = TimeOut;

            // API key authorization
            _postRequest.method = "POST";
            _postRequest.SetRequestHeader("Content-Type", "application/json");
            _postRequest.SetRequestHeader("Authorization", "Bearer " + apiKey);
            
            UnityWebRequestAsyncOperation operation = _postRequest.SendWebRequest();
            await operation;

            ReceivedMessage = "";
            if (operation.isDone)
            {
                var json = _postRequest.downloadHandler.text;
                var data = JsonUtility.FromJson<OpenAI.Response>(json);
                ReceivedMessage = data.choices[0].message.content.Replace("\n\n", "");
            };
            
            _postRequest.Dispose();
        }
    }
}