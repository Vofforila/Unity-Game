using Data;
using Database;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using TMPro;
using UI;
using UnityEngine;

namespace Auth
{
    public class FirebaseController : MonoBehaviour
    {
        [Header("Test")]
        public bool createTest = false;
        public bool manualTest = false;
        private bool manualTest1 = false;
        private bool manualTest2 = false;
        private bool manualTest3 = false;
        public bool test1 = false;
        public bool test2 = false;
        public bool test3 = false;

        [Header("References to other scripts")]
        [SerializeField]
        internal UIManager canvasManager;

        [Header("Data")]
        [SerializeField] private LocalData localData;

        [Header("Firestore")]
        [SerializeField] private Firestore firestore;

        [Header("Firebase")]
        public DependencyStatus status;

        private FirebaseAuth auth;
        private FirebaseUser user;

        [Header("Register")]
        public TMP_InputField userNameRegisterInput;
        public TMP_Text warningRegisterUserName;
        public TMP_InputField emailRegisterInput;
        public TMP_Text warningRegisterEmail;
        public TMP_InputField passwordRegisterInput;
        public TMP_Text warningRegisterPassword;
        public TMP_InputField confirmPasswordRegisterInput;

        [Header("Login")]
        public GameObject loginWrongCredentialsPanel;
        public TMP_InputField loginEmailInput;
        public TMP_Text warningLoginEmail;
        public TMP_InputField loginPasswordInput;
        public TMP_Text warningLoginPassword;

        [Header("ForgotPassword")]
        public TMP_InputField forgotPasswordEmailInput;
        public TMP_Text warningForgotPasswordEmail;
        public GameObject forgotPasswordConfirmationPanel;

        private void Test()
        {
            canvasManager.LoadingMainMenu("test1@gmail.com");
        }

        private void Test2()
        {
            canvasManager.LoadingMainMenu("test2@gmail.com");
        }

        private void Test3()
        {
            canvasManager.LoadingMainMenu("test3@gmail.com");
        }

