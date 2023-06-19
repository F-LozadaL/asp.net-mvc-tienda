using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace CapaDatos
{
    public class CD_Reporte
    {

        public List<Reporte> Ventas(string fechainicio, string fechafin, string idtransaccion)
        {
            List<Reporte> list = new List<Reporte>();
            try
            {

                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {

                    SqlCommand cmd = new SqlCommand("sp_ReporteVentas", oconexion); 
                    cmd.Parameters.AddWithValue("FechaInicio", fechainicio);
                    cmd.Parameters.AddWithValue("FechaFin", fechafin);
                    cmd.Parameters.AddWithValue("IdTransaccion", idtransaccion);
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {

                        while (dr.Read())
                        {
                            list.Add(new Reporte()
                            {


                                FechaVenta = dr["FechaVenta"].ToString(),
                                Cliente = dr["Cliente"].ToString(),
                                Producto = dr["Producto"].ToString(),
                                Precio = Convert.ToDecimal(dr["Precio"], new CultureInfo("es-CO")),
                                Cantidad = Convert.ToInt32(dr["Cantidad"]),
                                Total = Convert.ToDecimal(dr["Total"], new CultureInfo("es-CO")),
                                IdTransaccion = dr["IdTransaccion"].ToString()

                            });
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                list = new List<Reporte>();
            }
            return list;
        }



        public Dashboard VerDashboard()
        {
            Dashboard objeto = new Dashboard(); ;
            try
            {

                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {

                    SqlCommand cmd = new SqlCommand("sp_ReporteDashboard", oconexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {

                        while (dr.Read())
                        {

                            objeto = new Dashboard()
                            {
                                TotalClientes = Convert.ToInt32(dr["TotalClientes"]),
                                TotalVentas = Convert.ToInt32(dr["TotalVentas"]),
                                TotalProductos = Convert.ToInt32(dr["TotalProductos"])
                            };
                        }

                    }
                }

            }
            catch
            {
                objeto = new Dashboard();
            }

            return objeto;
        }


    }
}
