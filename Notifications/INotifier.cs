namespace _Project.Scripts.Notifications
{
    public interface INotifier
    {
        public void Notify(string notifyData = null, bool accepted = false, bool rejected = false);
    }
}