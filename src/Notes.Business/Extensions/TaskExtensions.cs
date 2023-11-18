namespace Notes.Business.Extensions
{
    public static class TaskExtensions
    {
        public static void Deconstruct<T>(
            this Task<TryResult<T>> resultTask, 
            out Task<bool> successTask, 
            out Task<ResultErrors> errorsTask, 
            out Task<T?> dataTask
        )
            where T: class
        {
            successTask = resultTask.ContinueWith(t => t.Result.Success);
            errorsTask = resultTask.ContinueWith(t => t.Result.Errors);
            dataTask = resultTask.ContinueWith(t => t.Result.Data);
        }
    }
}
