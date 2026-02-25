[System.Serializable]
public class ItemData    //json for Item Save data
{
    public string name; //name
    public string var;  //variable
    public int effect;  //effect scale
    public int rate;    //rate
    public string explain; //discription
    public ItemData(string _name, string _where, int _how, int _rare, string _explain)
    {
        name = _name;
        var = _where;
        effect = _how;
        rate = _rare;
        explain = _explain;
    }
}


