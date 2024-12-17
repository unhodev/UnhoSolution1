namespace SampleGame.Define;

public class HubReceiveChat
{
    public long playerid { get; set; }
    public string name { get; set; }
    public string message { get; set; }
    public DateTime time { get; set; }
}

public class HubOnPost
{
}