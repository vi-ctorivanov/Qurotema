public class Shot {
    public string name;
    public FMODUnity.EventReference fmodEvent;
    public (string name, float value)[] parameters;

    public Shot(string name, FMODUnity.EventReference fmodEvent, (string name, float value)[] parameters) {
        this.name = name;
        this.fmodEvent = fmodEvent;
        this.parameters = parameters;
    }
}
