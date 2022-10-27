using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Util
{
    public class MessageBox : MonoBehaviour
    {
        public Text Message;

        void Start()
        { }

        public void Show(string message)
        {
            Message.text = message;
        }
    }
}