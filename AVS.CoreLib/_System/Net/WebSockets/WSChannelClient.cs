using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AVS.CoreLib._System.Net.WebSockets
{
    public class WSChannelClient : IDisposable
    {
        private readonly ClientWebSocket _webSocket;
        private readonly Uri _addressUri;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        public bool DiagnosticEnabled { get; set; }
        
        public WSChannelClient(string uriString) :
            this(new Uri(uriString))
        {
        }

        public WSChannelClient(Uri addressUri)
        {
            _webSocket = new ClientWebSocket();
            _addressUri = addressUri;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        public Task SendAsync(IChannelCommand command)
        {
            ArraySegment<byte> messageToSend = GetMessageInBytes(command);
            return _webSocket.SendAsync(messageToSend, WebSocketMessageType, true, _cancellationToken);
        }

        private ArraySegment<byte> GetMessageInBytes(IChannelCommand command)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(command.ToJsonMessage());
            return new ArraySegment<byte>(bytes);
        }

        private WebSocketMessageType WebSocketMessageType => WebSocketMessageType.Text;

        public async Task ConnectAsync()
        {
            try
            {
                if(IsConnected)
                    return;

                await _webSocket.ConnectAsync(_addressUri, _cancellationToken)
                          .ConfigureAwait(false);

                Task task = Task.Run(this.RunAsync, _cancellationToken);
            }
            catch (Exception ex)
            {
                RaiseConnectionError(ex);
                RaiseConnectionClosed();
            }
        }

        private async Task RunAsync()
        {
            try
            {
                /*We define a certain constant which will represent
                  size of received data. It is established by us and 
                  we can set any value. We know that in this case the size of the sent
                  data is very small.
                */
                const int maxMessageSize = 2048;

                // Buffer for received bits.
                ArraySegment<byte> receivedDataBuffer = new ArraySegment<byte>(new byte[maxMessageSize]);

                MemoryStream memoryStream = new MemoryStream();

                // Checks WebSocket state.
                while (IsConnected && !_cancellationToken.IsCancellationRequested)
                {
                    // Reads data.
                    WebSocketReceiveResult webSocketReceiveResult =
                        await ReadMessage(receivedDataBuffer, memoryStream).ConfigureAwait(false);

                    if (webSocketReceiveResult.MessageType != WebSocketMessageType.Close)
                    {
                        memoryStream.Position = 0;
                        OnNewMessage(memoryStream);
                    }

                    memoryStream.Position = 0;
                    memoryStream.SetLength(0);
                }
            }
            catch (Exception ex)
            {
                if (!(ex is OperationCanceledException) ||
                    !_cancellationToken.IsCancellationRequested)
                {
                    RaiseConnectionError(ex);
                }
            }

            if (_webSocket.State != WebSocketState.CloseReceived &&
                _webSocket.State != WebSocketState.Closed)
            {
                await CloseWebSocket().ConfigureAwait(false);
            }

            RaiseConnectionClosed();
        }

        private async Task CloseWebSocket()
        {
            try
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                                            String.Empty,
                                            CancellationToken.None)
                                .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<WebSocketReceiveResult> ReadMessage(ArraySegment<byte> receivedDataBuffer, MemoryStream memoryStream)
        {
            WebSocketReceiveResult webSocketReceiveResult;

            do
            {
                webSocketReceiveResult =
                    await _webSocket.ReceiveAsync(receivedDataBuffer, _cancellationToken)
                                    .ConfigureAwait(false);

                await memoryStream.WriteAsync(receivedDataBuffer.Array,
                                              receivedDataBuffer.Offset,
                                              webSocketReceiveResult.Count,
                                              _cancellationToken)
                                  .ConfigureAwait(false);
            }
            while (!webSocketReceiveResult.EndOfMessage);

            return webSocketReceiveResult;
        }

        private void OnNewMessage(MemoryStream payloadData)
        {
            var streamReader = new StreamReader(payloadData);
            var message = streamReader.ReadToEnd();

            if(DiagnosticEnabled)
                System.Diagnostics.Debug.Write("Received message: {Message} " + message, this.GetType().Name);

            RaiseMessageArrived(message);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        private bool IsConnected => _webSocket.State == WebSocketState.Open;

        public event Action<string> MessageArrived;

        public event Action ConnectionClosed;

        public event Action<Exception> ConnectionError;

        protected virtual void RaiseMessageArrived(string message)
        {
            MessageArrived?.Invoke(message);
        }

        protected virtual void RaiseConnectionClosed()
        {
            if (DiagnosticEnabled)
                System.Diagnostics.Debug.Write("Connection has been closed", this.GetType().Name);
            ConnectionClosed?.Invoke();
        }

        protected virtual void RaiseConnectionError(Exception ex)
        {
            if (DiagnosticEnabled)
                System.Diagnostics.Debug.Write("A connection error occured: " + ex.Message, this.GetType().Name);
            ConnectionError?.Invoke(ex);

    
        }
    }
}
