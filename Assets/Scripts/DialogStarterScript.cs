using UnityEngine;
using cherrydev;

namespace DialogNodeBasedSystem.Scripts
{
    public class DialogStarterScript : MonoBehaviour
    {
        [SerializeField] private DialogBehaviour dialogBehaviour;
        [SerializeField] private DialogNodeGraph dialogGraph;

        private void Start()
        {
            dialogBehaviour.StartDialog(dialogGraph);
        }
    }
}

