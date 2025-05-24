using Microsoft.AspNetCore.Mvc;
using Cansat.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Data.OleDb;
using System.IO;

namespace Cansat.Controllers
{
    [ApiController]
    [Route("api/sensors")]
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class SensorController : ControllerBase
    {
        private string GetConnectionString()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases", "Sensoresaccdb.accdb");
            return $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;";
        }

        private List<Sensor> GetSensoresFromDb(bool? deleted = null)
        {
            var sensores = new List<Sensor>();
            string connString = GetConnectionString();
            string query = "SELECT * FROM sensor";

            if (deleted != null)
                query += $" WHERE deleted={(deleted.Value ? -1 : 0)}";

            using (var connection = new OleDbConnection(connString))
            {
                connection.Open();
                using (var cmd = new OleDbCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sensores.Add(new Sensor
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Tipo = reader["Tipo"].ToString(),
                            Grandeza = reader["Grandeza"].ToString(),
                            valor = Convert.ToDouble(reader["valor"]),
                            medido_em = Convert.ToDateTime(reader["medido_em"]),
                            deleted = Convert.ToBoolean(reader["deleted"])
                        });
                    }
                }
            }
            return sensores;
        }

        private bool ExecuteNonQuery(string query, Dictionary<string, object> parameters)
        {
            using (var connection = new OleDbConnection(GetConnectionString()))
            {
                connection.Open();
                using (var cmd = new OleDbCommand(query, connection))
                {
                    foreach (var param in parameters)
                        cmd.Parameters.AddWithValue(param.Key, param.Value);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Sensor>> Get()
        {
            return GetSensoresFromDb(deleted: false);
        }

        [HttpGet("trash")]
        public ActionResult<IEnumerable<Sensor>> GetTrash()
        {
            return GetSensoresFromDb(deleted: true);
        }

        [HttpGet("{id}")]
        public ActionResult<Sensor> Get(int id)
        {
            var sensor = GetSensoresFromDb()
                .FirstOrDefault(s => s.Id == id && s.deleted == false);

            if (sensor == null) return NotFound();
            return sensor;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Sensor novoSensor)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using (var connection = new OleDbConnection(GetConnectionString()))
            {
                connection.Open();

                string query = "INSERT INTO sensor (Tipo, Grandeza, valor, medido_em, deleted) VALUES (?, ?, ?, ?, 0)";
                using (var cmd = new OleDbCommand(query, connection))
                {
                    cmd.Parameters.Add("Tipo", OleDbType.VarWChar).Value = novoSensor.Tipo;
                    cmd.Parameters.Add("Grandeza", OleDbType.VarWChar).Value = novoSensor.Grandeza;
                    cmd.Parameters.Add("valor", OleDbType.Double).Value = novoSensor.valor;
                    cmd.Parameters.Add("medido_em", OleDbType.Date).Value = novoSensor.medido_em;

                    cmd.ExecuteNonQuery();
                }

                // Obter último ID inserido
                string getIdQuery = "SELECT @@IDENTITY";
                using (var cmdId = new OleDbCommand(getIdQuery, connection))
                {
                    novoSensor.Id = Convert.ToInt32(cmdId.ExecuteScalar());
                }
            }

            return CreatedAtAction(nameof(Get), new { id = novoSensor.Id }, novoSensor);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Sensor sensorAtualizado)
        {
            string query = @"UPDATE sensor 
                             SET Tipo = ?, Grandeza = ?, valor = ?, medido_em = ? 
                             WHERE Id = ?";

            bool atualizado = ExecuteNonQuery(query, new Dictionary<string, object>
            {
                { "@Tipo", sensorAtualizado.Tipo },
                { "@Grandeza", sensorAtualizado.Grandeza },
                { "@valor", sensorAtualizado.valor },
                { "@medido_em", sensorAtualizado.medido_em },
                { "@Id", id }
            });

            return atualizado ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string query = "UPDATE sensor SET deleted = true WHERE Id = ?";
            bool sucesso = ExecuteNonQuery(query, new Dictionary<string, object>
            {
                { "@Id", id }
            });

            return sucesso ? NoContent() : NotFound();
        }

        [HttpPost("restore/{id}")]
        public IActionResult Restore(int id)
        {
            string query = "UPDATE sensor SET deleted = false WHERE Id = ?";
            bool sucesso = ExecuteNonQuery(query, new Dictionary<string, object>
            {
                { "@Id", id }
            });

            return sucesso ? NoContent() : NotFound();
        }

        [HttpDelete("purge/{id}")]
        public IActionResult Purge(int id)
        {
            string query = "DELETE FROM sensor WHERE Id = ?";
            bool sucesso = ExecuteNonQuery(query, new Dictionary<string, object>
            {
                { "@Id", id }
            });

            return sucesso ? NoContent() : NotFound();
        }
    }
}
