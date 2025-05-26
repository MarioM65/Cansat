#include <SoftwareSerial.h>
#include <DHT.h>
#include <Wire.h>
#include <Adafruit_BMP280.h>
#define DHTPIN 2
#define DHTTYPE DHT11

DHT dht(DHTPIN, DHTTYPE);
Adafruit_BMP280 bmp; 

SoftwareSerial LoRaSerial(10, 11); 

void setup() {
  Serial.begin(9600);
  LoRaSerial.begin(9600);

  dht.begin();

  if (!bmp.begin(0x76)) {
    Serial.println("Erro no BMP280");
  }

  Serial.println("Iniciado.");
}

void loop() {
  float temp = dht.readTemperature();
  float hum = dht.readHumidity();

  int mq135_raw = analogRead(A0);
  float qualidade_ar = mq135_raw;

  float pressao = bmp.readPressure() / 100.0F; 

  String json = "{";
  json += "\"temperatura_dht\":" + String(temp, 1) + ",";
  json += "\"umidade\":" + String(hum, 1) + ",";
  json += "\"pressao\":" + String(pressao, 1) + ",";
  json += "\"qualidade_ar\":" + String(qualidade_ar);
  json += "}\n";

  LoRaSerial.print(json);
  Serial.print("Enviado: ");
  Serial.println(json);

  delay(5000);
}
