# Repository contains a repro for possible issue related to the Microsoft.AspNetCore.Mvc.Testing

## Steps to reproduce

1. Start the web app. Go to the `src/Microsoft.AspNetCore.Mvc.Testing` and call `dotnet run`. Let in run in the background.
2. In second terminal or IDE start the tests located in the `src/Tests`
3. `ShouldRetry_OnRemoteInstance` should pass
4. `ShouldRetry_OnInMemoryInstance` should fail with error

```csharp
System.Net.Http.HttpRequestException : Error while copying content to a stream.
---- System.IO.IOException :
-------- System.IO.IOException : The client aborted the request.
   at System.Net.Http.HttpContent.LoadIntoBufferAsyncCore(Task serializeToStreamTask, MemoryStream tempBuffer)
   at System.Net.Http.HttpClient.FinishSendAsyncBuffered(Task`1 sendTask, HttpRequestMessage request, CancellationTokenSource cts, Boolean disposeCts)
   at Tests.CheckRetry.ShouldRetry_OnInMemoryInstance() in C:\dev\testserverIssue\src\Tests\CheckRetry.cs:line 23
--- End of stack trace from previous location where exception was thrown ---
----- Inner Stack Trace -----
   at Microsoft.AspNetCore.TestHost.ResponseBodyReaderStream.CheckAborted()
   at Microsoft.AspNetCore.TestHost.ResponseBodyReaderStream.ReadAsync(Byte[] buffer, Int32 offset, Int32 count, CancellationToken cancellationToken)
   at System.IO.Stream.CopyToAsyncInternal(Stream destination, Int32 bufferSize, CancellationToken cancellationToken)
   at System.Net.Http.HttpContent.LoadIntoBufferAsyncCore(Task serializeToStreamTask, MemoryStream tempBuffer)
----- Inner Stack Trace -----
```
