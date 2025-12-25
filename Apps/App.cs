using Cosmos.System.Graphics;
using System.Drawing;

namespace WindOS.Apps
{
    public abstract class App
    {
        public string Name { get; private set; }
        public bool IsRunning { get; private set; }

        protected App(string name)
        {
            Name = name;
            IsRunning = false;
        }

        public void Start()
        {
            IsRunning = true;
            OnStart();
        }

        public void Stop()
        {
            IsRunning = false;
            OnStop();
        }

        public virtual void OnStart() { }
        public virtual void OnStop() { }
        public abstract void Update();
        public abstract void Draw(Canvas canvas);
    }
}
