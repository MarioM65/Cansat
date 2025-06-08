#include <SoftwareSerial.h>

SoftwareSerial lora(2, 3); // RX, TX

void setup() {
  Serial.begin(9600);
  lora.begin(9600); // Taxa padrão do E32
  Serial.println("Transmissor iniciado");
}

void loop() {
  lora.println("Olá receptor!");
  Serial.println("Mensagem enviada.");
  delay(2000);
}
