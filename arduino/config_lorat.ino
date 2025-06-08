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
  Serial.println("Configurando TRANSMISSOR...");

  sendCommand("AT+ADDR=0101\r\n");   // Endereço: 0x0101
  sendCommand("AT+NETID=01\r\n");    // ID da rede
  sendCommand("AT+CHAN=21\r\n");     // Canal 33 = 433MHz + 33*1MHz 
  sendCommand("AT+UART=9600,8,1,0\r\n"); // UART padrão
  sendCommand("AT+POWER=3\r\n");     // Potência máxima
  sendCommand("AT+SAVE\r\n");        // Salvar
  Serial.println("TRANSMISSOR configurado!");
}

void loop() {}
