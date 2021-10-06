using Unity.Entities;
using Unity.Tiny;
using Unity.Tiny.UI;
using Unity.Scenes;
using Unity.Tiny.Input;

[AlwaysUpdateSystem]
public class LoadingSystem : SystemBase
{
    bool isLoading = true;
    float timer = 0;
    protected override void OnUpdate()
    {
        var uiSys = World.GetExistingSystem<ProcessUIEvents>();
        var loadingEntity = uiSys.GetEntityByUIName("Loading");
        var startEntity = uiSys.GetEntityByUIName("StartMenu");
        var startButonEntity = uiSys.GetEntityByUIName("StartButton");


        if (startButonEntity == Entity.Null)
            return;
        var startButtonState = GetComponent<UIState>(startButonEntity);

        

        if (startButtonState.IsClicked)
        {
            var loadingTransformFinish = GetComponent<RectTransform>(loadingEntity);
            loadingTransformFinish.Hidden = true;
            SetComponent(loadingEntity, loadingTransformFinish);

            var startTransformFinish = GetComponent<RectTransform>(startEntity);
            startTransformFinish.Hidden = true;
            SetComponent(startEntity, startTransformFinish);

            var sceneSystem = World.GetExistingSystem<SceneSystem>();
            var playSceneEntity = GetSingletonEntity<Lv1>();
            var playScene = EntityManager.GetComponentData<SceneReference>(playSceneEntity);

            sceneSystem.LoadSceneAsync(playScene.SceneGUID, new SceneSystem.LoadParameters { AutoLoad = true, Flags = SceneLoadFlags.LoadAdditive });
        }

        if (!isLoading)
            return;

        timer += Time.DeltaTime;

        var isReady = false;
        Entities.WithAll<Image2DLoadFromFile>().ForEach((Entity e, ref Image2D img) =>
        {
            if (img.status == ImageStatus.LoadError)
                Debug.Log("Error loading images");

            isReady = true;
        }).WithStructuralChanges().Run();

        if(timer>3f)
            isLoading = isReady;
        
        var loadingTransform = GetComponent<RectTransform>(loadingEntity);
        loadingTransform.Hidden = !isLoading;
        SetComponent(loadingEntity, loadingTransform);

        var startTransform = GetComponent<RectTransform>(startEntity);
        startTransform.Hidden = isLoading;
        SetComponent(startEntity, startTransform);
    }
}
