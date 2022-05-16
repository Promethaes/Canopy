using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canopy
{
    [CreateAssetMenu(fileName = "Blackboard", menuName = "Canopy/Blackboard", order = 1)]
    public class Blackboard : ScriptableObject
    {
        [SerializeField]
        Dictionary<string, object> data = new Dictionary<string, object>();

        public void UpdateEntry(string name, object data)
        {
            this.data[name] = data;
        }

        public void SetBlackboardName(string name)
        {
            UpdateEntry("name", name);
        }

        //Causes immediate abort of execution of current
        //node and starts the update loop again
        public void InvokeInterrupt()
        {
            UpdateEntry("interrupt", true);
        }

        public T GetEntry<T>(string name)
        {
            try
            {
                //call to see if null
                data[name].ToString();
            }
            catch (System.Exception e)
            {
#if UNITY_EDITOR
                Debug.Log(e);
#endif
                return default(T);
            }


            return (T)data[name];
        }

        //Gets an entry whos contents belong to this object.
        //eg. this object's AINavigation component.
        public T GetEntryFromThis<T>(string name)
        {
            return GetEntry<T>(GetOwnerName() + name);
        }

        //returns name of the game object that owns the BT controller
        public string GetOwnerName()
        {
            return data["name"] as string;
        }

    }

}