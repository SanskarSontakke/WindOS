using System.Collections.Generic;
using WindOS.Apps;
using Cosmos.System.Graphics;

namespace WindOS.System
{
    public class ProcessManager
    {
        public List<App> Apps { get; private set; }
        public App CurrentApp { get; private set; }

        public ProcessManager()
        {
            Apps = new List<App>();
        }

        public void RegisterApp(App app)
        {
            Apps.Add(app);
        }

        public void StartApp(string name, object args = null)
        {
            foreach (var app in Apps)
            {
                if (app.Name == name)
                {
                    if (CurrentApp != null) CurrentApp.Stop();
                    CurrentApp = app;

                    // Handle args if the app supports it
                    if (args != null && app is IArgConsumer consumer)
                    {
                        consumer.ConsumeArgs(args);
                    }

                    CurrentApp.Start();
                    return;
                }
            }
        }

        public void StopCurrentApp()
        {
             if (CurrentApp != null)
             {
                 CurrentApp.Stop();
                 CurrentApp = null;
             }
        }

        public void Update()
        {
            if (CurrentApp != null)
                CurrentApp.Update();
        }

        public void Draw(Canvas canvas)
        {
            if (CurrentApp != null)
                CurrentApp.Draw(canvas);
        }
    }

    public interface IArgConsumer
    {
        void ConsumeArgs(object args);
    }
}
