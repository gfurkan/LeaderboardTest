using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;


namespace Backend
{
public class UILogin : MonoBehaviour
{
#region Fields

[SerializeField] private Button _loginButton;
[SerializeField] private TextMeshProUGUI _userIdText;
[SerializeField] private Transform _loginPanel,_userPanel;
[SerializeField] private GoogleLogInController _googleLogInController;


#endregion

#region Properties

#endregion

#region Unity Methods

private void OnEnable()
{
    _loginButton.onClick.AddListener(LoginButtonPressed);
    _googleLogInController.OnSignedIn += LoginController_SignedIn;
}

private void OnDisable()
{
    _loginButton.onClick.RemoveListener(LoginButtonPressed);
    _googleLogInController.OnSignedIn -= LoginController_SignedIn;
}

void Start()
{
    
}
void Update()
{
   
}

#endregion

#region Private Methods

private void LoginController_SignedIn(string playerName)
{
    _loginPanel.gameObject.SetActive(false);
    _userPanel.gameObject.SetActive(true);
    _userIdText.text = $"Name - {playerName}";
}
private async void LoginButtonPressed()
{
    await _googleLogInController.InitSignIn();
}
#endregion

#region PublicMethods

#endregion
}
}

