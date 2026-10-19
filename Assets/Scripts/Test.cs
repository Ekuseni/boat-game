using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    [SerializeReference] private InputActionReference test;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        test.action.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
