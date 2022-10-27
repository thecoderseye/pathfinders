using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class Alert
    {
        public Alert(string message, float duration = 2.0f)
        {
            var request = Resources.LoadAsync<MessageBox>("MessageBox");
            request.completed += operation =>
            {
                var messageBox = GameObject.Instantiate(request.asset) as MessageBox;
                messageBox?.Show(message);
                GameObject.Destroy(messageBox.gameObject, duration);
            };
        }
    }
}