using BuildingsAndGrid;
using Container.Project;
using UnityEngine;
using VContainer.Unity;

namespace Container.Game
{
    public sealed class GameSceneBootstrapper : MonoBehaviour
    {
        [SerializeField] private GameLifetimeScope gameScopePrefab;
        [SerializeField] private GridLifetimeScope gridLifetimeScope;

        private async void Awake()
        {
            await Localization.YG2Awaiter.WaitForSDKDataAsync();
            await Localization.LocalizationAwaiter.WaitUntilReadyAsync();
            
            var projectScope = LifetimeScope.Find<ProjectLifetimeScope>();
            var gameLifetimeScope = projectScope.CreateChildFromPrefab(gameScopePrefab);
            gameLifetimeScope.CreateChildFromPrefab(gridLifetimeScope);
        }
    }
} 