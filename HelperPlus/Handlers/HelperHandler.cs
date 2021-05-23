namespace HelperPlus.Handlers
{
    public class HelperHandler
    {
        public float ServerFps { get; private set; }
        public HelperHandler()
        {
            Synapse.Api.Events.EventHandler.Get.Server.UpdateEvent += Server_UpdateEvent;
        }

        private void Server_UpdateEvent()
        {
            ServerFps = 1.0f / UnityEngine.Time.smoothDeltaTime;
        }
    }
}
