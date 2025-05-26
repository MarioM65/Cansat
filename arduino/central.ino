#include <WiFi.h>
#include <HTTPClient.h>
#include <BlynkSimpleEsp32.h>
char ssid[] = "SEU_WIFI";
char pass[] = "SUA_SENHA";
char auth[] = "SEU_TOKEN_BLYNK";
#define RXD2 16
#define TXD2 17
#define VP_TEMP V0
#define VP_UMI V1
#define VP_PRESS V2
#define VP_AR V3
void setup() {
  Serial.begin(115200);
  Serial2.begin(9600, SERIAL_8N1, RXD2, TXD2);
  WiFi.begin(ssid, pass);
  Blynk.begin(auth, ssid, pass);
  Serial.println("ESP32 Central iniciado");
}

void loop() {
  Blynk.run();
  if (Serial2.available()) {
    String json = Serial2.readStringUntil('\n');
    Serial.println("Recebido LoRa: " + json);
    if (WiFi.status() == WL_CONNECTED) {
      HTTPClient http;
      http.begin("http://localhost:5082/api/sensors");
      http.addHeader("Content-Type", "application/json");
      int resp = http.POST(json);
      Serial.println("API Response: " + String(resp));
      http.end();
    }
    float temp = extrairValor(json, "temperatura_dht");
    float hum = extrairValor(json, "umidade");
    float press = extrairValor(json, "pressao");
    float ar = extrairValor(json, "qualidade_ar");
    Blynk.virtualWrite(VP_TEMP, temp);
    Blynk.virtualWrite(VP_UMI, hum);
    Blynk.virtualWrite(VP_PRESS, press);
    Blynk.virtualWrite(VP_AR, ar);
  }
}
float extrairValor(String json, String chave) {
  int idx = json.indexOf(chave);
  if (idx == -1) return 0;
  int start = json.indexOf(":", idx) + 1;
  int end = json.indexOf(",", start);
  if (end == -1) end = json.indexOf("}", start);
  String valStr = json.substring(start, end);
  valStr.trim();
  return valStr.toFloat();
}
