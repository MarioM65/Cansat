#define BLYNK_TEMPLATE_ID "TMPL2WvDKJCu7"
#define BLYNK_TEMPLATE_NAME "Cansat"
#include <WiFi.h>
#include <HTTPClient.h>
#include <BlynkSimpleEsp32.h>
char ssid[] = "cdci";
char pass[] = "cdci@2025";
char auth[] = "1uYT70Asu3yLXcMKsfQu7zHJtpBZQAm";

#define RXD2 16
#define TXD2 17
#define VP_TEMP V0
#define VP_UMI V1
#define VP_PRESS V2
#define VP_AR V3
void setup() {
  Serial.begin(115200);
  configTime(0, 0, "pool.ntp.org", "time.nist.gov");
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

    float temp = extrairValor(json, "temperatura_dht");
    float hum = extrairValor(json, "umidade");
    float press = extrairValor(json, "pressao");
    float ar = extrairValor(json, "qualidade_ar");

    Blynk.virtualWrite(VP_TEMP, temp);
    Blynk.virtualWrite(VP_UMI, hum);
    Blynk.virtualWrite(VP_PRESS, press);
    Blynk.virtualWrite(VP_AR, ar);

    if (WiFi.status() == WL_CONNECTED) {
      String timestamp = obterTimestampISO8601(); // Pega o horário atual

      enviarSensor("temperatura_dht11", "temperatura", temp, timestamp);
      enviarSensor("umidade_dht11", "umidade", hum, timestamp);
      enviarSensor("pressao_BMP180", "pressao", press, timestamp);
      enviarSensor("qualidade_ar_MQ135", "qualidade_ar", ar, timestamp);
    }
  }
}
void enviarSensor(String tipo, String grandeza, float valor, String data) {
  HTTPClient http;
  http.begin("http://<IP_DO_SERVIDOR>:5082/api/sensors"); 
  http.addHeader("Content-Type", "application/json");

  String body = "{";
  body += "\"Tipo\":\"" + tipo + "\",";
  body += "\"Grandeza\":\"" + grandeza + "\",";
  body += "\"valor\":" + String(valor, 2) + ",";
  body += "\"medido_em\":\"" + data + "\"";
  body += "}";

  int httpResponseCode = http.POST(body);
  Serial.println("POST " + grandeza + " => " + String(httpResponseCode));
  http.end();
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
String obterTimestampISO8601() {
  time_t now;
  struct tm timeinfo;
  if (!getLocalTime(&timeinfo)) return "2025-06-08T00:00:00"; // fallback
  char buf[30];
  strftime(buf, sizeof(buf), "%Y-%m-%dT%H:%M:%S", &timeinfo);
  return String(buf);
}
