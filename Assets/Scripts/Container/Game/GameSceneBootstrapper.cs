using BuildingsAndGrid;
using Container.Project;
using UI;
using UnityEngine;
using VContainer.Unity;

namespace Container.Game
{
    public sealed class GameSceneBootstrapper : MonoBehaviour
    {
        [SerializeField] private GameLifetimeScope gameScopePrefab;
        [SerializeField] private GridLifetimeScope gridLifetimeScope;
        [SerializeField] private CanvasLifetimeScope canvasLifetimeScope;

        private async void Awake()
        {
            await Localization.YG2Awaiter.WaitForSDKDataAsync();
            await Localization.LocalizationAwaiter.WaitUntilReadyAsync();
            
            var projectScope = LifetimeScope.Find<ProjectLifetimeScope>();
            var gameLifetimeScope = projectScope.CreateChildFromPrefab(gameScopePrefab);
            gameLifetimeScope.CreateChildFromPrefab(gridLifetimeScope);
            gameLifetimeScope.CreateChildFromPrefab(canvasLifetimeScope);
        }
    }
} 