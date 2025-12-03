#include <WiFi.h>

// --- CONFIGURATION ---
const char* ssid     = "ESP32_HOTSPOT";      // Name of the Wi-Fi network ESP32 will create
const char* password = "12345678";     // Password (min 8 chars)

// --- PIN DEFINITIONS ---
// Change these numbers to match where you plugged in your LEDs
const int PIN_BRAIN = 4;
const int PIN_LUNGS = 26;
const int PIN_GUT   = 27;

// Create the Server on Port 80
WiFiServer server(80);

void setup() {
  // 1. Initialize Serial for debugging
  Serial.begin(115200);
  delay(1000);
  Serial.println("\nStarting System...");

  // 2. Initialize LED Pins
  pinMode(PIN_BRAIN, OUTPUT);
  pinMode(PIN_LUNGS, OUTPUT);
  pinMode(PIN_GUT, OUTPUT);

  // Start with all lights OFF
  digitalWrite(PIN_BRAIN, HIGH);
  digitalWrite(PIN_LUNGS, HIGH);
  digitalWrite(PIN_GUT, HIGH);

  // 3. Start Wi-Fi in Access Point Mode
  Serial.println("Configuring Access Point...");
  WiFi.softAP(ssid, password);

  IPAddress myIP = WiFi.softAPIP();
  Serial.print("AP IP address: ");
  Serial.println(myIP); // This should print 192.168.4.1

  // 4. Start the Server
  server.begin();
  Serial.println("Server started. Ready for Unity connection.");
}

void TurnOnOff(int index)
{
  if(index == 0)
  {
    digitalWrite(PIN_BRAIN, LOW);
    digitalWrite(PIN_LUNGS, HIGH);
    digitalWrite(PIN_GUT, HIGH);
  }
  else if(index == 1)
  {
    digitalWrite(PIN_BRAIN, HIGH);
    digitalWrite(PIN_LUNGS, LOW);
    digitalWrite(PIN_GUT, HIGH);
  }
  else if(index == 2)
  {
    digitalWrite(PIN_BRAIN, HIGH);
    digitalWrite(PIN_LUNGS, HIGH);
    digitalWrite(PIN_GUT, LOW);
  }
  else
  {
    digitalWrite(PIN_BRAIN, HIGH);
    digitalWrite(PIN_LUNGS, HIGH);
    digitalWrite(PIN_GUT, HIGH);
  }
}

void loop() {
  // Check if a client (Unity App) is trying to connect
  WiFiClient client = server.available();

  if (client) {
    Serial.println("New Client Connected");
    
    while (client.connected()) {
      if (client.available()) {
        // Read the message from Unity until the newline character
        String request = client.readStringUntil('\n');
        
        // Clean up the string (remove spaces or extra newlines)
        request.trim();
        
        Serial.print("Received Command: ");
        Serial.println(request);

        // --- COMMAND LOGIC ---
        
        if (request == "LED_BRAIN") {
          TurnOnOff(0);
          Serial.println("Action: Brain LED ON");
        } 
        else if (request == "LED_LUNGS") {
          TurnOnOff(1);
          Serial.println("Action: Lungs LED ON");
        } 
        else if (request == "LED_GUT") {
          TurnOnOff(2);
          Serial.println("Action: Gut LED ON");
        } 
        else if (request == "RESET") {
          TurnOnOff(-1);
          Serial.println("Action: SYSTEM RESET");
        }
        else {
          Serial.println("Warning: Unknown Command");
        }
        
        // We only expect one command per connection, so we can break loop
        break; 
      }
    }
    
    // Close the connection
    client.stop();
    Serial.println("Client Disconnected");
    Serial.println("-----------------------");
  }
}