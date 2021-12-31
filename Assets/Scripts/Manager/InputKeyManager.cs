using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Onka.Manager.InputKey
{
    public class InputKeyManager : SingletonMonoBehaviour<InputKeyManager>
    {
        public UnityAction onEscKeyPress = null;
        public UnityAction onF12KeyPress = null;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(onEscKeyPress != null)
                {
                    onEscKeyPress();
                }
            }
            if (Input.GetKeyDown(KeyCode.F12))
            {
                if(onF12KeyPress != null)
                {
                    onF12KeyPress();
                }
            }
        }
    }
}