        private void Awake()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                status = task.Result;
                if (status == DependencyStatus.Available)
                {
                    auth = FirebaseAuth.DefaultInstance;
                    if (test1 == true)
                    {
                        StartCoroutine(LoginTest("test1@gmail.com", "test123"));
                    }
                    if (test2 == true)
                    {
                        StartCoroutine(LoginTest("test2@gmail.com", "test123"));
                    }
                    if (test3 == true)
                    {
                        StartCoroutine(LoginTest("test3@gmail.com", "test123"));
                    }
                    if (createTest == true)
                    {
                        StartCoroutine(Register("test1@gmail.com", "test123", "Test1"));
                        StartCoroutine(Register("test2@gmail.com", "test123", "Test2"));
                        StartCoroutine(Register("test3@gmail.com", "test123", "Test3"));
                    }
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependecies " + status);
                }
            });
        }

        #region TestGUI

        // Test GUI
        private void OnGUI()
        {
            if (manualTest == true)
            {
                if (GUI.Button(new Rect(0, 0, 200, 40), "Test1"))
                {
                    manualTest1 = true;
                    StartCoroutine(LoginTest("test1@gmail.com", "test123"));
                    manualTest = false;
                }
                if (GUI.Button(new Rect(0, 40, 200, 40), "Test2"))
                {
                    manualTest2 = true;
                    StartCoroutine(LoginTest("test2@gmail.com", "test123"));
                    manualTest = false;
                }
                if (GUI.Button(new Rect(0, 80, 200, 40), "Test3"))
                {
                    manualTest3 = true;
                    StartCoroutine(LoginTest("test3@gmail.com", "test123"));
                    manualTest = false;
                }
            }
        }

        #endregion TestGUI

        public void Register()
        {
            StartCoroutine(Register(emailRegisterInput.text, passwordRegisterInput.text, userNameRegisterInput.text));
        }

        public void Login()
        {
            warningLoginEmail.text = "";
            warningLoginPassword.text = "";
            StartCoroutine(Login(loginEmailInput.text, loginPasswordInput.text));
        }

        public void ForgotPassword()
        {
            StartCoroutine(ForgotPassword(forgotPasswordEmailInput.text));
        }

        private IEnumerator Register(string _email, string _password, string _username)
        {
            // Check if the provided username is empty
            if (_username == "")
            {
                warningRegisterUserName.text = "Missing Username";
            }
            // Check if the provided passwords do not match
            else if (passwordRegisterInput.text != confirmPasswordRegisterInput.text)
            {
                warningRegisterPassword.text = "Password doesn't match";
            }
            else
            {
                // Attempt to create a new user with the provided email and password
                var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

                // Wait until the user creation task is completed
                yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

                // Check if there was an exception during user creation
                if (RegisterTask.Exception != null)
                {
                    // Extract the FirebaseException and get the error code
                    FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                    AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                    // Handle different error cases
                    switch (errorCode)
                    {
                        case AuthError.MissingEmail:
                            warningRegisterEmail.text = "Missing Email";
                            break;

                        case AuthError.MissingPassword:
                            warningRegisterPassword.text = "Missing Password";
                            break;

                        case AuthError.WeakPassword:
                            warningRegisterPassword.text = "Weak Password";
                            break;

                        case AuthError.EmailAlreadyInUse:
                            warningRegisterEmail.text = "Email already in use";
                            break;
                    }
                }
                else
                {
                    // If user creation is successful, get the user information
                    user = RegisterTask.Result.User;

                    // Check if the user information is not null
                    if (user != null)
                    {
                        // Create a user profile with the provided username
                        UserProfile profile = new UserProfile { DisplayName = _username };

                        // Update the user's profile with the provided information
                        var ProfileTask = user.UpdateUserProfileAsync(profile);

                        // Wait until the profile update task is completed
                        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                        // Check if there was an exception during profile update
                        if (ProfileTask.Exception != null)
                        {
                            // Extract the FirebaseException and get the error code
                            FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        }
                        else
                        {
                            firestore.RegisterUser(_username, _email);

                            userNameRegisterInput.text = "";
                            emailRegisterInput.text = "";
                            passwordRegisterInput.text = "";
                            confirmPasswordRegisterInput.text = "";

                            UIManager.Instance.OpenLoginPanel();
                        }
                    }
                }
            }
        }

        private IEnumerator LoginTest(string _email, string _password)
        {
            // Attempt to sign in the user with the provided email and password
            var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);

            // Wait until the login task is completed
            yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

            // Check if there was an exception during the login process
            if (LoginTask.Exception != null)
            {
                // Extract the FirebaseException and get the error code
                FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                // Handle different error cases
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        warningLoginEmail.text = "Missing Email";
                        break;

                    case AuthError.InvalidEmail:
                        warningLoginEmail.text = "Invalid Email";
                        break;

                    case AuthError.MissingPassword:
                        warningLoginPassword.text = "Missing Password";
                        break;

                    case AuthError.Failure:
                        // Display a panel for wrong credentials and start a timer to hide it
                        StartCoroutine(LoginWrongCredentialsPanelTimer());
                        break;
                }
            }
            else
            {
                user = LoginTask.Result.User;

                if (test1 == true || manualTest1 == true)
                {
                    Test();
                    test1 = false;
                }
                else if (test2 == true || manualTest2 == true)
                {
                    Test2();
                    test2 = false;
                }
                else if (test3 == true || manualTest3 == true)
                {
                    Test3();
                    test3 = false;
                }
            }
        }

        private IEnumerator Login(string _email, string _password)
        {
            // Attempt to sign in the user with the provided email and password
            var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);

            // Wait until the login task is completed
            yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

            // Check if there was an exception during the login process
            if (LoginTask.Exception != null)
            {
                // Extract the FirebaseException and get the error code
                FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                // Handle different error cases
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        warningLoginEmail.text = "Missing Email";
                        break;

                    case AuthError.InvalidEmail:
                        warningLoginEmail.text = "Invalid Email";
                        break;

                    case AuthError.MissingPassword:
                        warningLoginPassword.text = "Missing Password";
                        break;

                    case AuthError.Failure:
                        // Display a panel for wrong credentials and start a timer to hide it
                        StartCoroutine(LoginWrongCredentialsPanelTimer());
                        break;
                }
            }
            else
            {
                user = LoginTask.Result.User;
                Debug.Log("Login");
                canvasManager.LoadingMainMenu(_email);
            }
        }

        private IEnumerator ForgotPassword(string email)
        {
            // Initiate the password reset process by sending an email to the provided address
            var resetTask = auth.SendPasswordResetEmailAsync(email);

            // Wait until the password reset task is completed
            yield return new WaitUntil(() => resetTask.IsCompleted);

            // Check if there was an exception during the password reset process
            if (resetTask.Exception != null)
            {
                FirebaseException firebaseEx = resetTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                // Handle different error cases
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        warningForgotPasswordEmail.text = "Missing Email";
                        break;
                }
            }
            else
            {
                forgotPasswordConfirmationPanel.SetActive(true);
            }
        }

        public IEnumerator LoginWrongCredentialsPanelTimer()
        {
            loginWrongCredentialsPanel.SetActive(true);
            yield return new WaitForSecondsRealtime(10);
        }
    }
}