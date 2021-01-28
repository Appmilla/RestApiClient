namespace Appmilla.RestApiClient
{
    public class ApiServiceResponse<T>
    {
        public ResponseTypes ResponseType { get; set; }

        public T Result { get; set; }

        public ErrorModel Error { get; set; }

        public ApiServiceResponse()
        {
            Error = new ErrorModel();
        }
    }
}
