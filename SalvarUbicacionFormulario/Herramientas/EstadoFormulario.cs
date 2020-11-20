using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Properties = SalvarUbicacionFormulario.Properties;

namespace Herramientas
{
    /// <summary>
    /// FUnciones para salvar y restaurar la ubicación y tamaño de formularios abiertos en pantalla
    /// </summary>
    /// <remarks>
    /// Se requiere crear una propiedad en Setting.Settings llamada UbicacionesFormulariosSerializadas, de tipo String.
    /// </remarks>
    public class EstadoFormulario
    {
        /// <summary>
        /// Salvar el tamaño, estado y ubicación de un Form
        /// </summary>
        public static void SalvarUbicacionVentana(Form formulario)
        {
            try
            {
                int codigoTamPantallaActual = ObtenerCodigoTamPantalla();

                ColeccionUbicaciones cu = Deserializar(Properties.Settings.Default.UbicacionesFormulariosSerializadas) ?? new ColeccionUbicaciones();
                UbicacionFormulario uf = new UbicacionFormulario();
                uf.CodigoTamVentana = codigoTamPantallaActual;
                uf.EstaMaximizado = formulario.WindowState == FormWindowState.Maximized;
                if (!uf.EstaMaximizado)
                {
                    uf.Size = formulario.Size;
                    uf.Location = formulario.Location;
                }

                string name = ObtenerNombreVentana(formulario);
                uf.Nombre = name;

                // Obtener ubicaciones distintas de la actual
                ColeccionUbicaciones cuNueva = new ColeccionUbicaciones(from f in cu where f.Nombre != name || f.CodigoTamVentana != codigoTamPantallaActual select f);

                // Agregar la actual a la coleccion
                cuNueva.Add(uf);

                // Salvar la nueva coleccion
                Properties.Settings.Default.UbicacionesFormulariosSerializadas = Serializar(cuNueva);
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Restore the size, state and location that were saved for a form
        /// </summary>
        /// <param name="form">The form whose properties are to be restored</param>
        public static void RecuperarUbicacionVentana(Form form)
        {
            try
            {
                int codigoTamPantallaActual = ObtenerCodigoTamPantalla();

                ColeccionUbicaciones cu = Deserializar(Properties.Settings.Default.UbicacionesFormulariosSerializadas);
                if (cu != null)
                {
                    string name = ObtenerNombreVentana(form);
                    UbicacionFormulario uf = (from f in cu where f.Nombre == name && f.CodigoTamVentana == codigoTamPantallaActual select f).FirstOrDefault();
                    if (uf != null)
                    {
                        if (uf.EstaMaximizado)
                        {
                            form.WindowState = FormWindowState.Maximized;
                        }
                        else
                        {
                            form.Size = uf.Size;
                            form.Location = uf.Location;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private static string ObtenerNombreVentana(Form form)
        {
            return form.GetType().Name;
        }

        private static int ObtenerCodigoTamPantalla()
        {
            Screen[] pantallas = Screen.AllScreens;
            ColeccionDeCoordenadasLocales ccl = new ColeccionDeCoordenadasLocales();
            for (int i = 0; i < pantallas.Length; i++)
            {
                CoordenadasLocales cl = new CoordenadasLocales(i, pantallas[i].WorkingArea.Width, pantallas[i].WorkingArea.Height);
                ccl.Add(cl);
            }

            return ccl.ObtenerCodigo();
        }

        private static string Serializar(ColeccionUbicaciones coleccionUbicaciones)
        {
            if (coleccionUbicaciones == null)
            {
                return null;
            }

            XmlSerializer xs = new XmlSerializer(typeof(ColeccionUbicaciones));
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            xs.Serialize(sw, coleccionUbicaciones);
            return sb.ToString();
        }

        private static ColeccionUbicaciones Deserializar(string textoSerializado)
        {
            if (string.IsNullOrEmpty(textoSerializado))
            {
                return null;
            }

            XmlSerializer xs = new XmlSerializer(typeof(ColeccionUbicaciones));
            StringReader sr = new StringReader(textoSerializado);
            ColeccionUbicaciones deserializado = (ColeccionUbicaciones)xs.Deserialize(sr);
            return deserializado;
        }

        [Serializable]
        public class UbicacionFormulario
        {
            public string Nombre { get; set; }

            public bool EstaMaximizado { get; set; }

            public System.Drawing.Size Size { get; set; }

            public System.Drawing.Point Location { get; set; }

            public int CodigoTamVentana { get; set; }
        }

        [Serializable]
        public class ColeccionUbicaciones : List<UbicacionFormulario>
        {
            public ColeccionUbicaciones()
                : base()
            {
            }

            public ColeccionUbicaciones(IEnumerable<UbicacionFormulario> luf)
                : this()
            {
                foreach (UbicacionFormulario uf in luf)
                {
                    this.Add(uf);
                }
            }
        }

        private class CoordenadasLocales
        {
            public CoordenadasLocales(int numero, int width, int height)
            {
                this.Numero = numero;
                this.Width = width;
                this.Height = height;
            }

            public int Numero { get; set; }

            public int Width { get; set; }

            public int Height { get; set; }

            public int ObtenerCodigo()
            {
                return (Width << Numero) ^ (Height << (Numero + 16));
            }
        }

        private class ColeccionDeCoordenadasLocales : List<CoordenadasLocales>
        {
            public int ObtenerCodigo()
            {
                int hash = 0;
                foreach (CoordenadasLocales cl in this)
                {
                    hash ^= cl.ObtenerCodigo();
                }

                return hash;
            }
        }
    }
}
