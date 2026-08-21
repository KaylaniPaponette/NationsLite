using UnityEngine;

[DefaultExecutionOrder(-100)]
public class Shell : MonoBehaviour
{
    public static Shell instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Debug.Log("Shell.init");
        instance = new GameObject("Shell", typeof(Shell)).GetComponent<Shell>();

        DontDestroyOnLoad(instance.gameObject);
    }

    void OnEnable()
    {
        Debug.Log("Shell.OnEnable");

        MouseInputManager.instance.mouseInputMap = InputManager.instance.mouseInputMap;
        MouseInputManager.instance.defaultInputHandler = GameModeManager.instance;
        InputManager.instance.mouseInputMap.SetHandler(MouseInputManager.instance);

        AssetManager.instance.LoadRepository();
        UIManager.instance.LoadViews();
        UIManager.instance.Init();

        AttractionManager.instance.LoadProfiles();

        GameModeManager.instance.EnterMode(new StandardMode());
        GameModeManager.instance.OnUpdate();
    }

    public void Update()
    {
        // Core
        InputManager.instance.OnUpdate();
        MouseInputManager.instance.OnUpdate();
        UIMenuManager.instance.OnUpdate();

        // Build Mode
        GameTimeManager.instance.OnUpdate();
        AttractionManager.instance.OnUpdate();
    }

    public void LateUpdate()
    {
        WorldUIManager.instance.OnLateUpdate(Camera.main);
    }
}
