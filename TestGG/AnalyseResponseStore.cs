using TestGG;

public class AnalyseResponseStore
{
    // The private static instance of the class
    private static AnalyseResponseStore _instance;

    // The stored AnalyseResponse object
    public AnalyseResponse CurrentAnalyseResponse { get; private set; }

    // Private constructor to prevent instantiation from outside
    private AnalyseResponseStore() { }

    // Public method to get the single instance of the class
    public static AnalyseResponseStore Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AnalyseResponseStore();
            }
            return _instance;
        }
    }

    // Method to set the AnalyseResponse object
    public void SetAnalyseResponse(AnalyseResponse response)
    {
        CurrentAnalyseResponse = response;
    }

    // Method to clear the stored data
    public void ClearAnalyseResponse()
    {
        CurrentAnalyseResponse = null;
    }
}
