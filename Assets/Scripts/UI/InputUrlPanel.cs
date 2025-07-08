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
        [SerializeField] private Button _startARSceneButton;
        [SerializeField] private Button _startTestButton;
        [SerializeField] private Button _pasteBufferButton;

        private void Start()
        {
            _downloadButton.interactable = false;
            _startARSceneButton.interactable = false;
            _startTestButton.interactable = false;
            
            _urlInput.onValueChanged.AddListener(OnURLChanged);
            
            _downloadButton.onClick.AddListener(OnDownloadButtonClick);
            _startARSceneButton.onClick.AddListener(OnStartARSceneButtonClick);
            _startTestButton.onClick.AddListener(OnStartTestSceneButtonClick);
            _pasteBufferButton.onClick.AddListener(OnPasteBufferButtonClick);

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

        private async void OnDownloadButtonClick()
        {
            _loadingIndicator.SetActive(true);
            _downloadButton.interactable = false;
            
            var url = _urlInput.text;

            bool result = await ModelLoader.Instance.LoadModelFromURL(url);

            if (result)
            {
                _loadingIndicator.SetActive(false);
                _startTestButton.interactable = true;
                _startARSceneButton.interactable = true;
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

        private void OnStartARSceneButtonClick()
        {
            SceneManager.LoadScene(ARSceneName); 
        }
        
        private void OnStartTestSceneButtonClick()
        {
            SceneManager.LoadScene(TestSceneName); 
        }
        
        private void OnPasteBufferButtonClick()
        {
            string clipboardText = GUIUtility.systemCopyBuffer;
            
            if (!string.IsNullOrEmpty(clipboardText))
            {
                _urlInput.text = clipboardText;
                _urlInput.ForceLabelUpdate();
            }
        }

        private void OnDestroy()
        {
            _urlInput.onValueChanged.RemoveListener(OnURLChanged);
            
            _downloadButton.onClick.RemoveListener(OnDownloadButtonClick);
            _startARSceneButton.onClick.RemoveListener(OnStartARSceneButtonClick);
            _startTestButton.onClick.RemoveListener(OnStartTestSceneButtonClick);

            ModelLoader.Instance.OnSendLoadProgress -= OnUpdateProgress;
            ModelLoader.Instance.OnSendMessage -= OnUpdateStatus;
        }
    }
}