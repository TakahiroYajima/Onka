
public class Event_AfterGetEntranceKey : EventBase
{
    protected override void EventActive()
    {
        base.EventActive();
        InitiationContact();
    }
}
