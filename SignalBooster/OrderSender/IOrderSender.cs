namespace Synapse.SignalBoosterExample.OrderSender
{
    public interface IOrderSender
    {
        void Submit(string orderJson, string apiUrl);
    }
}