using Il2CppSystem;
using UnityEngine;
using Action = System.Action;
using NotImplementedException = System.NotImplementedException;

namespace PowerwashSimAP;

public class Updatinator : MonoBehaviour
{
    public Action Action;
    /*
     ahhh~ perry the platypus, you are just in time to witness my latest invention-
     THE UPDATINATOR
     it will constantly do something!
     those annoying updates that can pause? NO MORE WILL THAT HAPPEN, as with my updatinator it will never stop
     */

    private void Awake() => DontDestroyOnLoad(gameObject);

    private void Update() => Action?.Invoke();
}