#include <DHT.h>
#include <Wire.h>
//#include <Adafruit_BMP280.h>

#define DHTPIN 2
#define DHTTYPE DHT11

DHT dht(DHTPIN, DHTTYPE);
//Adafruit_BMP280 bmp;

void setup() {
  Serial.begin(9600);
  dht.begin();

 /* if (!bmp.begin(0x76)) {
    Serial.println("Erro no BMP280");
  }*/

  Serial.println("Iniciado.");
}

void loop() {
  float temp = dht.readTemperature();
  float hum = dht.readHumidity();

  int mq135_raw = analogRead(A0);
  float qualidade_ar = mq135_raw;

  //float pressao = bmp.readPressure() / 100.0F; // em hPa
 // float altitude = bmp.readAltitude(1013.25);  // altitude estimada com base na pressão ao nível do mar

  String json = "{";
  json += "\"temperatura_dht\":" + String(temp, 1) + ",";
  json += "\"umidade\":" + String(hum, 1) + ",";
//  json += "\"pressao\":" + String(pressao, 1) + ",";
  //json += "\"altitude\":" + String(altitude, 1) + ",";
  json += "\"qualidade_ar\":" + String(qualidade_ar);
  json += "}\n";

  Serial.print("Enviado: ");
  Serial.println(json);

  delay(5000);
}
