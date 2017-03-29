using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int currentStage;
    public int life;
    public Text StageText;
    public Transform[] Lifes;

    // Use this for initialization
    void Start()
    {
        SetAllinActive();
        SetStageActive(currentStage);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetAllinActive()
    {
        this.gameObject.transform.Find("Stage1").gameObject.SetActive(false);
        this.gameObject.transform.Find("Stage2").gameObject.SetActive(false);
    }

    public void SetStageActive(int stageid)
    {
        this.gameObject.transform.Find("Stage" + stageid).gameObject.SetActive(true);
        StageText.text = "STAGE " + stageid;
    }

    public void ResetStage()
    {
        life = 4;
    }

    public void UpdateLife(int delta)
    {
        life += delta;
        if (life == 3)
        {
            Lifes[0].gameObject.SetActive(true);
            Lifes[1].gameObject.SetActive(true);
        }
        if (life == 2)
        {
            Lifes[0].gameObject.SetActive(true);
            Lifes[1].gameObject.SetActive(false);
        }
        if (life == 1)
        {
            Lifes[0].gameObject.SetActive(false);
            Lifes[1].gameObject.SetActive(false);
        }
        if (life <= 0)
        {
            //gameover
            StageText.text = "GameOver";
            StartCoroutine(WaitForEndScene("GameOver"));

        }
    }

    public void NextStage()
    {
        ResetStage();

        if (currentStage == 1)
        {
            StartCoroutine(WaitBulletFly());
        }
        else if (currentStage == 2)
        {
            
            //Win
            StartCoroutine(WaitForEndScene("Win"));
        }
    }

    IEnumerator WaitBulletFly()
    {
        currentStage++;
        yield return new WaitForSeconds(3);
        SetAllinActive();
        SetStageActive(currentStage);
        GameObject.Find("Ball").GetComponent<BallController>().ResetBall();

    }
    IEnumerator WaitForEndScene(string sceneName)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }
}
