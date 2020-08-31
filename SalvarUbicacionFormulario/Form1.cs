using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalvarUbicacionFormulario
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Herramientas.EstadoFormulario.RecuperarUbicacionVentana(this);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Herramientas.EstadoFormulario.SalvarUbicacionVentana(this);
        }
    }
}
