using System;
using System.Threading.Tasks;
using UnityEngine;

namespace TowerDefense.Background {
    public class BackgroundRunner : MonoBehaviour {
        public static BackgroundRunner Current;
        void OnEnable() => Current = this;
    }
    
    public class WaitForTask : CustomYieldInstruction {
        bool done = false;
        public override bool keepWaiting => !done;

        public WaitForTask(Task task) => exec( task );
        
        async void exec(Task task) {
            await task;
            done = true;
        }
    }
    
    public class WaitForTask<T> : CustomYieldInstruction {
        bool done = false;
        public override bool keepWaiting => !done;

        public WaitForTask(Task<T> task, Action<T> onResult) => exec( task, onResult );
        
        async void exec(Task<T> task, Action<T> onResult) {
            onResult?.Invoke( await task );
            done = true;
        }
    }
}
