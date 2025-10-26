using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Container;
using MessagePipe;
using Messages;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

namespace Sounds
{
    public class SoundsManager : MonoBehaviour
    {
        private SoundsConfig soundsConfig;
        PlayerLifetimeScope playerLifetimeScope;
        
        private IObjectPool<AudioSource> sourcePool = null!;
        private float despawnTimer = 2.0f;
        private GameObject parent;
        
        [Inject]
        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        private void Construct
            (
                SoundsConfig soundsConfig,
                PlayerLifetimeScope playerLifetimeScope,
                ISubscriber<PlaySoundMessage> PlaySoundSubscriber
            )
        {
            this.soundsConfig = soundsConfig;
            this.playerLifetimeScope = playerLifetimeScope;
            
            sourcePool = new ObjectPool<AudioSource>(
                                                     Create, //Метод создания объектов
                                                     OnGet, //Действие при извлечении из пула
                                                     OnRelease, //Действие при возврате в пул
                                                     DestroySource, //Очистка объектов (опционально)
                                                     false, //Коллекция для отслеживания объектов не используется (опционально)
                                                     200, //Минимальный размер пула
                                                     2000 //Максимальный размер пула
                                                    );
            parent = new GameObject("Sounds Parent");

            PlaySoundSubscriber.Subscribe(PlaySound);
        }

        private void PlaySound(PlaySoundMessage msg)
        {
            if (msg.SoundsSettings.isUISound)
            {
                Get(msg.SoundsSettings);
            }
            else if (Vector3.Distance(playerLifetimeScope.transform.position, msg.SoundsSettings.position) 
                  <= msg.SoundsSettings.DistanceToPlay)
            {
                Get(msg.SoundsSettings);
            }
        }
        
        private AudioSource Get(SoundsSettings settings, bool needToReturn = true)
        {
            var source = sourcePool.Get();
            var sourceTrans = source.transform;

            sourceTrans.position = settings.position;
            if (settings.parent)
            {
                sourceTrans.SetParent(settings.parent);
            }
            else
            {
                sourceTrans.SetParent(parent.transform);
            }
            source.volume = settings.volume;
            source.pitch = settings.pitch;
            source.minDistance = settings.MinDistance;
            source.maxDistance = settings.MaxDistance;
            source.PlayOneShot(settings.Clip);
            source.spatialBlend = settings.spatialBlend;

            if (needToReturn)
            {
                StartCoroutine(DespawnTimer(source));
            }

            return source;
        }
        
        private AudioSource Create() => Instantiate(soundsConfig.AudioSourcePrefab);
        private void OnRelease(AudioSource source) => source.transform.SetParent(null);
        private void OnGet(AudioSource source) { }
        private void DestroySource(AudioSource source) { }

        private IEnumerator DespawnTimer(AudioSource source)
        {
            yield return new WaitForSeconds(despawnTimer);
            if (source.gameObject)
            {
                Destroy(source.gameObject);
            }
        }
    }
}