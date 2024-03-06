using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using link.magic.unity.sdk;
using link.magic.unity.sdk.Provider;
using VoltstroStudios.UnityWebBrowser.Core;

namespace WindowsHandler{
    public class Windows_Handler : MonoBehaviour
    {
        public GameObject UWB_prefab;
        [SerializeField] private BaseUwbClientManager clientmanager;
        private WebBrowserClient webClient;

        void Start()
        {
            UWB_prefab = Instantiate(Resources.Load("UnityWebBrowser") as GameObject);
            clientmanager = GameObject.FindWithTag("Windows_Browser").GetComponent<BaseUwbClientManager>();

            webClient = clientmanager.browserClient;
            webClient.RegisterJsMethod<string>("_cb", _cb);
            webClient.RegisterJsMethod("Login", _login);
        }

        public void Update()
        {
        }

        private bool can_start()
        {
            var start = webClient.ReadySignalReceived;
            return start;
        }

        public void start_process(string function)
        {
            StartCoroutine(launch(function));
        }

        public IEnumerator launch(string function)
        {
            //Latency issue here that needs testing
            //UWB will not launch/pass ExecuteJS until ReadySignalReceived is true
            //But still needs a hard-wired delay for UWB to be completely loaded

            yield return new WaitUntil(can_start);
            yield return new WaitForSeconds(0.1f);
            Debug.Log("Sending Function to Relayer");
            webClient.ExecuteJs(function);

            var webObject = clientmanager.gameObject;

            // // webClient.LoadUrl(url);

            webObject.GetComponent<RawImage>().enabled = true;

        }

        public void LoadUrl(string url)
        {
            webClient.LoadUrl(url);
        }

        public void _cb(string json)
        {
            Debug.Log(json);
        }

        public void _login()
        {
            Debug.Log("Logged In.");
        }
    }
}
