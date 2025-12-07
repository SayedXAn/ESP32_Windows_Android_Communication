#include <WiFi.h>
#include <WebServer.h>

// Wi-Fi credentials
const char* ssid = "SayNaz";
const char* password = "11223344";

// LED Pins
const int PIN_BRAIN = 4;
const int PIN_LUNGS = 26;
const int PIN_GUT   = 27;

WebServer server(80);

void TurnOnOff(int index)
{
  digitalWrite(PIN_BRAIN, index == 0 ? LOW : HIGH);
  digitalWrite(PIN_LUNGS, index == 1 ? LOW : HIGH);
  digitalWrite(PIN_GUT, index == 2 ? LOW : HIGH);
}

void handleLED()
{
  String cmd = server.uri(); // e.g., /LED_BRAIN
  if (cmd == "/LED_BRAIN") TurnOnOff(0);
  else if (cmd == "/LED_LUNGS") TurnOnOff(1);
  else if (cmd == "/LED_GUT") TurnOnOff(2);
  else if (cmd == "/RESET") TurnOnOff(-1);
  
  server.send(200, "text/plain", "OK");
}

void setup() {
  Serial.begin(115200);

  // Initialize pins
  pinMode(PIN_BRAIN, OUTPUT);
  pinMode(PIN_LUNGS, OUTPUT);
  pinMode(PIN_GUT, OUTPUT);
  TurnOnOff(-1);

  // Connect to Wi-Fi
  Serial.print("Connecting to Wi-Fi...");
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nConnected! IP: " + WiFi.localIP().toString());

  // Setup server routes
  server.on("/LED_BRAIN", handleLED);
  server.on("/LED_LUNGS", handleLED);
  server.on("/LED_GUT", handleLED);
  server.on("/RESET", handleLED);

  server.begin();
  Serial.println("HTTP Server started");
}

void loop() {
  server.handleClient();
}
