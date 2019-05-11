using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AwaitTriggerExtensions : MonoBehaviour
{
    private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
    private CancellationToken _token;
    private SynchronizationContext _context;

    public async Task CancelableAsync(IEnumerator routine, bool throwCancellationRequest = false)
    {
        _token = _tokenSource.Token;
        _context = SynchronizationContext.Current;
        await Task.Run(() => {
            if (IsCanceled(throwCancellationRequest))
            {
                return;
            }

            _context.Post((state) =>
            {
                StartCoroutine(routine);
            }, _token);
        }, _token);
    }

    public void Cancel()
    {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
    }

    public bool IsCanceled(bool throwCancellationRequest = false)
    {
        if (throwCancellationRequest)
        {
            _token.ThrowIfCancellationRequested();
        }
        return _token.IsCancellationRequested;
    }

    private void OnDestroy()
    {
        Cancel();
    }
}
