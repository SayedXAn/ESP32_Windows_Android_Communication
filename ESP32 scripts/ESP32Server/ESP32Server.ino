#include <WiFi.h>

const char* ssid = "ESP32_HOTSPOT";
const char* password = "12345678";

// Relay pins
const int relays[3] = {26, 4, 27};

// Servers
WiFiServer androidServer(80);  // Android sends commands here
WiFiServer pcServer(81);       // PC connects here to receive signals

WiFiClient pcClient;           // Hold PC connection

void setup() {
  Serial.begin(115200);

  // WiFi Hotspot mode
  WiFi.softAP(ssid, password);
  Serial.println("AP started. IP:");
  Serial.println(WiFi.softAPIP());

  androidServer.begin();
  pcServer.begin();

  // Setup relays
  for (int i = 0; i < 4; i++) {
    pinMode(relays[i], OUTPUT);
    digitalWrite(relays[i], HIGH);
  }
}

void TurnOnOff(int index) {
  for (int i = 0; i < 4; i++) {
    digitalWrite(relays[i], (i == index ? LOW : HIGH));
  }
}

void NotifyPC(String message) {
  if (pcClient && pcClient.connected()) {
    pcClient.println(message);
  }
}

void loop() {

  // Accept PC connection on port 81
  if (!pcClient || !pcClient.connected()) {
    pcClient = pcServer.available();
  }

  // ---------- ANDROID COMMANDS ----------
  WiFiClient androidClient = androidServer.available();
  if (androidClient && androidClient.connected()) {
    String cmd = androidClient.readStringUntil('\n');
    cmd.trim();
    
    Serial.println("Android sent: " + cmd);

    if (cmd == "LED_BRAIN") {
      TurnOnOff(0);
      NotifyPC("LED_BRAIN");
    }
    else if (cmd == "LED_LUNGS") {
      TurnOnOff(1);
      NotifyPC("LED_LUNGS");
    }
    else if (cmd == "LED_GUT") {
      TurnOnOff(2);
      NotifyPC("LED_GUT");
    }
    else if (cmd == "RESET") {
      TurnOnOff(-1);
      NotifyPC("RESET");
    }

    androidClient.println("OK");
  }
}
