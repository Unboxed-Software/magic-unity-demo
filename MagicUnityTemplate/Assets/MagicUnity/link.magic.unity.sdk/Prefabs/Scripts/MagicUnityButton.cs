using System.Collections;
using System.Collections.Generic;
using link.magic.unity.sdk;
using UnityEngine;
using UnityEngine.UI;
using WindowsHandler;

public class MagicUnityButton : MonoBehaviour
{
    
    public GameObject windows_webView;

    public Magic magic;
    public Text result;
    // Start is called before the first frame update
    void Start()
    {
       magic = new Magic("pk_live_2C8DAF27FCBA05C9");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void Login()
    {
        #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        var token = await magic.Auth.LoginWithEmailOtp("nicholasobri@gmail.com");
        // StartCoroutine("_login");

        // windows_webView = GameObject.FindWithTag("win_web");
        // windows_webView.GetComponent<Windows_Handler>().start_process("Login");

        // var magic_i =  GameObject.FindWithTag("magic").GetComponent<MagicUnity>().magic;
        // Debug.Log(magic_i);
        // var token = await magic.Auth.LoginWithEmailOtp("nicholasobri@gmail.com");
        #endif

        // Debug.Log("logging in...");
        // Magic magic = new Magic("pk_live_2C8DAF27FCBA05C9");
        // var token = await magic.Auth.LoginWithEmailOtp("nicholasobri@gmail.com");
        // Debug.Log("Sending Token...");
        // result.text = $"token {token}";
        // Debug.Log("token: " + token);
    }

    public IEnumerator _login()
    {
        yield return new WaitForSeconds(1.0f);
        
    }

    public async void GetMetadata()
    {
        Magic magic = Magic.Instance;
        var metadata = await magic.User.GetMetadata();
        result.text = $"Metadata Email: {metadata.email} \n Public Address: {metadata.publicAddress}";
    }

    public async void Logout()
    {
        Magic magic = Magic.Instance;
        var isLogout = await magic.User.Logout();
        result.text = $"Logout: {isLogout.ToString()}";
    }
}
