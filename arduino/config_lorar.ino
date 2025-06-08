#include <SoftwareSerial.h>
SoftwareSerial lora(2, 3); // RX, TX

void sendCommand(String cmd) {
  lora.print(cmd);
  delay(300);
  while (lora.available()) {
    Serial.write(lora.read());
  }
  Serial.println();
}

void setup() {
  Serial.begin(9600);
  lora.begin(9600);
  delay(500);
  Serial.println("Configurando RECEPTOR...");

  sendCommand("AT+ADDR=0102\r\n");   // Endereço: 0x0102
  sendCommand("AT+NETID=01\r\n");    // Mesmo NetID
  sendCommand("AT+CHAN=21\r\n");     // Mesmo canal que o transmissor
  sendCommand("AT+UART=9600,8,1,0\r\n");
  sendCommand("AT+POWER=3\r\n");
  sendCommand("AT+SAVE\r\n");
  Serial.println("RECEPTOR configurado!");
}

void loop() {}
