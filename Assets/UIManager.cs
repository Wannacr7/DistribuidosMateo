using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreHud;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreHud.text = Projectile.score.ToString();

    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene("Demo_Scene");
    }
}
