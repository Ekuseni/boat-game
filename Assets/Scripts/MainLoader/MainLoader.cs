using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainLoader : MonoBehaviour
{
    [SerializeField] private AssetReference worldSceneReference;

    private ProgressBar _progressBar;

    private void Start()
    {
        _progressBar = GetComponent<UIDocument>().rootVisualElement.Q<ProgressBar>("progress");
        LoadGame().Forget();
    }

    private async UniTaskVoid LoadGame()
    {
        var loaderScene = gameObject.scene;

        var worldHandle = Addressables.LoadSceneAsync(worldSceneReference, LoadSceneMode.Additive);

        while (!worldHandle.IsDone)
        {
            SetProgress(worldHandle.PercentComplete);
            await UniTask.Yield();
        }

        if (worldHandle.Status == AsyncOperationStatus.Failed)
        {
            Debug.LogError("Failed to load game scenes.");
            return;
        }

        SetProgress(1f);
        await UniTask.Yield();
        await SceneManager.UnloadSceneAsync(loaderScene);
    }

    private void SetProgress(float normalized)
    {
        if (_progressBar == null) return;
        _progressBar.value = normalized * 100f;
        _progressBar.title = $"Loading... {Mathf.RoundToInt(normalized * 100f)}%";
    }
}