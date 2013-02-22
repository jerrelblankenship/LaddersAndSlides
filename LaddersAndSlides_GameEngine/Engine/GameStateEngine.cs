namespace LaddersAndSlidesW8UI.Processing
{
    public enum GameStateEngine
    {
        InitialGameState,
        ArrowEvent,
        ArrowDelayedEvent,
        PlayerEvent,
        TurnComplete,
        PlayerSpecialMoveTransportCalculateEvent,
        GetNextPlayer,
        PlayerSpecialMoveTransportMoveEvent
    }
}