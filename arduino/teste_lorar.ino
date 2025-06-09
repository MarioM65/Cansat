#include <HardwareSerial.h>
HardwareSerial lora(1); // UART1
void setup() {
  Serial.begin(115200); // Monitor serial
  lora.begin(9600, SERIAL_8N1, 16, 17); // RX=16, TX=17
  Serial.println("Receptor iniciado");
}
void loop() {
  if (lora.available())
   {
    String mensagem = lora.readStringUntil('\n');
    Serial.print("Recebido: ");
    Serial.println(mensagem);
  }
}