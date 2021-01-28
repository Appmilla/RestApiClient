namespace Appmilla.RestApiClient
{
    public enum ResponseTypes
    {
        Unknown,
        Success,
        HttpError,
        ConnectionError,
        DeserialisationError,
        Cancelled,
        AsyncFail,
        Timeout,
        Created,
        AutomapError
    }
}
