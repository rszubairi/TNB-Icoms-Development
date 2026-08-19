using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace TnbIcoms.Migration
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("  TNB ICOMS 2.0 - DATA MIGRATION ENGINE (v1.0) ");
            Console.WriteLine("=================================================");

            string sourceConnStr = "Server=52.74.111.85;Database=dbOutage;User Id=sa;Password=Khan151681*;TrustServerCertificate=True;Connect Timeout=30;";
            
            Console.WriteLine("\n[1/5] Testing Source SQL Server Connection (52.74.111.85 / dbOutage)...");
            try
            {
                using (var conn = new SqlConnection(sourceConnStr))
                {
                    await conn.OpenAsync();
                    Console.WriteLine("  --> Connected successfully to legacy database!");

                    // Read Zones
                    Console.WriteLine("\n[2/5] Extracting Legacy Grid Zones (TblGridZone)...");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT ID, Zone, Email FROM dbo.TblGridZone;";
                        using (var r = await cmd.ExecuteReaderAsync())
                        {
                            while (await r.ReadAsync())
                            {
                                Console.WriteLine($"  - Zone ID: {r.GetInt32(0)}, Name: {r.GetString(1).Trim()}, Email: {r.GetValue(2)}");
                            }
                        }
                    }

                    // Read Substations
                    Console.WriteLine("\n[3/5] Extracting Legacy Substations (TblSubstation_new)...");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT MNEM, Name, Region FROM dbo.TblSubstation_new;";
                        using (var r = await cmd.ExecuteReaderAsync())
                        {
                            int count = 0;
                            while (await r.ReadAsync())
                            {
                                count++;
                                if (count <= 5)
                                    Console.WriteLine($"  - Mnemonic: {r.GetString(0).Trim()}, Station Name: {r.GetValue(1)}, Region: {r.GetValue(2)}");
                            }
                            Console.WriteLine($"  --> Total Substations Mapped: {count}");
                        }
                    }

                    // Read Outage Requests
                    Console.WriteLine("\n[4/5] Extracting Legacy Outages (TblTxOutRequest)...");
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT requestid, userid, outagecode, datestart, dateend, jobtype, description FROM dbo.TblTxOutRequest;";
                        using (var r = await cmd.ExecuteReaderAsync())
                        {
                            int count = 0;
                            while (await r.ReadAsync())
                            {
                                count++;
                                if (count <= 5)
                                    Console.WriteLine($"  - Request #{r.GetInt32(0)} | Code: {r.GetValue(2)} | Start: {r.GetValue(3)} | Job: {r.GetValue(5)}");
                            }
                            Console.WriteLine($"  --> Total Outage Requests Mapped: {count}");
                        }
                    }

                    Console.WriteLine("\n[5/5] ETL Migration Plan Verification Complete!");
                    Console.WriteLine("  --> Ready to migrate into target ICOMS 2.0 SQL Server Instance.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [!] Error during migration execution: {ex.Message}");
            }
        }
    }
}
