[System.Serializable]
public class PlayerSaveData
{
    public string name;
    public int energy;
    public int fatigue;
    public int dayLv;

    public string item1;
    public string item2;
    public string item3;

    public float[] lvs = new float[6];
    public float[] exps = new float[6];
    public float[] maxExps = new float[6];
}

