using System;
using System.Threading.Tasks;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Firebase;
using Firebase.Auth;
using TMPro;

namespace Backend
{
    public class GoogleLogInController : MonoBehaviour
    {
        #region Fields

        public event Action<string> OnSignedIn;
        private FirebaseAuth _firebaseAuth;
        private FirebaseUser _firebaseUser;
        [SerializeField] private TextMeshProUGUI _errorText;
        private int a = 0;
        
        #endregion

        #region Unity Methods

        private async void Start()
        {
            // Firebase Authentication başlatılıyor
            await InitializeFirebase();
        }

        private async Task InitializeFirebase()
        {
            // Firebase'i başlatma
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus == DependencyStatus.Available)
            {
                _firebaseAuth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase initialized successfully.");
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                _errorText.text = $"Could not resolve all Firebase dependencies: {dependencyStatus}";
            }
        }

        #endregion

        #region Private Methods

        internal void ProcessAuthentication(SignInStatus status)
        {
            if (status == SignInStatus.Success)
            {
                string name = PlayGamesPlatform.Instance.GetUserDisplayName();
                OnSignedIn?.Invoke(name);
                Debug.Log("Google Play Games signed in successfully. Fetching token...");
                
                // Sunucu tarafı token alınır ve Firebase'e giriş yapılır
                SignInToFirebase();
            }
            else
            {
                Debug.LogError("Login failed!");
                _errorText.text = "Login failed!";
            }
        }

        private void SignInToFirebase()
        {
            try
            {
                // Google Play Games'ten sunucu tarafı erişim için token alınır
                PlayGamesPlatform.Instance.RequestServerSideAccess(false, (serverAuthCode) =>
                {
                    if (!string.IsNullOrEmpty(serverAuthCode))
                    {
                        Debug.Log("Server Auth Code: " + serverAuthCode);

                        // Google Play Games token'ını Firebase'e gönderip oturum açıyoruz
                        Credential credential = PlayGamesAuthProvider.GetCredential(serverAuthCode);
                        _firebaseAuth.SignInWithCredentialAsync(credential).ContinueWith(task =>
                        {
                            if (task.IsCanceled)
                            {
                                Debug.LogError("SignInWithCredentialAsync was canceled.");
                                _errorText.text = "SignInWithCredentialAsync was canceled.";
                                return;
                            }
                            if (task.IsFaulted)
                            {
                                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                                _errorText.text = "SignInWithCredentialAsync encountered an error: " + task.Exception;
                                return;
                            }

                            FirebaseUser newUser = task.Result;
                            Debug.Log("Firebase signed in successfully. User ID: " + newUser.UserId);
                            _errorText.text = "Signed in with Firebase successfully!";
                        });
                    }
                    else
                    {
                        Debug.LogError("Failed to get server auth code from Google Play Games.");
                        _errorText.text = "Failed to get server auth code from Google Play Games.";
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.LogError("Firebase sign-in failed: " + ex.Message);
                _errorText.text = "Firebase sign-in failed: " + ex.Message;
            }
        }

        #endregion

        #region Public Methods

        // Giriş işlemini başlatan metod
        public async Task InitSignIn()
        {
            // PlayGamesPlatform.Instance.Authenticate() çağrısı otomatik giriş sonucunu alır
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
            _errorText.text =($"Sıgning In - {a}");
            a++;
        }
        
        #endregion
    }
}
