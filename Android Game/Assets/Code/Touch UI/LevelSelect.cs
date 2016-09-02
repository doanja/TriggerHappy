using UnityEngine;

public class LevelSelect : MonoBehaviour {

    public string level1;
    public string level2;
    public string level3;
    public string level4;
    public string level5;
    public string backTo;

    public void Level1()
    {
        Application.LoadLevel(level1);
    }

    public void Level2()
    {
        Application.LoadLevel(level2);
    }

    public void Level3()
    {
        Application.LoadLevel(level3);
    }

    public void Level4()
    {
        Application.LoadLevel(level4);
    }

    public void Level5()
    {
        Application.LoadLevel(level5);
    }

    public void Back()
    {
        Application.LoadLevel(backTo);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
