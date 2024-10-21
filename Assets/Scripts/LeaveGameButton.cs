using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaveGameButton : MonoBehaviour
{
    Button leaveGameButton;
    private void Awake()
    {
        leaveGameButton = GetComponent<Button>();
    }
    // Start is called before the first frame update
    void Start()
    {
        leaveGameButton.onClick.AddListener(FindObjectOfType<RoomManager>().LeaveGame);
            
    }
}
