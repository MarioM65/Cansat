#include <SoftwareSerial.h>

SoftwareSerial lora(2, 3); // RX, TX

void setup() {
  Serial.begin(9600);
  lora.begin(9600);
  Serial.println("Receptor iniciado");
}

void loop() {
  if (lora.available()) {
    String mensagem = lora.readStringUntil('\n');
    Serial.print("Recebido: ");
    Serial.println(mensagem);
  }
}
