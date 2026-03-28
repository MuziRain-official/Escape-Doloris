// Unity Editor MCP Client - Extended
const net = require('net');
const { Buffer } = require('buffer');
const fs = require('fs');
const path = require('path');

const HOST = '127.0.0.1';
const PORT = 6400;

let requestId = 1;

function createFramedRequest(type, params = {}) {
  const json = JSON.stringify({
    id: String(requestId++),
    type: type,
    params: params
  });

  const jsonBuffer = Buffer.from(json, 'utf8');
  const lengthBuffer = Buffer.alloc(4);
  lengthBuffer.writeUInt32BE(jsonBuffer.length, 0);

  return Buffer.concat([lengthBuffer, jsonBuffer]);
}

function sendCommand(command, params = {}) {
  return new Promise((resolve, reject) => {
    const client = new net.Socket();
    let responseBuffer = Buffer.alloc(0);
    let resolved = false;

    client.connect(PORT, HOST, () => {
      const request = createFramedRequest(command, params);
      client.write(request);
    });

    client.on('data', (data) => {
      responseBuffer = Buffer.concat([responseBuffer, data]);

      while (responseBuffer.length >= 4) {
        const length = responseBuffer.readUInt32BE(0);
        if (responseBuffer.length >= 4 + length) {
          const jsonData = responseBuffer.slice(4, 4 + length).toString('utf8');
          responseBuffer = responseBuffer.slice(4 + length);

          try {
            const resp = JSON.parse(jsonData);
            if (!resolved) {
              resolved = true;
              resolve(resp);
            }
          } catch (e) {}
        } else {
          break;
        }
      }
    });

    client.on('close', () => {
      if (!resolved) {
        resolved = true;
        resolve({ error: 'Connection closed' });
      }
    });

    client.on('error', (err) => {
      if (!resolved) {
        resolved = true;
        reject(err);
      }
    });

    setTimeout(() => {
      if (!resolved) {
        resolved = true;
        client.destroy();
        resolve({ error: 'Timeout' });
      }
    }, 30000);
  });
}

// Main
async function main() {
  const args = process.argv.slice(2);

  if (args.length === 0) {
    console.log('Unity Editor MCP Client - Extended');
    console.log('Usage: node mcp-client.js <command> [params]');
    console.log('');
    console.log('Available commands:');
    console.log('  ping                           - Test connection');
    console.log('  get_editor_state               - Get editor state');
    console.log('  list_scenes                   - List scenes');
    console.log('  get_hierarchy                 - Get hierarchy');
    console.log('  create_gameobject <name> <type> <parent> - Create GameObject');
    console.log('  add_component <gameobject> <component> - Add component');
    console.log('  modify_gameobject <name> <property> <value> - Modify GameObject');
    console.log('  find_gameobject <name>       - Find GameObject');
    console.log('  delete_gameobject <name>     - Delete GameObject');
    console.log('  play_game                     - Start play mode');
    console.log('  stop_game                     - Stop play mode');
    console.log('  execute_menu_item <path>      - Execute menu item');
    console.log('  create_script <name> <code>   - Create C# script');
    console.log('');
    console.log('Examples:');
    console.log('  node mcp-client.js ping');
    console.log('  node mcp-client.js create_gameobject "Player" "capsule"');
    console.log('  node mcp-client.js add_component "Player" "Rigidbody"');
    process.exit(0);
  }

  const command = args[0];
  let params = {};

  if (args.length > 1) {
    try {
      params = JSON.parse(args[1]);
    } catch (e) {
      if (command === 'create_gameobject') {
        params = { name: args[1], type: args[2] || 'empty', parent: args[3] || null };
      } else if (command === 'add_component') {
        params = { gameobject: args[1], component: args[2] };
      } else if (command === 'modify_gameobject') {
        params = { name: args[1], property: args[2], value: args[3] };
      } else if (command === 'find_gameobject') {
        params = { name: args[1] };
      } else if (command === 'delete_gameobject') {
        params = { name: args[1] };
      } else if (command === 'execute_menu_item') {
        params = { path: args[1] };
      } else if (command === 'create_script') {
        params = { name: args[1], code: args[2] || '' };
      } else {
        params = { value: args[1] };
      }
    }
  }

  try {
    const response = await sendCommand(command, params);
    console.log(JSON.stringify(response, null, 2));
  } catch (err) {
    console.error('Error:', err.message);
    process.exit(1);
  }
}

main();
