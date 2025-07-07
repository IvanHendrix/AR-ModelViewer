using Assets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class InputURLPanel : MonoBehaviour
    {
        private const string ARSceneName = "ARScene";
        private const string TestSceneName = "TestScene";
        
        [Header("UI")]
        [SerializeField] private GameObject _loadingIndicator;
        [SerializeField] private Slider _progressBar;
        [SerializeField] private TMP_InputField _urlInput;
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private Button _downloadButton;
        [SerializeField] private Button _startARButton;

        private void Start()
        {
            _downloadButton.interactable = false;
            _startARButton.interactable = false;
            
            _urlInput.onValueChanged.AddListener(OnURLChanged);
            _downloadButton.onClick.AddListener(OnDownloadClicked);
            _startARButton.onClick.AddListener(OnStartARClicked);

            ModelLoader.Instance.OnSendLoadProgress += OnUpdateProgress;
            ModelLoader.Instance.OnSendMessage += OnUpdateStatus;
        }

        private void OnUpdateStatus(string message)
        {
            _statusText.text = message;
        }

        private void OnUpdateProgress(float progressValue)
        {
            _progressBar.value =  progressValue;
        }

        private void OnURLChanged(string url)
        {
            _downloadButton.interactable = URLValidator.IsValidGLB(url);
        }

        private async void OnDownloadClicked()
        {
            _loadingIndicator.SetActive(true);
            _downloadButton.interactable = false;
            
            var url = _urlInput.text;

            bool result = await ModelLoader.Instance.LoadModelFromURL(url);

            if (result)
            {
                _loadingIndicator.SetActive(false);
                _startARButton.interactable = true;
            }
            else
            {
                ResetPanel();
            }
        }

        private void ResetPanel()
        {
            _progressBar.value = 0f;
            _urlInput.text = "";
            _loadingIndicator.SetActive(false);
            _downloadButton.interactable = true;
        }

        private void OnStartARClicked()
        {
#if UNITY_EDITOR
            SceneManager.LoadScene(TestSceneName);
#else
            SceneManager.LoadScene(ARSceneName); 
#endif
        }

        private void OnDestroy()
        {
            _urlInput.onValueChanged.RemoveListener(OnURLChanged);
            _downloadButton.onClick.RemoveListener(OnDownloadClicked);
            _startARButton.onClick.RemoveListener(OnStartARClicked);

            ModelLoader.Instance.OnSendLoadProgress -= OnUpdateProgress;
            ModelLoader.Instance.OnSendMessage -= OnUpdateStatus;
        }
    }
}