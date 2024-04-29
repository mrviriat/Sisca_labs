namespace AzimutExportClient;

public class WPT_Route
{
    public string WPT_ID { get; set; }
    public string WPT_NAME { get; set; }

    public WPT_Route(string WPT_ID, string WPT_NAME)
    {
        this.WPT_ID = WPT_ID;
        this.WPT_NAME = WPT_NAME;
    }
}