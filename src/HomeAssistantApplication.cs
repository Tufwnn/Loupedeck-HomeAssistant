namespace Loupedeck.HomeAssistantByBatuPlugin
{
    using System;

    public class HomeAssistantApplication : ClientApplication
    {
        public HomeAssistantApplication()
        {
        }

        protected override String GetProcessName() => "";
        protected override String GetBundleName() => "";
    }
}
