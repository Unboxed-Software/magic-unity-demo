using System;
using System.Collections.Generic;
using link.magic.unity.sdk.Provider;
using UnityEngine;
using VoltstroStudios.UnityWebBrowser;
using WindowsHandler;


namespace link.magic.unity.sdk.Relayer
{
    public class WebviewController
    {
        // Switch variable depending on OS
#if !UNITY_EDITOR_WIN || !UNITY_STANDALONE_WIN
        private readonly WebViewObject _webViewObject;
       
#else
        private GameObject windows_webView;
#endif
         private readonly Dictionary<int, Func<string, bool>> _messageHandlers = new();


        private readonly Queue<string> _queue = new();
        private bool _relayerLoaded;
        private bool _relayerReady;


#if !UNITY_EDITOR_WIN || !UNITY_STANDALONE_WIN
        public WebviewController()
        {
            // instantiate webview 
            _webViewObject = new GameObject("WebViewObject").AddComponent<WebViewObject>();
            _webViewObject.Init(
                cb: _cb,
                ld: (msg) =>
                {
                    _relayerLoaded = true;
                },
                httpErr: (msg) =>
                {
                    Debug.Log(string.Format("MagicUnity, LoadRelayerHttpError[{0}]", msg));
                },
                err: (msg) =>
                {
                    Debug.Log(string.Format("MagicUnity, LoadRelayerError[{0}]", msg));
                }
            );
        }
#else
        public WebviewController()
        {
            windows_webView = new GameObject("Windows_Webview");
            windows_webView.AddComponent<Windows_Handler>();
            windows_webView.tag = "win_web";
            // Debug.Log("launching Webview");
        }
#endif

        internal void Load(string url)
        {
#if !UNITY_EDITOR_WIN || !UNITY_STANDALONE_WIN
            _webViewObject.LoadURL(url);
#else
            windows_webView.GetComponent<Windows_Handler>().LoadUrl(url);
#endif
        }

#if !UNITY_EDITOR_WIN || !UNITY_STANDALONE_WIN
        // callback js hooks
        private void _cb(string msg)
        {
            // Debug.Log($"MagicUnity Received Message from Relayer: {msg}");
            // Do SimRle Relayer JSON Deserialization just to fetch ids for handlers
            var res = JsonUtility.FromJson<RelayerResponse<object>>(msg);
            var msgType = res.msgType;

            var method = msgType.Split("-")[0];

            switch (method)
            {
                case nameof(InboundMessageType.MAGIC_OVERLAY_READY):
                    _relayerReady = true;
                    _dequeue();
                    break;
                case nameof(InboundMessageType.MAGIC_SHOW_OVERLAY):
                    _webViewObject.SetVisibility(true);
                    break;
                case nameof(InboundMessageType.MAGIC_HIDE_OVERLAY):
                    _webViewObject.SetVisibility(false);
                    break;
                case nameof(InboundMessageType.MAGIC_HANDLE_EVENT):
                    //Todo Unsupported for now
                    break;
                case nameof(InboundMessageType.MAGIC_HANDLE_RESPONSE):
                    _handleResponse(msg, res);
                    break;
            }
        }
#endif

        /// <summary>
        ///     Queue
        /// </summary>
        internal void Enqueue(string message, int id, Func<string, bool> callback)
        {
            _queue.Enqueue(message);
            _messageHandlers.Add(id, callback);
            _dequeue();
        }

        private void _dequeue()
        {
            if (_queue.Count != 0 && _relayerReady && _relayerLoaded)
            {
                var message = _queue.Dequeue();

                Debug.Log($"MagicUnity Send Message to Relayer: {message}");
#if !UNITY_EDITOR_WIN || !UNITY_STANDALONE_WIN
                _webViewObject.EvaluateJS(
                    $"window.dispatchEvent(new MessageEvent('message', {{ 'data': {message} }}));");
#else
                windows_webView.GetComponent<Windows_Handler>().launch($"window.dispatchEvent(new MessageEvent('message', {{ 'data': {message} }}));");
#endif
                _dequeue();
            }
        }

        private void _handleResponse(string originalMsg, RelayerResponse<object> relayerResponse)
        {
            var payloadId = relayerResponse.response.id;
            var handler = _messageHandlers[payloadId];
            handler(originalMsg);
            _messageHandlers.Remove(payloadId);
        }
    }
}