using System;

namespace Starfleet.Ops.Utility
{
    public class AppEnvironment
    {
        public static AppEnvironment Current { get; } = new AppEnvironment();

        public string AppData => AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
    }
}