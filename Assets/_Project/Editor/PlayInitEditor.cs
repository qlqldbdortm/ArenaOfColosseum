using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Unity 개발 환경에서 Play를 할 시, Init Scene에서 시작되게 하는 Editor Class
/// </summary>
[InitializeOnLoad]
public class PlayInitEditor
{
    /// <summary>
    /// Play 버튼을 클릭하면 동작하는 부분
    /// </summary>
    static PlayInitEditor()
    {
        string scenePath = EditorBuildSettings.scenes[0].path; // 0번 Scene이 Init 씬이므로 수정할 필요는 없어보임.
        SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
        EditorSceneManager.playModeStartScene = sceneAsset;
    }
    
    /// <summary>
    /// Debug/PlayThisScene을 클릭하면 동작하는 Method
    /// </summary>
    [MenuItem("Debug/PlayThisScene")]
    public static void StartFromThisScene()
    {
        EditorSceneManager.playModeStartScene = null;
        EditorApplication.isPlaying = true;
    }
}
