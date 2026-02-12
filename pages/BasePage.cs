namespace alttrashcat_tests_csharp.pages
{
    public class BasePage
    {
        AltDriver driver;
        public static Action<string, string> AnnotateCallback;

        public AltDriver Driver { get => driver; set => driver = value; }
        public BasePage(AltDriver driver)
        {
            Driver = driver;
        }

        protected void Log(string message, string level = "info")
        {
            AnnotateCallback?.Invoke(message, level);
        }
    }
}
