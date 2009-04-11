namespace Spring.Data.Generic
{
    public interface IBatchExecutorFactory
    {
        IBatchExecutor GetExecutor();
    }
}