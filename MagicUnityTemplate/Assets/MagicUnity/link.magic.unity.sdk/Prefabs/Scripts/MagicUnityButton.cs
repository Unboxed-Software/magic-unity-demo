using System.Collections;
using System.Collections.Generic;
using link.magic.unity.sdk;
using UnityEngine;
using UnityEngine.UI;

public class MagicUnityButton : MonoBehaviour
{
    public Text result;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void Login()
    {
        Debug.Log("logging in...");
        Magic magic = Magic.Instance;
        Debug.Log(magic);
        var token = await magic.Auth.LoginWithEmailOtp("jamesrp13@gmail.com");
        Debug.Log("token: " + token);
        result.text = $"token {token}";
    }

    public async void GetMetadata()
    {
        Magic magic = Magic.Instance;
        var metadata = await magic.User.GetMetadata();
        Debug.Log($"Metadata Email: {metadata.email} \n Public Address: {metadata.publicAddress}");
        result.text = $"Metadata Email: {metadata.email} \n Public Address: {metadata.publicAddress}";
    }

    public async void Logout()
    {
        Magic magic = Magic.Instance;
        var isLogout = await magic.User.Logout();
        Debug.Log($"Logout: {isLogout.ToString()}");
        result.text = $"Logout: {isLogout.ToString()}";
    }
}
