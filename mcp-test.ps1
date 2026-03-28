# PowerShell TCP Test for Unity MCP
$server = "127.0.0.1"
$port = 6400

Write-Host "==========================================="
Write-Host "Unity Editor MCP Connection Test (PowerShell)"
Write-Host "==========================================="
Write-Host ""

try {
    $client = New-Object System.Net.Sockets.TcpClient($server, $port)
    $stream = $client.GetStream()
    $stream.ReadTimeout = 5000
    $stream.WriteTimeout = 5000

    Write-Host "✓ Connected to $server`:$port"

    # Send initialize request
    $request = @{
        jsonrpc = "2.0"
        id = 1
        method = "initialize"
        params = @{
            protocolVersion = "2024-11-05"
            capabilities = @{}
            clientInfo = @{
                name = "powershell-mcp-client"
                version = "1.0.0"
            }
        }
    } | ConvertTo-Json -Compress

    $bytes = [System.Text.Encoding]::UTF8.GetBytes($request + "`n")
    $stream.Write($bytes, 0, $bytes.Length)
    Write-Host "✓ Sent initialize request"

    # Read response
    $buffer = New-Object byte[] 4096
    $bytesRead = $stream.Read($buffer, 0, $buffer.Length)
    $response = [System.Text.Encoding]::UTF8.GetString($buffer, 0, $bytesRead)

    Write-Host "✓ Received response: $response"

    # Send ping
    Start-Sleep -Milliseconds 500
    $pingRequest = @{
        jsonrpc = "2.0"
        id = 2
        method = "tools/call"
        params = @{
            name = "ping"
            arguments = @{}
        }
    } | ConvertTo-Json -Compress

    $bytes = [System.Text.Encoding]::UTF8.GetBytes($pingRequest + "`n")
    $stream.Write($bytes, 0, $bytes.Length)
    Write-Host "✓ Sent ping request"

    # Read ping response
    $buffer = New-Object byte[] 4096
    $bytesRead = $stream.Read($buffer, 0, $buffer.Length)
    $response = [System.Text.Encoding]::UTF8.GetString($buffer, 0, $bytesRead)

    Write-Host "✓ Received response: $response"

    if ($response -match "result") {
        Write-Host ""
        Write-Host "==========================================="
        Write-Host "✓ SUCCESS! Connected to Unity Editor MCP"
        Write-Host "==========================================="
    }

    $stream.Close()
    $client.Close()

} catch {
    Write-Host "✗ Error: $_"
}
