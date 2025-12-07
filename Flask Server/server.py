from flask import Flask, jsonify
import requests

app = Flask(__name__)

# ESP32 IP
ESP32_IP = "192.168.1.100"
ESP32_PORT = 80

latest_command = None

# Accept command as part of the URL
@app.route('/send/<cmd>', methods=['GET', 'POST'])
def send_command(cmd):
    global latest_command

    cmd_lower = cmd.lower()
    latest_command = cmd_lower

    # Map URL command to ESP32 endpoint
    esp_map = {
        "led_brain": "LED_BRAIN",
        "led_lungs": "LED_LUNGS",
        "led_gut": "LED_GUT",
        "reset": "RESET"
    }

    if cmd_lower in esp_map:
        try:
            requests.get(f"http://{ESP32_IP}/{esp_map[cmd_lower]}", timeout=2)
        except requests.exceptions.RequestException as e:
            return jsonify({"status": "error", "message": str(e)}), 500

    return jsonify({"status": "ok", "command": cmd_lower})

# Endpoint for Windows Unity app to get latest command
@app.route('/get_command', methods=['GET'])
def get_command():
    global latest_command
    return jsonify({"command": latest_command})

@app.route('/')
def index():
    return "Flask Server Running"

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000, debug=True)
